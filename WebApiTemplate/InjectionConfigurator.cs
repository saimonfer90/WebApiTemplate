using System;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;
using SimpleInjector;
using WebApiTemplate.Core;
using WebApiTemplate.Data;

namespace WebApiTemplate
{
    public static class InjectionConfigurator
    {
        public static Container GetContainerService()
            => new();

        public static void InitializeContainer(this Container container)
        {
            var appsettings = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";

            var customConfig = new ConfigurationBuilder()
                .AddJsonFile(appsettings, optional: false, reloadOnChange: true)
                .Build();

            Func<ILogger> loggerFactory = new (() => new LoggerConfiguration()
                .ReadFrom.Configuration(customConfig, sectionName: "WebApiTemplate:Serilog")
                .CreateLogger());

            Func<string, DbHandler<NpgsqlConnection>> postgresClientFactory = new (connectionStringName =>
            {
                var instanceLogger = container.GetInstance<ILogger>();
                var cstring = customConfig.GetSection($"WebApiTemplate:WebApiTemplate:{connectionStringName}").Value;

                return (DbHandler<NpgsqlConnection>)(new(instanceLogger, cstring));
            });

            container.Register<ILogger>(loggerFactory, Lifestyle.Transient);
            container.Register<WebApiTemplateCore>(Lifestyle.Transient);
        }
    }
}