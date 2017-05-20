using System.ComponentModel.DataAnnotations;

namespace OAuth2Server.App.Authorization.UI.Account
{
    public class ConfirmAccountViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z]{1,})(?=.*?[a-z]{1,})(?=(.*[\d]){1,})(?!.*\s).{0,}$", ErrorMessage = @"The {0} must contain at least 1 digit, 1 lowercase letter, and 1 uppercase letter.  ")]
        [DataType(DataType.Password)]
        [Display(Name = "Enter your Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Re-enter your password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public bool AlreadyConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Enter your account e-mail")]
        public string Email { get; set; }
    }
}
