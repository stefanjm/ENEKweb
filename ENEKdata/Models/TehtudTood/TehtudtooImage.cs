using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ENEKdata.Models.TehtudTood {
    /// <summary>
    /// Tehtud töö image model
    /// </summary>
   public class TehtudTooImage {

        public int Id { get; set; }
        [Required]
        public string ImageFileName { get; set; }
        [Required]
        public int? TehtudtooId { get; set; }
        [Required]
        public TehtudToo Tehtudtoo { get; set; }
    }
}
