using System;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json.Linq;
using BetterConfig.Logging;
using BetterConfig.ConfigService;
using BetterConfig.Cache;
using BetterConfig.Configuration;

namespace BetterConfig
{
    /// <summary>
    /// Client for BetterConfig platform
    /// </summary>
    public class BetterConfigClient : IBetterConfigClient, IDisposable
    {
        private ILogger log;

        private readonly IConfigService configService;

        private static string version = typeof(BetterConfigClient).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        /// <inheritdoc />
        public event OnConfigurationChangedEventHandler OnConfigurationChanged;

        /// <summary>
        /// Create an instance of BetterConfigClient and setup AutoPoll mode
        /// </summary>
        /// <param name="projectSecret">Project secret to access configuration</param>
        /// <exception cref="ArgumentException">When the <paramref name="projectSecret"/> is null or empty</exception>                
        public BetterConfigClient(string projectSecret) : this(new AutoPollConfiguration { ProjectSecret = projectSecret })
        {
        }       

        /// <summary>
        /// Create an instance of BetterConfigClient and setup AutoPoll mode
        /// </summary>
        /// <param name="configuration">Configuration for AutoPolling mode</param>
        /// <exception cref="ArgumentException">When the configuration contains any invalid property</exception>
        /// <exception cref="ArgumentNullException">When the configuration is null</exception>                
        public BetterConfigClient(AutoPollConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Validate();

            InitializeClient(configuration);

            var configService = new AutoPollConfigService(
                    new HttpConfigFetcher(configuration.Url, "a-" + version, configuration.LoggerFactory),
                    new InMemoryConfigCache(),
                    TimeSpan.FromSeconds(configuration.PollIntervalSeconds),
                    TimeSpan.FromSeconds(configuration.MaxInitWaitTimeSeconds),
                    configuration.LoggerFactory);

            configService.OnConfigurationChanged += (sender, args) => { this.OnConfigurationChanged?.Invoke(sender, args); };

            this.configService = configService;
        }

        /// <summary>
        /// Create an instance of BetterConfigClient and setup LazyLoad mode
        /// </summary>
        /// <param name="configuration">Configuration for LazyLoading mode</param>
        /// <exception cref="ArgumentException">When the configuration contains any invalid property</exception>
        /// <exception cref="ArgumentNullException">When the configuration is null</exception>  
        public BetterConfigClient(LazyLoadConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Validate();

            InitializeClient(configuration);

            var configService = new LazyLoadConfigService(
                new HttpConfigFetcher(configuration.Url, "l-" + version, configuration.LoggerFactory),
                new InMemoryConfigCache(),
                configuration.LoggerFactory,
                TimeSpan.FromSeconds(configuration.CacheTimeToLiveSeconds));

            this.configService = configService;
        }

        /// <summary>
        /// Create an instance of BetterConfigClient and setup ManualPoll mode
        /// </summary>
        /// <param name="configuration">Configuration for LazyLoading mode</param>
        /// <exception cref="ArgumentException">When the configuration contains any invalid property</exception>
        /// <exception cref="ArgumentNullException">When the configuration is null</exception>  
        public BetterConfigClient(ManualPollConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Validate();

            InitializeClient(configuration);

            var configService = new ManualPollConfigService(
                new HttpConfigFetcher(configuration.Url, "m-" + version, configuration.LoggerFactory),
                new InMemoryConfigCache(),
                configuration.LoggerFactory);

            this.configService = configService;
        }

        private void InitializeClient(ConfigurationBase configuration)
        {
            this.log = configuration.LoggerFactory.GetLogger(nameof(BetterConfigClient));
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

        private T GetValueLogic<T>(ProjectConfig config, string key, T defaultValue)
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

        private T GetConfigurationLogic<T>(ProjectConfig config, T defaultValue)
        {
            if (config.JsonString == null)
            {
                return defaultValue;
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(config.JsonString);
        }

        /// <summary>
        /// Create a <see cref="BetterConfigClientBuilder"/> instance to setup the client
        /// </summary>
        /// <param name="projectSecret"></param>
        /// <returns></returns>
        public static BetterConfigClientBuilder Create(string projectSecret)
        {
            return BetterConfigClientBuilder.Initialize(projectSecret);
        }
    }
}