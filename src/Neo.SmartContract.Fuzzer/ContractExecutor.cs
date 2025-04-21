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
        /// <returns>The execution result</returns>
        public ExecutionResult ExecuteMethod(ContractMethodDescriptor method, StackItem[] parameters, int iterationNumber)
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

            // Create a new engine
            using ApplicationEngine engine = ApplicationEngine.Create(TriggerType.Application, null, snapshot);

            List<LogEventArgs> currentLogs = new();
            void LogHandler(object? sender, LogEventArgs args) => currentLogs.Add(args);
            ApplicationEngine.Log += LogHandler;

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
                    result.ReturnValue = engine.ResultStack.Count > 0 ? engine.ResultStack.Peek() : null;
                    result.FeeConsumed = engine.FeeConsumed;
                    result.Engine = engine;
                }
                else
                {
                    result.Success = false;
                    result.Exception = engine.FaultException?.ToString() ?? "Unknown error";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex.ToString();
            }
            finally
            {
                ApplicationEngine.Log -= LogHandler;
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

                // Create a new engine
                using var engine = ApplicationEngine.Create(TriggerType.Application, null, snapshot);

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

                    // Create a JSON representation of the result
                    var resultJson = new
                    {
                        Method = result.Method,
                        Success = result.Success,
                        Exception = result.Exception,
                        GasConsumed = result.FeeConsumed,
                        ReturnValue = result.ReturnValue != null ? ConvertStackItemToJson(result.ReturnValue) : null,
                        Parameters = ConvertParametersToJson(result.Parameters),
                        IterationNumber = result.IterationNumber,
                        CollectedLogs = result.CollectedLogs
                    };

                    // Save the result to a file
                    string resultPath = Path.Combine(resultDir, $"result_{result.IterationNumber}.json");
                    string json = System.Text.Json.JsonSerializer.Serialize(resultJson, new JsonSerializerOptions { WriteIndented = true }); // Added indentation
                    File.WriteAllText(resultPath, json);

                    // Save the parameters to a separate file (useful for replaying)
                    string inputPath = Path.Combine(resultDir, $"input_{result.IterationNumber}.json");
                    var inputJson = new
                    {
                        Parameters = ConvertParametersToJson(result.Parameters)
                    };
                    json = System.Text.Json.JsonSerializer.Serialize(inputJson, new JsonSerializerOptions { WriteIndented = true }); // Added indentation
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
                result[i] = ConvertStackItemToJson(parameters[i]);
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
                switch (item)
                {
                    case Null _:
                        return null;
                    case VM.Types.Boolean b:
                        return b.GetBoolean();
                    case Integer i:
                        return i.GetInteger();
                    case ByteString byteString:
                        // Convert byte strings to hex strings
                        return "0x" + BitConverter.ToString(byteString.GetSpan().ToArray()).Replace("-", string.Empty);
                    case VM.Types.Array vmArray:
                        var arrayResult = new object?[vmArray.Count];
                        for (int i = 0; i < vmArray.Count; i++)
                        {
                            arrayResult[i] = ConvertStackItemToJson(vmArray[i]);
                        }
                        return arrayResult;
                    case Map map:
                        var mapResult = new Dictionary<string, object?>();
                        foreach (var pair in map)
                        {
                            string key = ConvertStackItemToJson(pair.Key)?.ToString() ?? "null";
                            mapResult[key] = ConvertStackItemToJson(pair.Value);
                        }
                        return mapResult;
                    default:
                        return item.ToString();
                }
            }
            catch (Exception)
            {
                return "Error converting stack item";
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
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    // Allow Null for Any type, or if the specific type is Void (though Void isn't usually a parameter type)
                    if (expectedType == ContractParameterType.Any || expectedType == ContractParameterType.Void)
                        return StackItem.Null;
                    else
                        throw new FormatException($"Expected {expectedType}, but got null JSON value.");

                case JsonValueKind.True:
                case JsonValueKind.False:
                    if (expectedType == ContractParameterType.Boolean || expectedType == ContractParameterType.Any)
                        // FIX: Use static instances
                        return element.GetBoolean() ? VM.Types.Boolean.True : VM.Types.Boolean.False;
                    else
                        throw new FormatException($"Expected {expectedType}, but got boolean JSON value.");

                case JsonValueKind.Number:
                    if (expectedType == ContractParameterType.Integer || expectedType == ContractParameterType.Any)
                    {
                        // FIX: Remove non-existent TryGetBigInteger, rely on TryParse with RawText
                        if (BigInteger.TryParse(element.GetRawText(), out BigInteger bigIntValue))
                            return new Integer(bigIntValue);
                        else
                            throw new FormatException($"Could not parse JSON number '{element.GetRawText()}' as BigInteger.");
                    }
                    else
                        throw new FormatException($"Expected {expectedType}, but got number JSON value.");

                case JsonValueKind.String:
                    string strValue = element.GetString() ?? "";
                    switch (expectedType)
                    {
                        case ContractParameterType.String:
                            return new ByteString(Encoding.UTF8.GetBytes(strValue));
                        case ContractParameterType.ByteArray:
                        case ContractParameterType.Hash160:
                        case ContractParameterType.Hash256:
                        case ContractParameterType.PublicKey:
                        case ContractParameterType.Signature:
                            // FIX: Replace Address with Hash160
                            // case ContractParameterType.Address:
                            if (strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    byte[] bytes = Convert.FromHexString(strValue.Substring(2));
                                    // Basic validation based on expected type
                                    // FIX: Replace Address with Hash160
                                    if (expectedType == ContractParameterType.Hash160 /*|| expectedType == ContractParameterType.Address*/)
                                    {
                                        if (bytes.Length != 20) throw new FormatException($"Expected 20 bytes for {expectedType}, got {bytes.Length}.");
                                    }
                                    else if (expectedType == ContractParameterType.Hash256)
                                    {
                                        if (bytes.Length != 32) throw new FormatException($"Expected 32 bytes for {expectedType}, got {bytes.Length}.");
                                    }
                                    else if (expectedType == ContractParameterType.PublicKey)
                                    {
                                        if (bytes.Length != 33) throw new FormatException($"Expected 33 bytes for {expectedType}, got {bytes.Length}."); // Assuming compressed
                                    }
                                    else if (expectedType == ContractParameterType.Signature)
                                    {
                                        if (bytes.Length != 64) throw new FormatException($"Expected 64 bytes for {expectedType}, got {bytes.Length}.");
                                    }
                                    return new ByteString(bytes);
                                }
                                catch (FormatException hexEx)
                                {
                                    throw new FormatException($"Invalid hex string format for {expectedType}: {strValue}", hexEx);
                                }
                            }
                            else
                                throw new FormatException($"Expected hex string starting with '0x' for {expectedType}, but got: {strValue}");
                        case ContractParameterType.Any: // If type is Any, assume hex string if starts with 0x, otherwise UTF8 string
                            if (strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            {
                                try { return new ByteString(Convert.FromHexString(strValue.Substring(2))); }
                                catch { return new ByteString(Encoding.UTF8.GetBytes(strValue)); } // Fallback to string if hex parse fails
                            }
                            else
                            {
                                return new ByteString(Encoding.UTF8.GetBytes(strValue));
                            }
                        default:
                            throw new FormatException($"Expected {expectedType}, but got string JSON value.");
                    }

                case JsonValueKind.Array:
                    if (expectedType == ContractParameterType.Array || expectedType == ContractParameterType.Any)
                    {
                        var neoArray = new VM.Types.Array();
                        foreach (var itemElement in element.EnumerateArray())
                        {
                            // Limitation: Cannot know the exact expected type of array elements. Assume 'Any'.
                            neoArray.Add(ConvertJsonElementToStackItem(itemElement, ContractParameterType.Any));
                        }
                        return neoArray;
                    }
                    else
                        throw new FormatException($"Expected {expectedType}, but got array JSON value.");

                case JsonValueKind.Object:
                    if (expectedType == ContractParameterType.Map || expectedType == ContractParameterType.Any)
                    {
                        var neoMap = new Map();
                        foreach (var property in element.EnumerateObject())
                        {
                            // FIX: Explicitly type keyItem as PrimitiveType
                            PrimitiveType keyItem;
                            if (BigInteger.TryParse(property.Name, out BigInteger keyInt))
                            {
                                keyItem = new Integer(keyInt);
                            }
                            else if (property.Name.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            {
                                try { keyItem = new ByteString(Convert.FromHexString(property.Name.Substring(2))); }
                                catch { keyItem = new ByteString(Encoding.UTF8.GetBytes(property.Name)); } // Fallback
                            }
                            else
                            {
                                keyItem = new ByteString(Encoding.UTF8.GetBytes(property.Name));
                            }

                            StackItem valueItem = ConvertJsonElementToStackItem(property.Value, ContractParameterType.Any);
                            neoMap[keyItem] = valueItem; // Now should compile
                        }
                        return neoMap;
                    }
                    else
                        throw new FormatException($"Expected {expectedType}, but got object JSON value.");

                default:
                    throw new NotSupportedException($"Unsupported JsonValueKind: {element.ValueKind}");
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
                        EmitPush(sb, pair.Key);
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
        public string? Exception { get; set; }

        /// <summary>
        /// The return value of the execution, if any
        /// </summary>
        public StackItem? ReturnValue { get; set; }

        /// <summary>
        /// The amount of gas consumed by the execution
        /// </summary>
        public long FeeConsumed { get; set; }

        /// <summary>
        /// The engine that executed the contract
        /// </summary>
        public ApplicationEngine? Engine { get; set; }

        /// <summary>
        /// The parameters that were passed to the method
        /// </summary>
        public StackItem[] Parameters { get; set; } = System.Array.Empty<StackItem>();

        /// <summary>
        /// Current iteration number
        /// </summary>
        public int IterationNumber { get; set; }

        /// <summary>
        /// Logs collected during execution via Runtime.Log
        /// </summary>
        public List<LogEventArgs> CollectedLogs { get; set; } = new();
    }
}
