namespace BetterConfig.Trace
{
    internal class NullTrace : ITraceWriter
    {
        public void Trace(string message) { }

        public void Trace(TraceLevel traceLevel, string message) { }
    }
}
