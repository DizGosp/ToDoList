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
    public class ToDoRepository : IToDo
    {

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ToDoRepository(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public bool Add(ToDo entity)
        {
           

            try
            {
                var sql = ("Insert into [ToDo] ( [Title], [Description], [IsCompleted], [CreatedBy], [CreatedOn], [ModifiedOn]) VALUES(@Title,@Description,@IsCompleted,@CreatedBy,@CreatedOn,@ModifiedOn)");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    int rowsAffected = connection.Execute(sql, entity);
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
            try
            {
                var sql = ("Delete [ToDo] where ToDoId=@toDoId");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.Execute(sql, new { @toDoId = id });
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        public ToDo Get(string id)
        {
            var sql = ("Select* from [ToDo] where ToDoId=@toDoId");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var result = connection.QueryFirstOrDefault(sql, new { @toDoId = id });
                return _mapper.Map<ToDo>(result);
            }
        }

        public List<ToDo> GetAll()
        {
            var sql = ("SELECT [ToDoId], [Title], [Description], [CreatedBy], [CreatedOn], [ModifiedOn],[IsCompleted] FROM [ToDo]");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var toDoo = connection.Query(sql).ToList();
                connection.Close(); 
                return _mapper.Map<List<ToDo>>(toDoo);
            }
        }

        public List<ToDo> getByUser(string user)
        {
            var sql = ("SELECT * FROM [ToDo] where CreatedBy=@userId");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var result = connection.Query(sql, new { @userId = user });
                connection.Close();
                return _mapper.Map<List<ToDo>>(result);
            }
        }
        public bool Update(ToDo entity)
        {
            try
            {
                var sql = ("UPDATE [ToDo] SET IsCompleted=@status, Title=@Title, Description=@Description, ModifiedOn=@ModifiedOn WHERE ToDoId=@toDoId");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.Execute(sql, new { @status = entity.IsCompleted, @toDoId = entity.ToDoId, @Title=entity.Title, @Description=entity.Description, @ModifiedOn=entity.ModifiedOn });
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateStatus(ToDo entity)
        {

            
            try
            {
                var sql = ("UPDATE [ToDo] SET IsCompleted=@status WHERE ToDoId=@toDoId");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.Execute(sql, new { @status = entity.IsCompleted, @toDoId=entity.ToDoId });
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
