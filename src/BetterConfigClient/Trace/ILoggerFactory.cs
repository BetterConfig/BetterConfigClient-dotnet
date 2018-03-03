namespace BetterConfig.Trace
{
    /// <summary>
    /// Provides logger factory interface
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Create a ILogger instance by name
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        ILogger GetLogger(string loggerName);
    }
}
