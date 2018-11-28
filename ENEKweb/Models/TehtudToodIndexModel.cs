using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENEKweb.Models {
    public class TehtudToodIndexModel {

        public IEnumerable<TehtudToodIndexListingModel> TehtudToodViewList { get; set; }

        public IEnumerable<int> TehtudToodYears { get; set; }

    }
}
