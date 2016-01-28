using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class LocalizedViewsTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes("name=Joey&age=15&birthdate=9-9-1985");

        static LocalizedViewsTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<LocalizedViews.Startup>();
            builder.UseProjectOf<LocalizedViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "LocalizedViews - HtmlHelpers", Iterations = 1, WarmupIterations = 1)]
        public async Task LocalizedViews_HtmlHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Benchmark(DisplayName = "LocalizedViews - TagHelpers", Iterations = 1, WarmupIterations = 1)]
        public async Task LocalizedViews_TagHelpers()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/TagHelpers");
            request.Content = new ByteArrayContent(ValidBytes);

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
