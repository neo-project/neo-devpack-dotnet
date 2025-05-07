using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using Neo.VM.Types;

namespace Neo.SmartContract.Fuzzer.Minimization
{
    /// <summary>
    /// Minimizes test cases while preserving the ability to trigger specific behaviors.
    /// </summary>
    public class TestCaseMinimizer
    {
        private readonly ContractExecutor _executor;
        private readonly ContractMethodDescriptor _method;
        private readonly Predicate<ExecutionResult> _predicate;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseMinimizer"/> class.
        /// </summary>
        /// <param name="executor">The contract executor.</param>
        /// <param name="method">The method descriptor.</param>
        /// <param name="predicate">A predicate that determines if a test case exhibits the target behavior.</param>
        /// <param name="seed">Random seed for reproducibility.</param>
        public TestCaseMinimizer(
            ContractExecutor executor,
            ContractMethodDescriptor method,
            Predicate<ExecutionResult> predicate,
            int seed)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _random = new Random(seed);
        }

        /// <summary>
        /// Minimizes a test case while preserving the ability to trigger the target behavior.
        /// </summary>
        /// <param name="testCase">The test case to minimize.</param>
        /// <returns>A minimized test case that still triggers the target behavior.</returns>
        public TestCase Minimize(TestCase testCase)
        {
            if (testCase == null)
                throw new ArgumentNullException(nameof(testCase));

            if (testCase.MethodName != _method.Name)
                throw new ArgumentException($"Test case method {testCase.MethodName} does not match minimizer method {_method.Name}", nameof(testCase));

            // Clone the test case to avoid modifying the original
            var minimizedTestCase = testCase.Clone();

            // Verify that the original test case triggers the target behavior
            var result = _executor.ExecuteMethod(_method, minimizedTestCase.Parameters, 0);
            if (!_predicate(result))
            {
                Console.WriteLine("Warning: Original test case does not trigger the target behavior");
                return minimizedTestCase;
            }

            // Minimize each parameter
            for (int i = 0; i < minimizedTestCase.Parameters.Length; i++)
            {
                minimizedTestCase.Parameters[i] = MinimizeParameter(
                    minimizedTestCase.Parameters,
                    i,
                    _method.Parameters[i].Type.ToString());
            }

            return minimizedTestCase;
        }

        private StackItem MinimizeParameter(StackItem[] parameters, int index, string type)
        {
            // Save the original parameter
            var originalParameter = parameters[index];

            // Try to minimize the parameter based on its type
            switch (originalParameter)
            {
                case VM.Types.Integer integer:
                    return MinimizeInteger(parameters, index, integer);

                case VM.Types.ByteString byteString:
                    return MinimizeByteString(parameters, index, byteString);

                case VM.Types.Array array:
                    return MinimizeArray(parameters, index, array, type);

                case VM.Types.Map map:
                    return MinimizeMap(parameters, index, map);

                default:
                    // For other types, try some common simplifications
                    return TryCommonSimplifications(parameters, index, originalParameter);
            }
        }

        private StackItem MinimizeInteger(StackItem[] parameters, int index, VM.Types.Integer integer)
        {
            var value = integer.GetInteger();
            var candidates = new List<System.Numerics.BigInteger>
            {
                0,
                1,
                -1,
                value / 2,
                value - 1,
                value + 1
            };

            // Try each candidate
            foreach (var candidate in candidates)
            {
                var newParameters = parameters.ToArray();
                newParameters[index] = new VM.Types.Integer(candidate);

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return new VM.Types.Integer(candidate);
                }
            }

            // If no simplification works, return the original
            return integer;
        }

        private StackItem MinimizeByteString(StackItem[] parameters, int index, VM.Types.ByteString byteString)
        {
            byte[] bytes = byteString.GetSpan().ToArray();

            // Try empty byte array
            if (bytes.Length > 0)
            {
                var newParameters = parameters.ToArray();
                newParameters[index] = new VM.Types.ByteString(System.Array.Empty<byte>());

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return new VM.Types.ByteString(System.Array.Empty<byte>());
                }
            }

            // Try shorter byte arrays
            if (bytes.Length > 1)
            {
                // Try binary search to find the minimum length
                int minLength = 1;
                int maxLength = bytes.Length - 1;

                while (minLength <= maxLength)
                {
                    int midLength = (minLength + maxLength) / 2;
                    byte[] shorterBytes = new byte[midLength];
                    System.Array.Copy(bytes, shorterBytes, midLength);

                    var newParameters = parameters.ToArray();
                    newParameters[index] = new VM.Types.ByteString(shorterBytes);

                    var result = _executor.ExecuteMethod(_method, newParameters, 0);
                    if (_predicate(result))
                    {
                        // We can go shorter
                        maxLength = midLength - 1;
                    }
                    else
                    {
                        // We need to go longer
                        minLength = midLength + 1;
                    }
                }

                // Use the minimum length that still triggers the behavior
                if (minLength < bytes.Length)
                {
                    byte[] minimizedBytes = new byte[minLength];
                    System.Array.Copy(bytes, minimizedBytes, minLength);
                    return new VM.Types.ByteString(minimizedBytes);
                }
            }

            // Try simplifying the content (replace with zeros)
            if (bytes.Length > 0)
            {
                byte[] simplifiedBytes = new byte[bytes.Length];
                var newParameters = parameters.ToArray();
                newParameters[index] = new VM.Types.ByteString(simplifiedBytes);

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return new VM.Types.ByteString(simplifiedBytes);
                }
            }

            // If no simplification works, return the original
            return byteString;
        }

        private StackItem MinimizeArray(StackItem[] parameters, int index, VM.Types.Array array, string type)
        {
            // Try empty array
            if (array.Count > 0)
            {
                var newParameters = parameters.ToArray();
                newParameters[index] = new VM.Types.Array();

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return new VM.Types.Array();
                }
            }

            // Try smaller arrays
            if (array.Count > 1)
            {
                // Try binary search to find the minimum size
                int minSize = 1;
                int maxSize = array.Count - 1;

                while (minSize <= maxSize)
                {
                    int midSize = (minSize + maxSize) / 2;
                    var smallerArray = new VM.Types.Array();
                    for (int i = 0; i < midSize; i++)
                    {
                        smallerArray.Add(array[i]);
                    }

                    var newParameters = parameters.ToArray();
                    newParameters[index] = smallerArray;

                    var result = _executor.ExecuteMethod(_method, newParameters, 0);
                    if (_predicate(result))
                    {
                        // We can go smaller
                        maxSize = midSize - 1;
                    }
                    else
                    {
                        // We need to go larger
                        minSize = midSize + 1;
                    }
                }

                // Use the minimum size that still triggers the behavior
                if (minSize < array.Count)
                {
                    var minimizedArray = new VM.Types.Array();
                    for (int i = 0; i < minSize; i++)
                    {
                        minimizedArray.Add(array[i]);
                    }
                    return minimizedArray;
                }
            }

            // Try simplifying array elements
            if (array.Count > 0)
            {
                var minimizedArray = new VM.Types.Array();
                for (int i = 0; i < array.Count; i++)
                {
                    // Create a temporary array with the current element
                    var tempParameters = parameters.ToArray();
                    var tempArray = new VM.Types.Array();
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (j == i)
                        {
                            // Try to minimize this element
                            var elementType = GetElementType(type);
                            var minimizedElement = MinimizeParameter(new StackItem[] { array[i] }, 0, elementType);
                            tempArray.Add(minimizedElement);
                        }
                        else
                        {
                            tempArray.Add(array[j]);
                        }
                    }
                    tempParameters[index] = tempArray;

                    // Check if it still triggers the behavior
                    var result = _executor.ExecuteMethod(_method, tempParameters, 0);
                    if (_predicate(result))
                    {
                        // Use the minimized element
                        minimizedArray = tempArray;
                    }
                    else
                    {
                        // Keep the original element
                        minimizedArray.Add(array[i]);
                    }
                }

                return minimizedArray;
            }

            // If no simplification works, return the original
            return array;
        }

        private StackItem MinimizeMap(StackItem[] parameters, int index, VM.Types.Map map)
        {
            // Try empty map
            if (map.Count > 0)
            {
                var newParameters = parameters.ToArray();
                newParameters[index] = new VM.Types.Map();

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return new VM.Types.Map();
                }
            }

            // Try smaller maps
            if (map.Count > 1)
            {
                var keys = map.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    var smallerMap = new VM.Types.Map();
                    for (int j = 0; j < keys.Length; j++)
                    {
                        if (j != i)
                        {
                            smallerMap[keys[j]] = map[keys[j]];
                        }
                    }

                    var newParameters = parameters.ToArray();
                    newParameters[index] = smallerMap;

                    var result = _executor.ExecuteMethod(_method, newParameters, 0);
                    if (_predicate(result))
                    {
                        // Recursively minimize the smaller map
                        return MinimizeMap(newParameters, index, smallerMap);
                    }
                }
            }

            // Try simplifying map values
            if (map.Count > 0)
            {
                var minimizedMap = new VM.Types.Map();
                var keys = map.Keys.ToArray();
                foreach (var key in keys)
                {
                    // Try to minimize the value
                    var value = map[key];
                    var minimizedValue = MinimizeParameter(new StackItem[] { value }, 0, "Any");

                    // Check if the minimized value still triggers the behavior
                    var tempMap = new VM.Types.Map();
                    foreach (var k in keys)
                    {
                        tempMap[k] = k.Equals(key) ? minimizedValue : map[k];
                    }

                    var newParameters = parameters.ToArray();
                    newParameters[index] = tempMap;

                    var result = _executor.ExecuteMethod(_method, newParameters, 0);
                    if (_predicate(result))
                    {
                        minimizedMap[key] = minimizedValue;
                    }
                    else
                    {
                        minimizedMap[key] = value;
                    }
                }

                return minimizedMap;
            }

            // If no simplification works, return the original
            return map;
        }

        private StackItem TryCommonSimplifications(StackItem[] parameters, int index, StackItem original)
        {
            // Try null/false/empty for various types
            var candidates = new List<StackItem>
            {
                VM.Types.StackItem.False,
                new VM.Types.Integer(0),
                new VM.Types.ByteString(System.Array.Empty<byte>()),
                new VM.Types.Array()
            };

            // Try each candidate
            foreach (var candidate in candidates)
            {
                var newParameters = parameters.ToArray();
                newParameters[index] = candidate;

                var result = _executor.ExecuteMethod(_method, newParameters, 0);
                if (_predicate(result))
                {
                    return candidate;
                }
            }

            // If no simplification works, return the original
            return original;
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
    }
}


