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

namespace ENEKweb.Areas.Admin.Controllers.Leiunurk {
    [Area("Admin")]
    public class LeiunurkController : Controller {

        // DB context
        private readonly ILeiunurk _leiunurk;

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
                // Images for the item
                ICollection<Image> ItemImages = new List<Image>();
                // Check if there are any images
                if (Images.Count > 0) {
                    // Upload Images
                    long size = Images.Sum(f => f.Length);
                    // Path where to store uploaded Images
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploaded/leiunurk");

                    foreach (var formFile in Images) {
                        // Check if file is valid and an image. Probably should create a static class to check for more exact filetypes
                        if (formFile.Length > 0 && formFile.ContentType.Contains("image")) {
                            var filename = Path.GetRandomFileName();
                            filename = Path.ChangeExtension(filename, Path.GetExtension(formFile.FileName));
                            var filePath = Path.Combine(uploadDir, filename);
                            using (var stream = new FileStream(filePath, FileMode.Create)) {
                                await formFile.CopyToAsync(stream);
                                ItemImages.Add(new Image {
                                    ImageFileName = filename
                                });
                            }
                        }
                    }
                }
                await _leiunurk.AddItem(item, ItemImages);

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
            return View(item);
        }

        // POST: Admin/Leiunurk/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Adding Images will be refactored to be an UTILITY CLASS OR CLASS LIBRARY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Item item, [Bind("Images")]ICollection<IFormFile> Images) {
            if (id != item.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                // This all should probably be in the Service logic class, not doing it right now because can't find a way to return NotFound() 404 from the Service class.
                try {
                    // add new images if any chosen
                    ICollection<Image> ItemImages = new List<Image>();
                    if (Images.Count > 0) {
                        if (Images.Count > 0) {
                            // Upload Images
                            long size = Images.Sum(f => f.Length);
                            // Path where to store uploaded Images
                            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploaded/leiunurk");

                            foreach (var formFile in Images) {
                                // Check if file is valid and an image. Probably should create a static class to check for more exact filetypes
                                if (formFile.Length > 0 && formFile.ContentType.Contains("image")) {
                                    var filename = Path.GetRandomFileName();
                                    filename = Path.ChangeExtension(filename, Path.GetExtension(formFile.FileName));
                                    var filePath = Path.Combine(uploadDir, filename);
                                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                                        await formFile.CopyToAsync(stream);
                                        ItemImages.Add(new Image {
                                            ImageFileName = filename
                                        });
                                    }
                                }
                            }
                        }
                    }
                    await _leiunurk.EditItem(item, ItemImages);
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
