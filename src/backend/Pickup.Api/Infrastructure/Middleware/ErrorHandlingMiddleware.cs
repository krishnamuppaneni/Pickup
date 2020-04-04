using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Pickup.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pickup.Api.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IEmailService _emailService)
        {
            try
            {
                await _next(httpContext);
            }
            catch (SqlException ex)
            {
                await _emailService.SendSqlException(ex);
                await HandleSqlExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await _emailService.SendException(ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = JsonConvert.SerializeObject(new
            {
                Type = "General Exception",
                Exception = new
                {
                    ex.Message,
                    Inner = ex.InnerException
                }
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(result);
        }

        private static Task HandleSqlExceptionAsync(HttpContext context, SqlException ex)
        {
            var errorList = new List<object>();

            for (int i = 0; i < ex.Errors.Count; i++)
            {
                errorList.Add(new
                {
                    ex.Errors[i].Message,
                    ex.Errors[i].Procedure,
                    ex.Errors[i].LineNumber,
                    ex.Errors[i].Source,
                    ex.Errors[i].Server
                });
            }

            var result = JsonConvert.SerializeObject(new
            {
                Type = "SQL Exception",
                Exceptions = errorList
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(result);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
