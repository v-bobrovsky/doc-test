using System;

using log4net;
using log4net.Config;

namespace Documents.Utils
{
    /// <summary>
    /// Logs different types of messages to a log file.
    /// </summary>
    public class SimpleLogger : ILogger
    {
        protected ILog logger { get; set; }

        public SimpleLogger()
        {
            logger = LogManager.GetLogger("Default");
        }

        static void Configure()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// Set the name of the logger using a string
        /// </summary>
        public void SetLoggerName(string name)
        {
            logger = LogManager.GetLogger(name);
        }

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">The message to log.</param>>
        public void LogInfo(string message)
        {
            if (logger.IsInfoEnabled)
                logger.Info(message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void LogError(string message)
        {
            if (logger.IsErrorEnabled)
                logger.Error(message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void LogError(string message, Exception exception)
        {
            if (logger.IsErrorEnabled)
                logger.Error(message, exception);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        public void LogError(Exception exception)
        {
            if (logger.IsErrorEnabled)
                logger.Error(exception);
        }
    }
}