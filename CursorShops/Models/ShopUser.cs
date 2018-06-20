using Microsoft.AspNetCore.Identity;

namespace CursorShops.Models
{
    public class ShopUser : IdentityUser
    {
        public string Department { get; set; }
    }
}
