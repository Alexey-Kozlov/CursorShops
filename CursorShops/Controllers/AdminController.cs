using Microsoft.AspNetCore.Mvc;
using CursorShops.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CursorShops.Controllers
{
    [Authorize(Roles ="Admins")]
    public class AdminController : Controller
    {
        private UserManager<ShopUser> userManager;
        private IPasswordHasher<ShopUser> passwordHasher;
        public AdminController(UserManager<ShopUser> usrMgr, IPasswordHasher<ShopUser> passwordHash)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
        }
        public ViewResult Index() => View(userManager.Users);
        public ViewResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser model)
        {
            if (ModelState.IsValid)
            {
                ShopUser user = new ShopUser { UserName = model.Name, Email = model.EMail,Department = model.Department };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ShopUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string username, string email, string password, string Department)
        {
            ShopUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                user.UserName = username;
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                user.Department = Department;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }

            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ShopUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }
            return View("Index", userManager.Users);
        }
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
