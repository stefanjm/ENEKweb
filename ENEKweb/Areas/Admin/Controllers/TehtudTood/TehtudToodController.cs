using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ENEKdata;
using ENEKdata.Models.TehtudTood;
using ENEKweb.Areas.Admin.Models.TehtudTood;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ENEKweb.Areas.Admin.Controllers.TehtudTood {

    [Area("Admin")]
    public class TehtudToodController : Controller {

        // Tehtud tööd service reference
        private readonly ITehtudTood _tehtudTood;

        // Path where the images are to be stored/uploaded
        private static readonly string imgUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploaded/tehtudtood");

        /// <summary>
        /// Store result messages to be displayed for the user
        /// </summary>
        /// <returns></returns>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Get an instance of the TehtudTood service
        /// </summary>
        /// <param name="tehtudTood"></param>
        public TehtudToodController(ITehtudTood tehtudTood) {
            _tehtudTood = tehtudTood;
        }


        // GET: TehtudTood
        /// <summary>
        /// Get a list of Tehtud tööd and parse them to view model list to be displayed
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index() {
            return View(ParseMultipleEntityModelsToViewModels(await _tehtudTood.GetAllTehtudTood()));
        }

        // GET: TehtudTood/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var tehtudToo = await _tehtudTood.GetTehtudTooById(id);
            if (tehtudToo == null) {
                return NotFound();
            }

            // Parse to view model
            TehtudTooModel tehtudTooViewModel = ParseSingleEntityModelToViewModel(tehtudToo);

            return View(tehtudTooViewModel);
        }

        // GET: TehtudTood/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TehtudTood/Create
        /// <summary>
        /// POST. Create a new Tehtud töö
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,YearDone,BuildingType,ImagesToAdd")] TehtudTooModel formTehtudToo) {
            if (ModelState.IsValid) {

                // Check if it's images that are being uploaded
                if (formTehtudToo.ImagesToAdd != null && formTehtudToo.ImagesToAdd.Count > 0) {
                    if (!CheckIfValidImages(formTehtudToo.ImagesToAdd)) {
                        return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                    }
                }

                TehtudToo newTehtudToo = new TehtudToo {
                    Name = formTehtudToo.Name,
                    YearDone = formTehtudToo.YearDone,
                    BuildingType = formTehtudToo.BuildingType
                };

                await _tehtudTood.AddTehtudToo(newTehtudToo, formTehtudToo.ImagesToAdd, imgUploadPath);
                StatusMessage = "Tehtud töö has been added";
                return RedirectToAction(nameof(Index));
            }

            return View(formTehtudToo);
        }

        // GET: TehtudTood/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var tehtudToo = await _tehtudTood.GetTehtudTooById(id);
            if (tehtudToo == null) {
                return NotFound();
            }

            var editModel = ParseSingleEntityModelToViewModel(tehtudToo);

            return View(editModel);
        }

        // POST: TehtudTood/Edit/5
        /// <summary>
        /// POST. Edit Tehtud töö
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TehtudTooModel tehtudToo) {
            if (id != tehtudToo.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {

                try {

                    ICollection<TehtudTooImage> tehtudTooImages = new List<TehtudTooImage>();
                    List<int> tehtudTooImagesToRemoveIds = new List<int>();

                    // Check if user has chosen to remove all the images
                    if (tehtudToo.Images != null && tehtudToo.Images.Any()) {

                        // Since we are using TehtudTooModel which doesn't know about the Image list that every item has, 
                        //  we'll add them to an Image list and after create the Item to be inserted to the database
                        foreach (var modelImage in tehtudToo.Images) {
                            tehtudTooImages.Add(new TehtudTooImage {
                                Id = modelImage.Id,
                                ImageFileName = modelImage.ImageFileName
                            });

                            // Check if user selected the image to be removed
                            if (modelImage.RemoveImage)
                                tehtudTooImagesToRemoveIds.Add(modelImage.Id);
                        }
                    }

                    // Create Tehtud töö entity model

                    TehtudToo editedTehtudToo = new TehtudToo {
                        Id = tehtudToo.Id,
                        Name = tehtudToo.Name,
                        YearDone = tehtudToo.YearDone,
                        BuildingType = tehtudToo.BuildingType,
                        Images = tehtudTooImages
                    };

                    // Check if newly added images are the right type
                    if (tehtudToo.ImagesToAdd != null && tehtudToo.ImagesToAdd.Any()) {
                        if (!CheckIfValidImages(tehtudToo.ImagesToAdd)) {
                            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                        }
                    }

                    await _tehtudTood.EditTehtudToo(editedTehtudToo, tehtudTooImagesToRemoveIds, tehtudToo.ImagesToAdd, imgUploadPath);
                    StatusMessage = "Tehtud töö has been edited!";

                }
                catch {
                    if (!(await _tehtudTood.TehtudTooExists(tehtudToo.Id))) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // show form again if model not valid
            return View(tehtudToo);
        }

        // GET: TehtudTood/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var tehtudToo = await _tehtudTood.GetTehtudTooById(id);
            if (tehtudToo == null) {
                return NotFound();
            }

            // Parse to view model
            TehtudTooModel tehtudTooViewModel = ParseSingleEntityModelToViewModel(tehtudToo);

            return View(tehtudTooViewModel);
        }

        // POST: TehtudTood/Delete/5
        /// <summary>
        /// POST. Delete Tehtud töö
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            await _tehtudTood.RemoveTehtudToo(id);
            StatusMessage = "Tehtud töö has been deleted!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Basic check if form posted image contains "image" in the header
        /// </summary>
        /// <param name="images"></param>
        /// <returns>False if any of the images do not contain "image" in the header</returns>
        private bool CheckIfValidImages(ICollection<IFormFile> images) {
            foreach (var img in images) {
                if (!img.ContentType.Contains("image")) {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Parse the list of entity DB Tehtud tööd objects to View model objects to be displayed on web
        /// </summary>
        /// <param name="EntityModelTehtudTood"></param>
        /// <returns></returns>
        private IList<TehtudTooModel> ParseMultipleEntityModelsToViewModels(IList<TehtudToo> entityModelTehtudTood) {
            IList<TehtudTooModel> viewModelTehtudTood = new List<TehtudTooModel>();

            foreach (TehtudToo too in entityModelTehtudTood) {
                TehtudTooModel tehtudTooViewModel = new TehtudTooModel {
                    Id = too.Id,
                    Name = too.Name,
                    YearDone = too.YearDone,
                    BuildingType = too.BuildingType,
                    Images = new List<ImageEditModel>()
                };

                if (too.Images != null & too.Images.Any()) {
                    foreach (var image in too.Images) {
                        tehtudTooViewModel.Images.Add(new ImageEditModel {
                            Id = image.Id,
                            ImageFileName = image.ImageFileName
                        });
                    }
                }

                viewModelTehtudTood.Add(tehtudTooViewModel);
            }

            return viewModelTehtudTood;
        }

        // Doing this because for displayed properties the names have to be changed to be more presentable or translated later
        /// <summary>
        /// Parse single entity Tehtud töö to View model object to be displayed on web
        /// </summary>
        /// <param name="entityModelTehtudToo"></param>
        /// <returns></returns>
        private TehtudTooModel ParseSingleEntityModelToViewModel(TehtudToo entityModelTehtudToo) {
            TehtudTooModel tehtudTooViewModel = new TehtudTooModel {
                Id = entityModelTehtudToo.Id,
                Name = entityModelTehtudToo.Name,
                YearDone = entityModelTehtudToo.YearDone,
                BuildingType = entityModelTehtudToo.BuildingType,
                Images = new List<ImageEditModel>()
            };

            if (entityModelTehtudToo.Images != null & entityModelTehtudToo.Images.Any()) {
                foreach (var image in entityModelTehtudToo.Images) {
                    tehtudTooViewModel.Images.Add(new ImageEditModel {
                        Id = image.Id,
                        ImageFileName = image.ImageFileName
                    });
                }
            }

            return tehtudTooViewModel;
        }
    }
}