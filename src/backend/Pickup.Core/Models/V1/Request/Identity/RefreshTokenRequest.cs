using System;
using System.Collections.Generic;
using System.Text;

namespace Pickup.Core.Models.V1.Request.Identity
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
