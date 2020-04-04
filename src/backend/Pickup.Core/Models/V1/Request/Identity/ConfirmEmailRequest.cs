using System.ComponentModel.DataAnnotations;

namespace Pickup.Core.Models.V1.Request.Identity
{
    public class ConfirmEmailRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
