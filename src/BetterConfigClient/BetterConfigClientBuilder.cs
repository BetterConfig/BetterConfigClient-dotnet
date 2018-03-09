using BetterConfig.Configuration;

namespace BetterConfig
{
    /// <summary>
    /// Configuration settings object for <see cref="BetterConfigClient"/>
    /// </summary>
    public class BetterConfigClientBuilder
    {
        internal string ProjectSecret { get; private set; }

        /// <summary>
        /// Create a <see cref="BetterConfigClientBuilder"/> instance with <paramref name="projectSecret"/>
        /// </summary>
        /// <returns></returns>
        public static BetterConfigClientBuilder Initialize(string projectSecret)
        {
            return new BetterConfigClientBuilder
            {
                ProjectSecret = projectSecret
            };
        }

        /// <summary>
        /// Set AutoPoll mode
        /// </summary>        
        public AutoPollConfigurationBuilder WithAutoPoll()
        {
            return new AutoPollConfigurationBuilder(this);
        }

        /// <summary>
        /// Set ManualPoll mode
        /// </summary>
        public ManualPollConfigurationBuilder WithManualPoll()
        {
            return new ManualPollConfigurationBuilder(this);
        }

        /// <summary>
        /// Set LazyLoad mode
        /// </summary>
        public LazyLoadConfigurationBuilder WithLazyLoad()
        {
            return new LazyLoadConfigurationBuilder(this);
        }
    }
}