using Microsoft.Extensions.DependencyInjection;
using Pickup.Mobile.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pickup.Mobile.ViewModels
{
    public class WelcomeViewModel
    {
        public ICommand LoginCommand { get; protected set; }
        public ICommand RegisterCommand { get; protected set; }

        public WelcomeViewModel()
        {
            LoginCommand = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(App.ServiceProvider.GetService<LoginPage>());
            });

            RegisterCommand = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(App.ServiceProvider.GetService<RegisterPage>());
            });
        }
    }
}
