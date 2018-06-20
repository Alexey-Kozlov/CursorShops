using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CursorShops.Models;
using CursorShops.Infrastructure;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace CursorShops.Controllers
{
    [Authorize]
    public class HotLineController : Controller
    {
        UserManager<ShopUser> _usermanager;
        IMyDI myDI;
        public HotLineController(IMyDI _myDI, UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;
            myDI = _myDI;
        }
        public IActionResult TasksFromMe()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            string lll = myDI.MyProp;


            return View(user);
        }


        public IActionResult ArhiveTasks()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }

        public IActionResult AddTask()
        {            
            HotLineModel model = new HotLineModel();
            model.CurrentUserName = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.UserName;
            model.CurrentUserDep = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.Department;
            Dictionary<string, string> pars = new Dictionary<string, string>();
            //заполняем справлчник отделов
            model.JsonDepartments = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineDepartments", HttpContext, MethodType.POST, pars).Result;
            //заполняем справлчник пользователей
            model.JsonUsers = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineUsers", HttpContext, MethodType.POST, pars).Result;
            return View("AddTask",model);
        }

        [HttpPost]
        public IActionResult AddTask(HotLineModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("AttachedFiles", model.JsonAttachedFiles);
            pars.Add("Body", model.Body);
            pars.Add("CurrentUserName", model.CurrentUserName);
            pars.Add("ListID", "172C14E8-0575-4D76-8BE6-1D37EA23B159");
            pars.Add("WebID", "45055F6F-32DC-43C6-B4AB-019EC5D2120A");
            pars.Add("SiteID", "06CE8DFE-0C98-47A0-A67F-FFD25D831BF1");
            pars.Add("RespName", model.RespName);
            pars.Add("RespDepId", model.SelDep);
            pars.Add("TaskAction", "NewTask");
            pars.Add("ID", "");
            pars.Add("Message", "");
            string t = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/RunHotLineAction", HttpContext, MethodType.POST, pars).Result;
            return RedirectToAction("TasksFromMe");
        }

        [HttpPost]
        public IActionResult GetHotLine(HotLineModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", model.ID);
            pars.Add("ListID",model.ListID);
            pars.Add("SiteID",model.SiteID);
            pars.Add("UserName", model.CurrentUserName);
            pars.Add("WebID", model.WebID);
            HotLineModel new_task = ObjectJsonConverter.ConvertFromJson<HotLineModel>(RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLine",
                HttpContext, MethodType.POST, pars).Result);
            new_task.CurrentUserName = model.CurrentUserName;
            new_task.CurrentUserDep = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.Department;
            //заполняем справлчник отделов
            pars.Clear();
            new_task.JsonDepartments = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineDepartments", HttpContext, MethodType.POST, pars).Result;
            //заполняем справочник пользователей
            new_task.JsonUsers = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineUsers", HttpContext, MethodType.POST, pars).Result;
            return View("Edit",new_task);
        }

        [HttpPost]
        public IActionResult Edit(HotLineModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("AttachedFiles", model.JsonAttachedFiles);
            pars.Add("Body", model.Body);
            pars.Add("CurrentUserName", model.CurrentUserName);
            pars.Add("ListID", model.ListID);
            pars.Add("WebID", model.WebID);
            pars.Add("SiteID", model.SiteID);
            pars.Add("RespName", model.RespName);
            pars.Add("RespDepId", model.SelDep);
            pars.Add("TaskAction", model.TaskAction);
            pars.Add("ID", model.ID);
            if (model.Message == "<div>​</div>")
                model.Message = "";
            pars.Add("Message", model.Message);
            string t = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/RunHotLineAction", HttpContext, MethodType.POST, pars).Result;
            return RedirectToAction("TasksFromMe");
        }

        public IActionResult PrintTask(HotLineModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", model.ID);
            pars.Add("ListID", model.ListID);
            pars.Add("SiteID", model.SiteID);
            pars.Add("UserName", model.CurrentUserName);
            pars.Add("WebID", model.WebID);
            HotLineModel new_task = ObjectJsonConverter.ConvertFromJson<HotLineModel>(RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLine",
                HttpContext, MethodType.POST, pars).Result);
            new_task.CurrentUserName = model.CurrentUserName;
            new_task.CurrentUserDep = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.Department;
            //заполняем справлчник отделов
            pars.Clear();
            new_task.JsonDepartments = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineDepartments", HttpContext, MethodType.POST, pars).Result;
            //заполняем справочник пользователей
            new_task.JsonUsers = RunWebServices.RunWebServise("EsMVCWebHotLine.asmx/GetHotLineUsers", HttpContext, MethodType.POST, pars).Result;
            return View(new_task);
        }
    }
}
