﻿using System.Threading.Tasks;
using BetterConfig.Cache;
using BetterConfig.Logging;

namespace BetterConfig.ConfigService
{
    internal abstract class ConfigServiceBase
    {
        protected readonly IConfigFetcher configFetcher;

        protected readonly IConfigCache configCache;

        protected readonly ILogger log;

        protected ConfigServiceBase(IConfigFetcher configFetcher, IConfigCache configCache, ILogger log)
        {
            this.configFetcher = configFetcher;

            this.configCache = configCache;

            this.log = log;
        }
    }
}