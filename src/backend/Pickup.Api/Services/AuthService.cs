using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Pickup.Api.Settings;
using Pickup.Core.Models;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Data;
using Pickup.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pickup.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SecurityContext _securityContext;
        private readonly IEmailService _emailService;
        private readonly ClientAppSettings _client;
        private readonly JwtSecurityTokenSettings _jwtSettings;

        public AuthService(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SecurityContext securityContext,
            IEmailService emailService,
            ClientAppSettings client,
            JwtSecurityTokenSettings jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _securityContext = securityContext;
            _emailService = emailService;
            _client = client;
            _jwtSettings = jwtSettings;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task ForgotPasswordAsync(User user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_client.Url}{_client.ResetPasswordPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";

            await _emailService.SendPasswordResetAsync(user.Email, callbackUrl);
        }

        public async Task<AuthenticationResult> LoginAsync(User user, string password)
        {
            if (user != null)
            {
                if (user.LockoutEnabled)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "This account has been locked." }
                    };
                }

                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    if (user.TwoFactorEnabled)
                    {
                        return new AuthenticationResult()
                        {
                            HasVerifiedEmail = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            Success = true
                        };
                    }
                    else
                    {
                        return await GenerateAuthenticationResultForUserAsync(user);
                    }
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Invalid credentials." }
                    };
                }
            }
            return new AuthenticationResult
            {
                Errors = new[] { "Invalid credentials." }
            };

        }

        public async Task<AuthenticationResult> LoginWith2FaAsync(User user, string twoFactorCode)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", twoFactorCode)
                ? await GenerateAuthenticationResultForUserAsync(user)
                : new AuthenticationResult
                {
                    Errors = new[] { "Unable to verify authenticator code." }
                };
        }

        public async Task<AuthenticationResult> RegisterAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"{_client.Url}{_client.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";

                await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl);
                return await GenerateAuthenticationResultForUserAsync(user);
            }
            return new AuthenticationResult()
            {
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task ResendVerificationEmailAsync(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = $"{_client.Url}{_client.EmailConfirmationPath}?userid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
            await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.DurationInMinutes),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _securityContext.RefreshTokens.AddAsync(refreshToken);
            await _securityContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                HasVerifiedEmail = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled
            };
        }
    }
}
