namespace BetterConfig.Trace
{
    /// <summary>
    /// Provides tracing interface
    /// </summary>
    public interface ITraceWriter
    {
        /// <summary>
        /// Writes the diagnostic message
        /// </summary>
        void Trace(string message);

        /// <summary>
        /// Writes the diagnostic message with TraceLevel
        /// </summary>
        void Trace(TraceLevel traceLevel, string message);
    }
}
