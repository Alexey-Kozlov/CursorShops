using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CursorShops.Models
{
    public class HotLineModel : CommentModel
    {
        [Required(ErrorMessage ="Необходимо заполнить поле темы заявки!")]
        [Display(Name = "Тема")]
        public string Body { get; set; }
        [Display(Name = "Номер задания")]
        public string TaskID { get; set; }
        [Display(Name = "Начало задания")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Планируемая дата завершения")]
        public DateTime DueDate { get; set; }
        [Display(Name = "Состояние задания")]
        public string Status { get; set; }
        [Display(Name = "Исполнитель задания")]
        public string RespName { get; set; }
        public string DepName { get; set; }
        public string UrlLink { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentUserDep { get; set; }
        public string Message { get; set; }
        public string TaskAction { get; set; }
        public string JsonAttachedFiles {get; set;}
        [Display(Name = "Отдел исполнения")]
        public string JsonDepartments { get; set; }
        [Display(Name = "Исполнитель")]
        public string JsonUsers { get; set; }
        public string SelDep { get; set; }

    }
}




