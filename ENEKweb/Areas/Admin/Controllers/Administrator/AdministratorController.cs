using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ENEKweb.Areas.Admin.Models.Administrator;
using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ENEKweb.Areas.Admin.Controllers.Administrator {
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AdministratorController : Controller {
        /// <summary>
        /// Identity application user service
        /// </summary>
        private readonly IApplicationUser _applicationUser;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// The manager for handling signing users in and out
        /// </summary>
        //private readonly SignInManager<ApplicationUser> _signInManager;


        /// <summary>
        /// Store result messages for user to see
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        public AdministratorController(IApplicationUser applicationuser, UserManager<ApplicationUser> userManager) {
            _applicationUser = applicationuser;
            _userManager = userManager;
        }


        // GET: Administrator
        /// <summary>
        /// List all registered user's.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index() {
            // double check if user is in admin role, if not then return to homepage
            if (!User.IsInRole("admin")) {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            IList<ApplicationUser> users = _userManager.Users.ToList();
            IList<AdminUserModel> userIndexModels = new List<AdminUserModel>();
            if (users != null) {
                foreach (var user in users) {
                    userIndexModels.Add(new AdminUserModel {
                        Id = user.Id,
                        Username = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    });
                }
            }
            return View(userIndexModels);
        }


        //// GET: Administrator/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: Administrator/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Administrator/Create
        /// <summary>
        /// Create a new user with email and password.
        /// </summary>
        /// <param name="userCreateModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateUserModel userCreateModel) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser { UserName = userCreateModel.Email, Email = userCreateModel.Email };
                var result = await _userManager.CreateAsync(user, userCreateModel.Password);
                if (result.Succeeded) {
                    // email confirmation here
                    return RedirectToAction("Index");
                }
                StatusMessage = "Error creating the user: ";
                foreach (var error in result.Errors) {
                    StatusMessage += error;
                }
                return View();
            }

            // If we got this far, something failed, redisplay form
            return View(userCreateModel);
        }

        // GET: Administrator/Edit/5
        /// <summary>
        /// Edit the given user's Email, Phonenumber
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(Guid? id) {
            if (id == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }

            AdminUserModel viewModel = new AdminUserModel {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        // POST: Administrator/Edit/5
        /// <summary>
        /// POST. Submit user changes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Email", "PhoneNumber")]AdminUserModel userModel) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) {
                    StatusMessage = StatusMessages.UserNotFoundMessage;
                    return RedirectToAction("Index");
                }

                var email = await _userManager.GetEmailAsync(user);
                if (userModel.Email != email) {
                    var setEmailResult = await _userManager.SetEmailAsync(user, userModel.Email);
                    var setUsername = await _userManager.SetUserNameAsync(user, userModel.Email);
                    if (!setEmailResult.Succeeded || !setUsername.Succeeded) {
                        StatusMessage = "Error occured when setting an email for the user";
                        return View();
                    }
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (userModel.PhoneNumber != phoneNumber) {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userModel.PhoneNumber);
                    if (!setPhoneResult.Succeeded) {
                        StatusMessage = "Error occured when setting a phone number for the user";
                        return View();
                    }
                }
                StatusMessage = "User has been updated";
                return RedirectToAction("Index");
            }
            StatusMessage = "Error occured, something went wrong. Please try again.";
            return View();
        }

        // GET: Administrator/EditPassword/5
        /// <summary>
        /// GET. Display change user password view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword(Guid? id) {
            if (id == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Administrator/EditPassword/5
        /// <summary>
        /// POST. Submit user password change
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPWModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Guid id, AdminChangeUserPasswordModel userPWModel) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) {
                    StatusMessage = StatusMessages.UserNotFoundMessage;
                    return RedirectToAction("Index");
                }

                // Remove the password first
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded) {
                    StatusMessage = "Error changing the user's password: ";
                    foreach (var error in removePasswordResult.Errors) {
                        StatusMessage += error;
                    }
                    return View();
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, userPWModel.Password);
                if (!addPasswordResult.Succeeded) {
                    StatusMessage = "Error changing the user's password: ";
                    foreach (var error in addPasswordResult.Errors) {
                        StatusMessage += error;
                    }
                    return View();
                }
                StatusMessage = "User's password has been changed.";

                return RedirectToAction("Index");
            }

            // If we got here, means something went wrong. Redisplay form
            StatusMessage = "Error occured when changing the password";
            return View();

        }



        // GET: Administrator/Delete/5
        /// <summary>
        /// Display the user to be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(Guid? id) {
            if (id == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }

            AdminUserModel viewModel = new AdminUserModel {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        // POST: Administrator/Delete/5
        /// <summary>
        /// POST. Delete the user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, AdminUserModel userModel) {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) {
                StatusMessage = StatusMessages.UserNotFoundMessage;
                return RedirectToAction("Index");
            }
            var removeUserResult = await _userManager.DeleteAsync(user);
            if(!removeUserResult.Succeeded) {
                StatusMessage = "Error deleting the user: ";
                foreach (var error in removeUserResult.Errors) {
                    StatusMessage += error;
                }
                return View();
            }
            StatusMessage = "The user has been deleted!";
            return RedirectToAction(nameof(Index));
        }

        // STATUS MESSAGES
        /// <summary>
        /// Status messages for user feedback
        /// </summary>
        public static class StatusMessages {
            public static string UserNotFoundMessage => "Error occured, no user with given ID found.";
        }
    }
}