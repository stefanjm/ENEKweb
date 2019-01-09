using ENEKdata.Models.TehtudTood;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ENEKweb.Areas.Admin.Models.TehtudTood {
    public class TehtudTooModel : TehtudToo {

        // Inherit main properties from the entity model

        [Required]
        [Range(2000, 2050)]
        [DisplayName("Year when finished")]
        public new int YearDone { get; set; }

        [DisplayName("Type of property worked on")]
        public new string BuildingType { get; set; }

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
        private readonly string _errorMsg = "The Image must be one of these types: jpg, jpeg, png";


        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

            ICollection<IFormFile> images = value as ICollection<IFormFile>;
            if (images != null && images.Any()) {
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
