using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ENEKdata.Models.Partnerid {
    public class Partner {

        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(95)]
        public string Description { get; set; }

        [MaxLength(80)]
        public string PartnerURL { get; set; }

        public PartnerImage Image { get; set; }
    }
}
