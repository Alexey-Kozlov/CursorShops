using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CursorShops.Models;
using Microsoft.AspNetCore.Identity;
using CursorShops.Infrastructure;
using System.Collections.Generic;

namespace CursorShops.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ShopUser> userManager;
        private SignInManager<ShopUser> signInManager;
        public AccountController(UserManager<ShopUser> userMgr, SignInManager<ShopUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ShopUser user = await userManager.FindByEmailAsync(details.EMail);
                if (user != null)
                {
                    HttpContext.Session.SetJson("User", user);
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.EMail),"Логин или пароль не найдены");
            }
            return View(details);
        }

        [NonAction]
        public static async Task<ShopUser> GetCurrentUser(UserManager<ShopUser> _usermanager,string UserName,Microsoft.AspNetCore.Http.HttpContext context)
        {
            ShopUser user = context.Session.GetJson<ShopUser>("User");
            if (user == null)
            {
                user = await _usermanager.FindByNameAsync(UserName);
                Dictionary<string, string> pars = new Dictionary<string, string>();
                pars.Add("UserName", user.UserName);
                user.Department = RunWebServices.RunWebServise("EsMVCWebUtils.asmx/GetUserDepartment", context, MethodType.POST, pars).Result;
                context.Session.SetJson("User", user);
            }
            return user;
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Main");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
