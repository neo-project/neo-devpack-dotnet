using System;
using System.IO;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Logger for the Dynamic Contract Fuzzer
    /// </summary>
    public class Logger
    {
        private static readonly object _lock = new object();
        private static string _logFilePath = string.Empty;
        private static LogLevel _logLevel = LogLevel.Info;

        /// <summary>
        /// Log levels
        /// </summary>
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal
        }

        /// <summary>
        /// Initialize the logger
        /// </summary>
        public static void Initialize(string logDirectory, LogLevel logLevel = LogLevel.Info)
        {
            _logLevel = logLevel;

            // Create log directory if it doesn't exist
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Create log file with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine(logDirectory, $"fuzzer_{timestamp}.log");

            // Write header
            Log(LogLevel.Info, "Dynamic Contract Fuzzer Log");
            Log(LogLevel.Info, $"Started at: {DateTime.Now}");
            Log(LogLevel.Info, $"Log level: {_logLevel}");
            Log(LogLevel.Info, "----------------------------------------");
        }

        /// <summary>
        /// Log a message
        /// </summary>
        public static void Log(LogLevel level, string message)
        {
            if (level < _logLevel)
            {
                return;
            }

            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            // Write to console
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColorForLogLevel(level);
            Console.WriteLine(logMessage);
            Console.ForegroundColor = originalColor;

            // Write to file
            if (!string.IsNullOrEmpty(_logFilePath))
            {
                lock (_lock)
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing to log file: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Log an exception
        /// </summary>
        public static void LogException(Exception ex, string? context = null)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(context))
            {
                sb.AppendLine($"Exception in {context}:");
            }
            else
            {
                sb.AppendLine("Exception:");
            }

            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine($"Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner exception:");
                sb.AppendLine($"Message: {ex.InnerException.Message}");
                sb.AppendLine($"Type: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"Stack trace: {ex.InnerException.StackTrace}");
            }

            Log(LogLevel.Error, sb.ToString());
        }

        /// <summary>
        /// Get console color for log level
        /// </summary>
        private static ConsoleColor GetColorForLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return ConsoleColor.Gray;
                case LogLevel.Info:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Fatal:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }

        /// <summary>
        /// Debug log
        /// </summary>
        public static void Debug(string message) => Log(LogLevel.Debug, message);

        /// <summary>
        /// Info log
        /// </summary>
        public static void Info(string message) => Log(LogLevel.Info, message);

        /// <summary>
        /// Warning log
        /// </summary>
        public static void Warning(string message) => Log(LogLevel.Warning, message);

        /// <summary>
        /// Error log
        /// </summary>
        public static void Error(string message) => Log(LogLevel.Error, message);

        /// <summary>
        /// Fatal log
        /// </summary>
        public static void Fatal(string message) => Log(LogLevel.Fatal, message);
    }
}
