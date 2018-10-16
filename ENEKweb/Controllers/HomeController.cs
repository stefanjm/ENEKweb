using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ENEKweb.Models;

namespace ENEKweb.Controllers {
    public class HomeController : Controller {
        public IActionResult Index()
        {
            return View();
        }

        // Alternative Design
        public IActionResult IndexOld() {
            return View();
        }

        public IActionResult Tehtudtood()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Partnerid() {
            return View();
        }

        public IActionResult Leiunurk() {
            return View();
        }

        public IActionResult Kontakt()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
