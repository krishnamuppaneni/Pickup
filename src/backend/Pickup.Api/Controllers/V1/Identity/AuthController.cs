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

            var result = await _authService.ConfirmEmailAsync(user, request.Code);

            if (result.Succeeded)
                return Ok();

            return BadRequest(ErrorHelper.CreateErrorRespose(result.Errors));
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
        [ProducesResponseType(typeof(ErrorResponse), 400)]
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

        /// <summary>
        /// Log in with TFA 
        /// </summary>
        /// <param name="request">LoginWith2faRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("tfa")]
        public async Task<IActionResult> LoginWith2faAsync([FromBody]LoginWith2faRequest request)
        {

            var user = await _userService.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest(ErrorHelper.CreateErrorRespose("Invalid credentials."));

            AuthenticationResult result = await _authService.LoginWith2FaAsync(user, request.TwoFactorCode);
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

        /// <summary>
        /// Register an account
        /// </summary>
        /// <param name="request">RegisterRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterRequest request)
        {

            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate
            };

            var result = await _authService.RegisterAsync(user, request.Password);

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

        /// <summary>
        /// Resend email verification email with token link
        /// </summary>
        /// <param name="request">ResendVerificationEmailRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("resendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmailAsync([FromBody]ResendVerificationEmailRequest request)
        {
            var user = await _userService.FindByEmailAsync(request.Email);
            if (user == null)
                return Ok();
            if (user.EmailConfirmed == true)
                return BadRequest(ErrorHelper.CreateErrorRespose("Email has already been confirmed."));

            await _authService.ResendVerificationEmailAsync(user);

            return Ok();
        }

        /// <summary>
        /// Reset account password with reset token
        /// </summary>
        /// <param name="request">ReSetPasswordRequest</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordRequest request)
        {
            var user = await _userService.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return BadRequest(ErrorHelper.CreateErrorRespose("Invalid credentials."));
            }
            IdentityResult result = await _authService.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(ErrorHelper.CreateErrorRespose(result.Errors));
        }
    }
}
