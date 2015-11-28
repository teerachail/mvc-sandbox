
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace BasicViews
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddViewLocalization(options => options.ResourcesPath = "Resources");
        }

        public void Configure(IApplicationBuilder app)
        {
            // Default to en-CA locale and make sure nothing overrides that choice.
            var options = new RequestLocalizationOptions();
            options.RequestCultureProviders.Clear();
            app.UseRequestLocalization(options, new RequestCulture("en-CA"));

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
