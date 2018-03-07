using BetterConfig.Cache;
using BetterConfig.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BetterConfig.ConfigService
{
    internal sealed class PollingConfigService : IConfigService, IDisposable
    {
        private readonly object lck = new object();

        private DateTime maxInitWaitExpire;       

        private readonly IConfigFetcher configFetcher;

        private readonly IConfigCache configCache;

        private readonly ILogger log;

        private readonly Timer timer;

        internal event OnConfigurationChangedEventHandler OnConfigurationChanged;

        internal PollingConfigService(
            IConfigFetcher configFetcher,
            IConfigCache configCache,
            TimeSpan pollingInterval,
            TimeSpan maxInitWaitTime,
            ILoggerFactory loggerFactory)
        {
            this.configFetcher = configFetcher;

            this.configCache = configCache;            

            this.log = loggerFactory.GetLogger(nameof(PollingConfigService));

            this.timer = new Timer(RefreshLogic, null, TimeSpan.Zero, pollingInterval);            

            this.maxInitWaitExpire = DateTime.UtcNow.Add(maxInitWaitTime);
        }

#pragma warning disable CS1591
        public void Dispose()
        {
            this.timer?.Dispose();
        }

        public Task<Config> GetConfigAsync()
        {
            var d = this.maxInitWaitExpire - DateTime.UtcNow;

            if (d > TimeSpan.Zero)
            {
                Task.Run(() => RefreshLogic(null)).Wait(d);
            }

            return Task.FromResult(this.configCache.Get());
        }

        public Task RefreshConfigAsync()
        {
            RefreshLogic(null);

            return Task.FromResult(true);
        }

        private void RefreshLogic(object state)
        {
            lock (this.lck)
            {
                this.log.Debug("Refresh() start");

                var latestConfig = this.configCache.Get();

                var newConfig = this.configFetcher.Fetch(latestConfig).Result;

                if (!latestConfig.Equals(newConfig))
                {
                    this.log.Debug("config changed");

                    this.configCache.Set(newConfig);

                    this.OnConfigurationChanged?.Invoke(this, OnConfigurationChangedEventArgs.Empty);
                } 
            }
        }
    }
}