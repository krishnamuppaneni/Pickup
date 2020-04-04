using Pickup.Data.Entities;
using System.Threading.Tasks;

namespace Pickup.Api.Services
{
    public interface IUserService
    {
        Task<User> FindByEmailAsync(string email);

        Task<User> FindByIdAsync(string userId);

        Task<bool> IsEmailConfirmedAsync(User user);
    }
}