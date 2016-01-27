using MediumApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace MediumApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters(json => json.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddDataAnnotations();

            services.AddSingleton<PetRepository>(new PetRepository());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
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
