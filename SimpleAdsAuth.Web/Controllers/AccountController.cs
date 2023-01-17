using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsAuth;Integrated Security=true;";
        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            var vm = new LoginViewModel();

            if (TempData["message"] != null)
            {
                vm.Message = TempData["message"].ToString();
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login!";
                return RedirectToAction("Login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/Home/NewAd");
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            repo.AddUser(user, password);
            return RedirectToAction("Login");
        }
    }
}
