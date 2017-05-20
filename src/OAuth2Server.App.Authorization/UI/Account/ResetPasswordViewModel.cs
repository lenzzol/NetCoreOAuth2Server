using System.ComponentModel.DataAnnotations;

namespace OAuth2Server.App.Authorization.UI.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z]{1,})(?=.*?[a-z]{1,})(?=(.*[\d]){1,})(?!.*\s).{0,}$", ErrorMessage = @"The {0} must contain at least 1 digit, 1 lowercase letter, and 1 uppercase letter.  ")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
