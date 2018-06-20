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
    public class JournalController : Controller
    {
        UserManager<ShopUser> _usermanager;
        public JournalController(UserManager<ShopUser> userMgr)
        {
            _usermanager = userMgr;
        }
        public IActionResult Index()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
        public IActionResult Period()
        {
            ShopUser user = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result;
            return View(user);
        }
        public IActionResult AddJournal()
        {
            JournalModel model = new JournalModel();
            model.CurrentUserName = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.UserName;
            model.JournalDate = DateTime.Now;
            return View("AddJournal", model);
        }

        public IActionResult EditJournal(string id)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", id);
            pars.Add("Action","GetJournalItem");
            string CurrentUserName = CursorShops.Controllers.AccountController.GetCurrentUser(_usermanager, this.User.Identity.Name, HttpContext).Result.UserName;
            pars.Add("CurrentUserName", CurrentUserName);
            JournalModel journal_item = ObjectJsonConverter.ConvertFromJson<JournalModel>(RunWebServices.RunWebServise("EsMVCWebJournal.asmx/RunJournalAction",
                HttpContext, MethodType.POST, pars).Result);
            journal_item.CurrentUserName = CurrentUserName;
            return View("EditJournal", journal_item);
        }

        [HttpPost]
        public IActionResult AddJournal(JournalModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", "");
            pars.Add("CurrentUserName", model.CurrentUserName);
            pars.Add("JournalDate", model.JournalDate.ToString());
            pars.Add("Destination", model.Destination);
            pars.Add("Purpose", model.Purpose);
            pars.Add("Transport", model.Transport);
            pars.Add("Action", "AddJournal");
            string t = RunWebServices.RunWebServise("EsMVCWebJournal.asmx/RunJournalAction", HttpContext, MethodType.POST, pars).Result;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditJournal(JournalModel model)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("ID", model.ID);
            pars.Add("CurrentUserName", model.CurrentUserName);
            pars.Add("JournalDate", model.JournalDate.ToString());
            pars.Add("Destination", model.Destination);
            pars.Add("Purpose", model.Purpose);
            pars.Add("Transport", model.Transport);
            pars.Add("Action", "UpdateRecord");
            string t = RunWebServices.RunWebServise("EsMVCWebJournal.asmx/RunJournalAction", HttpContext, MethodType.POST, pars).Result;
            return RedirectToAction("Index");
        }
    }
}
