using System;
using System.Collections.Generic;
using System.Text;

namespace Pickup.Core.Models.V1.Response.User
{
    public class UserInfoResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
