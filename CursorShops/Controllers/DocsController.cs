using System;
using CursorShops.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


namespace CursorShops.Controllers
{
    public class DocsController : Controller
    {
        UserManager<ShopUser> _usermanager;

        public DocsController(UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;
        }

        public IActionResult Index()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
    }
}
