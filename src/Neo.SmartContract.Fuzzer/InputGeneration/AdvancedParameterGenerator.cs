using Neo.VM.Types;
using Array = Neo.VM.Types.Array;
using Map = Neo.VM.Types.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Fuzzer.InputGeneration
{
    /// <summary>
    /// Generates advanced test parameters for fuzzing smart contracts.
    /// </summary>
    public class AdvancedParameterGenerator
    {
        private readonly Random _random;
        private readonly Dictionary<string, List<StackItem>> _knownGoodValues = new Dictionary<string, List<StackItem>>();
        private int _mutationRate = 50; // 50% chance of mutation by default

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedParameterGenerator"/> class.
        /// </summary>
        /// <param name="seed">Random seed for reproducibility.</param>
        public AdvancedParameterGenerator(int seed)
        {
            _random = new Random(seed);
            InitializeKnownGoodValues();
        }

        /// <summary>
        /// Generates a parameter of the specified type using advanced strategies.
        /// </summary>
        /// <param name="type">The type of parameter to generate.</param>
        /// <param name="depth">Current recursion depth for nested types.</param>
        /// <returns>A stack item of the specified type.</returns>
        public StackItem GenerateParameter(string type, int depth)
        {
            // Occasionally generate an edge case value to increase coverage
            if (_random.Next(100) < 20)
            {
                return GenerateEdgeCaseValue(type);
            }

            // Use known good values with some probability
            if (_knownGoodValues.ContainsKey(type) && _random.Next(100) < 30)
            {
                var values = _knownGoodValues[type];
                return values[_random.Next(values.Count)];
            }

            // Otherwise, generate a value based on the type
            switch (type.ToLowerInvariant())
            {
                case "boolean":
                    return GenerateBoolean();
                case "integer":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                    return GenerateInteger();
                case "string":
                    return GenerateString();
                case "bytearray":
                    return GenerateByteString("bytearray");
                case "hash160":
                    return GenerateByteString("hash160");
                case "hash256":
                    return GenerateByteString("hash256");
                case "publickey":
                    return GenerateByteString("publickey");
                case "signature":
                    return GenerateByteString("signature");
                case "array":
                    return GenerateArray(depth);
                case "map":
                    return GenerateMap(depth);
                default:
                    if (type.EndsWith("[]"))
                    {
                        return GenerateTypedArray(type.Substring(0, type.Length - 2), depth);
                    }
                    return GenerateByteString("bytearray"); // Default to ByteString for unknown types
            }
        }

        /// <summary>
        /// Generates a string parameter with special focus on Neo-specific values.
        /// </summary>
        /// <returns>A string stack item.</returns>
        private StackItem GenerateString()
        {
            // Generate Neo-specific strings with higher probability
            if (_random.Next(100) < 40)
            {
                string[] neoStrings = {
                    "neo", "gas", "transfer", "mint", "burn", "deploy", "update",
                    "balanceOf", "totalSupply", "decimals", "symbol", "name",
                    "verify", "owner", "admin", "oracle", "vote", "candidate",
                    "0x", "NeoToken", "GasToken", "contract", "storage", "runtime"
                };

                return new ByteString(Encoding.UTF8.GetBytes(neoStrings[_random.Next(neoStrings.Length)]));
            }

            return GenerateByteString("string");
        }

        /// <summary>
        /// Adds a known good value for a specific type.
        /// </summary>
        /// <param name="type">The type of parameter.</param>
        /// <param name="value">The known good value.</param>
        public void AddKnownGoodValue(string type, StackItem value)
        {
            if (!_knownGoodValues.ContainsKey(type))
            {
                _knownGoodValues[type] = new List<StackItem>();
            }

            _knownGoodValues[type].Add(value);
        }



        /// <summary>
        /// Sets the mutation rate for parameter generation.
        /// </summary>
        /// <param name="rate">The mutation rate (0-100). Higher values mean more mutation/randomness.</param>
        public void SetMutationRate(int rate)
        {
            _mutationRate = Math.Clamp(rate, 0, 100);
        }

        /// <summary>
        /// Generates a parameter with context information about the method and parameter name.
        /// </summary>
        /// <param name="type">The type of parameter to generate.</param>
        /// <param name="methodName">The name of the method being called.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="depth">Current recursion depth for nested types.</param>
        /// <returns>A stack item of the specified type.</returns>
        public StackItem GenerateParameterWithContext(string type, string methodName, string parameterName, int depth)
        {
            // Use known good values with higher probability when we have context
            if (_knownGoodValues.ContainsKey(type) && _random.Next(100) < (100 - _mutationRate))
            {
                var values = _knownGoodValues[type];
                return values[_random.Next(values.Count)];
            }

            // Generate parameter based on name hints
            if (!string.IsNullOrEmpty(parameterName))
            {
                string paramLower = parameterName.ToLowerInvariant();

                // Integer parameters
                if ((paramLower.Contains("amount") || paramLower.Contains("value") ||
                     paramLower.Contains("balance") || paramLower.Contains("supply") ||
                     paramLower.Contains("quantity") || paramLower.Contains("count") ||
                     paramLower.Contains("id") || paramLower.Contains("index")) &&
                    (type.ToLowerInvariant() == "integer" || type.ToLowerInvariant() == "int"))
                {
                    // Generate a reasonable value based on parameter name
                    if (paramLower.Contains("amount") || paramLower.Contains("value") || paramLower.Contains("balance"))
                    {
                        // Financial values (0-1000)
                        return new Integer(_random.Next(1001));
                    }
                    else if (paramLower.Contains("supply") || paramLower.Contains("quantity"))
                    {
                        // Supply values (100-10000)
                        return new Integer(_random.Next(100, 10001));
                    }
                    else if (paramLower.Contains("index") || paramLower.Contains("id"))
                    {
                        // Index values (0-10)
                        return new Integer(_random.Next(11));
                    }
                    else if (paramLower.Contains("count"))
                    {
                        // Count values (1-20)
                        return new Integer(_random.Next(1, 21));
                    }
                }

                // Hash parameters
                else if ((paramLower.Contains("hash") || paramLower.Contains("address") ||
                          paramLower.Contains("account") || paramLower.Contains("owner") ||
                          paramLower.Contains("from") || paramLower.Contains("to")) &&
                         (type.ToLowerInvariant() == "hash160" || type.ToLowerInvariant() == "bytearray"))
                {
                    // Generate a valid-looking address
                    byte[] hash = new byte[20];
                    _random.NextBytes(hash);
                    return new ByteString(hash);
                }

                // Boolean parameters
                else if ((paramLower.Contains("is") || paramLower.Contains("has") ||
                          paramLower.Contains("can") || paramLower.Contains("allow") ||
                          paramLower.Contains("enable")) &&
                         type.ToLowerInvariant() == "boolean")
                {
                    // Prefer true for positive-sounding parameters
                    if (paramLower.Contains("is") || paramLower.Contains("has") ||
                        paramLower.Contains("can") || paramLower.Contains("allow") ||
                        paramLower.Contains("enable"))
                    {
                        return _random.Next(4) > 0 ? StackItem.True : StackItem.False; // 75% true
                    }
                    else
                    {
                        return _random.Next(4) > 0 ? StackItem.False : StackItem.True; // 75% false
                    }
                }

                // String parameters
                else if ((paramLower.Contains("name") || paramLower.Contains("symbol") ||
                          paramLower.Contains("message") || paramLower.Contains("text") ||
                          paramLower.Contains("data")) &&
                         (type.ToLowerInvariant() == "string" || type.ToLowerInvariant() == "bytearray"))
                {
                    // Generate a reasonable string based on parameter name
                    if (paramLower.Contains("name"))
                    {
                        string[] names = { "Token", "Coin", "Asset", "Share", "NEP5", "NEP17", "Neo", "Gas" };
                        return new ByteString(Encoding.UTF8.GetBytes(names[_random.Next(names.Length)]));
                    }
                    else if (paramLower.Contains("symbol"))
                    {
                        string[] symbols = { "TKN", "COIN", "AST", "SHR", "NEO", "GAS", "BTC", "ETH" };
                        return new ByteString(Encoding.UTF8.GetBytes(symbols[_random.Next(symbols.Length)]));
                    }
                    else if (paramLower.Contains("message") || paramLower.Contains("text"))
                    {
                        string[] messages = { "Hello, world!", "Test message", "This is a test", "Neo Smart Contract" };
                        return new ByteString(Encoding.UTF8.GetBytes(messages[_random.Next(messages.Length)]));
                    }
                    else if (paramLower.Contains("data"))
                    {
                        // Random data
                        byte[] data = new byte[_random.Next(10, 50)];
                        _random.NextBytes(data);
                        return new ByteString(data);
                    }
                }
            }

            // Method-specific parameter generation
            if (!string.IsNullOrEmpty(methodName))
            {
                string methodLower = methodName.ToLowerInvariant();

                // Transfer method
                if (methodLower == "transfer" && !string.IsNullOrEmpty(parameterName))
                {
                    string paramLower = parameterName.ToLowerInvariant();

                    if (paramLower == "from" || paramLower == "to")
                    {
                        // Generate a valid-looking address
                        byte[] hash = new byte[20];
                        _random.NextBytes(hash);
                        return new ByteString(hash);
                    }
                    else if (paramLower == "amount" || paramLower == "value")
                    {
                        // Generate a reasonable transfer amount (1-1000)
                        return new Integer(_random.Next(1, 1001));
                    }
                }

                // Mint/burn methods
                else if ((methodLower == "mint" || methodLower == "burn") && !string.IsNullOrEmpty(parameterName))
                {
                    string paramLower = parameterName.ToLowerInvariant();

                    if (paramLower == "to" || paramLower == "from" || paramLower == "account")
                    {
                        // Generate a valid-looking address
                        byte[] hash = new byte[20];
                        _random.NextBytes(hash);
                        return new ByteString(hash);
                    }
                    else if (paramLower == "amount" || paramLower == "value")
                    {
                        // Generate a reasonable mint/burn amount (1-500)
                        return new Integer(_random.Next(1, 501));
                    }
                }

                // Deploy method
                else if (methodLower == "deploy" || methodLower == "update")
                {
                    // No specific parameters to handle
                }
            }

            // Fall back to standard parameter generation
            return GenerateParameter(type, depth);
        }

        /// <summary>
        /// Generates a special value that might trigger edge cases.
        /// </summary>
        /// <param name="type">The type of parameter.</param>
        /// <returns>A stack item that might trigger edge cases.</returns>
        public StackItem GenerateEdgeCaseValue(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "boolean":
                    return _random.Next(2) == 0 ? StackItem.True : StackItem.False;
                case "integer":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                    return GenerateEdgeCaseInteger();
                case "string":
                case "bytearray":
                    return GenerateEdgeCaseByteString();
                case "hash160":
                    return GenerateEdgeCaseHash160();
                case "hash256":
                    return GenerateEdgeCaseHash256();
                case "publickey":
                    return GenerateEdgeCasePublicKey();
                case "array":
                    return GenerateEdgeCaseArray();
                case "map":
                    return GenerateEdgeCaseMap();
                default:
                    if (type.EndsWith("[]"))
                    {
                        return GenerateEdgeCaseArray();
                    }
                    return GenerateEdgeCaseByteString(); // Default to ByteString for unknown types
            }
        }

        private StackItem GenerateBoolean()
        {
            return _random.Next(2) == 0 ? StackItem.True : StackItem.False;
        }

        private StackItem GenerateInteger()
        {
            // Choose a strategy for generating integers
            switch (_random.Next(10))
            {
                case 0: // Small positive integer
                    return new Integer(_random.Next(100));
                case 1: // Small negative integer
                    return new Integer(-_random.Next(100));
                case 2: // Medium positive integer
                    return new Integer(_random.Next(1000, 10000));
                case 3: // Medium negative integer
                    return new Integer(-_random.Next(1000, 10000));
                case 4: // Large positive integer
                    return new Integer(_random.Next(10000, 1000000));
                case 5: // Large negative integer
                    return new Integer(-_random.Next(10000, 1000000));
                case 6: // Very large positive integer
                    return new Integer(BigInteger.Pow(10, _random.Next(5, 10)));
                case 7: // Very large negative integer
                    return new Integer(-BigInteger.Pow(10, _random.Next(5, 10)));
                case 8: // Boundary value
                    return new Integer(GetBoundaryInteger());
                default: // Completely random integer
                    byte[] bytes = new byte[_random.Next(1, 8)];
                    _random.NextBytes(bytes);
                    return new Integer(new BigInteger(bytes));
            }
        }

        private StackItem GenerateByteString(string type)
        {
            byte[] bytes;

            switch (type.ToLowerInvariant())
            {
                case "string":
                    // Generate a random string
                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    int length = _random.Next(1, 100);
                    string randomString = new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[_random.Next(s.Length)]).ToArray());
                    bytes = Encoding.UTF8.GetBytes(randomString);
                    break;
                case "hash160":
                    // Generate a random Hash160 (20 bytes)
                    bytes = new byte[20];
                    _random.NextBytes(bytes);
                    break;
                case "hash256":
                    // Generate a random Hash256 (32 bytes)
                    bytes = new byte[32];
                    _random.NextBytes(bytes);
                    break;
                case "publickey":
                    // Generate a random public key (33 bytes for compressed, 65 for uncompressed)
                    bytes = new byte[_random.Next(2) == 0 ? 33 : 65];
                    _random.NextBytes(bytes);
                    // Set the first byte to 02, 03, or 04 to make it look like a valid public key
                    bytes[0] = (byte)(_random.Next(3) + 2);
                    break;
                case "signature":
                    // Generate a random signature (64-72 bytes)
                    bytes = new byte[_random.Next(64, 73)];
                    _random.NextBytes(bytes);
                    break;
                default:
                    // Generate a random byte array
                    bytes = new byte[_random.Next(1, 100)];
                    _random.NextBytes(bytes);
                    break;
            }

            return new ByteString(bytes);
        }

        private StackItem GenerateArray(int depth)
        {
            if (depth > 3) // Limit recursion depth
                return new Array();

            var array = new Array();
            int count = _random.Next(0, 10);

            for (int i = 0; i < count; i++)
            {
                // Choose a random type for each element
                string[] types = { "boolean", "integer", "string", "bytearray" };
                string type = types[_random.Next(types.Length)];

                // With some probability, add nested arrays or maps
                if (depth < 2 && _random.Next(10) < 3)
                {
                    if (_random.Next(2) == 0)
                        array.Add(GenerateArray(depth + 1));
                    else
                        array.Add(GenerateMap(depth + 1));
                }
                else
                {
                    array.Add(GenerateParameter(type, depth + 1));
                }
            }

            return array;
        }

        private StackItem GenerateTypedArray(string elementType, int depth)
        {
            if (depth > 3) // Limit recursion depth
                return new Array();

            var array = new Array();
            int count = _random.Next(0, 10);

            for (int i = 0; i < count; i++)
            {
                array.Add(GenerateParameter(elementType, depth + 1));
            }

            return array;
        }

        private StackItem GenerateMap(int depth)
        {
            if (depth > 3) // Limit recursion depth
                return new Map();

            var map = new Map();
            int count = _random.Next(0, 5);

            for (int i = 0; i < count; i++)
            {
                // Generate a key (must be a primitive type)
                var key = GenerateByteString("bytearray") as PrimitiveType;

                // Generate a value (can be any type)
                string[] types = { "boolean", "integer", "string", "bytearray" };
                string type = types[_random.Next(types.Length)];

                // With some probability, add nested arrays or maps as values
                StackItem value;
                if (depth < 2 && _random.Next(10) < 3)
                {
                    if (_random.Next(2) == 0)
                        value = GenerateArray(depth + 1);
                    else
                        value = GenerateMap(depth + 1);
                }
                else
                {
                    value = GenerateParameter(type, depth + 1);
                }

                // Add to map if the key doesn't already exist
                if (!map.ContainsKey(key))
                {
                    map[key] = value;
                }
            }

            return map;
        }

        private StackItem GenerateEdgeCaseInteger()
        {
            // Generate edge case integers that might trigger vulnerabilities
            switch (_random.Next(15))
            {
                case 0: return new Integer(0);
                case 1: return new Integer(1);
                case 2: return new Integer(-1);
                case 3: return new Integer(int.MaxValue);
                case 4: return new Integer(int.MinValue);
                case 5: return new Integer(long.MaxValue);
                case 6: return new Integer(long.MinValue);
                case 7: return new Integer(BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935")); // 2^256 - 1
                case 8: return new Integer(BigInteger.Parse("-115792089237316195423570985008687907853269984665640564039457584007913129639936")); // -2^256
                case 9: return new Integer(21_000_000); // Total NEO supply
                case 10: return new Integer(100_000_000); // GAS factor
                case 11: return new Integer(10_000); // Common fee amount
                case 12: return new Integer(2_147_483_648); // 2^31 (just over int.MaxValue)
                case 13: return new Integer(-2_147_483_649); // -2^31-1 (just under int.MinValue)
                default: return new Integer(BigInteger.Pow(2, _random.Next(8, 270))); // Large power of 2
            }
        }

        private StackItem GenerateEdgeCaseByteString()
        {
            // Generate edge case byte strings that might trigger vulnerabilities
            switch (_random.Next(12))
            {
                case 0: return new ByteString(new byte[0]); // Empty byte string
                case 1: return new ByteString(new byte[1024]); // Large byte string with all zeros
                case 2: return new ByteString(new byte[1024].Select(_ => (byte)0xFF).ToArray()); // Large byte string with all ones
                case 3: // String with special characters
                    return new ByteString(Encoding.UTF8.GetBytes("!@#$%^&*()_+{}|:<>?~`-=[]\\;',./"));
                case 4: // SQL injection attempt
                    return new ByteString(Encoding.UTF8.GetBytes("' OR 1=1 --"));
                case 5: // JavaScript injection attempt
                    return new ByteString(Encoding.UTF8.GetBytes("<script>alert('XSS')</script>"));
                case 6: // Neo-specific: Contract method names
                    string[] neoMethods = { "transfer", "balanceOf", "totalSupply", "decimals", "symbol", "name", "deploy", "update", "destroy" };
                    return new ByteString(Encoding.UTF8.GetBytes(neoMethods[_random.Next(neoMethods.Length)]));
                case 7: // Neo-specific: Contract event names
                    string[] neoEvents = { "Transfer", "Approval", "Mint", "Burn", "Deploy", "Update", "Destroy" };
                    return new ByteString(Encoding.UTF8.GetBytes(neoEvents[_random.Next(neoEvents.Length)]));
                case 8: // Neo-specific: Storage prefixes
                    byte[] prefix = new byte[1];
                    _random.NextBytes(prefix);
                    return new ByteString(prefix);
                case 9: // Neo-specific: Contract hashes
                    byte[] contractHash = new byte[20];
                    _random.NextBytes(contractHash);
                    return new ByteString(contractHash);
                case 10: // Unicode characters
                    return new ByteString(Encoding.UTF8.GetBytes("你好世界")); // "Hello World" in Chinese
                default: // Very large string
                    return new ByteString(new byte[10000]);
            }
        }

        private StackItem GenerateEdgeCaseHash160()
        {
            // Generate edge case Hash160 values
            switch (_random.Next(4))
            {
                case 0: return new ByteString(new byte[20]); // All zeros
                case 1: return new ByteString(new byte[20].Select(_ => (byte)0xFF).ToArray()); // All ones
                case 2: // Neo contract hash
                    return new ByteString(new byte[] {
                        0xef, 0x4a, 0x67, 0x65, 0x4c, 0x9d, 0x42, 0x26,
                        0x2d, 0x20, 0x52, 0x88, 0xdf, 0x21, 0x50, 0x92,
                        0xa0, 0xf4, 0xd4, 0x5c
                    });
                default: // Random hash
                    byte[] hash = new byte[20];
                    _random.NextBytes(hash);
                    return new ByteString(hash);
            }
        }

        private StackItem GenerateEdgeCaseHash256()
        {
            // Generate edge case Hash256 values
            switch (_random.Next(4))
            {
                case 0: return new ByteString(new byte[32]); // All zeros
                case 1: return new ByteString(new byte[32].Select(_ => (byte)0xFF).ToArray()); // All ones
                case 2: // Bitcoin genesis block hash
                    return new ByteString(new byte[] {
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0xd6, 0x68,
                        0x9c, 0x08, 0x5a, 0xe1, 0x65, 0x83, 0x1e, 0x93,
                        0x4f, 0xf7, 0x63, 0xae, 0x46, 0xa2, 0xa6, 0xc1,
                        0x72, 0xb3, 0xf1, 0xb6, 0x0a, 0x8c, 0xe2, 0x6f
                    });
                default: // Random hash
                    byte[] hash = new byte[32];
                    _random.NextBytes(hash);
                    return new ByteString(hash);
            }
        }

        private StackItem GenerateEdgeCasePublicKey()
        {
            // Generate edge case public key values
            switch (_random.Next(4))
            {
                case 0: // Invalid public key (all zeros)
                    return new ByteString(new byte[33]);
                case 1: // Compressed public key format with all other bytes zero
                    byte[] compressedKey = new byte[33];
                    compressedKey[0] = 0x02; // Compressed format
                    return new ByteString(compressedKey);
                case 2: // Uncompressed public key format with all other bytes zero
                    byte[] uncompressedKey = new byte[65];
                    uncompressedKey[0] = 0x04; // Uncompressed format
                    return new ByteString(uncompressedKey);
                default: // Random public key
                    byte[] randomKey = new byte[_random.Next(2) == 0 ? 33 : 65];
                    _random.NextBytes(randomKey);
                    randomKey[0] = (byte)(_random.Next(3) + 2); // 02, 03, or 04
                    return new ByteString(randomKey);
            }
        }

        private StackItem GenerateEdgeCaseArray()
        {
            // Generate edge case arrays that might trigger vulnerabilities
            switch (_random.Next(5))
            {
                case 0: return new Array(); // Empty array
                case 1: // Array with a single null element
                    var singleNull = new Array();
                    singleNull.Add(StackItem.Null);
                    return singleNull;
                case 2: // Very large array
                    var largeArray = new Array();
                    for (int i = 0; i < 100; i++)
                    {
                        largeArray.Add(new Integer(i));
                    }
                    return largeArray;
                case 3: // Nested arrays
                    var nestedArray = new Array();
                    var innerArray1 = new Array();
                    var innerArray2 = new Array();
                    innerArray1.Add(new Integer(1));
                    innerArray2.Add(new Integer(2));
                    innerArray1.Add(innerArray2);
                    nestedArray.Add(innerArray1);
                    return nestedArray;
                default: // Array with mixed types
                    var mixedArray = new Array();
                    mixedArray.Add(StackItem.True);
                    mixedArray.Add(new Integer(42));
                    mixedArray.Add(new ByteString(Encoding.UTF8.GetBytes("test")));
                    mixedArray.Add(new Map());
                    return mixedArray;
            }
        }

        private StackItem GenerateEdgeCaseMap()
        {
            // Generate edge case maps that might trigger vulnerabilities
            switch (_random.Next(5))
            {
                case 0: return new Map(); // Empty map
                case 1: // Map with a single entry with null value
                    var singleNull = new Map();
                    singleNull[(PrimitiveType)new ByteString(new byte[] { 0x01 })] = StackItem.Null;
                    return singleNull;
                case 2: // Very large map
                    var largeMap = new Map();
                    for (int i = 0; i < 50; i++)
                    {
                        largeMap[(PrimitiveType)new ByteString(new byte[] { (byte)i })] = new Integer(i);
                    }
                    return largeMap;
                case 3: // Map with nested maps
                    var nestedMap = new Map();
                    var innerMap = new Map();
                    innerMap[(PrimitiveType)new ByteString(new byte[] { 0x01 })] = new Integer(1);
                    nestedMap[(PrimitiveType)new ByteString(new byte[] { 0x02 })] = innerMap;
                    return nestedMap;
                default: // Map with mixed types
                    var mixedMap = new Map();
                    mixedMap[(PrimitiveType)new ByteString(new byte[] { 0x01 })] = StackItem.True;
                    mixedMap[(PrimitiveType)new ByteString(new byte[] { 0x02 })] = new Integer(42);
                    mixedMap[(PrimitiveType)new ByteString(new byte[] { 0x03 })] = new ByteString(Encoding.UTF8.GetBytes("test"));
                    mixedMap[(PrimitiveType)new ByteString(new byte[] { 0x04 })] = new Array();
                    return mixedMap;
            }
        }

        private BigInteger GetBoundaryInteger()
        {
            // Generate boundary values for integers
            switch (_random.Next(12))
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return -1;
                case 3: return int.MaxValue;
                case 4: return int.MinValue;
                case 5: return long.MaxValue;
                case 6: return long.MinValue;
                case 7: return int.MaxValue + 1L;
                case 8: return int.MinValue - 1L;
                case 9: return BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935"); // 2^256 - 1
                case 10: return BigInteger.Parse("-115792089237316195423570985008687907853269984665640564039457584007913129639936"); // -2^256
                default: return _random.Next(1000) - 500; // Small values around zero
            }
        }

        private void InitializeKnownGoodValues()
        {
            // Initialize known good values for various types
            _knownGoodValues["boolean"] = new List<StackItem>
            {
                StackItem.True,
                StackItem.False
            };

            _knownGoodValues["integer"] = new List<StackItem>
            {
                new Integer(0),
                new Integer(1),
                new Integer(-1),
                new Integer(42),
                new Integer(100),
                new Integer(-100)
            };

            _knownGoodValues["string"] = new List<StackItem>
            {
                new ByteString(Encoding.UTF8.GetBytes("")),
                new ByteString(Encoding.UTF8.GetBytes("test")),
                new ByteString(Encoding.UTF8.GetBytes("hello world")),
                new ByteString(Encoding.UTF8.GetBytes("12345"))
            };

            _knownGoodValues["bytearray"] = new List<StackItem>
            {
                new ByteString(new byte[0]),
                new ByteString(new byte[] { 0x01, 0x02, 0x03 }),
                new ByteString(new byte[] { 0xFF, 0xFF, 0xFF })
            };

            _knownGoodValues["hash160"] = new List<StackItem>
            {
                new ByteString(new byte[20]),
                new ByteString(new byte[20].Select(_ => (byte)0xFF).ToArray())
            };

            _knownGoodValues["hash256"] = new List<StackItem>
            {
                new ByteString(new byte[32]),
                new ByteString(new byte[32].Select(_ => (byte)0xFF).ToArray())
            };

            // Add more known good values for other types as needed
        }
    }
}
