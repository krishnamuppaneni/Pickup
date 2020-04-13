using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Mobile.Helpers;
using Pickup.Mobile.Services;
using Pickup.Mobile.Services.Api;
using Pickup.Mobile.Settings;
using Pickup.Mobile.ViewModels;
using Pickup.Mobile.Views;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;

namespace Pickup.Mobile
{
    public static class Startup
    {
        public static App Init(Action<HostBuilderContext, IServiceCollection> nativeConfigureServices)
        {
            var systemDir = FileSystem.CacheDirectory;
            ExtractSaveResource("Pickup.Mobile.appsettings.json", systemDir);
            var fullConfig = Path.Combine(systemDir, "Pickup.Mobile.appsettings.json");

            App.ServiceProvider = new HostBuilder()
                .ConfigureHostConfiguration(c =>
                {
                    c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });
                    c.AddJsonFile(fullConfig);
                })
                .ConfigureServices((c, x) =>
                {
                    nativeConfigureServices(c, x);
                    ConfigureServices(c, x);
                })
                .ConfigureLogging(l => l.AddConsole(o =>
                {
                    o.DisableColors = true;
                }))
                .Build().Services;
            return App.ServiceProvider.GetService<App>();
        }


        static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            // Settings
            services.Configure<ApiSettings>(ctx.Configuration.GetSection("ApiSettings"));

            //client
            services.AddSingleton<RestClient>();

            // Services            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<WelcomeViewModel>();

            //Pages
            services.AddTransient<LoginPage>();
            services.AddTransient<MainPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<WelcomePage>();

            services.AddSingleton<App>();
        }

        public static void ExtractSaveResource(string filename, string location)
        {
            var a = Assembly.GetExecutingAssembly();
            using var resFilestream = a.GetManifestResourceStream(filename);
            if (resFilestream != null)
            {
                var full = Path.Combine(location, filename);

                using var stream = File.Create(full);
                resFilestream.CopyTo(stream);
            }

        }
    }
}
