using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Neo.Persistence;
using Neo.SmartContract.Native;
using System.Numerics;
using Neo.Extensions;
using System.Text.Json.Serialization;
using Neo.SmartContract.Fuzzer.Utilities;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Executes Neo smart contract methods in a controlled environment
    /// </summary>
    public class ContractExecutor
    {
        private readonly byte[] _nefBytes;
        private readonly ContractManifest _manifest;
        private readonly long _gasLimit;
        private readonly string _outputDirectory;
        private readonly UInt160 _contractHash;
        private readonly FuzzerConfiguration _config;

        /// <summary>
        /// Initialize a new instance of the ContractExecutor class
        /// </summary>
        /// <param name="nefBytes">The NEF file bytes</param>
        /// <param name="manifest">The contract manifest</param>
        /// <param name="config">The fuzzer configuration</param>
        public ContractExecutor(byte[] nefBytes, ContractManifest manifest, FuzzerConfiguration config)
        {
            _nefBytes = nefBytes;
            _manifest = manifest;
            _gasLimit = config.GasLimit;
            _outputDirectory = config.OutputDirectory;
            _config = config;

            // Calculate the contract hash
            // Use a dummy checksum for testing purposes
            uint nefChecksum = BitConverter.ToUInt32(_nefBytes, 0);
            _contractHash = Helper.GetContractHash(UInt160.Zero, nefChecksum, _manifest.Name);
        }

        /// <summary>
        /// Execute a contract method with the given parameters
        /// </summary>
        /// <param name="method">The method to execute</param>
        /// <param name="parameters">The parameters to pass to the method</param>
        /// <param name="iterationNumber">The current iteration number</param>
        /// <param name="gasLimit">Optional gas limit for this execution (overrides the default)</param>
        /// <returns>The execution result</returns>
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iterationNumber, long gasLimit = 0)
        {
            // Create a new snapshot for this execution
            var snapshot = _config.PersistStateBetweenCalls
                ? TestBlockchain.GetOrCreatePersistentSnapshot()
                : TestBlockchain.GetSnapshot();

            // Create a script builder
            using ScriptBuilder sb = new ScriptBuilder();

            // Push parameters onto the stack in reverse order
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                EmitPush(sb, parameters[i]);
            }

            // Call the method using syscall
            sb.EmitPush(method.Name);
            sb.EmitPush(_contractHash.GetSpan().ToArray());
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

            // Create a new FuzzingApplicationEngine with improved tracking
            using FuzzingApplicationEngine engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                snapshot,
                null,
                null,
                _config.MaxExecutionTimeMs > 0 ? _config.MaxExecutionTimeMs : 10000, // Use configured timeout or default to 10 seconds
                gasLimit > 0 ? gasLimit : _gasLimit); // Use provided gas limit or default

            List<LogEventArgs> currentLogs = new();
            void LogHandler(object? sender, LogEventArgs args) => currentLogs.Add(args);
            ApplicationEngine.Log += LogHandler;

            // Track execution time
            var startTime = DateTime.Now;
            var instructionCount = 0L;
            var timedOut = false;

            // Create a list to track witness checks and external calls
            var witnessChecks = new List<WitnessCheckInfo>();
            var externalCalls = new List<ExternalCallInfo>();

            // Track witness checks
            engine.WitnessChecked += (sender, args) =>
            {
                witnessChecks.Add(new WitnessCheckInfo
                {
                    Account = args.Account,
                    Timestamp = args.InstructionCount,
                    Result = args.Result,
                    ResultUsed = true // Assume result is used by default
                });
            };

            // Track external calls
            engine.ExternalCallPerformed += (sender, args) =>
            {
                externalCalls.Add(new ExternalCallInfo
                {
                    Target = args.Target,
                    Method = args.Method,
                    Timestamp = args.InstructionCount,
                    Success = true // Will be updated after execution if needed
                });
            };

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Execute the script
            var result = new ExecutionResult
            {
                Method = method.Name,
                Parameters = parameters,
                IterationNumber = iterationNumber
            };

            try
            {
                // Execute the script
                engine.Execute();

                // Get the result
                if (engine.State == VMState.HALT)
                {
                    result.Success = true;
                    result.ReturnValue = engine.GetResult();
                    result.FeeConsumed = engine.FeeConsumed;
                    result.Engine = engine;
                }
                else
                {
                    result.Success = false;
                    result.Exception = engine.FaultException ?? new Exception("Unknown error");
                }

                // Set the execution statistics
                result.InstructionCount = engine.InstructionCount;
                result.ExecutionTime = engine.GetExecutionTime();
                result.TimedOut = engine.TimedOut;
                result.WitnessChecks = witnessChecks;
                result.ExternalCalls = externalCalls;
                result.StorageChanges = engine.StorageChanges;

                // Update external call results
                foreach (var call in result.ExternalCalls)
                {
                    // Check if the call was successful based on the notification data
                    bool callSuccess = false;

                    // Look for a notification that indicates the result of this call
                    foreach (var notification in engine.Notifications)
                    {
                        // Check if this is a notification related to the call
                        if (notification.EventName == "ContractCall" && notification.State.Count >= 3)
                        {
                            // Get the target contract and method name
                            string target = notification.State[0].GetString();
                            string methodName = notification.State[1].GetString();

                            // If this notification matches our call
                            if (target == call.TargetContract && methodName == call.Method)
                            {
                                // Check if there's a result indicator in the notification
                                if (notification.State.Count >= 3)
                                {
                                    // The third item is often a success indicator
                                    callSuccess = notification.State[2].GetBoolean();
                                    break;
                                }
                            }
                        }
                    }

                    // If we couldn't determine the success from notifications,
                    // check if there were any exceptions during the call
                    if (!callSuccess)
                    {
                        // Look for exceptions in the execution trace that might indicate failure
                        bool hasException = false;
                        foreach (var log in currentLogs)
                        {
                            if (log.Message.Contains("Exception") && log.Message.Contains(call.Method))
                            {
                                hasException = true;
                                break;
                            }
                        }

                        // If no exceptions were found, assume the call succeeded if the overall execution succeeded
                        callSuccess = !hasException && result.Success;
                    }

                    // Update the call's success status
                    call.Success = callSuccess;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
                result.InstructionCount = engine.InstructionCount;
                result.ExecutionTime = engine.GetExecutionTime();
                result.TimedOut = engine.TimedOut;
                result.WitnessChecks = witnessChecks;
                result.ExternalCalls = externalCalls;
                result.StorageChanges = engine.StorageChanges;
            }
            finally
            {
                // Remove event handlers
                ApplicationEngine.Log -= LogHandler;

                // Set the collected logs
                result.CollectedLogs = currentLogs;

                // Commit the snapshot if state persistence is enabled
                if (_config.PersistStateBetweenCalls)
                {
                    TestBlockchain.CommitPersistentSnapshot();
                }
            }

            // Save the result if configured
            SaveResult(method.Name, result);

            return result;
        }

        /// <summary>
        /// Deploy the contract to the test blockchain
        /// </summary>
        /// <returns>True if the deployment was successful</returns>
        public bool DeployContract()
        {
            bool success = false;
            try
            {
                // Get a snapshot
                var snapshot = _config.PersistStateBetweenCalls
                    ? TestBlockchain.GetOrCreatePersistentSnapshot()
                    : TestBlockchain.GetSnapshot();

                // Create a script to deploy the contract
                using ScriptBuilder sb = new ScriptBuilder();

                // Push the contract parameters in reverse order for the call
                sb.EmitPush(string.Empty); // Optional Data (pushed first)

                // Null check for _manifest before calling ToString()
                if (_manifest == null)
                {
                    throw new InvalidOperationException("Cannot deploy contract: Manifest not loaded.");
                }
                // Use null-coalescing operator in case ToString() returns null
                sb.EmitPush(_manifest.ToString() ?? string.Empty); // Manifest (second argument, pushed second)

                // Null check for _nefBytes
                if (_nefBytes == null)
                {
                    throw new InvalidOperationException("Cannot deploy contract: NEF bytes not loaded.");
                }
                sb.EmitPush(_nefBytes); // NEF (first argument, pushed last)

                // Call the deploy method of the contract management native contract
                sb.EmitPush("deploy"); // Method name
                sb.EmitPush(NativeContract.ContractManagement.Hash.GetSpan().ToArray()); // Contract hash
                sb.EmitSysCall(ApplicationEngine.System_Contract_Call); // Call

                // Create a new FuzzingApplicationEngine
                using var engine = FuzzingApplicationEngine.Create(
                    TriggerType.Application,
                    null,
                    snapshot,
                    null,
                    null,
                    10000); // 10 seconds timeout

                // Load the script
                engine.LoadScript(sb.ToArray());

                // Execute the script
                engine.Execute();

                // Check if the deployment was successful
                success = engine.State == VMState.HALT;
            }
            catch (Exception ex) // Capture exception details
            {
                Console.Error.WriteLine($"!!! Deployment failed with exception: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                success = false;
            }
            finally
            {
                // Commit the snapshot if state persistence is enabled
                if (_config.PersistStateBetweenCalls)
                {
                    TestBlockchain.CommitPersistentSnapshot();
                }
            }
            return success;
        }

        /// <summary>
        /// Save the execution result to a file
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="result">The execution result</param>
        private void SaveResult(string methodName, ExecutionResult result)
        {
            // Only save if SaveFailingInputsOnly is false, OR if it's true AND the execution failed
            if (!_config.SaveFailingInputsOnly || !result.Success)
            {
                try
                {
                    // Determine the base directory for saving
                    // Use a separate "Failures" directory if only saving failing inputs
                    string baseDirName = _config.SaveFailingInputsOnly ? "Failures" : "Results";
                    string resultDir = Path.Combine(_outputDirectory, baseDirName, methodName);
                    Directory.CreateDirectory(resultDir);

                    // Save the result to a file
                    string resultPath = Path.Combine(resultDir, $"result_{result.IterationNumber}.json");
                    string json = System.Text.Json.JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(resultPath, json);

                    // Save the parameters to a separate file (useful for replaying)
                    string inputPath = Path.Combine(resultDir, $"input_{result.IterationNumber}.json");
                    var inputJson = new
                    {
                        Parameters = result.SerializedParameters
                    };
                    json = System.Text.Json.JsonSerializer.Serialize(inputJson, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(inputPath, json);
                }
                catch (Exception ex)
                {
                    // Log saving error instead of ignoring silently
                    Console.WriteLine($"[WARN] Failed to save result/input for {methodName} iteration {result.IterationNumber}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Convert parameters to JSON
        /// </summary>
        /// <param name="parameters">The parameters to convert</param>
        /// <returns>A JSON representation of the parameters</returns>
        private object?[] ConvertParametersToJson(StackItem[] parameters)
        {
            if (parameters == null) return System.Array.Empty<object>();

            var result = new object?[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var jsonNode = JsonUtilities.ConvertStackItemToJson(parameters[i]);
                result[i] = jsonNode != null ? jsonNode.ToJsonString() : null;
            }

            return result;
        }

        /// <summary>
        /// Convert a stack item to a JSON-compatible object
        /// </summary>
        /// <param name="item">The stack item to convert</param>
        /// <returns>A JSON-compatible object</returns>
        public static object? ConvertStackItemToJson(StackItem item)
        {
            try
            {
                var jsonNode = JsonUtilities.ConvertStackItemToJson(item);
                return jsonNode != null ? jsonNode.ToJsonString() : null;
            }
            catch (Exception)
            {
                return "Error converting stack item";
            }
        }

        /// <summary>
        /// Convert a JSON representation back to a StackItem, given the expected type as a string.
        /// </summary>
        /// <param name="element">The JSON element to convert.</param>
        /// <param name="expectedType">The expected Neo VM type as a string.</param>
        /// <returns>The deserialized StackItem.</returns>
        public static StackItem ConvertJsonElementToStackItem(JsonElement element, string expectedType)
        {
            // Convert string type to ContractParameterType
            ContractParameterType paramType;
            if (Enum.TryParse(expectedType, true, out paramType))
            {
                return ConvertJsonElementToStackItem(element, paramType);
            }
            else
            {
                // Default to Any if type string is not recognized
                return ConvertJsonElementToStackItem(element, ContractParameterType.Any);
            }
        }

        /// <summary>
        /// Convert a JSON representation back to a StackItem, given the expected type.
        /// </summary>
        /// <param name="element">The JSON element to convert.</param>
        /// <param name="expectedType">The expected Neo VM type.</param>
        /// <returns>The deserialized StackItem.</returns>
        /// <exception cref="FormatException">Thrown if the JSON format doesn't match the expected type.</exception>
        /// <exception cref="NotSupportedException">Thrown for unsupported types or structures.</exception>
        public static StackItem ConvertJsonElementToStackItem(JsonElement element, ContractParameterType expectedType)
        {
            // Use our utility method for basic conversion
            StackItem baseItem = JsonUtilities.ConvertJsonElementToStackItem(element);

            // Validate against expected type
            switch (expectedType)
            {
                case ContractParameterType.Any:
                    return baseItem;

                case ContractParameterType.Boolean:
                    if (baseItem is VM.Types.Boolean)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.Integer:
                    if (baseItem is Integer)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.ByteArray:
                    if (baseItem is ByteString)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.String:
                    if (baseItem is ByteString)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.Hash160:
                    if (baseItem is ByteString bs1 && bs1.Size == 20)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType} (20 bytes), but got {baseItem.GetType().Name} of size {(baseItem is ByteString bs1a ? bs1a.Size : 0)}");

                case ContractParameterType.Hash256:
                    if (baseItem is ByteString bs2 && bs2.Size == 32)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType} (32 bytes), but got {baseItem.GetType().Name} of size {(baseItem is ByteString bs2a ? bs2a.Size : 0)}");

                case ContractParameterType.PublicKey:
                    if (baseItem is ByteString bs3 && (bs3.Size == 33 || bs3.Size == 65))
                        return baseItem;
                    throw new FormatException($"Expected {expectedType} (33 or 65 bytes), but got {baseItem.GetType().Name} of size {(baseItem is ByteString bs3a ? bs3a.Size : 0)}");

                case ContractParameterType.Signature:
                    if (baseItem is ByteString bs4 && bs4.Size == 64)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType} (64 bytes), but got {baseItem.GetType().Name} of size {(baseItem is ByteString bs4a ? bs4a.Size : 0)}");

                case ContractParameterType.Array:
                    if (baseItem is VM.Types.Array)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.Map:
                    if (baseItem is Map)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                case ContractParameterType.Void:
                    if (baseItem is Null)
                        return baseItem;
                    throw new FormatException($"Expected {expectedType}, but got {baseItem.GetType().Name}");

                default:
                    throw new NotSupportedException($"Unsupported ContractParameterType: {expectedType}");
            }
        }

        /// <summary>
        /// Emit a push instruction for a stack item
        /// </summary>
        /// <param name="sb">The script builder</param>
        /// <param name="item">The stack item to push</param>
        private void EmitPush(ScriptBuilder sb, StackItem item)
        {
            switch (item)
            {
                case Null _:
                    sb.EmitPush("");
                    break;
                case VM.Types.Boolean b:
                    sb.EmitPush(b.GetBoolean());
                    break;
                case Integer i:
                    sb.EmitPush(i.GetInteger());
                    break;
                case ByteString b:
                    sb.EmitPush(b.GetSpan());
                    break;
                case VM.Types.Array vmArray:
                    // For arrays, push each item and then the array count
                    for (int i = vmArray.Count - 1; i >= 0; i--)
                        EmitPush(sb, vmArray[i]);
                    sb.EmitPush(vmArray.Count);
                    sb.Emit(OpCode.PACK);
                    break;
                case Map map:
                    // For maps, push each key-value pair and then create the map
                    sb.EmitPush(0);
                    sb.Emit(OpCode.NEWMAP);
                    foreach (var pair in map)
                    {
                        sb.Emit(OpCode.DUP);
                        // Ensure key is a PrimitiveType
                        PrimitiveType primitiveKey = pair.Key as PrimitiveType ?? new ByteString(Encoding.UTF8.GetBytes(pair.Key.ToString()));
                        EmitPush(sb, primitiveKey);
                        EmitPush(sb, pair.Value);
                        sb.Emit(OpCode.SETITEM);
                    }
                    break;
                default:
                    // For unsupported types, push null
                    sb.EmitPush("");
                    break;
            }
        }
    }

    /// <summary>
    /// Represents the result of a contract execution
    /// </summary>
    public class ExecutionResult
    {
        /// <summary>
        /// The name of the method that was executed
        /// </summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// Whether the execution was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The exception that occurred during execution, if any
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Exception? Exception { get; set; }

        /// <summary>
        /// Exception message and type for serialization
        /// </summary>
        public string? ExceptionMessage => Exception?.Message;

        /// <summary>
        /// Exception type for serialization
        /// </summary>
        public string? ExceptionType => Exception?.GetType().Name;

        /// <summary>
        /// The return value of the execution, if any
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public StackItem? ReturnValue { get; set; }

        /// <summary>
        /// Serialized return value
        /// </summary>
        public string? SerializedReturnValue => ReturnValue != null ?
            JsonUtilities.ConvertStackItemToJson(ReturnValue)?.ToJsonString() : null;

        /// <summary>
        /// The amount of gas consumed by the execution
        /// </summary>
        public long FeeConsumed { get; set; }

        /// <summary>
        /// The engine that executed the contract
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public FuzzingApplicationEngine? Engine { get; set; }

        /// <summary>
        /// The parameters that were passed to the method
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public StackItem[] Parameters { get; set; } = System.Array.Empty<StackItem>();

        /// <summary>
        /// Serialized parameters for JSON serialization
        /// </summary>
        public string[] SerializedParameters
        {
            get => Parameters.Select(p => JsonUtilities.ConvertStackItemToJson(p)?.ToJsonString() ?? "null").ToArray();
            set
            {
                if (value == null)
                {
                    Parameters = System.Array.Empty<StackItem>();
                    return;
                }

                Parameters = new StackItem[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(value[i]);
                        Parameters[i] = JsonUtilities.ConvertJsonElementToStackItem(doc.RootElement);
                    }
                    catch
                    {
                        // If parsing fails, use the string as is
                        Parameters[i] = new ByteString(System.Text.Encoding.UTF8.GetBytes(value[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Current iteration number
        /// </summary>
        public int IterationNumber { get; set; }

        /// <summary>
        /// Logs collected during execution via Runtime.Log
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<LogEventArgs> CollectedLogs { get; set; } = new();

        /// <summary>
        /// Serialized logs for JSON serialization
        /// </summary>
        public List<string> SerializedLogs => CollectedLogs.Select(log => log.Message).ToList();

        /// <summary>
        /// Storage changes made during execution
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public StorageChangesCollection StorageChanges { get; set; } = new();

        /// <summary>
        /// External calls made during execution
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ExternalCallInfo> ExternalCalls { get; set; } = new();

        /// <summary>
        /// Witness checks made during execution
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<WitnessCheckInfo> WitnessChecks { get; set; } = new();

        /// <summary>
        /// Number of instructions executed
        /// </summary>
        public long InstructionCount { get; set; }

        /// <summary>
        /// Execution time
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }

        /// <summary>
        /// Whether the execution timed out
        /// </summary>
        public bool TimedOut { get; set; }
    }
}
