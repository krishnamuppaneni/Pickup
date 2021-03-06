﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pickup.Data.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            RegistrationTime = DateTime.UtcNow;
        }

        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)]
        public string FirstName { get; set; }

        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Text)]
        [StringLength(25)]
        public string LastName { get; set; }

        [ProtectedPersonalData]
        public Gender? Gender { get; set; }

        [ProtectedPersonalData]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [PersonalData]
        [DataType(DataType.DateTime)]
        public DateTimeOffset RegistrationTime { get; set; }

        public static implicit operator User(IdentityResult v)
        {
            throw new NotImplementedException();
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
