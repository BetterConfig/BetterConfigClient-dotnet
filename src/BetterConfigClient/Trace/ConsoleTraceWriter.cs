namespace BetterConfig.Trace
{
    internal class ConsoleTraceWriter : TraceWriterBase
    {
        internal ConsoleTraceWriter(TraceLevel traceLevel) : base(traceLevel) { }

        protected override void TraceMessage(string message)
        {
            System.Console.WriteLine(message);
        }

        private static string FormatMessage(TraceLevel traceLevel, string message)
        {
            return $"{System.DateTime.Now.ToString("s")} [{traceLevel.ToString()}] - {message}";
        }
    }
}
