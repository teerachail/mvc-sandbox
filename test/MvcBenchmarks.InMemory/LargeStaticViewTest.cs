using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class LargeStaticViewTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static LargeStaticViewTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<LargeStaticView.Startup>();
            builder.UseProjectOf<LargeStaticView.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "LargeStaticViews - No UrlResolutionTagHelper", Iterations = 1, WarmupIterations = 1)]
        public async Task LargeStaticViews_NoUrlResolutionTagHelper()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Benchmark(DisplayName = "LargeStaticViews - With UrlResolutionTagHelper", Iterations = 1, WarmupIterations = 1)]
        public async Task LargeStaticViews_WithUrlResolutionTagHelper()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/TagHelpers");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
