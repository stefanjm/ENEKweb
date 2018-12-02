using System.ComponentModel.DataAnnotations;

namespace ENEKweb.Areas.Admin.Models.Administrator {
    public class AdminUserModel {

        public string Id { get; set; }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
