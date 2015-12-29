
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
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
            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-CA");
                options.RequestCultureProviders.Clear();
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public static void Main(string[] args)
        {
            var application = new WebApplicationBuilder()
                .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
