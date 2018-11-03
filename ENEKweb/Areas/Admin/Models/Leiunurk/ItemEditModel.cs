using ENEKdata.Models.Leiunurk;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ENEKweb.Areas.Admin.Models.Leiunurk {
    public class ItemEditModel {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public IList<ImageEditModel> Images { get; set; }

        public ICollection<IFormFile> ImagesToAdd { get; set; }

    }

    public class ImageEditModel {
        public int Id { get; set; }

        public string ImageFileName { get; set; }

        public bool RemoveImage { get; set; }

    }
}
