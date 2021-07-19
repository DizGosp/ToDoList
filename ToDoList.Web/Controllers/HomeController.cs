
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DB.Repositories;
using ToDoList.Interfaces;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;
using ToDoList.Web.Helper;
using ToDoList.Web.Models;


namespace ToDoList.Web.Controllers
{
    [Authorization(true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIdentity _identity;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IToDo _toDo;
        private readonly INotification _notif;
        private readonly IIdentityToDo _idToDo;
        public HomeController(ILogger<HomeController> logger, IIdentity identity, IConfiguration configuration, IMapper mapper, IToDo toDo, IIdentityToDo idToDo, INotification notification)
        {
            _logger = logger;
            _identity = identity;
            _configuration = configuration;
            _mapper = mapper;
            _toDo = toDo;
            _idToDo = idToDo;
            _notif = notification;
        }
        public PartialViewResult SearchUsers(string searchText, DateTime dateSearch, DateTime dateSearchTo, string StatusSearch)
        {
            var result = _toDo.GetAll();
            var toDoList = _toDo.GetAll();

            foreach (var item in toDoList)
            {
                var user1 = _identity.Get(item.CreatedBy);
                item.CreatedBy = user1.UserName;
            }
            foreach (var item in result)
            {
                var user1 = _identity.Get(item.CreatedBy);
                item.CreatedBy = user1.UserName;
            }

            if (searchText == null && dateSearch.Year == 1 && dateSearchTo.Year == 1 && StatusSearch == null)
            {
                return PartialView("_Grid", toDoList);
            }

            if (searchText != null)
            {
                var filt = result.Where(a => a.Title.ToLower().Contains(searchText.ToLower()) || a.Description.ToLower().Contains(searchText.ToLower()));
                result = _mapper.Map<List<ToDo>>(filt);

            }

            if (dateSearch.Year != 1)
            {
                var filt = result.Where(a => a.CreatedOn.Date >= dateSearch.Date);
                result = _mapper.Map<List<ToDo>>(filt);

            }

            if (dateSearchTo.Year != 1)
            {
                var filt = result.Where(a => a.CreatedOn.Date <= dateSearchTo.Date);
                result = _mapper.Map<List<ToDo>>(filt);

            }

            if (StatusSearch != null)
            {
                if (StatusSearch == "Completed")
                {
                    var filt = result.Where(a => a.IsCompleted == true);
                    result = _mapper.Map<List<ToDo>>(filt);

                }
                if (StatusSearch == "In progress")
                {
                    var filt = result.Where(a => a.IsCompleted == false);
                    result = _mapper.Map<List<ToDo>>(filt);

                }
                if (StatusSearch == "" && result.Any())
                {
                    return PartialView("_Grid", result);
                }

            }


            return PartialView("_Grid", result);


        }
        public IActionResult Index()
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);

            if (token.UserName != null)
            {
                var user = _identity.GetByUserName(token.UserName);
                var notif=_notif.Get(user.UserId);
                return View(notif);
            }

            return RedirectToAction("Index", "Authentification");

        }
        public IActionResult AddToDo()
        {

            return View();
        }
        public IActionResult SaveToDo(string title, string desc)
        {
            ToDo toDo = new ToDo();

            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var userLoged = _identity.GetByUserName(token.UserName);
            toDo.Title = title;
            toDo.Description = desc;
            toDo.CreatedBy = userLoged.UserId;
            toDo.CreatedOn = DateTime.Now;
            toDo.ModifiedOn = DateTime.Now;
            toDo.IsCompleted = false;
            bool saved = _toDo.Add(toDo);

            if (saved)
            {
                TempData["error_mess"] = "Uspjesno izvrsena operacija!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error_mess"] = "Dodavanje nije uspjelo, molimo pokusajte ponovo!";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult EditUser()
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var user = _identity.GetByUserName(token.UserName);
            return View(user);
        }
        public IActionResult UpdateUser(Identity entity)
        {
            if (_identity.Update(entity))
            {
                TempData["succ_mess"] = "Operacija nije izvrsena, pokusajte ponovo!";
                return RedirectToAction("Index", "Home");
            }

            TempData["succ_mess"] = "Uspjesno izvrsena operacija!";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Edit()
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var user = _identity.GetByUserName(token.UserName);
            var toDoList = _toDo.getByUser(user.UserId);

            return View(toDoList);
        }
        public IActionResult EditTask(string id)
        {

            var toDoList = _toDo.Get(id);

            return View(toDoList);
        }
        public IActionResult EditTaskSave(ToDo entity)
        {

            entity.ModifiedOn = DateTime.Now;
            if (_toDo.Update(entity))
            {
                TempData["error_mess"] = "Operacija nije izvrsena, pokusajte ponovo!";
                return RedirectToAction("Index", "Home");
            }

            TempData["error_mess"] = "Uspjesno izvrsena operacija!";
            return RedirectToAction("Index", "Home");

        }
        public IActionResult EditStatus(string id)
        {
            var edit = _toDo.Get(id);
            edit.IsCompleted = true;

            if (_toDo.UpdateStatus(edit))
            {
                TempData["error_mess"] = "Uspjesno izvrsena operacija!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error_mess"] = "Brisanje nije uspjelo, molimo pokušajte ponovo!";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Remove()
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var user = _identity.GetByUserName(token.UserName);
            var toDoList = _toDo.getByUser(user.UserId);

            return View(toDoList);
        }
        public IActionResult RemoveTask(string id)
        {

            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var user = _identity.GetByUserName(token.UserName);
            var toDoList = _toDo.getByUser(user.UserId);
            bool postoji = false;
            foreach (var item in toDoList)
            {
                if (item.ToDoId == id)
                {
                    postoji = true;
                }

            }
            if (postoji)
            {
                if (_toDo.Delete(id))
                {
                    TempData["error_mess"] = "Operacija uspjesno izvrsena!";
                }
                else
                {
                    TempData["error_mess"] = "Operacija nije izvrsena, molimo pokusajte ponovo!";
                }
            }
            else
            {
                TempData["error_mess"] = "Operacija nije moguce izvrsiti, task je kreiran od strane drugog korisnika!";
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Assign(string id)
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);
            var user = _identity.GetByUserName(token.UserName);
            IdentityToDo result = new IdentityToDo()
            {
                ToDoId = id,
                users = new List<IdentityToDo.Rows>(),
                Assignedusers = new List<IdentityToDo.Rows>()
            };
            var users = _identity.GetAll();

            foreach (var item in users)
            {
                if (item.UserId != user.UserId)
                {
                    result.users.Add(new IdentityToDo.Rows
                    {
                        UserId = item.UserId,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        UserName = item.UserName
                    });
                }

            }

            var AssignedList = _idToDo.GetAll();

            foreach (var item in AssignedList)
            {
                foreach (var x in users)
                {
                    if(item.ToDoId==id && item.UserId == x.UserId) 
                    {
                        result.Assignedusers.Add(new IdentityToDo.Rows
                        {
                            UserId = x.UserId,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            UserName = x.UserName
                        });
                    }
                }
            }


            return View(result);
        }
        public IActionResult AssignedTo(string id, string uId)
        {
            IdentityToDoAdd x = new IdentityToDoAdd()
            {
                ToDoId = id,
                UserId = uId
            };
            var taskAssigned = _idToDo.GetAll();
            bool postoji = false;
            foreach (var item in taskAssigned)
            {
                if(item.ToDoId==id && item.UserId == uId) 
                {
                    postoji = true;
                }
            }

            if (postoji == false) 
            {
                if (_idToDo.Add(x))
                {
                    TempData["error_mess"] = "Operacija uspjesno izvrsena!";
                }
                else
                {
                    TempData["error_mess"] = "Operacija nije izvrsena, molimo pokusajte ponovo!";
                }
            }
            else 
            {
                TempData["error_mess"] = "Operacija nije izvrsena, korisniku je vec dodjeljen isti task!";
            }
            

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Notification()
        {
            var token = HttpContext.GetLogiraniKorisnik((Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);

            var user = _identity.GetByUserName(token.UserName);

            var notifications = _notif.Get(user.UserId);

            _notif.Update(user.UserId);

            return View(notifications);
        }
    }


}

