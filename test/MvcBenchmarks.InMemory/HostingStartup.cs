using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace MvcBenchmarks.InMemory
{
    public static class HostingStartup
    {
        public static Action<IServiceCollection> InitializeServices(Type startup, Action<IServiceCollection> next)
        {
            var libraryManager = PlatformServices.Default.LibraryManager;

            var applicationName = startup.GetTypeInfo().Assembly.GetName().Name;
            var library = libraryManager.GetLibrary(applicationName);
            var applicationRoot = Path.GetDirectoryName(library.Path);

            var applicationEnvironment = PlatformServices.Default.Application;

            var configureServices = startup.GetMethod("ConfigureServices");

            return (services) =>
            {
                services.AddInstance<IApplicationEnvironment>(new TestApplicationEnvironment(applicationEnvironment, applicationName, applicationRoot));

                var hostingEnvironment = new HostingEnvironment();
                hostingEnvironment.Initialize(applicationRoot, new WebHostOptions()
                {
                    Environment = "Production",
                });
                services.AddInstance<IHostingEnvironment>(new HostingEnvironment());
                next(services);
            };
        }

        private class TestApplicationEnvironment : IApplicationEnvironment
        {
            private readonly IApplicationEnvironment _original;

            public TestApplicationEnvironment(IApplicationEnvironment original, string name, string path)
            {
                _original = original;

                ApplicationName = name;
                ApplicationBasePath = path;
            }

            public string ApplicationBasePath { get; }

            public string ApplicationName { get; }

            public string ApplicationVersion => _original.ApplicationVersion;

            public string Configuration => _original.Configuration;

            public FrameworkName RuntimeFramework => _original.RuntimeFramework;

            public object GetData(string name)
            {
                return _original.GetData(name);
            }

            public void SetData(string name, object value)
            {
                _original.SetData(name, value);
            }
        }
    }
}
