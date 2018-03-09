namespace BetterConfig.Configuration
{
    /// <summary>
    /// Configuration builder for ManualPoll mode
    /// </summary>
    public class ManualPollConfigurationBuilder : ConfigurationBuilderBase<ManualPollConfiguration>
    {
        internal ManualPollConfigurationBuilder(BetterConfigClientBuilder clientBuilder) : base(clientBuilder) { }

        /// <summary>
        /// Create a <see cref="IBetterConfigClient"/> instance
        /// </summary>
        /// <returns></returns>
        public IBetterConfigClient Build()
        {
            return new BetterConfigClient(this.configuration);
        }
    }
}