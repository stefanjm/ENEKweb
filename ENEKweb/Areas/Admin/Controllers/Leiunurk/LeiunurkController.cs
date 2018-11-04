﻿using System;
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

        // DB context
        private readonly ILeiunurk _leiunurk;

        // Path where the images are to be stored
        private readonly string imgUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploaded/leiunurk");

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
        // Adding Images will be refactored to be an UTILITY CLASS or CLASS LIBRARY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Item item, [Bind("Images")]ICollection<IFormFile> Images) {
            if (ModelState.IsValid) {

                // Check if it's an image thats being uploaded
                if (Images.Count > 0) {
                    foreach (var img in Images) {
                        if (!img.ContentType.Contains("image")) {
                            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                        }
                    }
                }
                await _leiunurk.AddItem(item, Images, imgUploadPath);

                return RedirectToAction(nameof(Index));
            }
            return View(item);
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


            var editModel = new ItemEditModel {
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
        /// Edit the item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemEditModel item) {
            if (id != item.Id) {
                return NotFound();
            }
            if (ModelState.IsValid) {
                // This all should probably be in the Service logic class, not doing it right now because can't find a way to return NotFound() 404 from the Service class.
                // Try and Catch for database concurrency, what if someone else was already editing and saved changes, we would just overwrite his changes
                // doing this to prevent that
                // Might not actually work if we don't throw concurrency exception by ourselves.
                try {

                    ICollection<Image> ItemImages = new List<Image>();

                    // Since we are using ItemEditModel which doesn't know about the Image list that every item has, 
                    //  we'll add them to an Image list and after create the Item to be inserted to the database
                    foreach (var modelImage in item.Images) {
                        ItemImages.Add(new Image {
                            Id = modelImage.Id,
                            ImageFileName = modelImage.ImageFileName
                        });
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
                    if (item.ImagesToAdd != null && item.ImagesToAdd.Count > 0) {
                        // Path where to store uploaded Images
                        foreach (var img in item.ImagesToAdd) {
                            if (!img.ContentType.Contains("image")) {
                                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
                            }
                        }
                    }

                    // Add the images to remove - IDs to an int List
                    List<int> ItemImagesToRemoveIds = new List<int>();
                    foreach (var imageToRemove in item.Images) {
                        if (imageToRemove.RemoveImage)
                            ItemImagesToRemoveIds.Add(imageToRemove.Id);
                    }

                    await _leiunurk.EditItem(EditedItem, ItemImagesToRemoveIds, item.ImagesToAdd, imgUploadPath);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            await _leiunurk.RemoveItem(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
