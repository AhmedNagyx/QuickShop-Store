using System.ComponentModel.DataAnnotations;

namespace QuickShop.Models
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm password field required")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
