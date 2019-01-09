using Microsoft.AspNetCore.Http;
using ENEKdata.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ENEKdata.Models.Leiunurk;

namespace ENEKweb.Areas.Admin.Models.Leiunurk {
    public class ItemModel : Item{

        //public int Id { get; set; }

        //[Required]
        //[MaxLength(25)]
        //public string Name { get; set; }
        //[Required]
        //[MaxLength(95)]
        //public string Description { get; set; }

        //[DataType(DataType.Currency, ErrorMessage = "Must be a digit and separated by a comma ( , ), or 0 if there's no price")]
        //public decimal Price { get; set; }

        // Overwrite the entity model Image property
        public new IList<ImageEditModel> Images { get; set; }

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

    // Custom validation for image upload
    public class IsImageAttribute : ValidationAttribute {
        private readonly string  _errorMsg = "The Image must be one of these types: jpg, jpeg, png";

    
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
