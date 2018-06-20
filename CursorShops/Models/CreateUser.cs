using System.ComponentModel.DataAnnotations;

namespace CursorShops.Models
{
    public class CreateUser
    {
        [Required]
        [Display(Name ="Наименование")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string EMail { get; set; }
        [Required]
        [Display(Name = "Подразделение")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
