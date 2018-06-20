using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CursorShops.Models;
using System.Threading.Tasks;
using CursorShops.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace CursorShops.Controllers
{    
    [Authorize]
    public class TasksController : Controller
    {
        UserManager<ShopUser> _usermanager;
        public TasksController(UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;
        }
        public IActionResult Index()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }

        public IActionResult CompletedTasks()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
        public IActionResult ExecutedTasks()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }

        [HttpPost]
        public IActionResult GetTask(TaskModel task)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", task.ID);
            pars.Add("ListID", task.ListID);
            pars.Add("SiteID", task.SiteID);
            pars.Add("WebID", task.WebID);
            TaskModel new_task = ObjectJsonConverter.ConvertFromJson<TaskModel>(RunWebServices.RunWebServise("EsMVCWebTasks.asmx/GetTask",
                HttpContext, MethodType.POST, pars).Result);
            new_task.CurrentUserName = task.CurrentUserName;
            return View("Edit", new_task);

        }
        
        [HttpPost]
        public IActionResult Edit(TaskModel task)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", task.ID);
            pars.Add("ListID", task.ListID);
            pars.Add("SiteID", task.SiteID);
            pars.Add("WebID", task.WebID);
            if (task.Message == "<div>​</div>")
                task.Message = "";
            pars.Add("Message", task.Message);
            pars.Add("UserName", task.CurrentUserName);
            pars.Add("TaskAction", task.TaskAction);
            string t = RunWebServices.RunWebServise("EsMVCWebTasks.asmx/RunTaskAction", HttpContext, MethodType.POST, pars).Result;
            return RedirectToAction("Index");
        }

        public IActionResult PrintTask(TaskModel task)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", task.ID);
            pars.Add("ListID", task.ListID);
            pars.Add("SiteID", task.SiteID);
            pars.Add("WebID", task.WebID);
            TaskModel new_task = ObjectJsonConverter.ConvertFromJson<TaskModel>(RunWebServices.RunWebServise("EsMVCWebTasks.asmx/GetTask",
                HttpContext, MethodType.POST, pars).Result);
            return View(new_task);
        }
    }
}
