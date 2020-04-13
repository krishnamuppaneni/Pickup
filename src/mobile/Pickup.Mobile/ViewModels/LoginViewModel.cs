using Acr.UserDialogs;
using Microsoft.Extensions.DependencyInjection;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response;
using Pickup.Mobile.Models;
using Pickup.Mobile.Services;
using Pickup.Mobile.Views;
using Refit;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pickup.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        public LoginModel LoginModel { get; set; }
        public ICommand LoginCommand { get; protected set; }
        public ICommand EntryUnfocused { get; protected set; }

        private bool _IsLoginAllowed;
        public bool IsLoginAllowed
        {
            get { return _IsLoginAllowed; }
            set
            {
                _IsLoginAllowed = value;
                NotifyPropertyChanged();
            }
        }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginModel = new LoginModel();

            EntryUnfocused = new Command<string>((propertyName) =>
            {
                EntryUnfocusedCommand(propertyName, LoginModel);
            });
            LoginCommand = new Command(execute: async () => await LoginAsync(LoginModel));
        }        

        private void EntryUnfocusedCommand(string propertyName, BaseModel model)
        {
            model.Validate(propertyName);
            IsLoginAllowed = !model.HasErrors;
        }

        private async Task LoginAsync(LoginModel loginModel)
        {
            UserDialogs.Instance.ShowLoading("Signing in...");
            loginModel.Validate();
            if (!loginModel.HasErrors)
            {
                LoginRequest request = new LoginRequest()
                {
                    Email = loginModel.Email,
                    Password = loginModel.Password
                };

                try
                {
                    await _authService.LoginAsync(request);
                    App.Current.MainPage = new NavigationPage(App.ServiceProvider.GetService<MainPage>());
                }
                catch (ApiException ex)
                {
                    ErrorResponse errorResponse = await ex.GetContentAsAsync<ErrorResponse>();
                    await UserDialogs.Instance.AlertAsync(errorResponse.Errors.FirstOrDefault().Message, "Login Error");
                }
            }
            UserDialogs.Instance.HideLoading();
        }
    }
}
