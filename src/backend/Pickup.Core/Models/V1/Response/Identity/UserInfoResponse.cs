using System.Collections.Generic;

namespace Pickup.Core.Models.V1.Response.Identity
{
    public class UserInfoResponse
    {
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public IList<string> Roles { get; set; }
    }
}
