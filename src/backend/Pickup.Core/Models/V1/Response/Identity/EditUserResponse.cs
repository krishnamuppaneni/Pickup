﻿using System.ComponentModel.DataAnnotations;

namespace Pickup.Core.Models.V1.Response.Identity
{
    public class EditUserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
