using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> RegisterAsync(RegisterRequest request);
    }
}
