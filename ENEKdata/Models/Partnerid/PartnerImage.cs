using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENEKdata.Models.Partnerid {
    public class PartnerImage {

        [ForeignKey("Partner")]
        public int Id { get; set; }

        [Required]
        public string ImageFileName { get; set; }
        [Required]
        public Partner Partner { get; set; }
    }
}
