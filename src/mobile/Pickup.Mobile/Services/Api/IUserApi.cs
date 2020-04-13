using Pickup.Core.Models.V1.Response.User;
using Refit;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services.Api
{
    [Headers("Authorization: Bearer")]
    public interface IUserApi
    {        
        [Get("/api/v1/user/get")]
        public Task<ApiResponse<UserInfoResponse>> GetUserInfoAsync();
    }
}
