
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Model.Models;

namespace ToDoList.Web.Helper
{
    public  static class Authentification
    {
        private const string LogiraniKorisnik = "logirani_korisnik";
              
        public static void SetLogiraniKorisnik(this HttpContext context, Login korisnik,IConfiguration _configuration,IMapper _mapper ,bool snimiUCookie = true)
        {
            context.Session.Set(LogiraniKorisnik, korisnik);

            if (snimiUCookie)
            {

                var sqlQuary = ("Select * from [Identity] where [UserName] = @userName");
                Identity user = new Identity();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.QueryFirstOrDefault(sqlQuary, new { userName = korisnik.UserName });
                    user = _mapper.Map<Identity>(result);
                    connection.Close();
                }


                string token = Guid.NewGuid().ToString();
                AuthToken authToken = new AuthToken()
                {
                    UserTokenId=user.UserId,
                    Value=token,
                    CreatedOn=DateTime.Now
                };
                var sql = ("Insert into [AuthToken] (UserTokenId, [Value], CreatedOn) VALUES(@UserTokenId,@Value,@CreatedOn)");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    int rowsAffected = connection.Execute(sql, authToken);
                    connection.Close();
                }

              
                context.Response.SetCookieJson(LogiraniKorisnik, token);
            }
            else
                context.Response.SetCookieJson(LogiraniKorisnik, null);

        }

        public static Login GetLogiraniKorisnik(this HttpContext context, IConfiguration _configuration, IMapper _mapper)
        {

            Login user = context.Session.Get<Login>(LogiraniKorisnik);
            Login userLog = new Login();
            AuthToken authoToken = new AuthToken();

            if (user == null)
            {

                string token = context.Request.GetCookieJson<string>(LogiraniKorisnik);
                if (token == null)
                    return null;

                var sqlQuary = ("Select * from AuthToken where Value = @value");
              
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.QueryFirstOrDefault(sqlQuary, new { value = token });
                    authoToken = _mapper.Map<AuthToken>(result);
                    connection.Close();
                }

                if (authoToken != null)
                {
                    var sql = ("Select * from [Identity] where [UserId] = @user");

                    using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                    {
                        connection.Open();
                        var result = connection.QueryFirstOrDefault(sql, new { @user = authoToken.UserTokenId});
                        userLog = _mapper.Map<Login>(result);

                        connection.Close();
                    }
                    context.Session.Set(LogiraniKorisnik, userLog);
                }
                return userLog; 
            }
            

            return user;
        }


    }
}
