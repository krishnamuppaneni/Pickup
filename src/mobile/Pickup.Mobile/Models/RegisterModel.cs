using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Pickup.Mobile.Models
{
    public class RegisterModel : BaseModel
    {
        private string _email { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyPropertyChanged();
            }
        }

        private string _password { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged();
            }
        }

        private string _firstName { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        [DataType(DataType.Text)]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyPropertyChanged();
            }
        }

        private string _lastName { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        [DataType(DataType.Text)]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _birthDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                NotifyPropertyChanged();
            }
        }
    }
}
