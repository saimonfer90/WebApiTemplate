using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using SimpleInjector;
using WebApiTemplate.Core;

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

            container.Register<ILogger>(loggerFactory, Lifestyle.Transient);
            container.Register<WebApiTemplateCore>(Lifestyle.Transient);
        }
    }
}