using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ENEKdata.Models.Partnerid {
    public class Partner {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public PartnerImage Image { get; set; }
    }
}
