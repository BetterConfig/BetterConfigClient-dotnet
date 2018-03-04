﻿namespace BetterConfig.Logging
{
    /// <summary>
    /// Provides logging interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write debug level message into log
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Write information level message into log
        /// </summary>
        /// <param name="message"></param>
        void Information(string message);

        /// <summary>
        /// Write warning level message into log
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);

        /// <summary>
        /// Write error level message into log
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
    }
}
