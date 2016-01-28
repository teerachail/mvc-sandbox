using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class HelloWorldMiddlewareTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static HelloWorldMiddlewareTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<HelloWorldMiddleware.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "HelloWorldMiddleware", Iterations = 1, WarmupIterations = 1)]
        public async Task HelloWorldMiddleware()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
