using System;
using System.Net.Http;
using BetterConfig.Trace;

namespace BetterConfig
{
    /// <summary>
    /// Configuration settings object for <see cref="BetterConfigClient">BetterConfigClient</see>
    /// </summary>
    public sealed class BetterConfigClientConfiguration
    {
        private string projectToken;

        /// <summary>
        /// Cache time to live value in seconds        
        /// </summary>
        public ushort TimeToLiveSeconds { get; set; } = 2;

        /// <summary>
        /// Project token
        /// </summary>
        public string ProjectToken
        {
            set
            {
                this.projectToken = value;

                this.Url = CreateUrl(value);
            }
            get
            {
                return this.projectToken;
            }
        }

        /// <summary>
        /// Factory method of <c>ITraceWriter</c>
        /// </summary>
        public Func<ITraceWriter> TraceFactory { get; set; } = () => new NullTrace();

        /// <summary>
        /// Trace level
        /// <para />
        /// Default value: Error
        /// </summary>
        public TraceLevel TraceLevel { get; set; } = TraceLevel.Error;

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

            if (string.IsNullOrEmpty(this.ProjectToken))
            {
                throw new ArgumentException("Invalid project token value.", nameof(this.ProjectToken));
            }

            if (this.TraceFactory == null)
            {
                throw new ArgumentNullException(nameof(this.TraceFactory));
            }            
        }
    }
}