using System;
using System.ComponentModel.DataAnnotations;

namespace CursorShops.Models
{
    public class JournalModel
    {
        public string ID { get; set; }
        [Display(Name = "Дата и время убытия")]
        [Required(ErrorMessage = "Необходимо указать дату и время убытия!")]
        public DateTime JournalDate { get; set; }
        [Display(Name = "Сотрудник")]
        public string CurrentUserName { get; set; }
        [Display(Name = "Пункт назначения")]
        public string Destination { get; set; }
        [Display(Name = "Цель поездки")]
        [Required(ErrorMessage = "Необходимо заполнить поле цели поездки!")]
        public string Purpose { get; set; }
        [Display(Name = "Транспорт")]
        public string Transport { get; set; }
        public string Created { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
        public string UserClosed { get; set; }
    }
}
