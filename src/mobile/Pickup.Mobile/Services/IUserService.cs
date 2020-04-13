using Pickup.Mobile.Models;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public interface IUserService
    {
        Task<User> GetUserInfo();
    }
}