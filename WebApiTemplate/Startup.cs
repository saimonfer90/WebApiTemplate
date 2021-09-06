using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using SimpleInjector;
using WebApiTemplate.Metrics;

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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiTemplate", Version = "v1" });
            });

            services.AddHealthChecks()
                .AddCheck("Memory", new SystemMemoryHealthCheck())
                .ForwardToPrometheus();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            app.UseSimpleInjector(_serviceContainer);

            _serviceContainer.Verify();
        }
    }
}