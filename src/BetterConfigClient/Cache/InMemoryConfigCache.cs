using System.Threading;

namespace BetterConfig.Cache
{
    internal class InMemoryConfigCache : IConfigCache
    {
        private Config config;

        private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

        /// <inheritdoc />
        public void Set(Config config)
        {
            this.lockSlim.EnterWriteLock();

            try
            {
                this.config = config;
            }
            finally
            {
                this.lockSlim.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public Config Get()
        {
            this.lockSlim.EnterReadLock();

            try
            {
                return this.config;
            }
            finally
            {
                this.lockSlim.ExitReadLock();
            }
        }        
    }
}