using System.ComponentModel.DataAnnotations;

namespace Pickup.Core.Models.V1.Request.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
