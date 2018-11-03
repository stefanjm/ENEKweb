using ENEKdata.Models.Leiunurk;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ENEKdata {
    public interface ILeiunurk {
        /// <summary>
        /// Get all items 
        /// </summary>
        /// <returns>All items</returns>
        Task<List<Item>> GetAllItems();

        //IEnumerable<Image> GetAllImages();

        Task<Item> GetItemById(int? Id);
        Task AddItem(Item newItem, ICollection<IFormFile> images, string imgUploadPath);
        Task RemoveItem(int id);
        Task EditItem(Item editedItem, List<int> imagesToRemoveIds, ICollection<IFormFile> imagesToAdd, string imgUploadPath);

        Task<bool> IsImageOwnedByGivenItem(int itemId, int imageId);
        Task<bool> ItemExists(int id);
    }
}
