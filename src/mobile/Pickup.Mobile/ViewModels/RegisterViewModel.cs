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
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        public RegisterModel RegisterModel { get; set; }
        public ICommand RegisterCommand { get; protected set; }
        public ICommand EntryUnfocused { get; protected set; }

        private bool _IsRegisterAllowed;
        public bool IsRegisterAllowed
        {
            get { return _IsRegisterAllowed; }
            set
            {
                _IsRegisterAllowed = value;
                NotifyPropertyChanged();
            }
        }

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;

            RegisterModel = new RegisterModel();

            EntryUnfocused = new Command<string>((propertyName) =>
            {
                EntryUnfocusedCommand(propertyName, RegisterModel);
            });
            RegisterCommand = new Command(execute: async () => await RegisterAsync(RegisterModel));
        }

        private void EntryUnfocusedCommand(string propertyName, BaseModel model)
        {
            model.Validate(propertyName);
            IsRegisterAllowed = !model.HasErrors;
        }

        private async Task RegisterAsync(RegisterModel model)
        {
            UserDialogs.Instance.ShowLoading("Signing up...");
            model.Validate();
            if (!model.HasErrors)
            {
                RegisterRequest request = new RegisterRequest()
                {
                    Email = model.Email,
                    BirthDate = model.BirthDate,
                    Password = model.Password,
                    ConfirmPassword = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                try
                {
                    await _authService.RegisterAsync(request);
                    App.Current.MainPage = new NavigationPage(App.ServiceProvider.GetService<MainPage>());
                }
                catch (ApiException ex)
                {
                    ErrorResponse errorResponse = await ex.GetContentAsAsync<ErrorResponse>();
                    await UserDialogs.Instance.AlertAsync(errorResponse.Errors.FirstOrDefault().Message, "Error");
                }
            }
            UserDialogs.Instance.HideLoading();
        }
    }
}