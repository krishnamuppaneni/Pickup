using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Settings
{
    public class ClientAppSettings
    {
        public string Url { get; set; }
        public string EmailConfirmationPath { get; set; }
        public string ResetPasswordPath { get; set; }
    }
}
