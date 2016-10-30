using System;

using log4net;
using ObjectDumper;
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

        static SimpleLogger()
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

        /// <summary>
        /// Logs an object informational
        /// </summary>
        /// <param name="obj">The object to log.</param>>
        public void LogInfoObject(object obj)
        {
            if (logger.IsInfoEnabled)
            {
                var message = obj != null
                    ? obj.DumpToString("Object: ") : "NULL";
                logger.Info(message);
            }               
        }

        /// <summary>
        /// Logs an error object
        /// </summary>
        /// <param name="obj">The object to log.</param>>
        public void LogErrorObject(object obj)
        {
            if (logger.IsErrorEnabled)
            {
                var message = obj != null
                    ? obj.DumpToString("Object: ") : "NULL";
                logger.Error(message);
            }
        }
    }
}