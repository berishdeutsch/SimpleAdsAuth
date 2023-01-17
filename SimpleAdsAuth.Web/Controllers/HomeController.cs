using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdsAuth;Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            var vm = new IndexViewModel
            {
                Ads = repo.GetAllAds()
            };
            if (User.Identity.IsAuthenticated)
            {
                vm.User = repo.GetUserByEmail(User.Identity.Name);
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            var user = repo.GetUserByEmail(User.Identity.Name);
            ad.UserId = user.Id;
            repo.AddAd(ad);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            var user = repo.GetUserByEmail(User.Identity.Name);
            var vm = new MyAccountViewModel
            {
                Ads = repo.GetAdsByUserId(user.Id)
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var repo = new SimpleAdsAuteRepository(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
