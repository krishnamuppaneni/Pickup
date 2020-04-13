using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pickup.Api.Infrastructure.Helpers;
using Pickup.Api.Settings;
using Pickup.Core.Models;
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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthService(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SecurityContext securityContext,
            IEmailService emailService,
            TokenValidationParameters tokenValidationParameters,
            IOptions<ClientAppSettings> client,
            IOptions<JwtSecurityTokenSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _securityContext = securityContext;
            _emailService = emailService;
            _tokenValidationParameters = tokenValidationParameters;
            _client = client.Value;
            _jwtSettings = jwtSettings.Value;
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
                    return new AuthenticationResult()
                    {
                        Errors = ErrorHelper.CreateErrorList("This account has been locked.")
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
                        Errors = ErrorHelper.CreateErrorList("Invalid credentials.")
                    };
                }
            }
            return new AuthenticationResult
            {
                Errors = ErrorHelper.CreateErrorList("Invalid credentials.")
            };

        }

        public async Task<AuthenticationResult> LoginWith2FaAsync(User user, string twoFactorCode)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", twoFactorCode)
                ? await GenerateAuthenticationResultForUserAsync(user)
                : new AuthenticationResult
                {
                    Errors = ErrorHelper.CreateErrorList("Unable to verify authenticator code.")
                };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("Invalid Token") };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This token hasn't expired yet") };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _securityContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This refresh token does not exist") };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This refresh token has expired" ) };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This refresh token has been invalidated" )};
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This refresh token has been used" ) };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = ErrorHelper.CreateErrorList("This refresh token does not match this JWT" ) };
            }

            storedRefreshToken.Used = true;
            _securityContext.RefreshTokens.Update(storedRefreshToken);
            await _securityContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
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
                Errors = ErrorHelper.CreateErrorList(result.Errors)
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

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }


        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
