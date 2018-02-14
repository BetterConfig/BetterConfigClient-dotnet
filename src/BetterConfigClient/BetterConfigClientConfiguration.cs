using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace BetterConfig
{
    /// <summary>
    /// Configuration settings object for <see cref="BetterConfigClient">BetterConfigClient</see>
    /// </summary>
    public sealed class BetterConfigClientConfiguration
    {        
        internal string Url { get; set; }

        /// <summary>
        /// Poll interval in seconds        
        /// </summary>
        public ushort TimeToLiveSeconds { get; set; } = 2;

        /// <summary>
        /// Project token
        /// </summary>
        public string ProjectToken
        {
            set
            {
                this.Url = CreateUrl(value);
            }            
        }

        internal ILogger Logger { get; set; }

        internal Func<HttpClient> HttpClientFactory { get; set; } = () => new HttpClient();

        private static readonly LoggerFactory loggerFactory = new LoggerFactory();

        private static string CreateUrl(string projectToken)
        {
            return "https://cdn.betterconfig.com/configuration-files/" + projectToken + "/config.json";
        }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Url))
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