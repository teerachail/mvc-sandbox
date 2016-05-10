using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(cors =>
            {
                cors.AddPolicy("all", c => c.WithOrigins("*").WithMethods("DELETE", "GET", "POST", "PUT").WithHeaders("Accept"));
                cors.AddPolicy("specific", c => c.WithOrigins("http://corsweb2:5001").WithMethods("DELETE", "GET", "POST", "PUT").WithHeaders("Accept").AllowCredentials());
            });

            services.AddMvcCore().AddCors().AddJsonFormatters();
        }
        
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.Use(next => context =>
            {
                context.Response.Headers[HeaderNames.CacheControl] = "no-store";
                context.Response.Cookies.Append("foo", "Bar", new CookieOptions()
                {
                    Domain = "http://corsweb1:5000",
                    HttpOnly = false,
                });
                return next(context);
            });

            app.Map("/files", b =>
            {
                b.UseCors(cors => cors.WithOrigins("http://corsweb2:5001"));
                b.UseStaticFiles();
                b.UseStatusCodePages();
            });


            app.UseMvcWithDefaultRoute();
            app.UseStatusCodePages();
        }
    }
}
