using Microsoft.AspNetCore.Identity;
using QuickShop.Models;

namespace QuickShop.Services
{
    public class DatabaseIntializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }
            //check admin role
            var exists = await roleManager.RoleExistsAsync("Admin");
            if (!exists)
            {
                Console.WriteLine("admin not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //check seller role
            exists = await roleManager.RoleExistsAsync("Seller");
            if (!exists)
            {
                Console.WriteLine("seller not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("Seller"));
            }
            //check client role
            exists = await roleManager.RoleExistsAsync("Client");
            if (!exists)
            {
                Console.WriteLine("client not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("Client"));
            }

            //check admin user
            var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
            if (adminUsers.Any())
            {
                Console.WriteLine("admin user exists");
                return;
            }

            //create admin user
            var user = new ApplicationUser()
            {
                FirstName= "Admin",
                LastName = "Admin",
                UserName = "Admin@Admin.com",
                Email= "Admin@Admin.com",
                CreatedAt = DateTime.Now
            };
            string initialPassword = "Admin123";

            var result = await userManager.CreateAsync(user, initialPassword);
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
                Console.WriteLine("admin user created, please update password");
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Initial Password: "+ initialPassword);
            }
        }
    }
}
