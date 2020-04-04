using System.Collections.Generic;

namespace Pickup.Core.Models.V1.Response
{
    public class ErrorResponse
    {
        public ErrorResponse() { }

        public ErrorResponse(ErrorModel error)
        {
            Errors.Add(error);
        }

        public ErrorResponse(List<ErrorModel> errors)
        {
            Errors.AddRange(errors);
        }

        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();


    }
}