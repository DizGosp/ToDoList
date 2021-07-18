using AutoMapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Interfaces;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;

namespace ToDoList.DB.Repositories
{
    class NotificationRepository : INotification
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public NotificationRepository(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public List<Notification> Get(string id)
        {
            var sql = ("Select [Title],[UserNotId], [StatusOfNotification] from [Notification] where UserNotId=@user");
            using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
            {
                connection.Open();
                var result = connection.Query(sql,new {@user=id,@status=false }).ToList();
                return _mapper.Map<List<Notification>>(result);
            }
        }


        public bool Update(string id)
        {
            try
            {
                var sql = ("UPDATE [Notification] SET StatusOfNotification=@status WHERE UserNotId=@userId");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("db")))
                {
                    connection.Open();
                    var result = connection.Execute(sql, new {  @userId = id, @status=true });
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
