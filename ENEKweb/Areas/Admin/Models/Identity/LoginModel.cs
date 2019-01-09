using System.ComponentModel.DataAnnotations;

namespace ENEKweb.Areas.Admin.Models.Identity {
    public class LoginModel {

        public string ReturnUrl { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}
