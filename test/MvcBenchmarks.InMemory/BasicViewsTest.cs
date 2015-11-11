using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BasicViewsTest
    {
        private static readonly BasicViews.Startup Startup = new BasicViews.Startup();
        private static readonly Action<IServiceCollection> ConfigureServices = Startup.ConfigureServices;
        private static readonly Action<IApplicationBuilder> Configure = Startup.Configure;

        private static readonly TestServer Server = TestServer.Create(Configure, HostingStartup.InitializeServices(Startup.GetType(), ConfigureServices));
        private static readonly HttpClient Client = Server.CreateClient();

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes("name=Joey&age=15&birthdate=9-9-1985");

        //[Benchmark(DisplayName = "Basic Model Binding and View", Iterations = 10000, WarmupIterations = 20)]
        public async Task BasicViews_ValidInput()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
