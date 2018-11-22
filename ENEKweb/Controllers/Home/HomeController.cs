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
        private readonly IPartnerid _partnerid;

        /// <summary>
        /// Get the injected service methods
        /// </summary>
        /// <param name="leiunurkService"></param>
        /// <param name="partneridService"></param>
        public HomeController(ILeiunurk leiunurkService, IPartnerid partneridService) {
            _leiunurk = leiunurkService;
            _partnerid = partneridService;
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
        public async Task<IActionResult> Partnerid() {
            var partnerid = await _partnerid.GetAllPartners();
            // since an image can be null, we can't use Linq Select method to assing values like in Leiunurk method. Back to the old days bois
            List<PartneridIndexListingModel> partneridListing = new List<PartneridIndexListingModel>();
            foreach (var partner in partnerid) {
                PartneridIndexListingModel addPartner = new PartneridIndexListingModel {
                    Name = partner.Name,
                    Description = partner.Description
                };

                if (partner.Image != null)
                    addPartner.Image = new PartnerImageModel { ImageFileName = partner.Image.ImageFileName };

                partneridListing.Add(addPartner);
            }
            var model = new PartneridIndexModel() { PartneridViewList = partneridListing };
            return View(model);
        }

        /// <summary>
        /// Display all Leiunurk items
        /// </summary>
        /// <returns></returns>
        [Route("Leiunurk")]
        public async Task<IActionResult> Leiunurk() {

            var leiunurkItems = await _leiunurk.GetAllItems();
            var itemsListing = leiunurkItems.Select(result => new LeiunurkIndexListingModel {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Images = result.Images.Select(imgresult => new ItemImages {
                    ImageFileName = imgresult.ImageFileName
                })
            });

            var model = new LeiunurkIndexModel() { LeiunurkItemsViewList = itemsListing };
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
