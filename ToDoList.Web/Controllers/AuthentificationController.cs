using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Interfaces.Interfaces;
using ToDoList.Model.Models;
using ToDoList.Web.Helper;

namespace ToDoList.Web.Controllers
{
    public class AuthentificationController : Controller
    {
        private readonly IIdentity _identity;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public AuthentificationController(IIdentity identity, IConfiguration configuration, AutoMapper.IMapper mapper)
        {
            _identity = identity;
            _configuration = configuration;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View(new Login() { });
        }

        public IActionResult Login(Login input)
        {
            var user = _identity.GetByUserName(input.UserName);

            if (user == null)
            {
                TempData["error_mess"] = "Korisnik sa username: "+input.UserName+", ne postoji!";
                return RedirectToAction("Index","Authentification");
            }
            else
            {
                var Hash = GenerateHash(user.PasswordSalt, input.Password);
                if (Hash != user.PasswordHash) 
                {
                    TempData["error_mess"] = "Šifra koju ste unjeli nije ispravna!";
                    return RedirectToAction("Index","Authentification");
                }
            }

            HttpContext.SetLogiraniKorisnik(_mapper.Map<Login>(input), (Microsoft.Extensions.Configuration.IConfiguration)_configuration, _mapper);


            return RedirectToAction("Index", "Home");
        }


        public IActionResult Registration() 
        {

            return View();
        }

        public IActionResult SaveRegistration(Registration input) 
        {
            var listUsers = _identity.GetAll();

            foreach (var item in listUsers)
            {
                if (item.UserName == input.UserName) 
                {
                    TempData["error_mess"] = "Korisnik sa username: " + input.UserName + ", vec postoji!";
                    return RedirectToAction("Registration", "Authentification");
                }
            }

            Identity saveUser = new Identity()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                UserName = input.UserName,
                CreatedOn=DateTime.Now
            };
                saveUser.PasswordSalt = GenerateSalt();
                saveUser.PasswordHash = GenerateHash(saveUser.PasswordSalt, input.Password);


            bool saved= _identity.Add(saveUser);

            if (saved) 
            {
                return RedirectToAction("Index", "Authentification");
            }
            else 
          
                TempData["error_mess"] = "Korisnik nije spremljen, pokušajte ponovo!";
                return RedirectToAction("Registration", "Authentification");
           
        }

        public static string GenerateSalt()
        {
            var buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public static string GenerateHash(string salt, string password)
        {
            byte[] src = Convert.FromBase64String(salt);
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] dst = new byte[src.Length + bytes.Length];

            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);

            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        public IActionResult LogOut() 
        {
                return RedirectToAction("Index");  
        }

    }
}
