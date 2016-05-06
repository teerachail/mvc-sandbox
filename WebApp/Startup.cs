using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(source =>
                {
                    source.Path = "appsettings.json";
                    source.ReloadOnChange = true;
                })
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMiddlewareAnalysis();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, DiagnosticListener listener)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            listener.Subscribe(new ConsoleObserver());

            //app.UseStatusCodePages();
            //app.UseStatusCodePagesWithReExecute("~/status/{0}.html");
            //app.UseStatusCodePagesWithReExecute("/Error/Index");

            //app.UseExceptionHandler();
            //app.UseExceptionHandler("/status/500.html");
            //app.UseExceptionHandler("/error/Exception");
            //app.UseExceptionHandler("/Error/ThrowingHandler");

            //app.UseDeveloperExceptionPage();

            //app.UseWelcomePage();

            app.UseRuntimeInfoPage();

            app.UseStaticFiles();

            app.Map("/statuscode/set", b => b.Run(context =>
            {
                context.Response.StatusCode = int.Parse(context.Request.Query["code"]);
                return Task.CompletedTask;
            }));

            app.UseMvcWithDefaultRoute();

            app.Use(next => async context =>
            {
                await next(context);
            });
        }

        private class ConsoleObserver : IObserver<KeyValuePair<string, object>>
        {
            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(KeyValuePair<string, object> value)
            {
                Console.WriteLine($"{value.Key}: {value.Value}");
            }
        }
    }
}
