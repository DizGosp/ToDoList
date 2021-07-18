using Microsoft.Extensions.Configuration;
using AutoMapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;

namespace ToDoList.DB.Repositories
{
    public class IdentityRepository : IIdentity
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public IdentityRepository(IConfiguration configuration, IMapper mapper) 
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public bool Add(Identity entity)
        {
            RegistrationInsert userSave = new RegistrationInsert()
            {
                FirstName=entity.FirstName,
                LastName=entity.LastName,
                UserName=entity.UserName,
                PasswordHash=entity.PasswordHash,
                PasswordSalt=entity.PasswordSalt,
                CreatedOn=entity.CreatedOn
            };

            try
            {
                var sql = ("Insert into [Identity] ( [CreatedOn], [FirstName], [LastName], [PasswordHash], [PasswordSalt], [UserName]) VALUES(@CreatedOn,@FirstName,@LastName,@PasswordHash,@PasswordSalt,@UserName)");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    int rowsAffected = connection.Execute(sql, userSave);
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Identity Get(string id)
        {
            var sql = ("Select* from [Identity] where UserId=@userId");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var result = connection.QueryFirstOrDefault(sql, new { @userId = id });
                return _mapper.Map<Identity>(result);
            }
        }

        public List<Identity> GetAll()
        {
            var sql = ("SELECT [UserId], [FirstName], [LastName], [UserName], [PasswordHash], [PasswordSalt], [CreatedOn] FROM [Identity]");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var identity = connection.Query(sql).ToList();
                connection.Close();
                return _mapper.Map<List<Identity>>(identity);
            }
        }

        public Identity GetByUserName(string userName)
        {
            var sql = ("Select* from [Identity] where UserName=@username");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var result = connection.QueryFirstOrDefault(sql, new { username = userName });
                return _mapper.Map<Identity>(result); 
            }
        }

        public bool Update(Identity entity)
        {
            try
            {
                var sql = ("UPDATE [Identity] SET FirstName=@FirstName, LastName=@LastName WHERE UserId=@userId");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.Execute(sql, new { @FirstName = entity.FirstName, @LastName = entity.LastName, @userId = entity.UserId});
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
