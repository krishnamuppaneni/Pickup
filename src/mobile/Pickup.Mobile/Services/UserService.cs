using Pickup.Core.Models.V1.Response.User;
using Pickup.Mobile.Models;
using Pickup.Mobile.Services.Api;
using Refit;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public class UserService : IUserService
    {
        private readonly RestClient _restClient;

        public UserService(RestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<User> GetUserInfo()
        {
            ApiResponse<UserInfoResponse> response = await _restClient.GetClient<IUserApi>().GetUserInfoAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw response.Error;
            }

            UserInfoResponse userInfo = response.Content;
            User user = new User()
            {
                UserId = userInfo.Id,
                Email = userInfo.Email,
                EmailConfirmed = userInfo.EmailConfirmed,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName
            };
            App.CurrentUser = user;
            return user;
        }
    }
}
