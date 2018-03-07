using BetterConfig.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BetterConfig
{
    /// <summary>
    /// Defines configuration fetch
    /// </summary>
    internal interface IConfigFetcher
    {
        /// <summary>
        /// Fetch the configuration
        /// </summary>
        /// <param name="lastConfig">Last of fetched configuration if it is present</param>
        /// <returns></returns>
        Task<Config> Fetch(Config lastConfig);
    }
}