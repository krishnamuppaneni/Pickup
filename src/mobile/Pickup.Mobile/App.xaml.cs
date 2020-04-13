using Microsoft.Extensions.DependencyInjection;
using Pickup.Mobile.Models;
using Pickup.Mobile.Views;
using System;
using Xamarin.Forms;

namespace Pickup.Mobile
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static User CurrentUser { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(ServiceProvider.GetService<WelcomePage>());  
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
