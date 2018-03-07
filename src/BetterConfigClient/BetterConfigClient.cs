using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;
using BetterConfig.Logging;
using BetterConfig.ConfigService;
using BetterConfig.Cache;

namespace BetterConfig
{
    /// <summary>
    /// Client for BetterConfig platform
    /// </summary>
    public class BetterConfigClient : IBetterConfigClient, IDisposable
    {
        private readonly BetterConfigClientConfiguration configuration;

        private readonly ILogger log;

        private readonly IConfigService configService;

        private static string version = typeof(BetterConfigClient).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        /// <inheritdoc />
        public event OnConfigurationChangedEventHandler OnConfigurationChanged;

        /// <summary>
        /// Create an instance of BetterConfigClient
        /// </summary>
        /// <param name="projectSecret">Project secret to access configuration</param>
        /// <exception cref="ArgumentException">When the <paramref name="projectSecret"/> is null or empty</exception>                
        public BetterConfigClient(string projectSecret) : this(new BetterConfigClientConfiguration { ProjectSecret = projectSecret })
        {
        }

        /// <summary>
        /// Create an instance of BetterConfigClient
        /// </summary>
        /// <param name="configuration">BetterConfigClient configuration</param>
        /// <exception cref="ArgumentException">When the configuration contains any invalid property</exception>
        /// <exception cref="ArgumentNullException">When the configuration is null</exception>        
        /// <exception cref="ArgumentOutOfRangeException">When TimeToLive value is not in a proper range</exception>        
        public BetterConfigClient(BetterConfigClientConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Validate();

            this.configuration = configuration;

            this.log = configuration.LoggerFactory.GetLogger(typeof(BetterConfigClient).Name);

            var configFetcher = new HttpConfigFetcher(this.configuration.Url, version, this.configuration.LoggerFactory);

            var cache = new InMemoryConfigCache();

            if (this.configuration.PollIntervalSeconds > 0 && this.configuration.AutoPollingEnabled)
            {
                var pollingProcessor = new PollingConfigService(
                    configFetcher,
                    cache,
                    TimeSpan.FromSeconds(this.configuration.PollIntervalSeconds),
                    TimeSpan.FromSeconds(this.configuration.MaxInitWaitTimeSeconds),
                    this.configuration.LoggerFactory);

                pollingProcessor.OnConfigurationChanged += (sender, args) => { this.OnConfigurationChanged?.Invoke(sender, args); };

                this.configService = pollingProcessor;
            }
            else
            {
                this.configService = new LazyLoadingConfigService(
                    configFetcher,
                    cache,
                    this.configuration.LoggerFactory,
                    TimeSpan.FromSeconds(Math.Max(1, this.configuration.CacheTimeToLiveSeconds)));
            }
        }

        /// <inheritdoc />
        public string GetConfigurationJsonString()
        {
            var c = this.configService.GetConfigAsync().Result;

            return c.JsonString;
        }

        /// <inheritdoc />
        public async Task<string> GetConfigurationJsonStringAsync()
        {
            var c = await this.configService.GetConfigAsync();

            return c.JsonString;
        }

        /// <inheritdoc />
        public T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                var c = this.configService.GetConfigAsync().Result;

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.log.Error($"Error occured in 'GetValue' method.\n{ex.ToString()}");

                return defaultValue;
            }
        }

        /// <inheritdoc />
        public async Task<T> GetValueAsync<T>(string key, T defaultValue)
        {
            try
            {
                var c = await this.configService.GetConfigAsync().ConfigureAwait(false);

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.log.Error($"Error occured in 'GetValueAsync' method.\n{ex.ToString()}");

                return defaultValue;
            }

        }

        /// <inheritdoc />
        public T GetConfiguration<T>(T defaultValue)
        {
            try
            {
                var c = this.configService.GetConfigAsync().Result;

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.log.Error($"Error occured in 'GetConfiguration' method.\n{ex.ToString()}");

                return defaultValue;
            }
        }

        /// <inheritdoc />
        public async Task<T> GetConfigurationAsync<T>(T defaultValue)
        {
            try
            {
                var c = await this.configService.GetConfigAsync().ConfigureAwait(false);

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.log.Error($"Error occured in 'GetConfigurationAsync' method.\n{ex.ToString()}");

                return defaultValue;
            }
        }

        /// <inheritdoc />
        public void ForceRefresh()
        {
            this.configService.RefreshConfigAsync().Wait();
        }

        /// <inheritdoc />
        public async Task ForceRefreshAsync()
        {
            await this.configService.RefreshConfigAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.configService != null && this.configService is IDisposable)
            {
                ((IDisposable)this.configService).Dispose();
            }
        }

        private T GetValueLogic<T>(Config config, string key, T defaultValue)
        {
            if (config.JsonString == null)
            {
                this.log.Warning("ConfigJson is not present, returning defaultValue");

                return defaultValue;
            }

            var json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(config.JsonString);

            var rawValue = json.GetValue(key);

            if (rawValue == null)
            {
                this.log.Warning($"Unknown key: '{key}'");

                return defaultValue;
            }

            return rawValue.Value<T>();
        }

        private T GetConfigurationLogic<T>(Config config, T defaultValue)
        {
            if (config.JsonString == null)
            {
                return defaultValue;
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(config.JsonString);
        }
    }
}