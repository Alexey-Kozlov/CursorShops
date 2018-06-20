using System;
using System.ComponentModel.DataAnnotations;

namespace CursorShops.Models
{
    public class TaskModel : CommentModel
    {
        [Required]
        [Display(Name = "Наименование задания")]
        public string Title { get; set; }
        [Display(Name = "Номер задания")]
        public string TaskID { get; set; }
        [Display(Name = "Наименование проекта")]
        public string ProjectName { get; set; }
        [Display(Name = "Начало задания")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Окончание задания")]
        public DateTime DueDate { get; set; }
        [Display(Name = "Состояние задания")]
        public string Status { get; set; }
        [Display(Name = "Исполнитель задания")]
        public string Resp { get; set; }
        [Display(Name = "Контролирующий")]
        public string ControlPerson { get; set; }
        public string UrlLink { get; set; }
        [Display(Name = "Описание задания")]
        public string Body { get; set; }
        [Display(Name = "При выполнении задания обязательно указывать комментарий!")]
        public bool CommentMustBe { get; set; }
        public string CurrentUserName { get; set; }
        public string Message { get; set; }
        public string TaskAction { get; set; }

    }
}
