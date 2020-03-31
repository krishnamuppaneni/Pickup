using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pickup.Api.Data;
using Pickup.Api.Helpers;
using Pickup.Api.Middleware;
using Pickup.Api.Services;
using Pickup.Api.Services.Interfaces;
using Pickup.Api.Settings;

namespace Pickup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SecurityContext>(options =>
                options.UseSqlServer(
                    Configuration["ConnectionStrings:SecurityContextConnection"],
                    x => x.UseNetTopologySuite()));

            // Helpers
            IdentityHelper.ConfigureService(services);
            AuthenticationHelper.ConfigureService(services, Configuration["JwtSecurityToken:Issuer"], Configuration["JwtSecurityToken:Audience"], Configuration["JwtSecurityToken:Key"]);

            // Settings
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.Configure<ClientAppSettings>(Configuration.GetSection("ClientApp"));
            services.Configure<JwtSecurityTokenSettings>(Configuration.GetSection("JwtSecurityToken"));
            services.Configure<QRCodeSettings>(Configuration.GetSection("QRCode"));

            // Services
            services.AddTransient<IEmailService, EmailService>();

            // Data
            services.AddDbContextPool<DataContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DataContextConnection"]));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorHandlingMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
