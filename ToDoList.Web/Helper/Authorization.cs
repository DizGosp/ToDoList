using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;

namespace ToDoList.Web.Helper
{
    public class AuthorizationAttribute : TypeFilterAttribute
    {

        public AuthorizationAttribute(bool user)
              : base(typeof(MyAuthorizeImpl))
        {
            Arguments = new object[] { user };
        }
    }


    public class MyAuthorizeImpl : IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public MyAuthorizeImpl(bool user, IConfiguration configuration, IMapper mapper)
        {
            _user = user;
            _configuration = configuration;
            _mapper = mapper;
        }
        private readonly bool _user;
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            Login userLoged = filterContext.HttpContext.GetLogiraniKorisnik(_configuration,_mapper);
            if (userLoged==null)
            {
                if (filterContext.Controller is Controller controller)
                {
                    controller.TempData["error_poruka"] = "Niste logirani!";
                }

                filterContext.Result = new RedirectToActionResult("Index", "Authentification", new { @area = "" });
                return;
            }


            //Korisnici mogu pristupiti
            if (_user)
            {
                await next(); //ok - ima pravo pristupa
                return;
            }


            if (filterContext.Controller is Controller c1)
            {
                c1.ViewData["error_poruka"] = "Nemate pravo pristupa!";
            }
            filterContext.Result = new RedirectToActionResult("Index", "Authentification", new { @area = "" });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // throw new NotImplementedException();
        }


    }
}
