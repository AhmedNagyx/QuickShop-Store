using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Services;
using System.ComponentModel.DataAnnotations;

namespace QuickShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            //create new acc and authenticate
            var user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.EmailAddress,
                Email = registerDto.EmailAddress,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Client");
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerDto);
        }

        public async Task<IActionResult> LogOut()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var result = await signInManager.PasswordSignInAsync(loginDto.EmailAddress, loginDto.Password, loginDto.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt";
            }
            return View(loginDto);
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Login");
            }
            var profileDto = new ProfileDto
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                EmailAddress = appUser.Email ?? "",
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,
                
            };
            return View(profileDto);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill the required fields";
                return View(profileDto);
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Login");
            }
            appUser.UserName = profileDto.EmailAddress;
            appUser.FirstName = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.Email = profileDto.EmailAddress;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;
            var result = await userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Profile updated successfully";
                return View(profileDto);
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to update profile: " + result.Errors.First().Description;
            }
            return View(profileDto);
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid input";
                return View(passwordDto);
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index","Home");
            }

            var resut = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if (resut.Succeeded)
            {
                ViewBag.SuccessMessage = "Password changed successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "Error changing password: " + resut.Errors.First().Description;
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index","Home");
        }

        public IActionResult ForgotPassword()
        {
            if(signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required,EmailAddress]string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = email;
            if(!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email";
                return View();
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.EmailError = "No user associated with this email";
                return View();
            }
            else
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string restLink = Url.ActionLink("ResetPassword", "Account" , new { token }) ?? "Url Error";

                //send url by email
                string senderName = configuration["BrevoSettings:SenderName"] ?? "BookSmith";
                string senderEmail = configuration["BrevoSettings:SenderEmail"] ?? "Admin@BookSmith.com";
                string userName = user.FirstName + "" + user.LastName;
                string subject = "Password Reset Request" ?? "";
                string textContent = $"Hello {user.FirstName},\n\n" +
                    $"We received a request to reset your password. Please click the link below to reset your password:\n\n" +
                    $"{restLink}\n\n" +
                    $"If you did not request a password reset, please ignore this email.\n\n" +
                    $"Best regardss,\n" +
                    $"BookSmith Team";
                EmailSender.SendEmail(senderName, senderEmail, userName, email, textContent, subject);
            }

            ViewBag.SuccessMessage = "A password reset link has been sent.";
            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordDto resetPasswordDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(resetPasswordDto);
            }

            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if(user == null) {
                ViewBag.ErrorMessage = "No user associated with this email";
                return View(resetPasswordDto);
            }

            var result = await userManager.ResetPasswordAsync(user, token, resetPasswordDto.Password);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password has been reset successfully.";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Error resetting password: " + result.Errors.First().Description;
            }
            return View(resetPasswordDto);
        }
    }
}
