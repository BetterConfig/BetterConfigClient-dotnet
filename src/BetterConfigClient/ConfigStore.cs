using System.Threading;

namespace BetterConfig
{
    internal class ConfigStore
    {
        private Config config;

        private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

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