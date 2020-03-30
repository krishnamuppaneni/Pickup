using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pickup.Entity.Common
{
    public class User : IdentityUser
    {
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Review> ReviewsPosted { get; set; }
        public virtual ICollection<Review> ReviewsReceived { get; set; }
    }
}
