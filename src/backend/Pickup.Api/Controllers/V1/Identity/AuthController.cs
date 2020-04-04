using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pickup.Api.Infrastructure.Helpers;
using Pickup.Api.Services;
using Pickup.Api.Settings;
using Pickup.Core.Models;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response;
using Pickup.Core.Models.V1.Response.Identity;
using Pickup.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pickup.Api.Controllers.V1.Identity
{
    [Produces("application/json")]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Confirms a user email address
        /// </summary>
        /// <param name="request">ConfirmEmailRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody]ConfirmEmailRequest request)
        {
            var user = await _userService.FindByIdAsync(request.UserId);
            if (user == null)
                return BadRequest(ErrorHelper.CreateErrorRespose("Invalid confirmation code."));

            var response = await _authService.ConfirmEmailAsync(user, request.Code);

            if (response.Succeeded)
                return Ok();

            return BadRequest(ErrorHelper.CreateErrorRespose(response));
        }

        /// <summary>
        /// Forgot email sends an email with a link containing reset token
        /// </summary>
        /// <param name="request">ForgotPasswordRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("forgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody]ForgotPasswordRequest request)
        {
            var user = await _userService.FindByEmailAsync(request.Email);

            if (user == null || !await _userService.IsEmailConfirmedAsync(user))
            {
                return BadRequest(ErrorHelper.CreateErrorRespose("Please verify your email address"));
            }

            await _authService.ForgotPasswordAsync(user);
            return Ok();
        }

        /// <summary>
        /// Log into account
        /// </summary>
        /// <param name="request">LoginRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request)
        {
            var user = await _userService.FindByEmailAsync(request.Email);
            AuthenticationResult result = await _authService.LoginAsync(user, request.Password);
            if (result.Success)
            {
                TokenResponse response = new TokenResponse()
                {
                    HasVerifiedEmail = result.HasVerifiedEmail,
                    TFAEnabled = result.TwoFactorEnabled,
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                };
                return Ok(response);
            }

            return BadRequest(ErrorHelper.CreateErrorRespose(result.Errors));
        }

       

        /*
        /// <summary>
        /// Register an account
        /// </summary>
        /// <param name="model">RegisterRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"{_client.Url}{_client.EmailConfirmationPath}?uid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";

                await _emailService.SendEmailConfirmationAsync(model.Email, callbackUrl);

                return Ok();
            }

            return BadRequest(result.Errors.Select(x => x.Description));
        }
       

        /// <summary>
        /// Log in with TFA 
        /// </summary>
        /// <param name="model">LoginWith2faRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("tfa")]
        public async Task<IActionResult> LoginWith2fa([FromBody]LoginWith2faRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new string[] { "Invalid credentials." });

            if (await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", model.TwoFactorCode))
            {
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);

                var TokenResponse = new TokenResponse()
                {
                    HasVerifiedEmail = true,
                    TFAEnabled = false,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
                };

                return Ok(TokenResponse);
            }
            return BadRequest(new string[] { "Unable to verify Authenticator Code!" });
        }        

        /// <summary>
        /// Reset account password with reset token
        /// </summary>
        /// <param name="model">ReSetPasswordRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(new string[] { "Invalid credentials." });
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors.Select(x => x.Description));
        }

        /// <summary>
        /// Resend email verification email with token link
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IEnumerable<string>), 400)]
        [Route("resendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody]ResendVerificationEmailRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new string[] { "Could not find user." });
            if (user.EmailConfirmed == true)
                return BadRequest(new string[] { "Email has already been confirmed." });

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = $"{_client.Url}{_client.EmailConfirmationPath}?userid={user.Id}&code={System.Net.WebUtility.UrlEncode(code)}";
            await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl);

            return Ok();
        }

        */
    }
}
