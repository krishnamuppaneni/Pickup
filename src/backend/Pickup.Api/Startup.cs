using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pickup.Api.Infrastructure.Filters;
using Pickup.Api.Infrastructure.Installers;
using Pickup.Api.Infrastructure.Middleware;
using Pickup.Api.Services;
using Pickup.Api.Settings;
using Pickup.Data;

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
                    Configuration["ConnectionStrings:SecurityContextConnection"]));

            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<ValidationFilter>();
                })
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            // Helpers
            IdentityInstaller.ConfigureService(services);
            AuthenticationInstaller.ConfigureService(services, Configuration);
            SwaggerInstaller.ConfigureService(services);

            // Settings
            services.Configure<EmailSettings>(Configuration.GetSection("Email"));
            services.Configure<ClientAppSettings>(Configuration.GetSection("ClientApp"));
            services.Configure<JwtSecurityTokenSettings>(Configuration.GetSection("JwtSecurityToken"));
            services.Configure<QRCodeSettings>(Configuration.GetSection("QRCode"));

            // Services
            services.AddTransient<IEmailService, EmailService>();

            // Data
            services.AddDbContextPool<DataContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DataContextConnection"], x => x.UseNetTopologySuite()));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseErrorHandlingMiddleware();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
                c.RoutePrefix = "";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
