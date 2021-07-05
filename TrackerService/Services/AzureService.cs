using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PCMonitor.Shared;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace TrackerService.Services
{
    public class AzureService : IAzureService
    {
        public IConfiguration _configuration { get; private set; }

        public AzureService (IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <summary>
        /// Send updates to azure
        /// </summary>
        /// <param name="pcInfoWithStatus">device info</param>
        public async void Send(PcInfoWithStatus pcInfoWithStatus)
        {
            var url = _configuration.GetSection("AppSettings:UpdatePcStatusFunctionUrl").Value;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var httpContent = CreateHttpContent(pcInfoWithStatus))
            {
                request.Content = httpContent;

                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None)
                    .ConfigureAwait(false))
                {
                    var resualtList = response.Content;
                }
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }
}
