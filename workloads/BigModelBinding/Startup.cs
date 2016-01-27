
using BigModelBinding.Controllers;
using BigModelBinding.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BigModelBinding
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            HomeController.DatabaseModel = ModelGenerator.GenerateModel();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args)
        {
            var application = new WebHostBuilder()
                .UseConfiguration(WebHostConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
