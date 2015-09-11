
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace HelloWorldMvc
{
    public class Startup
    {
        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
