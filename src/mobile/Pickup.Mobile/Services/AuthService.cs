using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response.Identity;
using Pickup.Mobile.Helpers;
using Pickup.Mobile.Models;
using Pickup.Mobile.Services.Api;
using Pickup.Mobile.Settings;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pickup.Mobile.Services
{
    public class AuthService : IAuthService
    {       
        private readonly RestClient _restClient;
        private readonly IUserService _userService;

        public AuthService(RestClient restClient, IUserService userService)
        {
            _restClient = restClient;
            _userService = userService;
        }

        bool IsTokenExpired(string token)
        {
            // Get just the JWT part of the token (without the signature).
            var jwt = token.Split(new Char[] { '.' })[1];

            // Undo the URL encoding.
            jwt = jwt.Replace('-', '+').Replace('_', '/');
            switch (jwt.Length % 4)
            {
                case 0: break;
                case 2: jwt += "=="; break;
                case 3: jwt += "="; break;
                default:
                    throw new ArgumentException("The token is not a valid Base64 string.");
            }

            // Convert to a JSON String
            var bytes = Convert.FromBase64String(jwt);
            string jsonString = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            // Parse as JSON object and get the exp field value,
            // which is the expiration date as a JavaScript primative date.
            JObject jsonObj = JObject.Parse(jsonString);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // Calculate the expiration by adding the exp value (in seconds) to the
            // base date of 1/1/1970.
            DateTime minTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var expire = minTime.AddSeconds(exp);
            return (expire < DateTime.UtcNow);
        }

        public async Task<User> LoginAsync()
        {
            TokenModel token = await TokenHelper.RetrieveTokenFromSecureStoreAsync();
            if (token != null)
            {
                if (IsTokenExpired(token.Token))
                {
                    try
                    {
                        RefreshTokenRequest tokenRequest = new RefreshTokenRequest()
                        {
                            RefreshToken = token.RefreshToken,
                            Token = token.Token
                        };
                        ApiResponse<TokenResponse> response = await _restClient.GetClient<IAuthApi>().RefreshTokenAsync(tokenRequest);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw response.Error;
                        }
                        token.RefreshToken = response.Content.RefreshToken;
                        token.Token = response.Content.Token;
                        await TokenHelper.StoreTokenInSecureStore(token);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Could not refresh token: {ex.Message}");
                    }
                }
                return await _userService.GetUserInfo();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">LoginRequest</param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<User> LoginAsync(LoginRequest request)
        {
            ApiResponse<TokenResponse> response = await _restClient.GetClient<IAuthApi>().LoginAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw response.Error;
            }

            TokenModel token = new TokenModel()
            {
                RefreshToken = response.Content.RefreshToken,
                Token = response.Content.Token
            };
            await TokenHelper.StoreTokenInSecureStore(token);
            User user = await _userService.GetUserInfo();
            App.CurrentUser = user;
            return App.CurrentUser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">RegisterRequest</param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            ApiResponse<TokenResponse> response = await _restClient.GetClient<IAuthApi>().RegisterAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw response.Error;
            }

            TokenModel token = new TokenModel()
            {
                RefreshToken = response.Content.RefreshToken,
                Token = response.Content.Token
            };
            await TokenHelper.StoreTokenInSecureStore(token);
            User user = await _userService.GetUserInfo();
            App.CurrentUser = user;
            return App.CurrentUser;
        }
    }
}