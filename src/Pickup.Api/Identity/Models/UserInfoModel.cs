using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Identity.Models
{
    public class UserInfoModel
    {
        public string Email { get;  set; }
        public bool EmailConfirmed { get;  set; }
        public bool LockoutEnabled { get;  set; }
        public IList<string> Roles { get;  set; }
    }
}
