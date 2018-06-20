using System;
using CursorShops.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace CursorShops.Controllers
{
    public class ConferenceController : Controller
    {

        UserManager<ShopUser> _usermanager;

        public ConferenceController(UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;
        }

        public IActionResult Index()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }

        //обработка вызовов конференций перехватывается MiddleWare классом ProxyServerMiddleware

        //записи конференций
        public IActionResult Records()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            ViewBag.host = Request.Scheme + "://" + HttpContext.Request.Host.Value;
            return View(user);
        }

    }
}
