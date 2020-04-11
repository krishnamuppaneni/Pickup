using Acr.UserDialogs;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Core.Models.V1.Response;
using Pickup.Core.Models.V1.Response.Identity;
using Pickup.Mobile.Models;
using Pickup.Mobile.Services;
using Refit;
using System;
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
        public ICommand RegisterCommand { get; }
        public ICommand EntryUnfocused { get; protected set; }
        public bool IsBusy { get; set; }

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
            IsBusy = true;
            RegisterModel = new RegisterModel();

            EntryUnfocused = new Command<string>((propertyName) =>
            {
                EntryUnfocusedCommand(propertyName, RegisterModel);
            });
            RegisterCommand = new Command(execute: async () => await Register(RegisterModel));
        }

        private void EntryUnfocusedCommand(string propertyName, BaseModel model)
        {
            model.Validate(propertyName);
            IsRegisterAllowed = !model.HasErrors;
        }

        private async Task Register(RegisterModel model)
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
                ApiResponse<TokenResponse> response = await _authService.RegisterAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse errorResponse = await response.Error.GetContentAsAsync<ErrorResponse>();
                    await UserDialogs.Instance.AlertAsync(errorResponse.Errors.FirstOrDefault().Message, "Please try again!");
                }
            }
            UserDialogs.Instance.HideLoading();
        }
    }
}