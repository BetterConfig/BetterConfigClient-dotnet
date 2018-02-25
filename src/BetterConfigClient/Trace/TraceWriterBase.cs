namespace BetterConfig.Trace
{
    internal abstract class TraceWriterBase : ITraceWriter
    {
        private readonly TraceLevel traceLevel;

        
        private bool TargetTraceEnabled(TraceLevel targetTrace)
        {
            return (byte)this.traceLevel <= (byte)targetTrace;
        }

        protected abstract void TraceMessage(string message);

        public TraceWriterBase(TraceLevel traceLevel)
        {
            this.traceLevel = traceLevel;
        }

        public void Trace(string message)
        {
            this.Trace(TraceLevel.Verbose, message);
        }

        public void Trace(TraceLevel traceLevel, string message)
        {
            if (this.TargetTraceEnabled(traceLevel))
            {
                this.TraceMessage(message);
            }
        }
    }
}
