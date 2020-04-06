using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response.Identity;
using Pickup.Mobile.Services;
using Refit;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pickup.Mobile.ViewModels
{
    public class RegisterViewModel
    {
        private readonly IAuthService _authService;

        public RegisterRequest RegisterRequest { get; set; }

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterRequest = new RegisterRequest();
            Register = new Command(async () =>
            {
                try
                {
                    TokenResponse tokenResponse = await _authService.RegisterAsync(RegisterRequest);
                }
                catch (ApiException ex)
                {

                }
            });
        }

        public Command Register { get; }
    }
}