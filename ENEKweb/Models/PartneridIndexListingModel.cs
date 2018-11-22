
namespace ENEKweb.Models {
    public class PartneridIndexListingModel {
        public string Name { get; set; }
        public string Description { get; set; }

        public PartnerImageModel Image { get; set; }
    }

    public class PartnerImageModel {
        public string ImageFileName { get; set; }
    }
}
