using System.Threading.Tasks;

namespace BetterConfig
{
    public interface IBetterConfigClient
    {
        /// <summary>
        /// Return configuration as a json string
        /// </summary>
        /// <returns>All configuration in json string</returns>
        string GetConfigurationJsonString();

        /// <summary>
        /// Return configuration as a json string
        /// </summary>
        /// <returns>All configuration in json string</returns>
        Task<string> GetConfigurationJsonStringAsync();

        /// <summary>
        /// Serialize the configuration to a passed <typeparamref name="T"/> type.  
        /// You can customize your T with Newtonsoft attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        T GetConfiguration<T>(T defaultValue);

        /// <summary>
        /// Serialize the configuration to a passed <typeparamref name="T"/> type.        
        /// You can customize your T with Newtonsoft attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        Task<T> GetConfigurationAsync<T>(T defaultValue);

        /// <summary>
        /// Return a value of the key (Key for programs)
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Key for programs</param>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        T GetValue<T>(string key, T defaultValue);

        /// <summary>
        /// Return a value of the key (Key for programs)
        /// </summary>
        /// <typeparam name="T">Setting type</typeparam>
        /// <param name="key">Key for programs</param>
        /// <param name="defaultValue">In case of failure return this value</param>
        /// <returns></returns>
        Task<T> GetValueAsync<T>(string key, T defaultValue);
    }
}