using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;

namespace Pickup.Api.Infrastructure.Installers
{
    public class SwaggerInstaller
    {
        public static void ConfigureService(IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pickup",
                    Version = "v1",
                    Description = "A crowdsorced pickup and delivery platform.",
                    Contact = new OpenApiContact
                    {
                        Name = "Krishna Chaitanya Muppaneni",
                        Url = new Uri("https://github.com/krishnamuppaneni/Pickup")
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
