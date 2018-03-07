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
        private static Uri CreateUrl(string projectSecret)
        {
            return new Uri("https://cdn.betterconfig.com/configuration-files/" + projectSecret + "/config.json");
        }

        /// <summary>
        /// Configuration refresh period (Default value is 60.)
        /// </summary>
        public uint PollIntervalSeconds { get; set; } = 60;

        /// <summary>
        /// Enable autopolling (Default value is true.)
        /// </summary>
        public bool AutoPollingEnabled { get; set; } = true;

        /// <summary>
        /// Cache time to live value in seconds, minimum value is 1. (Default value is 60.)     
        /// </summary>
        public uint CacheTimeToLiveSeconds { get; set; } = 60;

        /// <summary>
        /// Maximum waiting time between initialization and the first config acquisition in secconds. (Default value is 5.)
        /// </summary>
        public uint MaxInitWaitTimeSeconds { get; set; } = 5;

        /// <summary>
        /// Project secret to get your configuration
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

        private string projectSecret;        

        internal void Validate()
        {
            if (this.CacheTimeToLiveSeconds == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(this.CacheTimeToLiveSeconds), "Value must be greater than zero.");
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