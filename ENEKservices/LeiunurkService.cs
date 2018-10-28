using ENEKdata;
using ENEKdata.Models.Leiunurk;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ENEKservices {
    public class LeiunurkService : ILeiunurk {

        private readonly ENEKdataDbContext _context;
        /// <summary>
        /// get DbContext
        /// </summary>
        /// <param name="context"></param>
        public LeiunurkService(ENEKdataDbContext context) {
            _context = context;
        }
        
        /// <summary>
        /// Get all the items including the Images every item has
        /// </summary>
        /// <returns></returns>
        public async Task<List<Item>> GetAllItems() {
            return await _context.Items
                .Include(item => item.Images).ToListAsync();
        }

        public async Task<Item> GetItemById(int? id) {
            // Extra null check just in case.
            if (id == null) {
                return null;
            }
            else {
                return await _context.Items
                    .Include(item => item.Images).FirstOrDefaultAsync(i => i.Id == id);
            }
        }

        // Not using .FirstAsync() because it will just bring another variety in the code, possibly making it more complicated.
        // Should probably use it because it first checks if that item with the given ID is being traced by the context already
        //  e.g. The item is queried to be deleted, which the db context traces. Then if you choose to Delete it right away, it's
        //       already being traced and will not have to be queried again.
        //public async Task<Item> GetItemById(int? id) {
        //    if (id == null) {
        //        return null;
        //    }
        //    else {
        //        return await _context.Items.FirstAsync();
        //    }
        //}

        public async Task EditItem(Item item, ICollection<Image> Images) {
            // Add images to the item
            if(Images.Count > 0) {
                item.Images = new List<Image>();
                foreach (Image image in Images) {
                    item.Images.Add(image);
                }
            }
            _context.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddItem(Item newItem, ICollection<Image> Images) {
            // Add images to the item
            if(Images.Count > 0) {
                newItem.Images = new List<Image>();
                foreach (Image image in Images) {
                    newItem.Images.Add(image);
                }
            }

            _context.Add(newItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItem(int id) {
            var item = await GetItemById(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

        }

        public Task<bool> ItemExists(int id) {
            return _context.Items.AnyAsync(e => e.Id == id);
        }
    }
}
