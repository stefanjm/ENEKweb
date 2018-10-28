using ENEKdata.Models.Leiunurk;
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
        Task AddItem(Item newItem);
        Task RemoveItem(int id);
        Task EditItem(Item item);

        Task<bool> ItemExists(int id);
    }
}
