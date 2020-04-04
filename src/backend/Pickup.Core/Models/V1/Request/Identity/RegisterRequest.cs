using System;
using System.ComponentModel.DataAnnotations;

namespace Pickup.Core.Models.V1.Request.Identity
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
