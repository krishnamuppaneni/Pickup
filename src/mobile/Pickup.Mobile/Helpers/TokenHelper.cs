using Newtonsoft.Json;
using Pickup.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pickup.Mobile.Helpers
{
    public static class TokenHelper
    {
        private const string tokenKey = "token";

        public static async Task<TokenModel> RetrieveTokenFromSecureStoreAsync()
        {
            var json = await SecureStorage.GetAsync(tokenKey);
            try
            {
                return JsonConvert.DeserializeObject<TokenModel>(json);
            }
            catch { }

            return new TokenModel();
        }

        public static async Task StoreTokenInSecureStore(TokenModel token)
        {
            SecureStorage.Remove(tokenKey);
            var json = JsonConvert.SerializeObject(token);
            await SecureStorage.SetAsync(tokenKey, json);
        }
    }
}
