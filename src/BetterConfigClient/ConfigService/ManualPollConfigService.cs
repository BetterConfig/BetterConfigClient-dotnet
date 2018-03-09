using BetterConfig.Cache;
using BetterConfig.Logging;
using System.Threading.Tasks;

namespace BetterConfig.ConfigService
{
    internal sealed class ManualPollConfigService : ConfigServiceBase, IConfigService
    {
        internal ManualPollConfigService(IConfigFetcher configFetcher, IConfigCache configCache, ILoggerFactory loggerFactory)
            : base(configFetcher, configCache, loggerFactory.GetLogger(nameof(ManualPollConfigService))) { }

        public Task<ProjectConfig> GetConfigAsync()
        {
            var config = this.configCache.Get();

            return Task.FromResult(config);
        }

        public async Task RefreshConfigAsync()
        {
            var config = this.configCache.Get();

            config = await this.configFetcher.Fetch(config).ConfigureAwait(false);

            this.configCache.Set(config);
        }
    }
}