using MediumApi.Models;
using Microsoft.AspNet.Builder;
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
                .AddJsonFormatters(json => json.ContractResolver = new DefaultContractResolver(true))
                .AddDataAnnotations();

            services.AddInstance<PetRepository>(new PetRepository());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
