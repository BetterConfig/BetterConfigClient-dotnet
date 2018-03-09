﻿namespace BetterConfig.Configuration
{
    /// <summary>
    /// Base configuration builder
    /// </summary>    
    public abstract class ConfigurationBuilderBase<T> where T : ConfigurationBase, new()
    {
#pragma warning disable CS1591
        protected readonly T configuration;

        internal ConfigurationBuilderBase(BetterConfigClientBuilder clientBuilder)
        {
            this.configuration = new T
            {
                ProjectSecret = clientBuilder.ProjectSecret
            };
        }
    }
}