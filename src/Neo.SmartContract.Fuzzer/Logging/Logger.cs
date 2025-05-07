using System;
using System.IO;

namespace Neo.SmartContract.Fuzzer.Logging
{
    /// <summary>
    /// Provides logging functionality for the Neo Smart Contract Fuzzer.
    /// </summary>
    public static class Logger
    {
        private static LogLevel _logLevel = LogLevel.Info;
        private static TextWriter _logWriter = Console.Out;
        private static bool _isVerbose = false;

        /// <summary>
        /// Gets or sets the current log level.
        /// </summary>
        public static LogLevel LogLevel
        {
            get => _logLevel;
            set => _logLevel = value;
        }

        /// <summary>
        /// Gets or sets whether verbose logging is enabled.
        /// </summary>
        public static bool IsVerbose
        {
            get => _isVerbose;
            set => _isVerbose = value;
        }

        /// <summary>
        /// Sets the log writer to use for logging.
        /// </summary>
        /// <param name="writer">The text writer to use for logging.</param>
        public static void SetLogWriter(TextWriter writer)
        {
            _logWriter = writer ?? Console.Out;
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Debug(string message)
        {
            if (_logLevel <= LogLevel.Debug)
            {
                _logWriter.WriteLine($"[DEBUG] {message}");
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message)
        {
            if (_logLevel <= LogLevel.Info)
            {
                _logWriter.WriteLine(message);
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warning(string message)
        {
            if (_logLevel <= LogLevel.Warning)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                _logWriter.WriteLine($"[WARN] {message}");
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Error(string message)
        {
            if (_logLevel <= LogLevel.Error)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                _logWriter.WriteLine($"[ERROR] {message}");
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="context">Optional context information.</param>
        public static void Exception(Exception ex, string? context = null)
        {
            if (_logLevel <= LogLevel.Error)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                if (!string.IsNullOrEmpty(context))
                {
                    _logWriter.WriteLine($"[ERROR] {context}: {ex.Message}");
                }
                else
                {
                    _logWriter.WriteLine($"[ERROR] {ex.Message}");
                }

                if (_isVerbose)
                {
                    _logWriter.WriteLine(ex.StackTrace);
                }

                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Logs a message only if verbose logging is enabled.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Verbose(string message)
        {
            if (_isVerbose && _logLevel <= LogLevel.Debug)
            {
                _logWriter.WriteLine($"[VERBOSE] {message}");
            }
        }

        /// <summary>
        /// Sets the minimum log level.
        /// </summary>
        /// <param name="level">The minimum log level.</param>
        public static void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }
    }

    /// <summary>
    /// Defines the available log levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug level logging.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Informational level logging.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning level logging.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error level logging.
        /// </summary>
        Error = 3,

        /// <summary>
        /// No logging.
        /// </summary>
        None = 4
    }
}
