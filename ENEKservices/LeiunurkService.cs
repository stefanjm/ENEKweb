using ENEKdata;
using ENEKdata.Models.Leiunurk;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Get the item by id with images tied to it, given id can be null because the user might want to edit an item that doesn't exist. 
        /// Won't start tracking the returned item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Item> GetItemById(int? id) {
            if (id == null) {
                return null;
            }
            else {
                return await _context.Items.AsNoTracking()
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

        public async Task EditItem(Item editedItem, List<int> ImagesToRemoveIds) {

            // Check if an item with Edited Item ID exists in the database
            if(!await ItemExists(editedItem.Id)) {
                return;
            }

            // Check if any images to remove and do so
            if(ImagesToRemoveIds.Any()) {
                foreach(int imageId in ImagesToRemoveIds) {
                    if(await IsImageOwnedByGivenItem(editedItem.Id, imageId)) {
                        // Remove the image from this list and also add it to the DB queue to query when SaveChanges is called to remove it from DB as well
                        // Have to do both because, if just removing from list. Updating thinks we didn't touch the image at all,
                        //  if we would remove the image only from the Database and not from the list right away. The list would add it back:(
                        editedItem.Images.Remove(editedItem.Images.First(x => x.Id == imageId));
                        await RemoveImageWithoutSaveChanges(imageId);
                    }
                }
            }

            _context.Update(editedItem);
            await _context.SaveChangesAsync();
        }

        public async Task AddItem(Item newItem, ICollection<Image> Images) {
            // Add images to the item
            if(Images.Count > 0) {
                AddImagesToItem(newItem, Images);
            }
            _context.Add(newItem);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove an item. Since images are tied to the Item, they will also be removed from the Database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveItem(int id) {
            var item = await GetItemById(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Remove an image from the Item without saving changes to database
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task RemoveImageWithoutSaveChanges(int imageId) {
            Image image = await GetImageById(imageId);
            _context.Images.Remove(image);
        }

        /// <summary>
        /// Add given images to the given Item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public Item AddImagesToItem(Item item, ICollection<Image> images) {
            item.Images = new List<Image>();
            foreach(Image image in images) {
                item.Images.Add(image);
            }
            return item;
        }

        /// <summary>
        /// Add an image to the Database.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task AddImage(Image image) {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Image> GetImageById(int? imageId) {
            if (imageId == null) {
                return null;
            }
            else {
                return await _context.Images.FirstOrDefaultAsync(img => img.Id == imageId);
            }
        }


        /// <summary>
        /// Check if given Image id is associated with the given Item id
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<bool> IsImageOwnedByGivenItem(int itemId, int imageId) {
            Item item = await GetItemById(itemId);
            if(item.Images.Any(x => x.Id == imageId)) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Remove an image from the Database.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task RemoveImage(int imageId) {
            await RemoveImageWithoutSaveChanges(imageId);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ItemExists(int id) {
            return _context.Items.AnyAsync(e => e.Id == id);
        }

    }
}
