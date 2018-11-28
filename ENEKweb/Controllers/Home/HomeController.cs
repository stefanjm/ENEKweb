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
        private readonly ITehtudTood _tehtudTood;

        /// <summary>
        /// Get the injected service methods
        /// </summary>
        /// <param name="leiunurkService"></param>
        /// <param name="partneridService"></param>
        public HomeController(ILeiunurk leiunurkService, IPartnerid partneridService, ITehtudTood tehtudToodService) {
            _leiunurk = leiunurkService;
            _partnerid = partneridService;
            _tehtudTood = tehtudToodService;
        }

        [Route("Home")]
        [Route("/")]
        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// Display Firmast
        /// </summary>
        /// <returns></returns>
        //[Route("Firmast")]
        //public IActionResult Firmast() {
        //    return View();
        //}

        /// <summary>
        /// Display tehtud tööd
        /// </summary>
        /// <returns></returns>
        [Route("Tehtudtood")]
        public async Task<IActionResult> Tehtudtood() {
            var tehtudTood = await _tehtudTood.GetAllTehtudTood();
            IList<TehtudToodIndexListingModel> tehtudToodListing = new List<TehtudToodIndexListingModel>();
            // List for the years when the jobs were done.
            IList<int> yearsDoneIn = new List<int>();

            // Assing queried objects to a list
            foreach(var tehtudToo in tehtudTood) {
                // Check if we have the job done year already in the list, if not add it.
                if(!yearsDoneIn.Contains(tehtudToo.YearDone)) {
                    yearsDoneIn.Add(tehtudToo.YearDone);
                }
                // Create and add a view model to the listing
                tehtudToodListing.Add(new TehtudToodIndexListingModel {
                    Name = tehtudToo.Name,
                    YearDone = tehtudToo.YearDone,
                    BuildingType = tehtudToo.BuildingType,
                    Images = tehtudToo.Images.Select(imgresult => new TehtudTooImages {
                        ImageFileName = imgresult.ImageFileName
                    })
                });
            }

            // Sort the years done in list to show highest first.
            yearsDoneIn.OrderByDescending(i => i);

            var model = new TehtudToodIndexModel() { TehtudToodViewList = tehtudToodListing , TehtudToodYears = yearsDoneIn };
            return View(model);
        }

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
                    Description = partner.Description,
                    PartnerURL = partner.PartnerURL                    
                };

                if (partner.Image != null)
                    addPartner.Image = new PartnerImageModel { ImageFileName = partner.Image.ImageFileName };

                partneridListing.Add(addPartner);
            }
            var model = new PartneridIndexModel() { PartneridViewList = partneridListing };
            return View(model);
        }

        /// <summary>
        /// Display Leiunurk items
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
