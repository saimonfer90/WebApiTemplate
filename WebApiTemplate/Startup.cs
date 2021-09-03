using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using SimpleInjector;

namespace WebApiTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private Container _serviceContainer;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiTemplate", Version = "v1" });
            });

            _serviceContainer = InjectionConfigurator.GetContainerService();

            services.AddSimpleInjector(_serviceContainer, options =>
            {
                options.AddAspNetCore();
            });

            _serviceContainer.InitializeContainer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiTemplate v1"));
            }

            app.UseCors(options =>
            {
                options.AllowAnyMethod().AllowAnyHeader();
                options.SetIsOriginAllowed((host) => true);
                options.AllowCredentials();
            });

            app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSimpleInjector(_serviceContainer);

            _serviceContainer.Verify();

            var logger = _serviceContainer.GetInstance<ILogger>();
        }
    }
}