using System.ComponentModel.DataAnnotations;

namespace LiftNet.Api.Contracts.Requests
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email
        {
            get; set;
        }
        [Required]
        public string Username
        {
            get; set;
        }
        [Required]
        public string Name
        {
            get; set;
        }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z]).{6,}$", ErrorMessage = "Password must contain at least one digit and one lowercase letter.")]
        public string Password
        {
            get; set;
        }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmedPassword
        {
            get; set;
        }
    }
}
