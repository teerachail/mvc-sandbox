
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LargeContentMiddleware
{
    public class Startup
    {
        private readonly byte[] bytes = Encoding.UTF8.GetBytes(new string('a', 1024));
        private readonly int multiplier = 300;

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            var contentLength = bytes.Length * multiplier;

            app.Run(async (context) =>
            {
                //context.Response.ContentLength = contentLength;

                for (var i = 0; i < 300; i++)
                {
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
            });
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
