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
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}
