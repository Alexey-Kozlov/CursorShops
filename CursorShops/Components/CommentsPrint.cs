using System;
using System.Collections.Generic;
using CursorShops.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using CursorShops.Models;
using System.Threading.Tasks;


namespace CursorShops.Components
{
    public class CommentsPrint : ViewComponent
    {
        public IViewComponentResult Invoke(ICommentModel task)
        {
            CommentModel model = (CommentModel)task;
            IEnumerable<CommentModel> CommentsList = GetComments(model);
            return View("CommentView",CommentsList);
        }

        private IEnumerable<CommentModel> GetComments(CommentModel task)
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("SiteID", task.SiteID);
            pars.Add("WebID", task.WebID);
            pars.Add("TaskListID", task.ListID);
            pars.Add("TaskID", task.ID);
            IEnumerable<CommentModel> rez = CursorShops.Infrastructure.ObjectJsonConverter.ConvertFromJson<IEnumerable<CommentModel>>(RunWebServices.RunWebServise("EsMVCWebTasks.asmx/GetComments",
                HttpContext, MethodType.POST, pars).Result);
            return rez;

        }
    }
}
