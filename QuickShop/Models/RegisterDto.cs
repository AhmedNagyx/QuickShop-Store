using System.ComponentModel.DataAnnotations;

namespace QuickShop.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First Name field is required"), MaxLength(20)]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Last Name field is required"), MaxLength(20)]
        public string LastName { get; set; } = "";
        [Required(ErrorMessage = "Email field is required"), EmailAddress, MaxLength(50)]
        public string EmailAddress { get; set; } = "";
        [Phone(ErrorMessage = "Phone Number is not valid"), MaxLength(12)]
        public string? PhoneNumber { get; set; }
        [Required, MaxLength(100)]
        public string Address { get; set; } = "";
        [Required, MaxLength(20)]
        public string Password { get; set; } = "";
        [Required(ErrorMessage ="Confirmation of password is required")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = "";

    }
}
