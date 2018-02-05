using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace BetterConfig
{
    public sealed class BetterConfigClientConfiguration
    {
        /// <summary>
        /// Access url to configuration
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Poll interval in seconds        
        /// </summary>
        public ushort TimeToLiveSeconds { get; set; } = 2;

        internal ILogger Logger { get; set; }

        internal Func<HttpClient> HttpClientFactory { get; set; } = () => new HttpClient();

        private static readonly LoggerFactory loggerFactory = new LoggerFactory();

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new ArgumentNullException("Invalid url value", nameof(this.Url));
            }

            if (this.TimeToLiveSeconds == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(this.TimeToLiveSeconds), "Value must be greater than zero.");
            }

            if (this.Logger == null)
            {
                this.Logger = loggerFactory.CreateLogger(typeof(BetterConfigClient));
            }
        }
    }
}