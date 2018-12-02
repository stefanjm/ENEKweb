using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENEKweb.Areas.Admin.Models.Manage;
using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ENEKweb.Areas.Admin.Controllers.Identity.Manage {
    /// <summary>
    /// Manage User Account
    /// </summary>
    [Area("Admin")]
    public class ManageController : Controller {
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
        /// Store result messages for user to see
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        public ManageController(IApplicationUser applicationuser, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) {
            _applicationUser = applicationuser;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Landing page. Display logged in user basic info if user available.
        /// </summary>
        /// <returns>Return To View with User Model</returns>
        public async Task<IActionResult> Index() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            UserModel inputModel = new UserModel {
                Username = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                IsEmailConfirmed = isEmailConfirmed
            };

           

            return View(inputModel);
        }

        /// <summary>
        /// Make user profile changes. Since EMAIL is TIED to USERNAME, will also change the UserName to Email
        /// </summary>
        /// <returns>Return to View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Email", "PhoneNumber")] UserModel userModel) {
            if (!ModelState.IsValid) {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (userModel.Email != email) {
                var setEmailResult = await _userManager.SetEmailAsync(user, userModel.Email);
                var setUsername = await _userManager.SetUserNameAsync(user, userModel.Email);
                if (!setEmailResult.Succeeded || !setUsername.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }
            
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (userModel.PhoneNumber != phoneNumber) {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userModel.PhoneNumber);
                if (!setPhoneResult.Succeeded) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Change password landing page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        /// <summary>
        /// Change User password
        /// </summary>
        /// <param name="changePWModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind("OldPassword", "NewPassword", "ConfirmPassword")] ChangePWModel changePWModel) {
            if (!ModelState.IsValid) {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePWModel.OldPassword, changePWModel.NewPassword);
            if (!changePasswordResult.Succeeded) {
                foreach (var error in changePasswordResult.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been changed.";

            return RedirectToAction("Index");
        }
    }
}