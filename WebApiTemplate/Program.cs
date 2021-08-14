using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

/*replace "WebApiTemplate" with the name of the app on the whole solution and in the project properties also correctly set the namespace and assembly name*/

namespace WebApiTemplate
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}