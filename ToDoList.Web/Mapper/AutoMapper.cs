using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Model.Models;

namespace ToDoList.Web.Mapper
{
    public class AutoMapper:Profile
    {

        public AutoMapper() 
        {
            CreateMap<Identity, Identity>();
            CreateMap<Identity, Login>().ReverseMap();
            CreateMap<AuthToken, AuthToken>();
            CreateMap<ToDo, ToDo>();
            CreateMap<Notification, Notification>();
        }

    }
}
