using ENEKdata;
using ENEKdata.Models.Partnerid;
using ENEKweb.Areas.Admin.Models.Partnerid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ENEKweb.Areas.Admin.Controllers.Partnerid
{
    [Area("Admin")]
    public class PartneridController : Controller
    {

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
        public async Task<IActionResult> Index()
        {
            return View(await _partnerid.GetAllPartners());
        }

        // GET: Admin/Partnerid/Details/5
        /// <summary>
        /// Display details about Partner with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            Partner partner = await _partnerid.GetPartnerById(id);
            if(partner == null) {
                return NotFound();
            }

            return View(partner);
        }

        // GET: Admin/Partnerid/Create
        public ActionResult Create()
        {
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
        public async Task<IActionResult> Create(PartnerFormModel partnerForm)
        {
            if (ModelState.IsValid) {

                // Check if it's an image thats being uploaded
                if (partnerForm.Image != null) {
                    if (!partnerForm.Image.ContentType.Contains("image")) {
                        StatusMessage = "Error: uploaded image is not an image!";
                        return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                    }
                }
                Partner partner = new Partner {
                    Name = partnerForm.Name,
                    Description = partnerForm.Description
                };

                await _partnerid.AddPartner(partner, partnerForm.Image, imgUploadPath);
                StatusMessage = "The Partner has been created!";
                return RedirectToAction(nameof(Index));
            }

            return View(partnerForm);
        }

        // GET: Admin/Partnerid/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Partnerid/Edit/5
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

        // GET: Admin/Partnerid/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Partnerid/Delete/5
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