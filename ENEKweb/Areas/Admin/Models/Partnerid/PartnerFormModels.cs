using ENEKdata.Models.Partnerid;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ENEKweb.Areas.Admin.Models.Partnerid {


    public class PartnerFormModel {

        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [MaxLength(95)]
        public string Description { get; set; }

        public PartnerFormImageModel Image { get; set; }

        [IsImage]
        public IFormFile UploadImage { get; set; }

        public bool RemoveImage { get; set; } = false;
    }


    public class PartnerFormImageModel {
        public int Id { get; set; }

        public string ImageFileName { get; set; }

        public bool RemoveImage { get; set; }

    }

    // Custom validation for image upload
    public class IsImageAttribute : ValidationAttribute {
        private readonly string _errorMsg = "The Image must be one of these types: jpg, jpeg, gif, png";


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
