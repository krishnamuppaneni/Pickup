using Microsoft.Extensions.Options;
using Pickup.Mobile.Helpers;
using Pickup.Mobile.Models;
using Pickup.Mobile.Services.Handlers;
using Pickup.Mobile.Settings;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pickup.Mobile.Services
{
    public class RestClient
    {
        private readonly ApiSettings _apiSettings;

        public RestClient(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public T GetClient<T>()
        {
            AuthenticatedHttpClientHandler httpClientHandler = new AuthenticatedHttpClientHandler(GetToken);
#if DEBUG
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
#endif
            return RestService.For<T>(new HttpClient(httpClientHandler) { BaseAddress = new Uri(_apiSettings.BaseUrl) });
        }

        private async Task<string> GetToken()
        {
            TokenModel token = await TokenHelper.RetrieveTokenFromSecureStoreAsync();
            return token?.Token;
        }
    }
}
