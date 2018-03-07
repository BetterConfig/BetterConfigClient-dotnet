using BetterConfig.Cache;
using BetterConfig.Logging;
using System;
using System.Threading.Tasks;

namespace BetterConfig.ConfigService
{
    internal sealed class LazyLoadingConfigService : IConfigService
    {
        private readonly TimeSpan cacheTimeToLive;

        private IConfigFetcher configFetcher;

        private IConfigCache configCache;

        private ILogger log;

        internal LazyLoadingConfigService(IConfigFetcher configFetcher, IConfigCache configCache, ILoggerFactory loggerFactory, TimeSpan cacheTimeToLive)
        {
            this.configFetcher = configFetcher;

            this.configCache = configCache;

            this.log = loggerFactory.GetLogger(nameof(LazyLoadingConfigService));

            this.cacheTimeToLive = cacheTimeToLive;
        }

        public async Task<Config> GetConfigAsync()
        {
            var config = this.configCache.Get();

            if (config.TimeStamp < DateTime.UtcNow.Add(-this.cacheTimeToLive))
            {
                this.log.Debug("config expired");

                return await RefreshConfigLogic(config).ConfigureAwait(false);
            }

            return config;
        }

        public async Task RefreshConfigAsync()
        {
            await RefreshConfigLogic(Config.Empty).ConfigureAwait(false);
        }

        private async Task<Config> RefreshConfigLogic(Config config)
        {
            config = await this.configFetcher.Fetch(config).ConfigureAwait(false);

            this.configCache.Set(config);

            return config;
        }
    }
}