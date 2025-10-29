using System.ComponentModel.DataAnnotations;

namespace QuickShop.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "New password is required"), MaxLength(20)]
        public string NewPassword { get; set; } = "";
        [Required(ErrorMessage = "Current password is required"), MaxLength(20)]
        public string CurrentPassword { get; set; } = "";
        [Required(ErrorMessage = "Confirmation of password is required")]
        [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
