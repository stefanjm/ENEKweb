using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ENEKdata;
using ENEKdata.Models;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Item item) {
            if (ModelState.IsValid) {
                await _leiunurk.AddItem(item);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Item item) {
            if (id != item.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                // This all should probably be in the Service class, not doing it right now because can't find a way to return NotFound() 404 from the Service class.
                try {
                    await _leiunurk.EditItem(item);
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
