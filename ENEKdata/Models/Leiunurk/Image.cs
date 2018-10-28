using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ENEKdata.Models.Leiunurk {
    public class Image {
        public int Id { get; set; }
        [Required]
        public string ImagePath { get; set; }

        public int? ItemId { get; set; }
        public Item Item { get; set; }
     
    }
}
