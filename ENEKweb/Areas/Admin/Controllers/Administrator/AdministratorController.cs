using System;
using System.Collections.Generic;
using System.Linq;
using ENEKweb.Areas.Admin.Models.Administrator;
using IdentityData;
using IdentityData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ENEKweb.Areas.Admin.Controllers.Administrator
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AdministratorController : Controller
    {
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


        public AdministratorController(IApplicationUser applicationuser, UserManager<ApplicationUser> userManager
            /*SignInManager<ApplicationUser> signInManager*/) {
            _applicationUser = applicationuser;
            _userManager = userManager;
            //_signInManager = signInManager;
        }


        // GET: Administrator
        /// <summary>
        /// List all registered users.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
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


        /// <summary>
        /// Create a new user
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateUser() {
            return View();
        }


        // GET: Administrator/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Administrator/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administrator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Administrator/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Administrator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Administrator/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Administrator/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}