
using BigModelBinding.Controllers;
using BigModelBinding.Models;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

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
    }
}
