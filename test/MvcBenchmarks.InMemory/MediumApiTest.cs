﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class MediumApiTest
    {
        private static readonly TestServer Server;
        private static readonly HttpClient Client;

        private static readonly byte[] ValidBytes = new UTF8Encoding(false).GetBytes(@"
{
  ""category"" : {
    ""id"" : 2,
    ""name"" : ""Cats""
  },
  ""name"" : ""fluffy"",
  ""status"" : ""available""
}");

        static MediumApiTest()
        {
            var builder = new WebHostBuilder();
            builder.UseStartup<MediumApi.Startup>();
            builder.UseProjectOf<MediumApi.Startup>();
            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        [Benchmark(DisplayName = "MediumApi", Iterations = 1, WarmupIterations = 1)]
        public async Task MediumApi()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/pet");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", .9));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", .6));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

            request.Content = new ByteArrayContent(ValidBytes);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
