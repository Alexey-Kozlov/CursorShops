using System.ComponentModel.DataAnnotations;

namespace CursorShops.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name ="Логин")]
        [UIHint("login")]
        public string EMail { get; set; }
        [Required]
        [UIHint("password")]
        [Display(Name ="Пароль")]
        public string Password { get; set; }
    }
}
