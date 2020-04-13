using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response.Identity;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services.Api
{
    public interface IAuthApi
    {        
        [Post("/api/v1/auth/login")]
        public Task<ApiResponse<TokenResponse>> LoginAsync([Body] LoginRequest loginRequest);

        [Post("/api/v1/auth/refresh")]
        public Task<ApiResponse<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest registerRequest);

        [Post("/api/v1/auth/register")]
        public Task<ApiResponse<TokenResponse>> RegisterAsync([Body] RegisterRequest registerRequest);
    }
}
