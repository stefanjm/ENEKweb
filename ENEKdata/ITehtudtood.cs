using System.Collections.Generic;
using System.Threading.Tasks;
using ENEKdata.Models.TehtudTood;
using Microsoft.AspNetCore.Http;

namespace ENEKdata {
    public interface ITehtudTood {

        /// <summary>
        /// Get all tehtud tööd
        /// </summary>
        /// <returns>All items</returns>
        Task<List<TehtudToo>> GetAllTehtudTood();

        /// <summary>
        /// Get Tehtud töö by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<TehtudToo> GetTehtudTooById(int? id);

        /// <summary>
        /// Add new Tehtud töö
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="images"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        Task AddTehtudToo(TehtudToo newItem, ICollection<IFormFile> images, string imgUploadPath);

        /// <summary>
        /// Remove Tehtud töö
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveTehtudToo(int id);

        /// <summary>
        /// Edit Tehtud töö
        /// </summary>
        /// <param name="editedTehtudToo"></param>
        /// <param name="imagesToRemoveIds"></param>
        /// <param name="imagesToAdd"></param>
        /// <param name="imgUploadPath"></param>
        /// <returns></returns>
        Task EditTehtudToo(TehtudToo editedTehtudToo, List<int> imagesToRemoveIds, ICollection<IFormFile> imagesToAdd, string imgUploadPath);

        /// <summary>
        /// Check if image is owned by the given Tehtud töö
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task<bool> IsImageOwnedByGivenTehtudToo(int tehtudTooId, int imageId);

        /// <summary>
        /// Check if Tehtud töö exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> TehtudTooExists(int id);

    }
}
