using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class BasicViewsTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes("name=Joey&age=15&birthdate=9-9-1985");

        static BasicViewsTest()
        {
            var builder = new WebApplicationBuilder();
            builder.UseStartup<BasicViews.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

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
