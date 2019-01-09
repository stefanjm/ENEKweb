using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ENEKdata.Models.Leiunurk {
    public class ItemImage {
        public int Id { get; set; }
        [Required]
        public string ImageFileName { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public Item Item { get; set; }
     
    }
}
