using System;
using CursorShops.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CursorShops.Infrastructure;

namespace CursorShops.Controllers
{
    public class WorkPlaceController : Controller
    {
        UserManager<ShopUser> _usermanager;
        public WorkPlaceController(UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Electro()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }

        public IActionResult CheckList()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
        public IActionResult Foto()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
        public IActionResult Convers()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            ViewBag.LocalServerAddress = Startup.Configuration["RootWebSericeAddress"];
            return View(user);
        }
    }
}
