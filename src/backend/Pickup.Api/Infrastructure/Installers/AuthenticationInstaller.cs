﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Pickup.Api.Infrastructure.Installers
{
    public class AuthenticationInstaller
    {
        public static void ConfigureService(IServiceCollection service, IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = configuration["JwtSecurityToken:Issuer"],
                ValidAudience = configuration["JwtSecurityToken:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityToken:Key"]))
            };

            service.AddSingleton(tokenValidationParameters);

            service
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}
