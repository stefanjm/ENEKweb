using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ENEKweb.Areas.Admin.Data {

    /// <summary>
    /// Initialize database on every first run. Adding the default admin user
    /// </summary>
    public static class DbInitializer {



        /// <summary>
        /// Add the default admin user
        /// </summary>
        /// <param name="userManager"></param>
        public static void SeedUsers(IdentityDataDbContext _context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) {

            string adminEmail = configuration.GetSection("AdminAccount").GetSection("email").Value;
            string adminPassword = configuration.GetSection("AdminAccount").GetSection("password").Value;

            if(adminEmail == null || adminEmail == string.Empty)
            {
                return;
            }

            // Make sure database is created
            _context.Database.EnsureCreated();

            // Add the role to database
            if (!roleManager.RoleExistsAsync("admin").Result) {
                IdentityRole role = new IdentityRole {
                    Name = "admin"
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            // Add the admin user to database
            if (userManager.FindByEmailAsync(adminEmail).Result == null) {
                ApplicationUser user = new ApplicationUser {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                IdentityResult result = userManager.CreateAsync(user, adminPassword).Result;

                if (result.Succeeded) {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }
        }
    }
}

