using System;

namespace BetterConfig.Configuration
{
    /// <summary>
    /// LazyLoad configuration settings object for <see cref="BetterConfigClient"/>
    /// </summary>
    public class LazyLoadConfiguration : ConfigurationBase
    {
        /// <summary>
        /// Cache time to live value in seconds, minimum value is 1. (Default value is 60.)     
        /// </summary>
        public uint CacheTimeToLiveSeconds { get; set; } = 60;

        internal override void Validate()
        {
            base.Validate();

            if (this.CacheTimeToLiveSeconds == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(this.CacheTimeToLiveSeconds), "Value must be greater than zero.");
            }
        }
    }
}