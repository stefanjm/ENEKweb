using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ENEKdata.Models.TehtudTood {
    /// <summary>
    /// Tehtud töö model
    /// </summary>
    public class TehtudToo {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(2000,2050)]
        public int YearDone { get; set; }


        public string BuildingType { get; set; }

        /// <summary>
        /// Create EF One to many relationship with images.
        /// </summary>
        public ICollection<TehtudTooImage> Images { get; set; }

    }
}
