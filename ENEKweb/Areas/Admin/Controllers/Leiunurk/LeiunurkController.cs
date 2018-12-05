using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ENEKdata;
using ENEKdata.Models.Leiunurk;
using Microsoft.AspNetCore.Http;
using System.IO;
using ENEKweb.Areas.Admin.Models.Leiunurk;

namespace ENEKweb.Areas.Admin.Controllers.Leiunurk {
    [Area("Admin")]
    public class LeiunurkController : Controller {

        // Leiunurk service instance
        private readonly ILeiunurk _leiunurk;

        /// <summary>
        /// Store result messages to be displayed for the user
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        // Path where the images are to be stored
        private readonly string imgUploadPath = "wwwroot/images/uploaded/leiunurk";

        /// <summary>
        /// Get an instance of the Leiunurk service
        /// </summary>
        /// <param name="leiunurk"></param>
        public LeiunurkController(ILeiunurk leiunurk) {
            _leiunurk = leiunurk;
        }

        // GET: Admin/Leiunurk
        public async Task<IActionResult> Index() {
            return View(await _leiunurk.GetAllItems());
        }

        // GET: Admin/Leiunurk/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var item = await _leiunurk.GetItemById(id);
            if (item == null) {
                return NotFound();
            }

            return View(item);
        }

        // GET: Admin/Leiunurk/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Admin/Leiunurk/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// POST. Create a new Item and upload images
        /// </summary>
        /// <param name="formItem"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImagesToAdd")] ItemModel formItem) {
            if (ModelState.IsValid) {

                // Check if it's images that are being uploaded
                if (formItem.ImagesToAdd != null && formItem.ImagesToAdd.Count > 0) {
                    foreach (var img in formItem.ImagesToAdd) {
                        if (!img.ContentType.Contains("image")) {
                            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                        }
                    }
                }

                Item newItem = new Item {
                    Name = formItem.Name,
                    Description = formItem.Description,
                    Price = formItem.Price
                };
                await _leiunurk.AddItem(newItem, formItem.ImagesToAdd, imgUploadPath);
                StatusMessage = "The Item has been created!";
                return RedirectToAction(nameof(Index));
            }

            return View(formItem);
        }

        // GET: Admin/Leiunurk/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            // not using items.FirstAsync() because it just adds another variety
            var item = await _leiunurk.GetItemById(id);
            if (item == null) {
                return NotFound();
            }

            IList<ImageEditModel> editModelImages = new List<ImageEditModel>();
            foreach (var image in item.Images) {
                editModelImages.Add(new ImageEditModel {
                    Id = image.Id,
                    ImageFileName = image.ImageFileName
                });
            }


            var editModel = new ItemModel {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Images = editModelImages
            };
            return View(editModel);
        }

        // POST: Admin/Leiunurk/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Adding Images will be refactored to be an UTILITY CLASS OR CLASS LIBRARY
        // Currently Images with checkboxes on the website itself have weird Ids and Names because the Edit Model has another list with the images,
        //  so it binds them as Images_._[1]_RemoveImage which looks not so good.
        /// <summary>
        /// POST. Edit the item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemModel item) {
            if (id != item.Id) {
                return NotFound();
            }
            if (ModelState.IsValid) {
                // This all should probably be in the Service logic class, not doing it right now because can't find a way to return NotFound() 404 from the Service class.
                // Try and Catch for database concurrency, what if someone else was already editing and saved changes, we would just overwrite his changes
                // doing this to prevent that
                // Might not actually work if we don't throw concurrency exception by ourselves.
                try {

                    ICollection<ItemImage> ItemImages = new List<ItemImage>();
                    List<int> ItemImagesToRemoveIds = new List<int>();


                    // Check if user has chosen to remove all the images
                    if (item.Images != null && item.Images.Any()) {

                        // Since we are using ItemModel which doesn't know about the Image list that every item has, 
                        //  we'll add them to an Image list and after create the Item to be inserted to the database
                        foreach (var modelImage in item.Images) {
                            ItemImages.Add(new ItemImage {
                                Id = modelImage.Id,
                                ImageFileName = modelImage.ImageFileName
                            });

                            // Check if user selected the image to be removed
                            if (modelImage.RemoveImage)
                                ItemImagesToRemoveIds.Add(modelImage.Id);
                        }
                    }


                    // Create Item for database insertion from the ItemEditModel class
                    Item EditedItem = new Item {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Images = ItemImages,
                        Price = item.Price
                    };

                    // Check if new added images are the right type.
                    if (item.ImagesToAdd != null && item.ImagesToAdd.Any()) {
                        foreach (var img in item.ImagesToAdd) {
                            if (!img.ContentType.Contains("image")) {
                                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                            }
                        }
                    }

                    await _leiunurk.EditItem(EditedItem, ItemImagesToRemoveIds, item.ImagesToAdd, imgUploadPath);
                    StatusMessage = "The Item has been edited!";
                }
                catch (DbUpdateConcurrencyException) {
                    if (!(await _leiunurk.ItemExists(item.Id))) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // show form again if model not valid
            return View(item);
        }

        // GET: Admin/Leiunurk/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var item = await _leiunurk.GetItemById(id);
            if (item == null) {
                return NotFound();
            }

            return View(item);
        }

        // POST: Admin/Leiunurk/Delete/5
        /// <summary>
        /// POST. Delete the item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            await _leiunurk.RemoveItem(id);
            StatusMessage = "The Item has been deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}
