using Microsoft.AspNetCore.Http;
using ENEKdata.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ENEKweb.Areas.Admin.Models.Leiunurk {
    public class ItemModel {

        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MaxLength(95)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public IList<ImageEditModel> Images { get; set; }

        [IsImage]
        [DataType(DataType.Upload)]
        [DisplayName("Images")]
        public ICollection<IFormFile> ImagesToAdd { get; set; }

    }

    public class ImageEditModel {
        public int Id { get; set; }

        public string ImageFileName { get; set; }

        public bool RemoveImage { get; set; }

    }

    // Custom validation
    public class IsImageAttribute : ValidationAttribute {
        private readonly string  _errorMsg = "The Image must be one of these types: jpg, jpeg, gif, png";

    
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

            ICollection<IFormFile> images = value as ICollection<IFormFile>;
            if( images != null && images.Any()) {
                foreach (var img in images) {
                    if (!FormFileExtensions.IsImage(img)) {
                        return new ValidationResult(_errorMsg);
                    }

                }
            }
         

            return ValidationResult.Success;
        }
    }
}
