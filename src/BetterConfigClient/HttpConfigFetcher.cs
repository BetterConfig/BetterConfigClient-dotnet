using BetterConfig.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BetterConfig
{
    internal sealed class HttpConfigFetcher : IConfigFetcher
    {
        private readonly object lck = new object();

        private readonly string productVersion;

        private ILogger log;

        private HttpClient httpClient;

        private Uri requestUri;

        public HttpConfigFetcher(Uri requestUri, string productVersion, ILoggerFactory loggerFactory)
        {
            this.requestUri = requestUri;

            this.productVersion = productVersion;

            this.log = loggerFactory.GetLogger(nameof(HttpConfigFetcher));

            ReInitializeHttpClient();
        }

        public async Task<Config> Fetch(Config lastConfig)
        {
            Config newConfig = Config.Empty;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = this.requestUri
            };

            if (lastConfig.HttpETag != null)
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(lastConfig.HttpETag));
            }

            try
            {
                var response = await this.httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    newConfig = lastConfig;
                }
                else if (response.IsSuccessStatusCode)
                {
                    newConfig.HttpETag = response.Headers.ETag.Tag;

                    newConfig.JsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);                    
                }
                else
                {
                    this.log.Warning("Unexpected statuscode - " + response.StatusCode);

                    this.ReInitializeHttpClient();
                }
            }
            catch (Exception ex)
            {
                this.log.Error($"Error occured in 'Fetch' method.\n{ex.ToString()}");

                this.ReInitializeHttpClient();
            }

            newConfig.TimeStamp = DateTime.UtcNow;

            return newConfig;
        }

        private void ReInitializeHttpClient()
        {
            lock (this.lck)
            {
                this.httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                this.httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BetterConfigClient-Dotnet", productVersion));
            }
        }
    }
}