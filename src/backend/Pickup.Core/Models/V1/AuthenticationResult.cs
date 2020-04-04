using System.Collections.Generic;

namespace Pickup.Core.Models
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public bool Success { get; set; }

        public bool? HasVerifiedEmail { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}