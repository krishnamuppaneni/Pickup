using System.ComponentModel.DataAnnotations;

namespace Pickup.Api.Models.Identity
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
