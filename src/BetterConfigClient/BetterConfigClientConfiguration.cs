using BetterConfig.Logging;
using System;
using System.Net.Http;

namespace BetterConfig
{
    /// <summary>
    /// Configuration settings object for <see cref="BetterConfigClient">BetterConfigClient</see>
    /// </summary>
    public sealed class BetterConfigClientConfiguration
    {
        private string projectSecret;

        /// <summary>
        /// Cache time to live value in seconds        
        /// </summary>
        public ushort TimeToLiveSeconds { get; set; } = 2;

        /// <summary>
        /// Pool interval in seconds        
        /// </summary>
        public ushort PollIntervalSeconds { get; set; } = 0;

        /// <summary>
        /// Project token
        /// </summary>
        public string ProjectSecret
        {
            set
            {
                this.projectSecret = value;

                this.Url = CreateUrl(value);
            }
            get
            {
                return this.projectSecret;
            }
        }

        /// <summary>
        /// Factory method of <c>ILogger</c>
        /// </summary>
        public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();        

        internal Uri Url { get; private set; }

        internal Func<HttpClient> HttpClientFactory { get; set; } = () => new HttpClient();        

        private static Uri CreateUrl(string projectToken)
        {
            return new Uri("https://cdn.betterconfig.com/configuration-files/" + projectToken + "/config.json");
        }

        internal void Validate()
        {
            if (this.TimeToLiveSeconds == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(this.TimeToLiveSeconds), "Value must be greater than zero.");
            }

            if (string.IsNullOrEmpty(this.ProjectSecret))
            {
                throw new ArgumentException("Invalid project secret value.", nameof(this.ProjectSecret));
            }

            if (this.LoggerFactory == null)
            {
                throw new ArgumentNullException(nameof(this.LoggerFactory));
            }
        }
    }
}