using ENEKdata.Models.Partnerid;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ENEKdata {
    public interface IPartnerid {

        /// <summary>
        /// Get all Partners
        /// </summary>
        /// <returns>List containing all Partners</returns>
        Task<List<Partner>> GetAllPartners();

        //IEnumerable<Image> GetAllImages();

        /// <summary>
        /// Get Partner by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Partner> GetPartnerById(int? id);

        /// <summary>
        /// Add a new Partner to the database, upload image if given
        /// </summary>
        /// <param name="newPartner"></param>
        /// <param name="image"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        Task AddPartner(Partner newPartner, IFormFile image, string imgUploadPath);

        /// <summary>
        /// Remove Partner with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemovePartner(int id);

        /// <summary>
        /// Edit Partner, upload/remove the image
        /// </summary>
        /// <param name="editedPartner"></param>
        /// <param name="image"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        Task EditPartner(Partner editedPartner, IFormFile uploadedImage, bool removeImage, string imgUploadPath);

        /// <summary>
        /// Check if Partner exists in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> PartnerExists(int id);
    }
}
