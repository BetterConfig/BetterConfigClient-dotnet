using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace BetterConfig
{
    public class BetterConfigClient : IBetterConfigClient, IDisposable
    {
        private TimeSpan timeToLive;

        private Uri url;

        private ConfigStore ConfigStore = new ConfigStore();

        private HttpClient httpClient;

        private readonly object lck = new object();

        private readonly ILogger logger;

        private readonly BetterConfigClientConfiguration configuration;

        /// <summary>
        /// Create an instance of BetterConfigClient
        /// </summary>
        /// <param name="url">Url to access configuration</param>
        /// <exception cref="ArgumentException">When the <paramref name="url"/> is null or empty</exception>        
        /// <exception cref="UriFormatException">When the <paramref name="url"/> is invalid</exception>
        public BetterConfigClient(string url) : this(new BetterConfigClientConfiguration { Url = url})
        {            
        }

        /// <summary>
        /// Create an instance of BetterConfigClient
        /// </summary>
        /// <param name="configuration">BetterConfigClient configuration</param>
        /// <exception cref="ArgumentException">When the configuration contains any invalid property</exception>
        /// <exception cref="ArgumentNullException">When the configuration is null</exception>        
        /// <exception cref="ArgumentOutOfRangeException">When TimeToLive value is not in a proper range</exception>
        /// <exception cref="UriFormatException">When the Url is invalid</exception>
        public BetterConfigClient(BetterConfigClientConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Validate();            

            this.url = new Uri(configuration.Url);

            this.timeToLive = TimeSpan.FromSeconds(configuration.TimeToLiveSeconds);

            this.logger = configuration.Logger;

            this.configuration = configuration;

            this.EnsureHttpClient();
        }        
        
        public string GetConfigurationJsonString()
        {
            var c = this.GetCacheItemAsync().Result;

            return c.JsonString;
        }

        public async Task<string> GetConfigurationJsonStringAsync()
        {
            var c = await this.GetCacheItemAsync();

            return c.JsonString;
        }
        
        public T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                var c = this.GetCacheItemAsync().Result;

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured in 'GetValue' method.\n{0}", ex.ToString());               
                
                return defaultValue;
            }
        }

        public async Task<T> GetValueAsync<T>(string key, T defaultValue)
        {
            try
            {
                var c = await this.GetCacheItemAsync().ConfigureAwait(false);

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured in 'GetValueAsync' method.\n{0}", ex.ToString());

                return defaultValue;
            }
            
        }

        public T GetConfiguration<T>(T defaultValue)
        {
            try
            {
                var c = this.GetCacheItemAsync().Result;

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured in 'GetConfiguration' method.\n{0}", ex.ToString());

                return defaultValue;
            }           
        }

        public async Task<T> GetConfigurationAsync<T>(T defaultValue)
        {
            try
            {
                var c = await this.GetCacheItemAsync().ConfigureAwait(false);

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured in 'GetConfigurationAsync' method.\n{0}", ex.ToString());

                return defaultValue;
            }
        }
        
        public void Dispose()
        {
            if (this.httpClient != null) this.httpClient.Dispose();
        }

        private async Task<Config> UpdateAsync(Config lastConfig)
        {
            Config newConfig = Config.Empty;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = this.url
            };

            if (lastConfig.HttpETag != null)
            {
                request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(lastConfig.HttpETag));
            }

            try
            {
                var response = await this.httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    newConfig.HttpETag = response.Headers.ETag.Tag;
                    newConfig.JsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    newConfig.TimeStamp = DateTime.UtcNow;
                }
                else
                {
                    this.EnsureHttpClient(true);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured in 'UpdateAsync' method.\n{0}", ex.ToString());
                
                this.EnsureHttpClient(true);
            }          

            this.ConfigStore.Set(newConfig);

            return newConfig;
        }       
        
        private async Task<Config> GetCacheItemAsync()
        {
            var config = this.ConfigStore.Get();

            if (config.TimeStamp < DateTime.UtcNow.Add(-this.timeToLive))
            {
                return await UpdateAsync(config);
            }

            return config;
        }

        private T GetValueLogic<T>(Config config, string key, T defaultValue)
        {
            if (config.JsonString == null)
            {
                this.logger.LogWarning("ConfigJson is not present, returning defaultValue");

                return defaultValue;
            }

            var json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(config.JsonString);

            var rawValue = json.GetValue(key);

            if (rawValue == null)
            {
                this.logger.LogWarning("Unknown key: '{0}'", key);

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

        private void EnsureHttpClient(bool force = false)
        {
            if (this.httpClient == null || force)
            {
                lock (this.lck)
                {
                    if (this.httpClient == null || force)
                    {
                        this.httpClient = this.configuration.HttpClientFactory();

                        this.httpClient.Timeout = TimeSpan.FromSeconds(30);

                        this.httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("BetterConfigClient-Dotnet", "1.0"));
                    }
                }
            }
        }
    }    
}