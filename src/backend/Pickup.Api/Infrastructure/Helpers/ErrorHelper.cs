using Microsoft.AspNetCore.Identity;
using Pickup.Core.Models.V1.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Infrastructure.Helpers
{
    public class ErrorHelper
    {
        public static ErrorResponse CreateErrorRespose(string message)
        {
            return new ErrorResponse(new ErrorModel { Message = message });
        }

        public static ErrorResponse CreateErrorRespose(IEnumerable<string> errors)
        {
            return new ErrorResponse(errors.Select(error => new ErrorModel()
            {
                Message = error
            }).ToList());
        }

        public static ErrorResponse CreateErrorRespose(IdentityResult identityResult)
        {
            return CreateErrorRespose(identityResult.Errors.Select(e => e.Description));
        }
    }
}
