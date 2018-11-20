﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ENEKdata;
using ENEKdata.Models.Partnerid;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ENEKservices {
    public class PartneridService : IPartnerid {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly ENEKdataDbContext _context;

        /// <summary>
        /// Initialize database context
        /// </summary>
        /// <param name="context"></param>
        public PartneridService(ENEKdataDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Get all Partners
        /// </summary>
        /// <returns>List containing all Partners</returns>
        public async Task<List<Partner>> GetAllPartners() {
            return await _context.Partners.Include(partner => partner.Image).ToListAsync();
        }

        /// <summary>
        /// Add a new Partner to the database, upload image if given
        /// </summary>
        /// <param name="newPartner"></param>
        /// <param name="image"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        public async Task AddPartner(Partner newPartner, IFormFile image, string imgUploadPath) {
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // Add image to the item
                    if (image != null) {
                        string uploadedImgName = await ImageManager.UploadImage(image, imgUploadPath);
                        if (uploadedImgName != null)
                            newPartner.Image = new PartnerImage { ImageFileName = uploadedImgName };
                    }
                    // Add item to Database
                    _context.Add(newPartner);
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
        /// Edit Partner, upload/remove the image
        /// </summary>
        /// <param name="editedPartner"></param>
        /// <param name="image"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        public async Task EditPartner(Partner editedPartner, IFormFile image, string imgUploadPath) {
            // Stop executing method if Partner with given ID doesn't exist in database
            if (!await PartnerExists(editedPartner.Id)) {
                return;
            }

            // Transactions should also check for the file IO, if that throws an error then it wont update the database
            try {
                using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    // If image is not null then change out the image
                    if (image != null) {
                        string uploadedImgName = await ImageManager.UploadImage(image, imgUploadPath);
                        editedPartner.Image = new PartnerImage { ImageFileName = uploadedImgName };
                    }


                    _context.Update(editedPartner);
                    await _context.SaveChangesAsync();
                    ts.Complete();
                }
            }
            catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Get Partner by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Partner object</returns>
        public async Task<Partner> GetPartnerById(int? id) {
            if (id != null) {
                return await _context.Partners.AsNoTracking().Include(partner => partner.Image).FirstOrDefaultAsync(partner => partner.Id == id);
            }
            else {
                return null;
            }
        }


        /// <summary>
        /// Remove Partner with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemovePartner(int id) {
            _context.Partners.Remove(await GetPartnerById(id));
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Check if Partner exists in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Boolean of whether Partner exists</returns>
        public Task<bool> PartnerExists(int id) {
            throw new System.NotImplementedException();
        }
    }
}
