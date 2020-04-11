using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Pickup.Mobile.Models
{
    public class BaseModel : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        public IDictionary<string, string> Errors { get; set; }

        public BaseModel()
        {
            Errors = new Dictionary<string, string>();
        }

        public bool HasErrors
        {
            get
            {
                return Errors?.Any() == true;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return Errors.SelectMany(x => x.Value);
            }

            if (Errors.ContainsKey(propertyName)
                && Errors[propertyName].Any())
            {
                return Errors[propertyName];
            }

            return new List<string>();
        }

        public void Validate([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            var value = this.GetType().GetProperty(propertyName).GetValue(this, null);

            var results = new List<ValidationResult>(1);

            var context = new ValidationContext(this, null, null) { MemberName = propertyName };

            bool isvalid = Validator.TryValidateProperty(value, context, results);

            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
            }

            if (!isvalid)
            {
                Errors.Add(propertyName, results.FirstOrDefault().ErrorMessage);
            }           

            NotifyPropertyChanged(nameof(HasErrors));
            NotifyPropertyChanged(nameof(Errors));
            ErrorsChanged?.Invoke(this,
                new DataErrorsChangedEventArgs(propertyName));
        }

        public void Validate()
        {
            foreach(PropertyInfo property in this.GetType().GetProperties())
            {
                Validate(property.Name);
            }
        }
    }
}
