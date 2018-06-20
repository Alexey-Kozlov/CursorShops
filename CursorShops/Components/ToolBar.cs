using System;
using CursorShops.Models;
using Microsoft.AspNetCore.Mvc;

namespace CursorShops.Components
{
    public class ToolBar : ViewComponent
    {
        public IViewComponentResult Invoke(ToolBarModel model)
        {
            switch (model.TaskModuleName)
            {
                case "HotLineNewTask":
                    model.AttachFile = true;
                    break;
                case "HotLine":
                    if (model.Status != "Выполнено")
                    {
                        model.CancelTask = true;
                        model.CompleteTask = true;
                        model.AttachFile = true;
                        model.SaveTask = true;
                    }
                    model.PrintTask = true;
                    if(model.Status == "Уточнение" || model.Status == "На подтверждении")
                        model.ReturnTask = true;
                    break;
                case "Task":
                    switch(model.Status)
                    {
                        case "В работе":
                            model.CompleteTask = true;
                            model.QuestionTask = true;
                            model.AttachFile = true;
                            model.SaveTask = true;
                            break;
                        case "Завершена":
                            break;
                        case "Утверждение":
                            model.AttachFile = true;
                            model.SaveTask = true;
                            break;
                    }
                    model.PrintTask = true;
                    break;
                case "Journal":
                    switch(model.Status)
                    {
                        case "Открыта":
                            model.SaveTask = true;
                            break;
                    }
                    break;
            }          
            return View("ToolBarView",model);
        }
    }
}
