using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Utils
{
    /// <summary>
    /// Provides the logger functionality
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Set the name of the logger using a string
        /// </summary>
        void SetLoggerName(string name);

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">The message to log.</param>>
        void LogInfo(string message);

        /// <summary>
        /// Logs an object informational
        /// </summary>
        /// <param name="obj">The object to log.</param>>
        void LogInfoObject(object obj);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogError(string message);

        /// <summary>
        /// Logs an error object
        /// </summary>
        /// <param name="obj">The object to log.</param>>
        void LogErrorObject(object obj);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void LogError(Exception exception);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void LogError(string message, Exception exception);
    }
}