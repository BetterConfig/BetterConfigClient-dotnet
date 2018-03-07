namespace BetterConfig.Cache
{
    /// <summary>
    /// Defines cache
    /// </summary>
    internal interface IConfigCache
    {
        /// <summary>
        /// Set a <see cref="Config"/> into cache
        /// </summary>
        /// <param name="config"></param>
        void Set(Config config);

        /// <summary>
        /// Get a <see cref="Config"/> from cache
        /// </summary>
        /// <returns></returns>
        Config Get();
    }
}