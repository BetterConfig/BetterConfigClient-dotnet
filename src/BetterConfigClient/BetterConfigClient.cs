using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using BetterConfig.Trace;
using System.Reflection;

namespace BetterConfig
{
    /// <summary>
    /// Client for BetterConfig platform
    /// </summary>
    public class BetterConfigClient : IBetterConfigClient, IDisposable
    {
        private TimeSpan timeToLive;

        private Uri url;

        private ConfigStore ConfigStore = new ConfigStore();

        private HttpClient httpClient;

        private readonly object lck = new object();

        private readonly BetterConfigClientConfiguration configuration;

        private readonly ITraceWriter traceWriter;

        /// <summary>
        /// Create an instance of BetterConfigClient
        /// </summary>
        /// <param name="projectToken">ProjectToken to access configuration</param>
        /// <exception cref="ArgumentException">When the <paramref name="projectToken"/> is null or empty</exception>                
        public BetterConfigClient(string projectToken) : this(new BetterConfigClientConfiguration { ProjectToken = projectToken })
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

            this.url = configuration.Url;

            this.timeToLive = TimeSpan.FromSeconds(configuration.TimeToLiveSeconds);            

            this.traceWriter = configuration.TraceFactory();

            this.configuration = configuration;

            this.EnsureHttpClient();
        }

        /// <summary>
        /// Return configuration as a json string
        /// </summary>
        /// <returns>All configuration in json string</returns>
        public string GetConfigurationJsonString()
        {
            var c = this.GetCacheItemAsync().Result;

            return c.JsonString;
        }

        /// <summary>
        /// Return configuration as a json string
        /// </summary>
        /// <returns>All configuration in json string</returns>
        public async Task<string> GetConfigurationJsonStringAsync()
        {
            var c = await this.GetCacheItemAsync();

            return c.JsonString;
        }

        /// <summary>
        /// Return a value of the key (Key for programs)
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Key for programs</param>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                var c = this.GetCacheItemAsync().Result;

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.traceWriter.Trace(TraceLevel.Error, $"Error occured in 'GetValue' method.\n{ex.ToString()}");                
                
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Return a value of the key (Key for programs)
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Key for programs</param>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        public async Task<T> GetValueAsync<T>(string key, T defaultValue)
        {
            try
            {
                var c = await this.GetCacheItemAsync().ConfigureAwait(false);

                return this.GetValueLogic<T>(c, key, defaultValue);
            }
            catch (Exception ex)
            {
                this.traceWriter.Trace(TraceLevel.Error, $"Error occured in 'GetValueAsync' method.\n{ex.ToString()}");

                return defaultValue;
            }
            
        }

        /// <summary>
        /// Serialize the configuration to a passed <typeparamref name="T"/> type.  
        /// You can customize your T with Newtonsoft attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        public T GetConfiguration<T>(T defaultValue)
        {
            try
            {
                var c = this.GetCacheItemAsync().Result;

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.traceWriter.Trace(TraceLevel.Error, $"Error occured in 'GetConfiguration' method.\n{ex.ToString()}");
                
                return defaultValue;
            }           
        }

        /// <summary>
        /// Serialize the configuration to a passed <typeparamref name="T"/> type.        
        /// You can customize your T with Newtonsoft attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        public async Task<T> GetConfigurationAsync<T>(T defaultValue)
        {
            try
            {
                var c = await this.GetCacheItemAsync().ConfigureAwait(false);

                return GetConfigurationLogic<T>(c, defaultValue);
            }
            catch (Exception ex)
            {
                this.traceWriter.Trace(TraceLevel.Error, $"Error occured in 'GetConfigurationAsync' method.\n{ex.ToString()}");

                return defaultValue;
            }
        }

        /// <summary>
        /// Remove all items from cache
        /// </summary>
        public void ClearCache()
        {
            this.ConfigStore.Clear();
        }

#pragma warning disable CS1591
        public void Dispose()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    newConfig = lastConfig;
                    newConfig.TimeStamp = DateTime.UtcNow;
                }
                else if(response.IsSuccessStatusCode)
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
                this.traceWriter.Trace(TraceLevel.Error, $"Error occured in 'UpdateAsync' method.\n{ex.ToString()}");

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
                this.traceWriter.Trace(TraceLevel.Warn, "ConfigJson is not present, returning defaultValue");

                return defaultValue;
            }

            var json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(config.JsonString);

            var rawValue = json.GetValue(key);

            if (rawValue == null)
            {
                this.traceWriter.Trace(TraceLevel.Warn, "Unknown key: '{key}'");

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

                        this.httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BetterConfigClient-Dotnet", "1.0"));
                    }
                }
            }
        }
    }    
}