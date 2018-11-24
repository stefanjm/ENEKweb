using ENEKdata;
using ENEKdata.Models.Partnerid;
using ENEKweb.Areas.Admin.Models.Partnerid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace ENEKweb.Areas.Admin.Controllers.Partnerid {
    [Area("Admin")]
    public class PartneridController : Controller {

        // Partner service instance
        private readonly IPartnerid _partnerid;

        /// <summary>
        /// Store result messages to be displayed for the user
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        // Path where the images are to be stored
        private readonly string imgUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploaded/partnerid");

        /// <summary>
        /// Get an instance of the Partnerid service
        /// </summary>
        /// <param name="partnerid"></param>
        public PartneridController(IPartnerid partnerid) {
            _partnerid = partnerid;
        }

        // GET: Admin/Partnerid
        /// <summary>
        /// Display list of all Partners
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index() {
            return View(await _partnerid.GetAllPartners());
        }

        // GET: Admin/Partnerid/Details/5
        /// <summary>
        /// Display details about Partner with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            Partner partner = await _partnerid.GetPartnerById(id);
            if (partner == null) {
                return NotFound();
            }

            return View(partner);
        }

        // GET: Admin/Partnerid/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Admin/Partnerid/Create
        /// <summary>
        /// POST. Create new Partner
        /// </summary>
        /// <param name="partnerForm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Description, PartnerURL, UploadImage")] PartnerFormModel partnerForm) {
            if (ModelState.IsValid) {

                // Check if it's an image thats being uploaded
                if (partnerForm.UploadImage != null) {
                    if (!partnerForm.UploadImage.ContentType.Contains("image")) {
                        StatusMessage = "Error: uploaded image is not an image!";
                        return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                    }
                }
                Partner partner = new Partner {
                    Name = partnerForm.Name,
                    Description = partnerForm.Description,
                    PartnerURL = partnerForm.PartnerURL
                };

                await _partnerid.AddPartner(partner, partnerForm.UploadImage, imgUploadPath);
                StatusMessage = "The Partner has been created!";
                return RedirectToAction(nameof(Index));
            }

            return View(partnerForm);
        }

        // GET: Admin/Partnerid/Edit/5
        /// <summary>
        /// Display Edit Partner form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            // not using items.FirstAsync() because it just adds another variety
            Partner partner = await _partnerid.GetPartnerById(id);
            if (partner == null) {
                return NotFound();
            }

            // Initialize View Model
            PartnerFormModel formModel = new PartnerFormModel {
                Id = partner.Id,
                Name = partner.Name,
                Description = partner.Description,
                PartnerURL = partner.PartnerURL
            };

            // If Partner has an image then add it
            if (partner.Image != null) {
                formModel.Image = new PartnerFormImageModel {
                    Id = partner.Image.Id,
                    ImageFileName = partner.Image.ImageFileName
                };
            }

            return View(formModel);
        }

        // POST: Admin/Partnerid/Edit/5
        /// <summary>
        /// POST. Convert the form model to entity model and set image to null if bool removeImage is true or a new image was supplied
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PartnerFormModel formModel) {
            if (id != formModel.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {

                try {

                    // First check if a new image has been uploaded, if so then check if it's valid and set the remove image boolean to true since we'll replace it
                    //  , If a new image has not been uploaded(its null) then we'll check if the current image should be removed set the boolean removeImage accordingly

                    // Create edited partner object
                    Partner editedPartner = new Partner {
                        Id = formModel.Id,
                        Name = formModel.Name,
                        Description = formModel.Description,
                        PartnerURL = formModel.PartnerURL
                    };
                    // Check if an image is uploaded(not null) and valid, 
                    // if null then check if image was chosen to be removed
                    if (formModel.UploadImage != null) {

                        // Check if supplied image is indeed an image
                        if (!formModel.UploadImage.ContentType.Contains("image")) {
                            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                        }

                        // image will be replaced anyway so set the current one to be removed
                        formModel.RemoveImage = true;
                    }


                    await _partnerid.EditPartner(editedPartner, formModel.UploadImage, formModel.RemoveImage, imgUploadPath);
                    StatusMessage = "The Item has been edited!";
                }
                catch {
                    if (!(await _partnerid.PartnerExists(formModel.Id))) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }


            // show form again if model not valid
            return View(formModel);
        }

        // GET: Admin/Partnerid/Delete/5
        /// <summary>
        /// Get the partner and assign it to a view model for showing info about which Partner will be deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var partner = await _partnerid.GetPartnerById(id);
            if (partner == null) {
                return NotFound();
            }

            PartnerFormModel partnerModel = new PartnerFormModel {
                Id = partner.Id,
                Name = partner.Name,
                Description = partner.Description
            };

            // Add the image to viewmodel if Partner has one
            if (partner.Image != null) {
                partnerModel.Image = new PartnerFormImageModel { Id = partner.Image.Id, ImageFileName = partner.Image.ImageFileName };
            }

            return View(partnerModel);
        }

        // POST: Admin/Partnerid/Delete/5
        /// <summary>
        /// Delete Partner with given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            await _partnerid.RemovePartner(id);
            StatusMessage = "The Item has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}