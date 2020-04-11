using Microsoft.AspNetCore.Identity;
using Pickup.Core.Models.V1.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Infrastructure.Helpers
{
    public class ErrorHelper
    {              public static List<ErrorModel> CreateErrorList(string message, string fieldName = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            errors.Add(new ErrorModel { Message = message, FieldName = fieldName });
            return errors;
        }

        public static List<ErrorModel> CreateErrorList(IEnumerable<IdentityError> errors)
        {
            return CreateErrorList(errors.Select(e => e.Description));
        }

        public static List<ErrorModel> CreateErrorList(IEnumerable<string> errors)
        {
            return errors.Select(error => new ErrorModel()
            {
                Message = error
            }).ToList();
        }

        public static ErrorResponse CreateErrorRespose(string message, string fieldName = null)
        {
            return new ErrorResponse(new ErrorModel { Message = message, FieldName = fieldName });
        }

        public static ErrorResponse CreateErrorRespose(IEnumerable<string> errors)
        {
            return new ErrorResponse(errors.Select(error => new ErrorModel()
            {
                Message = error
            }).ToList());
        }

        public static ErrorResponse CreateErrorRespose(IEnumerable<IdentityError> errors)
        {
            return CreateErrorRespose(errors.Select(e => e.Description));
        }

        public static ErrorResponse CreateErrorRespose(List<ErrorModel> errors)
        {
            return new ErrorResponse(errors);
        }
    }
}
