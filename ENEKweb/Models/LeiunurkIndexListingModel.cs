using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENEKweb.Models {
    public class LeiunurkIndexListingModel {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<ItemImages> Images { get; set; }
    }

    public class ItemImages {
        public string ImageFileName { get; set; }
    }
}
