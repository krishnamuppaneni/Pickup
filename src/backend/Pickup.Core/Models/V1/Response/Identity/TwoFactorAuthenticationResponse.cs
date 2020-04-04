namespace Pickup.Core.Models.V1.Response.Identity
{
    public class TwoFactorAuthenticationResponse
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }
    }
}
