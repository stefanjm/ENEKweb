using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENEKweb.Areas.Admin.Models.Identity;
using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ENEKweb.Areas.Admin.Controllers.Identity {
    [Area("Admin")]
    public class IdentityController : Controller {
        /// <summary>
        /// Scoped service
        /// </summary>
        private readonly IApplicationUser _applicationUser;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// The manager for handling signing users in and out
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;


        /// <summary>
        /// Store Error messages
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationuser"></param>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public IdentityController(IApplicationUser applicationuser, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) {
            _applicationUser = applicationuser;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Landing page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// Temporary create an account
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Create() {

            if(await _userManager.FindByEmailAsync("stefan@andromatech.com") == null) {
                await _userManager.CreateAsync(new ApplicationUser {
                    UserName = "stefan@andromatech.com",
                    Email = "stefan@andromatech.com"
                }, "password");
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        /// <summary>
        /// Logout user and return to Home webpage
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns>Redirects to Home action, root</returns>
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            //if (returnUrl != null) {
            //    return LocalRedirect("/Base/Playground/Index");
            //}
            //else {
            //    return RedirectToAction("Index", "Home");
            //}
            return RedirectToAction("Index", "Home", new { area = "" });
        }



        /// <summary>
        /// Login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null) {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        /// <summary>
        /// Login request form. Check if user credentials match and return to the dashboard if so.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email", "Password", "RememberMe")] LoginModel loginModel, string returnUrl = null) {
            // If no url specified, return to dashboard
            returnUrl = returnUrl ?? Url.Content(Url.Action("Index", "Dashboard"));

            if (ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded) {
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor) {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginModel.RememberMe });
                }
                if (result.IsLockedOut) {
                    return RedirectToAction("Lockout");
                }
                else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(loginModel);
        }


        public IActionResult LoginWith2fa() {
            throw new NotImplementedException();
        }

        public IActionResult Lockout() {
            throw new NotImplementedException();

        }

        public IActionResult ForgotPassword() {
            throw new NotImplementedException();
        }

    }
}