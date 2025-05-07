using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Fuzzer.StaticAnalysis;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Neo.SmartContract.Fuzzer.InputGeneration
{
    /// <summary>
    /// Generates test cases for fuzzing smart contracts.
    /// </summary>
    public class InputGenerator
    {
        private readonly Random _random;
        private readonly ContractManifest _manifest;
        private readonly FeedbackAggregator _feedbackAggregator;
        private readonly List<TestCase> _corpus = new List<TestCase>();
        private readonly ParameterGenerator _parameterGenerator;
        private readonly AdvancedParameterGenerator _advancedParameterGenerator;
        private readonly string _corpusDirectory;
        private readonly int _maxCorpusSize;
        private int _iteration = 0;
        private readonly Dictionary<string, int> _methodSuccessCount = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _methodFailureCount = new Dictionary<string, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGenerator"/> class.
        /// </summary>
        /// <param name="manifest">The contract manifest.</param>
        /// <param name="feedbackAggregator">The feedback aggregator.</param>
        /// <param name="seed">Random seed for reproducibility.</param>
        /// <param name="corpusDirectory">Directory for storing and loading corpus.</param>
        /// <param name="maxCorpusSize">Maximum number of test cases in corpus.</param>
        public InputGenerator(
            ContractManifest manifest,
            FeedbackAggregator feedbackAggregator,
            int seed,
            string corpusDirectory,
            int maxCorpusSize = 1000)
        {
            _random = new Random(seed);
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _feedbackAggregator = feedbackAggregator ?? throw new ArgumentNullException(nameof(feedbackAggregator));
            _corpusDirectory = corpusDirectory ?? throw new ArgumentNullException(nameof(corpusDirectory));
            _maxCorpusSize = maxCorpusSize;
            _parameterGenerator = new ParameterGenerator(seed);
            _advancedParameterGenerator = new AdvancedParameterGenerator(seed);

            // Initialize method success/failure counts
            foreach (var method in _manifest.Abi.Methods)
            {
                _methodSuccessCount[method.Name] = 0;
                _methodFailureCount[method.Name] = 0;
            }

            // Create corpus directory if it doesn't exist
            Directory.CreateDirectory(_corpusDirectory);

            // Load existing corpus if available
            LoadCorpus();
        }

        /// <summary>
        /// Generates a test case for a method.
        /// </summary>
        /// <param name="methodName">The name of the method to test.</param>
        /// <returns>A test case for the method.</returns>
        public TestCase GenerateTestCase(string methodName)
        {
            _iteration++;

            // Get method descriptor
            var method = _manifest.Abi.Methods.FirstOrDefault(m => m.Name == methodName);
            if (method == null)
                throw new ArgumentException($"Method {methodName} not found in manifest", nameof(methodName));

            // Get feedback for this method
            var feedback = _feedbackAggregator.GetNextFeedback();

            // Determine the generation strategy based on iteration, feedback, and method success/failure rates
            // This helps balance exploration and exploitation
            int strategy;

            // Calculate success rate for this method
            double successRate = 0.5; // Default to 50% if no data
            if (_methodSuccessCount.ContainsKey(methodName) && _methodFailureCount.ContainsKey(methodName))
            {
                int totalExecutions = _methodSuccessCount[methodName] + _methodFailureCount[methodName];
                if (totalExecutions > 0)
                {
                    successRate = (double)_methodSuccessCount[methodName] / totalExecutions;
                }
            }

            if (_iteration < 10)
            {
                // In early iterations, focus on exploration
                strategy = _random.Next(6); // Expanded to include new strategies
            }
            else if (_iteration % 10 == 0)
            {
                // Every 10th iteration, generate a completely random test case
                // to avoid getting stuck in local optima
                strategy = 3;
            }
            else if (successRate < 0.1 && _iteration > 20)
            {
                // If success rate is very low, focus on edge cases and known good values
                strategy = _random.Next(4, 6); // Use advanced strategies
            }
            else
            {
                // Otherwise, use a weighted random selection based on what's been effective
                // Adjust weights based on success rate
                int[] weights;

                if (successRate < 0.3)
                {
                    // Low success rate: favor advanced strategies
                    weights = new int[] { 20, 15, 15, 10, 25, 15 }; // More weight to advanced strategies
                }
                else if (successRate > 0.7)
                {
                    // High success rate: favor mutation of existing test cases
                    weights = new int[] { 40, 20, 25, 5, 5, 5 }; // More weight to mutation
                }
                else
                {
                    // Medium success rate: balanced approach
                    weights = new int[] { 30, 20, 20, 10, 10, 10 }; // Balanced weights
                }

                int totalWeight = weights.Sum();
                int randomValue = _random.Next(totalWeight);
                int currentSum = 0;

                strategy = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    currentSum += weights[i];
                    if (randomValue < currentSum)
                    {
                        strategy = i;
                        break;
                    }
                }
            }

            // Apply the selected strategy
            switch (strategy)
            {
                case 0: // Use feedback-based test case if available
                    if (feedback?.RelatedTestCase != null && feedback.RelatedTestCase.MethodName == methodName)
                    {
                        return MutateTestCase(feedback.RelatedTestCase, method);
                    }
                    goto case 1; // Fall through to next strategy if this one isn't applicable

                case 1: // Use static hint if available
                    if (feedback?.StaticHint != null && feedback.StaticHint.MethodName == methodName)
                    {
                        return GenerateTestCaseFromHint(method, feedback.StaticHint);
                    }
                    goto case 2; // Fall through to next strategy if this one isn't applicable

                case 2: // Use corpus-based test case if available
                    if (_corpus.Any(tc => tc.MethodName == methodName))
                    {
                        // Select a test case from the corpus using weighted random selection
                        // Higher energy test cases are more likely to be selected
                        var methodCorpus = _corpus.Where(tc => tc.MethodName == methodName).ToList();
                        double totalEnergy = methodCorpus.Sum(tc => tc.Energy);
                        double randomValue = _random.NextDouble() * totalEnergy;
                        double currentSum = 0;

                        foreach (var testCase in methodCorpus.OrderByDescending(tc => tc.Energy))
                        {
                            currentSum += testCase.Energy;
                            if (randomValue < currentSum)
                            {
                                return MutateTestCase(testCase, method);
                            }
                        }

                        // Fallback to the highest energy test case
                        return MutateTestCase(methodCorpus.OrderByDescending(tc => tc.Energy).First(), method);
                    }
                    goto case 3; // Fall through to next strategy if this one isn't applicable

                case 3: // Generate a completely random test case
                    return GenerateRandomTestCase(method);

                case 4: // Generate test case with edge case values
                    return GenerateEdgeCaseTestCase(method);

                case 5: // Generate test case with known good values
                    return GenerateKnownGoodTestCase(method);

                default:
                    return GenerateRandomTestCase(method);
            }
        }

        /// <summary>
        /// Adds a test case to the corpus if it's interesting.
        /// </summary>
        /// <param name="testCase">The test case to add.</param>
        /// <param name="isInteresting">Whether the test case is interesting.</param>
        public void AddToCorpus(TestCase testCase, bool isInteresting)
        {
            if (!isInteresting)
                return;

            // Clone the test case to avoid modifications
            var clone = testCase.Clone();

            // Add to corpus
            _corpus.Add(clone);

            // Save to disk
            SaveTestCase(clone);

            // Prune corpus if it's too large
            if (_corpus.Count > _maxCorpusSize)
            {
                // Remove the least interesting test case
                var leastInteresting = _corpus
                    .OrderBy(tc => tc.Energy)
                    .First();

                _corpus.Remove(leastInteresting);

                // Delete from disk
                string filePath = Path.Combine(_corpusDirectory, $"{leastInteresting.MethodName}_{leastInteresting.Iteration}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// Updates the success/failure counts for a method based on execution result.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="success">Whether the execution was successful.</param>
        public void UpdateMethodStatistics(string methodName, bool success)
        {
            if (!_methodSuccessCount.ContainsKey(methodName))
            {
                _methodSuccessCount[methodName] = 0;
            }

            if (!_methodFailureCount.ContainsKey(methodName))
            {
                _methodFailureCount[methodName] = 0;
            }

            if (success)
            {
                _methodSuccessCount[methodName]++;
            }
            else
            {
                _methodFailureCount[methodName]++;
            }
        }

        /// <summary>
        /// Adds a known good value for a specific parameter type.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The known good value.</param>
        public void AddKnownGoodValue(string type, StackItem value)
        {
            _advancedParameterGenerator.AddKnownGoodValue(type, value);
        }

        private TestCase GenerateRandomTestCase(ContractMethodDescriptor method)
        {
            var parameters = new StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                // Use the advanced parameter generator with context information
                string paramType = method.Parameters[i].Type.ToString();
                string paramName = method.Parameters[i].Name;

                // Use the advanced parameter generator with 80% probability
                if (_random.Next(10) < 8)
                {
                    // Use context-aware parameter generation when parameter name is available
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        parameters[i] = _advancedParameterGenerator.GenerateParameterWithContext(paramType, method.Name, paramName, 0);
                    }
                    else
                    {
                        parameters[i] = _advancedParameterGenerator.GenerateParameter(paramType, 0);
                    }
                }
                else
                {
                    parameters[i] = _parameterGenerator.GenerateParameter(paramType, 0);
                }
            }

            return new TestCase
            {
                MethodName = method.Name,
                Parameters = parameters,
                Energy = 1.0,
                Iteration = _iteration,
                Timestamp = DateTime.Now
            };
        }

        private TestCase GenerateEdgeCaseTestCase(ContractMethodDescriptor method)
        {
            var parameters = new StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                parameters[i] = _advancedParameterGenerator.GenerateEdgeCaseValue(method.Parameters[i].Type.ToString());
            }

            return new TestCase
            {
                MethodName = method.Name,
                Parameters = parameters,
                Energy = 1.5, // Higher energy for edge cases
                Iteration = _iteration,
                Timestamp = DateTime.Now
            };
        }

        private TestCase GenerateKnownGoodTestCase(ContractMethodDescriptor method)
        {
            var parameters = new StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                // Use known good values with high probability
                parameters[i] = _advancedParameterGenerator.GenerateParameter(method.Parameters[i].Type.ToString(), 0);
            }

            return new TestCase
            {
                MethodName = method.Name,
                Parameters = parameters,
                Energy = 1.2, // Higher energy for known good values
                Iteration = _iteration,
                Timestamp = DateTime.Now
            };
        }

        private TestCase MutateTestCase(TestCase baseTestCase, ContractMethodDescriptor method)
        {
            // Clone the base test case
            var newTestCase = baseTestCase.Clone();
            newTestCase.Iteration = _iteration;
            newTestCase.Timestamp = DateTime.Now;

            // Determine mutation strategy based on iteration
            double mutationRate;
            int mutationIntensity;

            if (_iteration < 20)
            {
                // Early iterations: high mutation rate, low intensity
                mutationRate = 0.7; // 70% chance to mutate each parameter
                mutationIntensity = 1; // Low intensity mutations
            }
            else if (_iteration < 50)
            {
                // Middle iterations: medium mutation rate, medium intensity
                mutationRate = 0.5; // 50% chance to mutate each parameter
                mutationIntensity = 2; // Medium intensity mutations
            }
            else
            {
                // Later iterations: lower mutation rate, higher intensity
                mutationRate = 0.3; // 30% chance to mutate each parameter
                mutationIntensity = 3; // High intensity mutations
            }

            // Every 10th iteration, do a more aggressive mutation
            if (_iteration % 10 == 0)
            {
                mutationRate = 0.9; // 90% chance to mutate each parameter
                mutationIntensity = 3; // High intensity mutations
            }

            // Mutate parameters
            for (int i = 0; i < newTestCase.Parameters.Length; i++)
            {
                // Apply mutation based on calculated rate
                if (_random.NextDouble() < mutationRate)
                {
                    // Apply multiple mutations based on intensity
                    for (int j = 0; j < mutationIntensity; j++)
                    {
                        newTestCase.Parameters[i] = MutateParameter(newTestCase.Parameters[i], method.Parameters[i].Type.ToString());
                    }
                }
            }

            // Preserve the energy from the base test case but slightly reduce it
            // This encourages exploration of new variants while still respecting past success
            newTestCase.Energy = baseTestCase.Energy * 0.95;

            return newTestCase;
        }

        private TestCase GenerateTestCaseFromHint(ContractMethodDescriptor method, StaticAnalysisHint hint)
        {
            var parameters = new StackItem[method.Parameters.Length];

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                var parameter = method.Parameters[i];

                // If the hint is for a specific parameter, generate a special value for it
                if (hint.ParameterName == parameter.Name)
                {
                    parameters[i] = GenerateSpecialValueForHint(parameter.Type.ToString(), hint);
                }
                else
                {
                    parameters[i] = _parameterGenerator.GenerateParameter(parameter.Type.ToString(), 0);
                }
            }

            return new TestCase
            {
                MethodName = method.Name,
                Parameters = parameters,
                Energy = 1.0 + hint.Priority / 100.0, // Higher priority hints get more energy
                Iteration = _iteration,
                Timestamp = DateTime.Now
            };
        }

        private StackItem MutateParameter(StackItem parameter, string type)
        {
            switch (parameter)
            {
                case VM.Types.Boolean boolean:
                    // Flip boolean
                    return boolean.GetBoolean() ? VM.Types.StackItem.False : VM.Types.StackItem.True;

                case VM.Types.Integer integer:
                    // Mutate integer
                    return MutateInteger(integer);

                case VM.Types.ByteString byteString:
                    // Mutate byte string
                    return MutateByteString(byteString);

                case VM.Types.Array array:
                    // Mutate array
                    return MutateArray(array, type);

                case VM.Types.Map map:
                    // Mutate map
                    return MutateMap(map);

                default:
                    // For other types, generate a new value
                    return _parameterGenerator.GenerateParameter(type, 0);
            }
        }

        private StackItem MutateInteger(VM.Types.Integer integer)
        {
            var value = integer.GetInteger();

            // Choose a mutation strategy
            switch (_random.Next(6))
            {
                case 0: // Add a small value
                    return new VM.Types.Integer(value + _random.Next(1, 100));
                case 1: // Subtract a small value
                    return new VM.Types.Integer(value - _random.Next(1, 100));
                case 2: // Multiply by a small value
                    return new VM.Types.Integer(value * _random.Next(2, 10));
                case 3: // Divide by a small value (avoid division by zero)
                    int divisor = _random.Next(1, 10);
                    return new VM.Types.Integer(value / divisor);
                case 4: // Set to a boundary value
                    return new VM.Types.Integer(GetBoundaryInteger());
                default: // Generate a new random value
                    return _parameterGenerator.GenerateInteger();
            }
        }

        private StackItem MutateByteString(VM.Types.ByteString byteString)
        {
            byte[] bytes = byteString.GetSpan().ToArray();

            // Choose a mutation strategy
            switch (_random.Next(5))
            {
                case 0: // Flip a random bit
                    if (bytes.Length > 0)
                    {
                        int index = _random.Next(bytes.Length);
                        int bit = _random.Next(8);
                        bytes[index] ^= (byte)(1 << bit);
                    }
                    break;
                case 1: // Replace a random byte
                    if (bytes.Length > 0)
                    {
                        int index = _random.Next(bytes.Length);
                        bytes[index] = (byte)_random.Next(256);
                    }
                    break;
                case 2: // Add a byte
                    System.Array.Resize(ref bytes, bytes.Length + 1);
                    bytes[bytes.Length - 1] = (byte)_random.Next(256);
                    break;
                case 3: // Remove a byte
                    if (bytes.Length > 0)
                    {
                        System.Array.Resize(ref bytes, bytes.Length - 1);
                    }
                    break;
                case 4: // Generate a new random byte string
                    return _parameterGenerator.GenerateByteString();
            }

            return new VM.Types.ByteString(bytes);
        }

        private StackItem MutateArray(VM.Types.Array array, string type)
        {
            var result = new VM.Types.Array();

            // Copy existing items
            foreach (var item in array)
            {
                result.Add(item);
            }

            // Choose a mutation strategy
            switch (_random.Next(4))
            {
                case 0: // Add an item
                    result.Add(_parameterGenerator.GenerateParameter(GetElementType(type), 0));
                    break;
                case 1: // Remove an item
                    if (result.Count > 0)
                    {
                        result.RemoveAt(_random.Next(result.Count));
                    }
                    break;
                case 2: // Mutate an item
                    if (result.Count > 0)
                    {
                        int index = _random.Next(result.Count);
                        result[index] = MutateParameter(result[index], GetElementType(type));
                    }
                    break;
                case 3: // Clear and generate new items
                    result.Clear();
                    int count = _random.Next(5);
                    for (int i = 0; i < count; i++)
                    {
                        result.Add(_parameterGenerator.GenerateParameter(GetElementType(type), 0));
                    }
                    break;
            }

            return result;
        }

        private StackItem MutateMap(VM.Types.Map map)
        {
            var result = new VM.Types.Map();

            // Copy existing items
            foreach (var key in map.Keys)
            {
                result[key] = map[key];
            }

            // Choose a mutation strategy
            switch (_random.Next(4))
            {
                case 0: // Add an item
                    var newKey = _parameterGenerator.GenerateParameter("ByteArray", 0);
                    var newValue = _parameterGenerator.GenerateParameter("Any", 0);
                    // Convert key to PrimitiveType if needed
                    PrimitiveType primitiveKey = newKey as PrimitiveType ?? new ByteString(Encoding.UTF8.GetBytes(newKey.ToString()));
                    if (!result.ContainsKey(primitiveKey))
                    {
                        result[primitiveKey] = newValue;
                    }
                    break;
                case 1: // Remove an item
                    if (result.Count > 0)
                    {
                        var keys = result.Keys.ToArray();
                        result.Remove(keys[_random.Next(keys.Length)]);
                    }
                    break;
                case 2: // Mutate a value
                    if (result.Count > 0)
                    {
                        var keys = result.Keys.ToArray();
                        var key = keys[_random.Next(keys.Length)];
                        result[key] = MutateParameter(result[key], "Any");
                    }
                    break;
                case 3: // Clear and generate new items
                    result.Clear();
                    int count = _random.Next(5);
                    for (int i = 0; i < count; i++)
                    {
                        var key = _parameterGenerator.GenerateParameter("ByteArray", 0);
                        var value = _parameterGenerator.GenerateParameter("Any", 0);
                        // Convert key to PrimitiveType if needed
                        PrimitiveType primitiveKey2 = key as PrimitiveType ?? new ByteString(Encoding.UTF8.GetBytes(key.ToString()));
                        if (!result.ContainsKey(primitiveKey2))
                        {
                            result[primitiveKey2] = value;
                        }
                    }
                    break;
            }

            return result;
        }

        private StackItem GenerateSpecialValueForHint(string type, StaticAnalysisHint hint)
        {
            // Generate special values based on the hint type
            switch (hint.RiskType)
            {
                case "IntegerParameter":
                    // For integer parameters, generate boundary values
                    return new VM.Types.Integer(GetBoundaryInteger());

                case "StorageOperation":
                    // For storage operations, generate special storage keys
                    if (type == "ByteArray")
                    {
                        return new VM.Types.ByteString(GetSpecialStorageKey());
                    }
                    break;

                case "ArithmeticOperation":
                    // For arithmetic operations, generate values that might cause overflow
                    if (IsIntegerType(type))
                    {
                        return new VM.Types.Integer(GetOverflowInteger());
                    }
                    break;

                case "MissingWitnessCheck":
                    // For missing witness checks, generate valid addresses
                    if (type == "Hash160")
                    {
                        return new VM.Types.ByteString(GetValidAddress());
                    }
                    break;
            }

            // Default to normal parameter generation
            return _parameterGenerator.GenerateParameter(type, 0);
        }

        private System.Numerics.BigInteger GetBoundaryInteger()
        {
            // Generate boundary values for integers
            switch (_random.Next(8))
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return -1;
                case 3: return int.MaxValue;
                case 4: return int.MinValue;
                case 5: return long.MaxValue;
                case 6: return long.MinValue;
                default: return _random.Next(1000) - 500; // Small values around zero
            }
        }

        private System.Numerics.BigInteger GetOverflowInteger()
        {
            // Generate values that might cause overflow
            switch (_random.Next(4))
            {
                case 0: return int.MaxValue;
                case 1: return int.MinValue;
                case 2: return long.MaxValue;
                case 3: return long.MinValue;
                default: return int.MaxValue - _random.Next(100); // Close to max value
            }
        }

        private byte[] GetSpecialStorageKey()
        {
            // Generate special storage keys
            switch (_random.Next(4))
            {
                case 0: return new byte[0]; // Empty key
                case 1: return new byte[1024]; // Very large key
                case 2: return new byte[] { 0x01 }; // Single byte key
                default: // Random key with special prefix
                    byte[] key = new byte[_random.Next(1, 32)];
                    _random.NextBytes(key);
                    key[0] = (byte)_random.Next(4); // Use common prefixes (0, 1, 2, 3)
                    return key;
            }
        }

        private byte[] GetValidAddress()
        {
            // Generate a valid Neo address (20 bytes)
            byte[] address = new byte[20];
            _random.NextBytes(address);
            return address;
        }

        private string GetElementType(string arrayType)
        {
            // Extract element type from array type
            if (arrayType.EndsWith("[]"))
            {
                return arrayType.Substring(0, arrayType.Length - 2);
            }
            return "Any"; // Default to Any if type is unknown
        }

        private static bool IsIntegerType(string type)
        {
            return type == "Integer" || type == "Hash160" || type == "Hash256" ||
                   type == "ByteArray" || type == "PublicKey";
        }

        private void LoadCorpus()
        {
            // Load corpus from directory
            foreach (var file in Directory.GetFiles(_corpusDirectory, "*.json"))
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var testCase = TestCase.FromJson(json);
                    _corpus.Add(testCase);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading corpus file {file}: {ex.Message}");
                }
            }

            Console.WriteLine($"Loaded {_corpus.Count} test cases from corpus");
        }

        private void SaveTestCase(TestCase testCase)
        {
            try
            {
                string filePath = Path.Combine(_corpusDirectory, $"{testCase.MethodName}_{testCase.Iteration}.json");
                File.WriteAllText(filePath, testCase.ToJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving test case: {ex.Message}");
            }
        }
    }
}


