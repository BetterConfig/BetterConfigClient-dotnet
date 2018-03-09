namespace BetterConfig.Configuration
{
    /// <summary>
    /// Configuration builder for LazyLoad mode
    /// </summary>
    public class LazyLoadConfigurationBuilder : ConfigurationBuilderBase<LazyLoadConfiguration>
    {
        internal LazyLoadConfigurationBuilder(BetterConfigClientBuilder clientBuilder) : base(clientBuilder) { }

        /// <summary>
        /// Cache time to live value in seconds, minimum value is 1.
        /// </summary>
        /// <param name="cacheTimeToLiveSeconds"></param>        
        public LazyLoadConfigurationBuilder WithCacheTimeToLiveSeconds(uint cacheTimeToLiveSeconds)
        {
            this.configuration.CacheTimeToLiveSeconds = cacheTimeToLiveSeconds;

            return this;
        }

        /// <summary>
        /// Create a <see cref="IBetterConfigClient"/> instance
        /// </summary>
        /// <returns></returns>
        public IBetterConfigClient Create()
        {
            return new BetterConfigClient(this.configuration);
        }
    }    
}