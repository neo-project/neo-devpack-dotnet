using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Main program for the Dynamic Contract Fuzzer.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for the Dynamic Contract Fuzzer.
        /// </summary>
        public static void Main(string[] args)
        {
            Console.WriteLine("Smart Contract Compiler Fuzzer for Neo N3");
            Console.WriteLine("==================================");
            Console.WriteLine();

            // Check if we have any arguments
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            // Check if the first argument is a command
            string command = args[0].ToLower();
            if (command == "--help" || command == "-h" || command == "help")
            {
                PrintHelp();
                return;
            }

            // Check if the command is valid
            if (command != "dynamic")
            {
                Console.WriteLine($"Unknown command: {command}");
                PrintHelp();
                return;
            }

            // Parse command line arguments
            int iterations = 5;
            string outputDir = "GeneratedContracts";
            bool testExecution = true;
            int featuresPerContract = 3;
            Logger.LogLevel logLevel = Logger.LogLevel.Info;
            string duration = null;
            int checkpointIntervalMinutes = 30;

            // Skip the command argument
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--iterations" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], out int parsedIterations))
                    {
                        iterations = parsedIterations;
                    }
                }
                else if (args[i] == "--features" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], out int parsedFeatures))
                    {
                        featuresPerContract = parsedFeatures;
                    }
                }
                else if (args[i] == "--output" && i + 1 < args.Length)
                {
                    outputDir = args[i + 1];
                }
                else if (args[i] == "--no-execution")
                {
                    testExecution = false;
                }
                else if (args[i] == "--log-level" && i + 1 < args.Length)
                {
                    if (Enum.TryParse<Logger.LogLevel>(args[i + 1], true, out var parsedLogLevel))
                    {
                        logLevel = parsedLogLevel;
                    }
                }
                else if (args[i] == "--duration" && i + 1 < args.Length)
                {
                    duration = args[i + 1];
                }
                else if (args[i] == "--checkpoint-interval" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], out int parsedInterval))
                    {
                        checkpointIntervalMinutes = parsedInterval;
                    }
                }
                else if (args[i] == "--help")
                {
                    PrintHelp();
                    return;
                }
            }

            // Create output directory if it doesn't exist
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Initialize logger
            string logDirectory = Path.Combine(outputDir, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            Logger.Initialize(logDirectory, logLevel);

            bool success = false;

            // Create the dynamic contract fuzzer
            DynamicContractFuzzer fuzzer = new DynamicContractFuzzer(outputDir, testExecution);

            // Set up console cancellation
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nCancellation requested. Stopping fuzzer gracefully...");
                e.Cancel = true; // Don't terminate the process immediately
            };

            // Run the fuzzer based on the arguments
            if (!string.IsNullOrEmpty(duration))
            {
                Console.WriteLine("Running Dynamic Contract Fuzzer for duration...");
                Console.WriteLine($"Duration: {duration}, Features per contract: {featuresPerContract}");
                Console.WriteLine($"Checkpoint interval: {checkpointIntervalMinutes} minutes");

                success = fuzzer.RunTestsForDuration(duration, featuresPerContract, checkpointIntervalMinutes);
            }
            else
            {
                Console.WriteLine("Running Dynamic Contract Fuzzer for iterations...");
                Console.WriteLine($"Iterations: {iterations}, Features per contract: {featuresPerContract}");

                success = fuzzer.RunTests(iterations, featuresPerContract);
            }

            Console.WriteLine($"Dynamic Contract Fuzzer completed. Success: {success}");
            Console.WriteLine($"Results saved to: {Path.GetFullPath(outputDir)}");
        }

        /// <summary>
        /// Format a duration in a human-readable way
        /// </summary>
        private static string FormatDuration(TimeSpan duration)
        {
            if (duration == TimeSpan.MaxValue)
            {
                return "Indefinite";
            }

            List<string> parts = new List<string>();

            if (duration.Days > 0)
            {
                parts.Add($"{duration.Days} day{(duration.Days != 1 ? "s" : "")}");
            }

            if (duration.Hours > 0)
            {
                parts.Add($"{duration.Hours} hour{(duration.Hours != 1 ? "s" : "")}");
            }

            if (duration.Minutes > 0)
            {
                parts.Add($"{duration.Minutes} minute{(duration.Minutes != 1 ? "s" : "")}");
            }

            if (duration.Seconds > 0 || parts.Count == 0)
            {
                parts.Add($"{duration.Seconds} second{(duration.Seconds != 1 ? "s" : "")}");
            }

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Print help information.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("Usage: Neo.Compiler.Fuzzer <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  dynamic                     Run the dynamic contract fuzzer");
            Console.WriteLine("  help                        Show this help message");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --iterations <n>            Number of contracts to generate (default: 5)");
            Console.WriteLine("  --features <n>              Number of features per contract (default: 3)");
            Console.WriteLine("  --output <dir>              Output directory for generated contracts (default: GeneratedContracts)");
            Console.WriteLine("  --no-execution              Disable contract execution testing");
            Console.WriteLine("  --log-level <level>         Set log level: Debug, Info, Warning, Error, Fatal (default: Info)");
            Console.WriteLine("  --duration <time>           Run for a specified duration instead of fixed iterations");
            Console.WriteLine("                              Format: <n>m (minutes), <n>h (hours), <n>d (days), <n>w (weeks), or 'indefinite'");
            Console.WriteLine("  --checkpoint-interval <n>   Interval in minutes between checkpoints in long-running mode (default: 30)");
            Console.WriteLine("  --help                      Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --iterations 10 --features 3");
            Console.WriteLine("  dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --output CustomOutput --log-level Debug");
            Console.WriteLine("  dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --duration 24h --features 3");
            Console.WriteLine("  dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --duration 7d --checkpoint-interval 60");
            Console.WriteLine("  dotnet run --project src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj -- dynamic --duration indefinite --log-level Debug");
            Console.WriteLine();
            Console.WriteLine("Or using the shell script:");
            Console.WriteLine("  ./scripts/fuzzer/run-compiler-fuzzer.sh --iterations 10 --features 3");
            Console.WriteLine("  ./scripts/fuzzer/run-compiler-fuzzer.sh --duration 24h --features 3");
        }
    }
}
