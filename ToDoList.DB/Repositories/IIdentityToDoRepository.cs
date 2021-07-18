using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;

namespace ToDoList.DB.Repositories
{
    class IIdentityToDoRepository : IIdentityToDo
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public IIdentityToDoRepository(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }


        public bool Add(IdentityToDoAdd entity)
        {
            try
            {
                var sql = ("Insert into [IdentityToDo] ( [UserId], [ToDoId]) VALUES(@UserId,@ToDoId)");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    int rowsAffected = connection.Execute(sql, new { @UserId=entity.UserId,@ToDoId=entity.ToDoId });
                    connection.Close();
                }


                var sql2 = ("Insert into [Notification] ( [UserNotId], [Title],[StatusOfNotification]) VALUES(@UserId,@Title,@Status)");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    int rowsAffected = connection.Execute(sql2, new { @UserId = entity.UserId, @Title = "New notification!", @Status=false });
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

        public IdentityToDoAdd Get(string id)
        {
            throw new NotImplementedException();
        }

        public List<IdentityToDoAdd> GetAll()
        {
            var sql = ("SELECT * FROM [IdentityToDo]");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var identity = connection.Query(sql).ToList();
                connection.Close();
                return _mapper.Map<List<IdentityToDoAdd>>(identity);
            }
        }

        public bool Update(IdentityToDoAdd entity)
        {
            throw new NotImplementedException();
        }

     
    }
}
