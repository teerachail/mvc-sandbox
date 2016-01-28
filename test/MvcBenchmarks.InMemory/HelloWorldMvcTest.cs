using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class HelloWorldMvcTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static HelloWorldMvcTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<HelloWorldMvc.Startup>();
            builder.UseProjectOf<HelloWorldMvc.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "HelloWorldMvc", Iterations = 1, WarmupIterations = 1)]
        public async Task HelloWorldMvc()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
