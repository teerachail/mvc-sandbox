
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;
using Sandbox.Web.Controllers;

namespace Sandbox.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.ConfigureMvc(options =>
            {
                options
                    .Conventions
                    .Build()
                    .ForAllActions()
                    .UseRoute("/api/[controller")
                    .AddRouteParameters(whitelist: "id")
                    .InferHttpVerbFromPrefix();

                options
                    .Conventions
                    .Build()
                    .ForParameters(p => p.ParameterName == "id")
                    .SetBindingSource(BindingSource.Path);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
            });
        }
    }
}
