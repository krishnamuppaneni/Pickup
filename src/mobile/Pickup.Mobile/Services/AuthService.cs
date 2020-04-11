using Microsoft.Extensions.Options;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response.Identity;
using Pickup.Mobile.Services.Api;
using Pickup.Mobile.Settings;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly RestClient _restClient;

        public AuthService(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<ApiResponse<TokenResponse>> RegisterAsync(RegisterRequest request)
        {
            return await _restClient.GetClient<IAuthApi>().RegisterAsync(request);
        }
    }
}