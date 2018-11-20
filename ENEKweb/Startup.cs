using ENEKdata;
using ENEKservices;
using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ENEKweb {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add Database contexts, and declare connections
            services.AddDbContext<IdentityDataDbContext>(options
                => {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    options.EnableSensitiveDataLogging(true);
                });

            services.AddDbContext<ENEKdataDbContext>(options
                => {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    options.EnableSensitiveDataLogging(true);
                });

            // AddIdentity adds cookie based Authentication
            // Adds scoped classes for UserManagement, Signing in, Passwords etc...

            services.AddIdentity<ApplicationUser, IdentityRole>()
                // Adds UserStore and RoleStore from this context
                .AddEntityFrameworkStores<IdentityDataDbContext>()
                // Adds a provider that generates unique keys and hashes for 
                //  forgot password links, phone number verification codes etc...
                .AddDefaultTokenProviders();

            // Change Password policy
            services.Configure<IdentityOptions>(options
                => {
                    // Make really weak passwords possible heh
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                });


            // Alter application cookie info
            services.ConfigureApplicationCookie(options => {
                // Login path
                options.LoginPath = "/admin/identity/login";
                options.LogoutPath = "/admin/identity/login";
                options.AccessDeniedPath = "/home/index";

                // Change cookie timeout
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            // Require Authentication For every page by default
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Identity service
            services.AddScoped<IApplicationUser, IdentityService>();

            // Leiunurk service
            services.AddScoped<ILeiunurk, LeiunurkService>();

            // Partnerid service
            services.AddScoped<IPartnerid, PartneridService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            // for Kestrel reverse proxy so redirects and authentication will work right
            // AUTHENTICATION AFTER THIS UseForwardedHeaders
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Setup Identity
            app.UseAuthentication();

            app.UseMvc(routes => {

                routes.MapRoute(
                  name: "areaRoute",
                  template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );



            });
        }
    }
}
