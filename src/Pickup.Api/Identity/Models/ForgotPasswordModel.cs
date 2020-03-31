using System.ComponentModel.DataAnnotations;

namespace Pickup.Api.Identity.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
