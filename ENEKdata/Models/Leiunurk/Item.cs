using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ENEKdata.Models.Leiunurk {
    public class Item {
        
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MaxLength(95)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        /// <summary>
        /// Create EF One to many relationship with images.
        /// </summary>
        public ICollection<ItemImage> Images { get; set; }
    }
}
