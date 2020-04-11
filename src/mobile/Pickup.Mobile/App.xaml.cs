using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Xamarin.Forms;
using System.IO;
using Xamarin.Essentials;
using Pickup.Mobile.Views;
using Acr.UserDialogs;

namespace Pickup.Mobile
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = ServiceProvider.GetService<RegisterPage>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
