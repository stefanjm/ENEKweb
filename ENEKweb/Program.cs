using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ENEKweb.Areas.Admin.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using IdentityData;
using IdentityData.Models;

namespace ENEKweb {
    public class Program {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // Initialize the database, adding default admin user
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    var context = services.GetRequiredService<IdentityDataDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    DbInitializer.SeedUsers(context, userManager, roleManager);
                }
                catch (Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        /// <summary>
        /// Run specified startup configuration ( Development, Production, Default ) and specify port to run on
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

            // For this one use 
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup(assemblyName)
                .UseUrls("http://localhost:5001/");
        }
    }
}
