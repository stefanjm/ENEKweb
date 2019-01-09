using ENEKdata;
using ENEKdata.Models.Leiunurk;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ENEKservices {
    public class LeiunurkService : ILeiunurk {

        /// <summary>
        /// Database context
        /// </summary>
        private readonly ENEKdataDbContext _context;

        /// <summary>
        /// Initialize database context
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

            return await _context.Items.AsNoTracking()
                .Include(item => item.Images).FirstOrDefaultAsync(i => i.Id == id);

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

        /// <summary>
        /// Edit the Item
        /// </summary>
        /// <param name="editedItem"></param>
        /// <param name="imagesToRemoveIds"></param>
        /// <param name="imagesToAdd"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        public async Task EditItem(Item editedItem, List<int> imagesToRemoveIds, ICollection<IFormFile> imagesToAdd, string imgUploadPath) {

            // Check if an item with Edited Item ID exists in the database
            if (!await ItemExists(editedItem.Id)) {
                return;
            }
            // Transactions should also check for the file IO, if that throws an error then it wont update the database
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // Check if any images to Add and do so
                    if (imagesToAdd != null && imagesToAdd.Any()) {
                        // List for new Images to add
                        List<ItemImage> newImages = new List<ItemImage>();
                        // upload the images to the given path
                        List<string> uploadedImgNames = await ImageManager.UploadImages(imagesToAdd, imgUploadPath);
                        foreach (string imgFileName in uploadedImgNames) {
                            newImages.Add(new ItemImage {
                                ImageFileName = imgFileName
                            });
                        }
                        AddImagesToItem(editedItem, newImages);
                    }

                    // Check if any images to remove and do so
                    if (imagesToRemoveIds != null && imagesToRemoveIds.Any()) {
                        foreach (int imageId in imagesToRemoveIds) {
                            if (await IsImageOwnedByGivenItem(editedItem.Id, imageId)) {
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
                    ts.Complete();
                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Add the Item to the database
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="Images"></param>
        /// <returns></returns>
        public async Task AddItem(Item newItem, ICollection<IFormFile> images, string imgUploadPath) {
            // Might not be working the way I want it to but Transactions should ensure that when the file does not get created then it wont add the item to the database
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // Add images to the item
                    if (images != null && images.Any()) {
                        // instantiate a new image list for the item
                        newItem.Images = new List<ItemImage>();
                        // List for Images to add
                        List<ItemImage> imagesToAdd = new List<ItemImage>();
                        List<string> uploadedImgNames = await ImageManager.UploadImages(images, imgUploadPath);
                        foreach (string imgFileName in uploadedImgNames) {
                            imagesToAdd.Add(new ItemImage {
                                ImageFileName = imgFileName
                            });
                        }
                        AddImagesToItem(newItem, imagesToAdd);
                    }
                    // Add item to Database
                    _context.Add(newItem);
                    await _context.SaveChangesAsync();
                    // Complete the transaction
                    ts.Complete();
                }
            }
            catch (Exception e) {
                throw e;
            }
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
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task RemoveImageWithoutSaveChanges(int imageId) {
            ItemImage image = await GetImageById(imageId);
            _context.ItemImages.Remove(image);
        }

        /// <summary>
        /// Add given images to the given Item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public Item AddImagesToItem(Item item, ICollection<ItemImage> images) {
            foreach (ItemImage image in images) {
                item.Images.Add(image);
            }
            return item;
        }

        /// <summary>
        /// Add an image to the Database.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task AddImage(ItemImage image) {
            _context.ItemImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task<ItemImage> GetImageById(int? imageId) {
            if (imageId == null) {
                return null;
            }

            return await _context.ItemImages.FirstOrDefaultAsync(img => img.Id == imageId);

        }


        /// <summary>
        /// Check if given Image id is associated with the given Item id
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<bool> IsImageOwnedByGivenItem(int itemId, int imageId) {
            Item item = await GetItemById(itemId);
            if (item.Images.Any(x => x.Id == imageId)) {
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
