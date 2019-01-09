using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ENEKdata;
using ENEKdata.Models.TehtudTood;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ENEKservices {
    public class TehtudToodService : ITehtudTood {

        /// <summary>
        /// Database context
        /// </summary>
        private readonly ENEKdataDbContext _context;

        /// <summary>
        /// Initialize database context
        /// </summary>
        /// <param name="context"></param>
        public TehtudToodService(ENEKdataDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Get all Tehtud tööd including images
        /// </summary>
        /// <returns>List of all Tehtud tlld</returns>
        public async Task<List<TehtudToo>> GetAllTehtudTood() {
            return await _context.TehtudTood.Include(too => too.Images).ToListAsync();
        }

        /// <summary>
        /// Get Tehtud töö by given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Tehtud too if found, otherwise NULL</returns>
        public async Task<TehtudToo> GetTehtudTooById(int? id) {
            if (id == null) {
                return null;
            }

            return await _context.TehtudTood.AsNoTracking()
                .Include(too => too.Images).FirstOrDefaultAsync(i => i.Id == id);

        }

        /// <summary>
        /// Add Tehtud töö to the database and upload images
        /// </summary>
        /// <param name="newTehtudToo"></param>
        /// <param name="images"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        public async Task AddTehtudToo(TehtudToo newTehtudToo, ICollection<IFormFile> images, string imgUploadPath) {
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // Add images to Tehtud töö
                    if (images != null && images.Any()) {
                        // instantiate a new image list for Tehtud töö
                        newTehtudToo.Images = new List<TehtudTooImage>();
                        // List for Images to add
                        List<TehtudTooImage> imagesToAdd = new List<TehtudTooImage>();
                        List<string> uploadedImgNames = await ImageManager.UploadImages(images, imgUploadPath);
                        foreach (string imgFileName in uploadedImgNames) {
                            imagesToAdd.Add(new TehtudTooImage {
                                ImageFileName = imgFileName
                            });
                        }
                        // Add images to the new Tehtud töö
                        foreach (TehtudTooImage img in imagesToAdd) { newTehtudToo.Images.Add(img); };
                    }
                    // Add Tehtud töö to Database
                    _context.Add(newTehtudToo);
                    await _context.SaveChangesAsync();
                    // Complete the transaction
                    ts.Complete();
                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        public async Task EditTehtudToo(TehtudToo editedTehtudToo, List<int> imagesToRemoveIds, ICollection<IFormFile> imagesToAdd, string imgUploadPath) {

            // Check if Tehtud töö with Edited Tehtud töö ID exists in the database
            if (!await TehtudTooExists(editedTehtudToo.Id)) {
                return;
            }

            // Transaction for IO not working as of now
            //  try and catch for DbUpdateConcurrencyException
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // Check if any images to Add and do so
                    if (imagesToAdd != null && imagesToAdd.Any()) {
                        // List for new Images to add
                        List<TehtudTooImage> newImages = new List<TehtudTooImage>();
                        // upload the images to the given path
                        List<string> uploadedImgNames = await ImageManager.UploadImages(imagesToAdd, imgUploadPath);
                        foreach (string imgFileName in uploadedImgNames) {
                            newImages.Add(new TehtudTooImage {
                                ImageFileName = imgFileName
                            });
                        }
                        // Add images to the new Tehtud töö
                        foreach (TehtudTooImage img in newImages) { editedTehtudToo.Images.Add(img); };
                    }

                    // Check if any images to remove and do so
                    if (imagesToRemoveIds != null && imagesToRemoveIds.Any()) {
                        foreach (int imageId in imagesToRemoveIds) {
                            if (await IsImageOwnedByGivenTehtudToo(editedTehtudToo.Id, imageId)) {
                                // Remove the image from this list and also add it to the DB queue to query when SaveChanges is called to remove it from DB as well
                                // Have to do both because, if just removing from list. Updating thinks we didn't touch the image at all,
                                //  if we would remove the image only from the Database and not from the list right away. The list would add it back:(
                                editedTehtudToo.Images.Remove(editedTehtudToo.Images.First(x => x.Id == imageId));
                                await RemoveImageWithoutSaveChanges(imageId);
                            }
                        }
                    }

                    _context.Update(editedTehtudToo);
                    await _context.SaveChangesAsync();
                    ts.Complete();
                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Remove Tehtud töö, since images are tied to Tehtud töö, they will also be removed from the Database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveTehtudToo(int id) {
            TehtudToo tehtudToo = await GetTehtudTooById(id);
            _context.TehtudTood.Remove(tehtudToo);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove an image from the Tehtud töö without saving changes to database
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task RemoveImageWithoutSaveChanges(int imageId) {
            TehtudTooImage image = await _context.TehtudTooImages.FirstOrDefaultAsync(img => img.Id == imageId);
            if (image != null) {
                _context.TehtudTooImages.Remove(image);
            }

        }

        /// <summary>
        /// Check if an image is owned by the given Tehtud töö
        /// </summary>
        /// <param name="tehtudTooId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task<bool> IsImageOwnedByGivenTehtudToo(int tehtudTooId, int imageId) {
            TehtudToo tehtudToo = await GetTehtudTooById(tehtudTooId);
            if (tehtudToo.Images.Any(x => x.Id == imageId)) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Check if Tehtud töö exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> TehtudTooExists(int id) {
            return _context.TehtudTood.AnyAsync(e => e.Id == id);
        }


    }
}
