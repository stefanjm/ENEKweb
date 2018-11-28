using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENEKweb.Models {
    public class TehtudToodIndexListingModel {


        public string Name { get; set; }
        public int YearDone { get; set; }

        public string BuildingType { get; set; }

        public IEnumerable<TehtudTooImages> Images { get; set; }
    }

    public class TehtudTooImages {
        public string ImageFileName { get; set; }
    }

}
