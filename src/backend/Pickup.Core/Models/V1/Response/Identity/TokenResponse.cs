namespace Pickup.Core.Models.V1.Response.Identity
{
    public class TokenResponse
    {
        public bool? HasVerifiedEmail { get; set; }

        public bool? TFAEnabled { get; set; }

        public string RefreshToken { get; set; }

        public string Token { get; set; }
    }
}
