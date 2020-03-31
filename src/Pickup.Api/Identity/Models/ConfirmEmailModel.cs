using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Identity.Models
{
    public class ConfirmEmailModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}
