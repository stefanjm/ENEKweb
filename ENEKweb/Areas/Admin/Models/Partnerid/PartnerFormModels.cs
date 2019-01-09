using ENEKdata.Models.Partnerid;
using ENEKdata.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ENEKweb.Areas.Admin.Models.Partnerid {


    public class PartnerFormModel  : Partner{

        //public int Id { get; set; }

        //[Required]
        //[MaxLength(25)]
        //public string Name { get; set; }

        //[MaxLength(95)]
        //public string Description { get; set; }

        // override PartnerURL property 
        
        [Url]
        [MaxLength(80)]
        [Display(Name = "Partners website link")]
        public new string PartnerURL { get; set; }

        // override Image property 
        public new PartnerFormImageModel Image { get; set; }

        [IsImage]
        [DataType(DataType.Upload)]
        public IFormFile UploadImage { get; set; }

        public bool RemoveImage { get; set; } = false;
    }


    public class PartnerFormImageModel {
        public int Id { get; set; }

        public string ImageFileName { get; set; }

    }

    // Custom validation for image upload
    public class IsImageAttribute : ValidationAttribute {
        private readonly string _errorMsg = "The Image must be one of these types: jpg, jpeg, png";


        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

            IFormFile image = value as IFormFile;
            if (image != null) {
                if (!FormFileExtensions.IsImage(image)) {
                    return new ValidationResult(_errorMsg);
                }
            }
            return ValidationResult.Success;
        }
    }
}
