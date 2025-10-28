using System.ComponentModel.DataAnnotations;

namespace QuickShop.Models
{
    public class LoginDto
    {
        [Required]
        public string EmailAddress { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }
}
