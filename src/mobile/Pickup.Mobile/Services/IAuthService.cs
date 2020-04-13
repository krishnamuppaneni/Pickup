using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Mobile.Models;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(LoginRequest request);

        Task<User> RegisterAsync(RegisterRequest request);
    }
}
