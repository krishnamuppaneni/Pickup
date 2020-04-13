using System;
using System.Collections.Generic;
using System.Text;

namespace Pickup.Mobile.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
