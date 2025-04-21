using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Long-running fuzzer that can run for extended periods
    /// </summary>
    public class LongRunningFuzzer
    {
        private readonly DynamicContractFuzzer _fuzzer;
        private readonly string _outputDirectory;
        private readonly bool _testExecution;
        private readonly int _featuresPerContract;
        private readonly int _checkpointIntervalMinutes;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Stopwatch _stopwatch;
        private int _contractsGenerated;
        private int _contractsSucceeded;
        private int _contractsFailed;
        private DateTime _startTime;
        private DateTime _endTime;

        /// <summary>
        /// Initialize the long-running fuzzer
        /// </summary>
        public LongRunningFuzzer(
            string outputDirectory = "GeneratedContracts",
            bool testExecution = false,
            int featuresPerContract = 5,
            int checkpointIntervalMinutes = 30)
        {
            _outputDirectory = outputDirectory;
            _testExecution = false; // Always set to false to focus only on compilation
            _featuresPerContract = featuresPerContract;
            _checkpointIntervalMinutes = checkpointIntervalMinutes;
            _fuzzer = new DynamicContractFuzzer(outputDirectory, false);
            _cancellationTokenSource = new CancellationTokenSource();
            _stopwatch = new Stopwatch();
            _contractsGenerated = 0;
            _contractsSucceeded = 0;
            _contractsFailed = 0;
        }

        /// <summary>
        /// Run the fuzzer for a specified duration
        /// </summary>
        public async Task<bool> RunForDuration(TimeSpan duration, CancellationToken? externalToken = null)
        {
            // Create a linked token source that can be cancelled either by the duration or externally
            using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token,
                externalToken ?? CancellationToken.None))
            {
                CancellationToken token = linkedTokenSource.Token;

                // Set up a timer to cancel after the specified duration
                if (duration != TimeSpan.MaxValue)
                {
                    linkedTokenSource.CancelAfter(duration);
                }

                // Set up checkpoint timer
                Timer checkpointTimer = new Timer(
                    _ => CreateCheckpoint(),
                    null,
                    TimeSpan.FromMinutes(_checkpointIntervalMinutes),
                    TimeSpan.FromMinutes(_checkpointIntervalMinutes));

                try
                {
                    _startTime = DateTime.Now;
                    Logger.Info($"Starting long-running fuzzer at {_startTime}");
                    Logger.Info($"Duration: {FormatDuration(duration)}");
                    Logger.Info($"Checkpoint interval: {_checkpointIntervalMinutes} minutes");

                    _stopwatch.Start();

                    // Run until cancelled
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            // Generate a unique contract name based on timestamp and counter
                            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string contractName = $"Contract_{timestamp}_{_contractsGenerated}";

                            // Generate and test the contract
                            bool success = await Task.Run(() => GenerateAndTestContract(contractName), token);

                            // Update statistics
                            _contractsGenerated++;
                            if (success)
                            {
                                _contractsSucceeded++;
                            }
                            else
                            {
                                _contractsFailed++;
                            }

                            // Log progress every 10 contracts
                            if (_contractsGenerated % 10 == 0)
                            {
                                LogProgress();
                            }

                            // Small delay to prevent overwhelming the system
                            await Task.Delay(100, token);
                        }
                        catch (OperationCanceledException)
                        {
                            // Expected when token is cancelled
                            break;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex, "Contract generation");
                            _contractsFailed++;
                        }
                    }

                    _stopwatch.Stop();
                    _endTime = DateTime.Now;

                    // Create final checkpoint
                    CreateCheckpoint(isFinal: true);

                    Logger.Info($"Long-running fuzzer completed at {_endTime}");
                    Logger.Info($"Total duration: {FormatDuration(_stopwatch.Elapsed)}");
                    Logger.Info($"Contracts generated: {_contractsGenerated}");
                    Logger.Info($"Contracts succeeded: {_contractsSucceeded}");
                    Logger.Info($"Contracts failed: {_contractsFailed}");
                    Logger.Info($"Success rate: {(_contractsGenerated > 0 ? (double)_contractsSucceeded / _contractsGenerated * 100 : 0):F2}%");

                    return _contractsFailed == 0;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "RunForDuration");
                    return false;
                }
                finally
                {
                    checkpointTimer.Dispose();
                }
            }
        }

        /// <summary>
        /// Stop the fuzzer
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Generate and test a contract
        /// </summary>
        private bool GenerateAndTestContract(string contractName)
        {
            try
            {
                Logger.Debug($"Generating contract: {contractName}");

                // Use the existing fuzzer to generate and test the contract
                return _fuzzer.GenerateAndTestSingleContract(contractName, _featuresPerContract);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"GenerateAndTestContract: {contractName}");
                return false;
            }
        }

        /// <summary>
        /// Create a checkpoint with current statistics
        /// </summary>
        private void CreateCheckpoint(bool isFinal = false)
        {
            try
            {
                TimeSpan elapsed = _stopwatch.Elapsed;
                DateTime now = DateTime.Now;

                // Create checkpoint directory if it doesn't exist
                string checkpointDir = Path.Combine(_outputDirectory, "Checkpoints");
                if (!Directory.Exists(checkpointDir))
                {
                    Directory.CreateDirectory(checkpointDir);
                }

                // Create checkpoint file
                string timestamp = now.ToString("yyyyMMdd_HHmmss");
                string checkpointFile = Path.Combine(checkpointDir, $"checkpoint_{timestamp}.md");

                using (StreamWriter writer = new StreamWriter(checkpointFile))
                {
                    writer.WriteLine("# Fuzzer Checkpoint Report");
                    writer.WriteLine();
                    writer.WriteLine($"Generated on: {now}");
                    writer.WriteLine();

                    writer.WriteLine("## Summary");
                    writer.WriteLine();
                    writer.WriteLine($"Start time: {_startTime}");
                    writer.WriteLine($"Current time: {now}");
                    writer.WriteLine($"Elapsed time: {FormatDuration(elapsed)}");
                    writer.WriteLine();

                    writer.WriteLine("## Statistics");
                    writer.WriteLine();
                    writer.WriteLine($"Contracts generated: {_contractsGenerated}");
                    writer.WriteLine($"Contracts succeeded: {_contractsSucceeded}");
                    writer.WriteLine($"Contracts failed: {_contractsFailed}");
                    writer.WriteLine($"Success rate: {(_contractsGenerated > 0 ? (double)_contractsSucceeded / _contractsGenerated * 100 : 0):F2}%");
                    writer.WriteLine();

                    writer.WriteLine("### Feature Coverage");
                    writer.WriteLine();
                    writer.WriteLine("The fuzzer tests the following Neo N3 features:");
                    writer.WriteLine();
                    writer.WriteLine("- **Data Types**: Primitive types, complex types, arrays, collections");
                    writer.WriteLine("- **Control Flow**: If statements, for loops");
                    writer.WriteLine("- **Storage Operations**: Basic storage, StorageMap, Storage.Find, contexts");
                    writer.WriteLine("- **Runtime Operations**: Properties, notifications, logging, CheckWitness, gas operations");
                    writer.WriteLine("- **Native Contract Calls**: NEO, GAS, ContractManagement, CryptoLib, Ledger, Oracle, Policy, RoleManagement, StdLib");
                    writer.WriteLine("- **Contract Features**: Attributes, contract calls, stored properties, methods with attributes, events");
                    writer.WriteLine();

                    writer.WriteLine("## Performance");
                    writer.WriteLine();
                    double contractsPerHour = _contractsGenerated / elapsed.TotalHours;
                    writer.WriteLine($"Contracts per hour: {contractsPerHour:F2}");
                    writer.WriteLine($"Average time per contract: {(elapsed.TotalMilliseconds / Math.Max(1, _contractsGenerated)):F2} ms");
                    writer.WriteLine();

                    if (isFinal)
                    {
                        writer.WriteLine("## Final Status");
                        writer.WriteLine();
                        writer.WriteLine($"End time: {_endTime}");
                        writer.WriteLine($"Total duration: {FormatDuration(_endTime - _startTime)}");
                        writer.WriteLine($"Final result: {(_contractsFailed == 0 ? "SUCCESS" : "FAILURE")}");
                    }
                }

                Logger.Info($"Checkpoint created: {checkpointFile}");
                LogProgress();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "CreateCheckpoint");
            }
        }

        /// <summary>
        /// Log current progress
        /// </summary>
        private void LogProgress()
        {
            TimeSpan elapsed = _stopwatch.Elapsed;
            double contractsPerHour = _contractsGenerated / Math.Max(0.01, elapsed.TotalHours);

            Logger.Info($"Progress: {_contractsGenerated} contracts generated ({_contractsSucceeded} succeeded, {_contractsFailed} failed)");
            Logger.Info($"Elapsed time: {FormatDuration(elapsed)}, Rate: {contractsPerHour:F2} contracts/hour");
        }

        /// <summary>
        /// Format a duration in a human-readable way
        /// </summary>
        private string FormatDuration(TimeSpan duration)
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
    }
}
