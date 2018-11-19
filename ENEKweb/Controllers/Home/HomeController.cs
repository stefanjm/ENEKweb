using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ENEKweb.Models;
using Microsoft.AspNetCore.Authorization;
using ENEKdata;

namespace ENEKweb.Controllers {
    [AllowAnonymous]
    public class HomeController : Controller {

        // Database context interface
        private readonly ILeiunurk _leiunurk;

        /// <summary>
        /// Get the injected database service methods
        /// </summary>
        /// <param name="leiunurkDatabase"></param>
        public HomeController(ILeiunurk leiunurkDatabase) {
            _leiunurk = leiunurkDatabase;
        }

        [Route("Home")]
        [Route("/")]
        public IActionResult Index() {
            return View();
        }

        //public IActionResult Tehtudtood()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        /// <summary>
        /// Display partners
        /// </summary>
        /// <returns></returns>
        [Route("Partnerid")]
        public IActionResult Partnerid() {
            return View();
        }

        /// <summary>
        /// Display all Leiunurk items
        /// </summary>
        /// <returns></returns>
        [Route("Leiunurk")]
        public async Task<IActionResult> Leiunurk() {

            var leiuNurkItems = await _leiunurk.GetAllItems();
            var itemsListing = leiuNurkItems.Select(result => new LeiunurkIndexListingModel {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Images = result.Images.Select(imgresult => new ItemImages {
                    ImageFileName = imgresult.ImageFileName
                })
            });

            var model = new LeiunurkIndexModel() { LeiunurkItems = itemsListing };
            return View(model);
        }

        /// <summary>
        /// Display Kontakt page, also when url is /Kontakt
        /// </summary>
        /// <returns></returns>
        [Route("Kontakt")]
        public IActionResult Kontakt() {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        /// <summary>
        /// Display a custom error page
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null) {
            if (statusCode.HasValue) {
                if (statusCode.Value == 404 || statusCode.Value == 500) {
                    var viewName = statusCode.ToString();
                    return View(new ErrorViewModel { ErrorCode = statusCode });
                }
            }
            return View(new ErrorViewModel { ErrorCode = 0 });
        }
    }
}
