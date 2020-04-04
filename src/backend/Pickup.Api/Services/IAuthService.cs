using Microsoft.AspNetCore.Identity;
using Pickup.Core.Models;
using Pickup.Data.Entities;
using System.Threading.Tasks;

namespace Pickup.Api.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task ForgotPasswordAsync(User user);
        Task<AuthenticationResult> LoginAsync(User user, string password);
        Task<AuthenticationResult> LoginWith2FaAsync(User user, string twoFactorCode);
        Task<AuthenticationResult> RegisterAsync(User user, string password);
        Task ResendVerificationEmailAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    }
}