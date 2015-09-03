
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Logging.Console;

namespace BasicViews
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            //logger.AddProvider(new ConsoleLoggerProvider((s, l) => true));
            app.UseMvcWithDefaultRoute();
        }
    }
}
