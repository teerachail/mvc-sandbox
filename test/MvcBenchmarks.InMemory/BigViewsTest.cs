using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BigViewsTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        static BigViewsTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<BigViews.Startup>();
            builder.UseProjectOf<BigViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "BigViews - HtmlHelpers", Iterations = 1, WarmupIterations = 1)]
        public async Task BigViews_HtmlHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/Index");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Benchmark(DisplayName = "BigViews - TagHelpers", Iterations = 1, WarmupIterations = 1)]
        public async Task BigViews_TagHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/IndexWithTagHelpers");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Benchmark(DisplayName = "BigViews - TagHelpers - Static Options", Iterations = 1, WarmupIterations = 1)]
        public async Task BigViews_TagHelpers_StaticOptions()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Home/IndexWithStaticOptions");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
