using Neo.VM.Types;
using System;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Fuzzer.InputGeneration
{
    /// <summary>
    /// Generates parameters for smart contract method calls.
    /// </summary>
    public class ParameterGenerator
    {
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterGenerator"/> class.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        public ParameterGenerator(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Generates a parameter of the specified type.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="depth">The current recursion depth for nested types.</param>
        /// <returns>A stack item representing the parameter.</returns>
        public StackItem GenerateParameter(string type, int depth)
        {
            // Prevent excessive recursion
            if (depth > 3)
            {
                return GenerateSimpleParameter();
            }

            // Occasionally generate edge cases to improve coverage
            if (_random.Next(10) == 0)
            {
                return GenerateEdgeCaseValue(type);
            }

            switch (type)
            {
                case "Boolean":
                    return _random.Next(2) == 1 ? VM.Types.StackItem.True : VM.Types.StackItem.False;
                case "Integer":
                    return GenerateInteger();
                case "ByteArray":
                case "Buffer":
                    return GenerateByteString();
                case "String":
                    return GenerateString();
                case "Hash160":
                    return GenerateHash160();
                case "Hash256":
                    return GenerateHash256();
                case "PublicKey":
                    return GeneratePublicKey();
                case "Signature":
                    return GenerateSignature();
                case "Array":
                    return GenerateArray("Any", depth + 1);
                case "Map":
                    return GenerateMap(depth + 1);
                case "InteropInterface":
                    // Cannot directly generate interop interfaces
                    return GenerateNull();
                case "Void":
                    // Void parameters don't make sense, return null
                    return GenerateNull();
                default:
                    // Check if it's an array type
                    if (type.EndsWith("[]"))
                    {
                        string elementType = type.Substring(0, type.Length - 2);
                        return GenerateArray(elementType, depth + 1);
                    }
                    // Default to Any
                    return GenerateAny(depth + 1);
            }
        }

        /// <summary>
        /// Generates a random integer.
        /// </summary>
        /// <returns>A stack item representing an integer.</returns>
        public VM.Types.Integer GenerateInteger()
        {
            // Choose a strategy for generating integers
            switch (_random.Next(6))
            {
                case 0: // Small positive integer
                    return new VM.Types.Integer(_random.Next(100));
                case 1: // Small negative integer
                    return new VM.Types.Integer(-_random.Next(1, 100));
                case 2: // Zero
                    return new VM.Types.Integer(0);
                case 3: // Medium integer
                    return new VM.Types.Integer(_random.Next(100, 10000));
                case 4: // Large integer
                    return new VM.Types.Integer(_random.Next(10000, 1000000));
                case 5: // Boundary values
                    return GenerateBoundaryInteger();
                default:
                    return new VM.Types.Integer(_random.Next(100));
            }
        }

        /// <summary>
        /// Generates a byte string.
        /// </summary>
        /// <returns>A stack item representing a byte string.</returns>
        public VM.Types.ByteString GenerateByteString()
        {
            // Choose a strategy for generating byte strings
            switch (_random.Next(5))
            {
                case 0: // Empty byte string
                    return new VM.Types.ByteString(System.Array.Empty<byte>());
                case 1: // Small byte string
                    return GenerateRandomByteString(1, 10);
                case 2: // Medium byte string
                    return GenerateRandomByteString(10, 50);
                case 3: // Large byte string
                    return GenerateRandomByteString(50, 100);
                case 4: // Special byte string
                    return GenerateSpecialByteString();
                default:
                    return GenerateRandomByteString(1, 10);
            }
        }

        /// <summary>
        /// Generates a string.
        /// </summary>
        /// <returns>A stack item representing a string.</returns>
        public VM.Types.ByteString GenerateString()
        {
            // Choose a strategy for generating strings
            switch (_random.Next(5))
            {
                case 0: // Empty string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(""));
                case 1: // Short string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(1, 10)));
                case 2: // Medium string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(10, 50)));
                case 3: // Long string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(50, 100)));
                case 4: // Special string
                    return GenerateSpecialString();
                default:
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(1, 10)));
            }
        }

        /// <summary>
        /// Generates a Hash160 (20 bytes).
        /// </summary>
        /// <returns>A stack item representing a Hash160.</returns>
        public VM.Types.ByteString GenerateHash160()
        {
            byte[] hash = new byte[20];
            _random.NextBytes(hash);
            return new VM.Types.ByteString(hash);
        }

        /// <summary>
        /// Generates a Hash256 (32 bytes).
        /// </summary>
        /// <returns>A stack item representing a Hash256.</returns>
        public VM.Types.ByteString GenerateHash256()
        {
            byte[] hash = new byte[32];
            _random.NextBytes(hash);
            return new VM.Types.ByteString(hash);
        }

        /// <summary>
        /// Generates a public key (33 bytes).
        /// </summary>
        /// <returns>A stack item representing a public key.</returns>
        public VM.Types.ByteString GeneratePublicKey()
        {
            byte[] publicKey = new byte[33];
            _random.NextBytes(publicKey);
            // Set the first byte to either 0x02 or 0x03 to represent a compressed public key
            publicKey[0] = (byte)(_random.Next(2) == 0 ? 0x02 : 0x03);
            return new VM.Types.ByteString(publicKey);
        }

        /// <summary>
        /// Generates a signature (64 bytes).
        /// </summary>
        /// <returns>A stack item representing a signature.</returns>
        public VM.Types.ByteString GenerateSignature()
        {
            byte[] signature = new byte[64];
            _random.NextBytes(signature);
            return new VM.Types.ByteString(signature);
        }

        /// <summary>
        /// Generates an array.
        /// </summary>
        /// <param name="elementType">The type of elements in the array.</param>
        /// <param name="depth">The current recursion depth.</param>
        /// <returns>A stack item representing an array.</returns>
        public VM.Types.Array GenerateArray(string elementType, int depth)
        {
            var array = new VM.Types.Array();

            // Choose array size
            int size = _random.Next(6); // 0 to 5 elements

            for (int i = 0; i < size; i++)
            {
                array.Add(GenerateParameter(elementType, depth));
            }

            return array;
        }

        /// <summary>
        /// Generates a map.
        /// </summary>
        /// <param name="depth">The current recursion depth.</param>
        /// <returns>A stack item representing a map.</returns>
        public VM.Types.Map GenerateMap(int depth)
        {
            var map = new VM.Types.Map();

            // Choose map size
            int size = _random.Next(4); // 0 to 3 key-value pairs

            for (int i = 0; i < size; i++)
            {
                // Generate key (usually a ByteString or Integer)
                StackItem key = _random.Next(2) == 0
                    ? GenerateByteString()
                    : GenerateInteger();

                // Generate value (any type)
                StackItem value = GenerateParameter("Any", depth);

                // Add to map if key doesn't already exist
                // Convert key to PrimitiveType if needed
                PrimitiveType primitiveKey = key as PrimitiveType ?? new ByteString(Encoding.UTF8.GetBytes(key.ToString()));
                if (!map.ContainsKey(primitiveKey))
                {
                    map[primitiveKey] = value;
                }
            }

            return map;
        }

        /// <summary>
        /// Generates a null value.
        /// </summary>
        /// <returns>A stack item representing null.</returns>
        public StackItem GenerateNull()
        {
            return VM.Types.StackItem.Null;
        }

        /// <summary>
        /// Generates a parameter of any type.
        /// </summary>
        /// <param name="depth">The current recursion depth.</param>
        /// <returns>A stack item of a random type.</returns>
        public StackItem GenerateAny(int depth)
        {
            // Choose a random type
            switch (_random.Next(7))
            {
                case 0:
                    return _random.Next(2) == 1 ? VM.Types.StackItem.True : VM.Types.StackItem.False;
                case 1:
                    return GenerateInteger();
                case 2:
                    return GenerateByteString();
                case 3:
                    return GenerateString();
                case 4:
                    return GenerateArray("Any", depth);
                case 5:
                    return GenerateMap(depth);
                default:
                    return GenerateSimpleParameter();
            }
        }

        /// <summary>
        /// Generates a simple parameter (Boolean, Integer, or ByteString).
        /// </summary>
        /// <returns>A simple stack item.</returns>
        private StackItem GenerateSimpleParameter()
        {
            switch (_random.Next(3))
            {
                case 0:
                    return _random.Next(2) == 1 ? VM.Types.StackItem.True : VM.Types.StackItem.False;
                case 1:
                    return new VM.Types.Integer(_random.Next(100));
                default:
                    return GenerateRandomByteString(1, 10);
            }
        }

        /// <summary>
        /// Generates a boundary integer value.
        /// </summary>
        /// <returns>A stack item representing a boundary integer.</returns>
        private VM.Types.Integer GenerateBoundaryInteger()
        {
            switch (_random.Next(12))
            {
                case 0:
                    return new VM.Types.Integer(0);
                case 1:
                    return new VM.Types.Integer(1);
                case 2:
                    return new VM.Types.Integer(-1);
                case 3:
                    return new VM.Types.Integer(int.MaxValue);
                case 4:
                    return new VM.Types.Integer(int.MinValue);
                case 5:
                    return new VM.Types.Integer(long.MaxValue);
                case 6:
                    return new VM.Types.Integer(long.MinValue);
                case 7:
                    return new VM.Types.Integer(byte.MaxValue); // 255
                case 8:
                    return new VM.Types.Integer(ushort.MaxValue); // 65535
                case 9:
                    return new VM.Types.Integer(uint.MaxValue); // 4294967295
                case 10:
                    return new VM.Types.Integer(ulong.MaxValue); // 18446744073709551615
                default:
                    // Use a smaller value than the max 256-bit integer to avoid overflow
                    return new VM.Types.Integer(BigInteger.Pow(2, 32) - 1); // Max value for 32-bit integer
            }
        }

        /// <summary>
        /// Generates a random byte string of the specified length range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>A byte string stack item.</returns>
        private VM.Types.ByteString GenerateRandomByteString(int minLength, int maxLength)
        {
            int length = _random.Next(minLength, maxLength + 1);
            byte[] bytes = new byte[length];
            _random.NextBytes(bytes);
            return new VM.Types.ByteString(bytes);
        }

        /// <summary>
        /// Generates a special byte string.
        /// </summary>
        /// <returns>A byte string stack item with special values.</returns>
        private VM.Types.ByteString GenerateSpecialByteString()
        {
            switch (_random.Next(5))
            {
                case 0: // All zeros
                    return new VM.Types.ByteString(new byte[_random.Next(1, 20)]);
                case 1: // All ones
                    byte[] allOnes = new byte[_random.Next(1, 20)];
                    for (int i = 0; i < allOnes.Length; i++)
                    {
                        allOnes[i] = 0xFF;
                    }
                    return new VM.Types.ByteString(allOnes);
                case 2: // Single byte
                    return new VM.Types.ByteString(new byte[] { (byte)_random.Next(256) });
                case 3: // Repeated pattern
                    byte[] pattern = new byte[_random.Next(5, 20)];
                    byte value = (byte)_random.Next(256);
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        pattern[i] = value;
                    }
                    return new VM.Types.ByteString(pattern);
                default: // Random bytes
                    return GenerateRandomByteString(1, 20);
            }
        }

        /// <summary>
        /// Generates a special string.
        /// </summary>
        /// <returns>A byte string stack item with special string values.</returns>
        private VM.Types.ByteString GenerateSpecialString()
        {
            switch (_random.Next(5))
            {
                case 0: // Empty string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(""));
                case 1: // Common strings
                    string[] commonStrings = { "true", "false", "null", "undefined", "0", "1", "-1", "NaN", "Infinity" };
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(commonStrings[_random.Next(commonStrings.Length)]));
                case 2: // Special characters
                    string[] specialChars = { "\0", "\n", "\r", "\t", "\\", "\"", "'", "<", ">", "&" };
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(specialChars[_random.Next(specialChars.Length)]));
                case 3: // Long string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(100, 200)));
                default: // Random string
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes(GenerateRandomString(1, 20)));
            }
        }

        /// <summary>
        /// Generates a random string of the specified length range.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>A random string.</returns>
        private string GenerateRandomString(int minLength, int maxLength)
        {
            int length = _random.Next(minLength, maxLength + 1);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// Generates an edge case value for the specified type
        /// </summary>
        /// <param name="type">Parameter type as a string</param>
        /// <returns>Generated edge case value</returns>
        public StackItem GenerateEdgeCaseValue(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "integer":
                    return GenerateEdgeCaseInteger();
                case "string":
                case "bytearray":
                case "buffer":
                    return GenerateEdgeCaseByteString();
                case "array":
                    return GenerateEdgeCaseArray();
                case "map":
                    return GenerateEdgeCaseMap();
                case "boolean":
                    // For booleans, just return true or false
                    return _random.Next(2) == 1 ? VM.Types.StackItem.True : VM.Types.StackItem.False;
                case "hash160":
                    return GenerateEdgeCaseHash160();
                case "hash256":
                    return GenerateEdgeCaseHash256();
                case "publickey":
                    return GenerateEdgeCasePublicKey();
                case "signature":
                    return GenerateEdgeCaseSignature();
                default:
                    // For other types, use the standard generator
                    return GenerateAny(0);
            }
        }

        /// <summary>
        /// Generates an edge case integer value
        /// </summary>
        /// <returns>An integer stack item with an edge case value</returns>
        private VM.Types.Integer GenerateEdgeCaseInteger()
        {
            switch (_random.Next(20))
            {
                case 0: // Minimum integer value
                    return new VM.Types.Integer(BigInteger.MinusOne);
                case 1: // Maximum integer value
                    return new VM.Types.Integer(BigInteger.Parse("9223372036854775807")); // long.MaxValue
                case 2: // Minimum long value
                    return new VM.Types.Integer(BigInteger.Parse("-9223372036854775808")); // long.MinValue
                case 3: // Zero
                    return new VM.Types.Integer(BigInteger.Zero);
                case 4: // One
                    return new VM.Types.Integer(BigInteger.One);
                case 5: // Negative one
                    return new VM.Types.Integer(BigInteger.MinusOne);
                case 6: // Large power of 2
                    return new VM.Types.Integer(BigInteger.Pow(2, 31) - 1); // int.MaxValue
                case 7: // Large negative power of 2
                    return new VM.Types.Integer(BigInteger.Pow(2, 31) * -1); // int.MinValue
                case 8: // Very large integer
                    return new VM.Types.Integer(BigInteger.Pow(2, 63) - 1); // long.MaxValue
                case 9: // Very large negative integer
                    return new VM.Types.Integer(BigInteger.Pow(2, 63) * -1); // long.MinValue
                case 10: // Boundary value
                    return new VM.Types.Integer(255); // byte.MaxValue
                case 11: // Boundary value
                    return new VM.Types.Integer(65535); // ushort.MaxValue
                case 12: // Boundary value
                    return new VM.Types.Integer(4294967295); // uint.MaxValue
                case 13: // Boundary value
                    return new VM.Types.Integer(BigInteger.Parse("18446744073709551615")); // ulong.MaxValue
                case 14: // Neo-specific boundary
                    return new VM.Types.Integer(BigInteger.Parse("10000000000000000")); // 10^16, used in NEO asset calculations
                case 15: // Neo-specific boundary
                    return new VM.Types.Integer(BigInteger.Parse("100000000")); // 10^8, used in GAS calculations
                case 16: // Neo-specific boundary
                    return new VM.Types.Integer(21000000); // Total NEO supply
                case 17: // Neo-specific boundary
                    return new VM.Types.Integer(100000000); // GAS factor
                case 18: // Extremely large value (but still within Neo VM limits)
                    return new VM.Types.Integer(BigInteger.Parse("9999999999999999999999999999999"));
                case 19: // Extremely large negative value
                    return new VM.Types.Integer(BigInteger.Parse("-9999999999999999999999999999999"));
                default:
                    return new VM.Types.Integer(_random.Next(100));
            }
        }

        /// <summary>
        /// Generates an edge case byte string
        /// </summary>
        /// <returns>A byte string stack item with an edge case value</returns>
        private VM.Types.ByteString GenerateEdgeCaseByteString()
        {
            switch (_random.Next(15))
            {
                case 0: // Empty byte string
                    return new VM.Types.ByteString(System.Array.Empty<byte>());
                case 1: // Single zero byte
                    return new VM.Types.ByteString(new byte[] { 0 });
                case 2: // Single non-zero byte
                    return new VM.Types.ByteString(new byte[] { 255 });
                case 3: // All zeros (small)
                    return new VM.Types.ByteString(new byte[10]);
                case 4: // All zeros (large)
                    return new VM.Types.ByteString(new byte[1024]);
                case 5: // All ones (small)
                    byte[] allOnes = new byte[10];
                    for (int i = 0; i < allOnes.Length; i++) allOnes[i] = 255;
                    return new VM.Types.ByteString(allOnes);
                case 6: // All ones (large)
                    byte[] allOnesLarge = new byte[1024];
                    for (int i = 0; i < allOnesLarge.Length; i++) allOnesLarge[i] = 255;
                    return new VM.Types.ByteString(allOnesLarge);
                case 7: // Alternating pattern
                    byte[] alternating = new byte[100];
                    for (int i = 0; i < alternating.Length; i++) alternating[i] = (byte)(i % 2 == 0 ? 0 : 255);
                    return new VM.Types.ByteString(alternating);
                case 8: // ASCII string
                    return new VM.Types.ByteString(Encoding.ASCII.GetBytes("Hello, Neo Smart Contract!"));
                case 9: // UTF-8 string with special characters
                    return new VM.Types.ByteString(Encoding.UTF8.GetBytes("你好，Neo智能合约！"));
                case 10: // Very large string
                    byte[] veryLarge = new byte[10000];
                    _random.NextBytes(veryLarge);
                    return new VM.Types.ByteString(veryLarge);
                case 11: // Neo-specific: Contract script
                    // Simplified example of a contract script
                    return new VM.Types.ByteString(new byte[] {
                        0x00, 0xC5, 0x66, 0x6F, 0x6F, 0x67, 0x00, 0xC1, 0x01, 0x0C, 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64
                    });
                case 12: // Neo-specific: Public key
                    byte[] pubKey = new byte[33];
                    _random.NextBytes(pubKey);
                    pubKey[0] = (byte)(_random.Next(2) == 0 ? 0x02 : 0x03); // Compressed public key format
                    return new VM.Types.ByteString(pubKey);
                case 13: // Neo-specific: Script hash
                    byte[] scriptHash = new byte[20];
                    _random.NextBytes(scriptHash);
                    return new VM.Types.ByteString(scriptHash);
                case 14: // Neo-specific: Transaction hash
                    byte[] txHash = new byte[32];
                    _random.NextBytes(txHash);
                    return new VM.Types.ByteString(txHash);
                default:
                    return GenerateRandomByteString(1, 10);
            }
        }

        /// <summary>
        /// Generates an edge case array
        /// </summary>
        /// <returns>An array stack item with edge case values</returns>
        private VM.Types.Array GenerateEdgeCaseArray()
        {
            switch (_random.Next(10))
            {
                case 0: // Empty array
                    return new VM.Types.Array();
                case 1: // Array with a single null element
                    var singleNull = new VM.Types.Array();
                    singleNull.Add(VM.Types.StackItem.Null);
                    return singleNull;
                case 2: // Array with a single integer element
                    var singleInt = new VM.Types.Array();
                    singleInt.Add(GenerateEdgeCaseInteger());
                    return singleInt;
                case 3: // Array with multiple identical elements
                    var identicalElements = new VM.Types.Array();
                    var element = GenerateEdgeCaseInteger();
                    for (int i = 0; i < 5; i++) identicalElements.Add(element);
                    return identicalElements;
                case 4: // Array with mixed types
                    var mixedTypes = new VM.Types.Array();
                    mixedTypes.Add(VM.Types.StackItem.True);
                    mixedTypes.Add(GenerateEdgeCaseInteger());
                    mixedTypes.Add(GenerateEdgeCaseByteString());
                    mixedTypes.Add(VM.Types.StackItem.Null);
                    return mixedTypes;
                case 5: // Nested array (shallow)
                    var nestedShallow = new VM.Types.Array();
                    var innerArray = new VM.Types.Array();
                    innerArray.Add(GenerateEdgeCaseInteger());
                    nestedShallow.Add(innerArray);
                    return nestedShallow;
                case 6: // Large array
                    var largeArray = new VM.Types.Array();
                    for (int i = 0; i < 100; i++) largeArray.Add(new VM.Types.Integer(i));
                    return largeArray;
                case 7: // Array with boundary integers
                    var boundaryInts = new VM.Types.Array();
                    boundaryInts.Add(new VM.Types.Integer(int.MinValue));
                    boundaryInts.Add(new VM.Types.Integer(int.MaxValue));
                    boundaryInts.Add(new VM.Types.Integer(0));
                    boundaryInts.Add(new VM.Types.Integer(1));
                    boundaryInts.Add(new VM.Types.Integer(-1));
                    return boundaryInts;
                case 8: // Array with special byte strings
                    var specialStrings = new VM.Types.Array();
                    specialStrings.Add(new VM.Types.ByteString(System.Array.Empty<byte>()));
                    specialStrings.Add(new VM.Types.ByteString(new byte[] { 0 }));
                    specialStrings.Add(new VM.Types.ByteString(new byte[] { 255 }));
                    specialStrings.Add(new VM.Types.ByteString(Encoding.UTF8.GetBytes("Hello")));
                    return specialStrings;
                case 9: // Array with booleans
                    var booleans = new VM.Types.Array();
                    booleans.Add(VM.Types.StackItem.True);
                    booleans.Add(VM.Types.StackItem.False);
                    return booleans;
                default:
                    return new VM.Types.Array();
            }
        }

        /// <summary>
        /// Generates an edge case map
        /// </summary>
        /// <returns>A map stack item with edge case values</returns>
        private VM.Types.Map GenerateEdgeCaseMap()
        {
            switch (_random.Next(10))
            {
                case 0: // Empty map
                    return new VM.Types.Map();
                case 1: // Map with a single entry
                    var singleEntry = new VM.Types.Map();
                    singleEntry[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("key"))] = new VM.Types.Integer(1);
                    return singleEntry;
                case 2: // Map with integer keys
                    var intKeys = new VM.Types.Map();
                    for (int i = 0; i < 5; i++)
                    {
                        intKeys[(PrimitiveType)new VM.Types.Integer(i)] = new VM.Types.ByteString(Encoding.UTF8.GetBytes($"value{i}"));
                    }
                    return intKeys;
                case 3: // Map with string keys
                    var stringKeys = new VM.Types.Map();
                    for (int i = 0; i < 5; i++)
                    {
                        stringKeys[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes($"key{i}"))] = new VM.Types.Integer(i);
                    }
                    return stringKeys;
                case 4: // Map with mixed value types
                    var mixedValues = new VM.Types.Map();
                    mixedValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("bool"))] = VM.Types.StackItem.True;
                    mixedValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("int"))] = new VM.Types.Integer(42);
                    mixedValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("string"))] = new VM.Types.ByteString(Encoding.UTF8.GetBytes("value"));
                    mixedValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("null"))] = VM.Types.StackItem.Null;
                    return mixedValues;
                case 5: // Map with nested map as value
                    var nestedMap = new VM.Types.Map();
                    var innerMap = new VM.Types.Map();
                    innerMap[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("inner"))] = new VM.Types.Integer(1);
                    nestedMap[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("outer"))] = innerMap;
                    return nestedMap;
                case 6: // Map with array as value
                    var arrayValue = new VM.Types.Map();
                    var array = new VM.Types.Array();
                    array.Add(new VM.Types.Integer(1));
                    array.Add(new VM.Types.Integer(2));
                    arrayValue[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("array"))] = array;
                    return arrayValue;
                case 7: // Large map
                    var largeMap = new VM.Types.Map();
                    for (int i = 0; i < 20; i++)
                    {
                        largeMap[(PrimitiveType)new VM.Types.Integer(i)] = new VM.Types.Integer(i * 10);
                    }
                    return largeMap;
                case 8: // Map with edge case keys
                    var edgeCaseKeys = new VM.Types.Map();
                    edgeCaseKeys[(PrimitiveType)new VM.Types.ByteString(System.Array.Empty<byte>())] = new VM.Types.Integer(1);
                    edgeCaseKeys[(PrimitiveType)new VM.Types.Integer(int.MaxValue)] = new VM.Types.Integer(2);
                    edgeCaseKeys[(PrimitiveType)new VM.Types.Integer(int.MinValue)] = new VM.Types.Integer(3);
                    return edgeCaseKeys;
                case 9: // Map with edge case values
                    var edgeCaseValues = new VM.Types.Map();
                    edgeCaseValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("max"))] = new VM.Types.Integer(int.MaxValue);
                    edgeCaseValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("min"))] = new VM.Types.Integer(int.MinValue);
                    edgeCaseValues[(PrimitiveType)new VM.Types.ByteString(Encoding.UTF8.GetBytes("empty"))] = new VM.Types.ByteString(System.Array.Empty<byte>());
                    return edgeCaseValues;
                default:
                    return new VM.Types.Map();
            }
        }

        /// <summary>
        /// Generates an edge case Hash160 value
        /// </summary>
        /// <returns>A byte string stack item with an edge case Hash160 value</returns>
        private VM.Types.ByteString GenerateEdgeCaseHash160()
        {
            switch (_random.Next(5))
            {
                case 0: // All zeros
                    return new VM.Types.ByteString(new byte[20]);
                case 1: // All ones
                    byte[] allOnes = new byte[20];
                    for (int i = 0; i < allOnes.Length; i++) allOnes[i] = 255;
                    return new VM.Types.ByteString(allOnes);
                case 2: // Neo native contract hash (GAS)
                    return new VM.Types.ByteString(new byte[] {
                        0x8c, 0x23, 0xf1, 0x96, 0xd1, 0x16, 0xf8, 0x84, 0x75, 0x57, 0x24, 0x15, 0x94, 0x90, 0x4c, 0xf4, 0x9d, 0x7a, 0xd0, 0x75
                    });
                case 3: // Neo native contract hash (NEO)
                    return new VM.Types.ByteString(new byte[] {
                        0xef, 0x4a, 0x87, 0x07, 0x77, 0x0b, 0x7c, 0x7e, 0xb6, 0x13, 0xd6, 0x19, 0x83, 0xf7, 0x3b, 0x5e, 0x29, 0x13, 0x4d, 0x3b
                    });
                case 4: // Pattern
                    byte[] pattern = new byte[20];
                    for (int i = 0; i < pattern.Length; i++) pattern[i] = (byte)i;
                    return new VM.Types.ByteString(pattern);
                default:
                    byte[] random = new byte[20];
                    _random.NextBytes(random);
                    return new VM.Types.ByteString(random);
            }
        }

        /// <summary>
        /// Generates an edge case Hash256 value
        /// </summary>
        /// <returns>A byte string stack item with an edge case Hash256 value</returns>
        private VM.Types.ByteString GenerateEdgeCaseHash256()
        {
            switch (_random.Next(5))
            {
                case 0: // All zeros
                    return new VM.Types.ByteString(new byte[32]);
                case 1: // All ones
                    byte[] allOnes = new byte[32];
                    for (int i = 0; i < allOnes.Length; i++) allOnes[i] = 255;
                    return new VM.Types.ByteString(allOnes);
                case 2: // Neo genesis block hash
                    return new VM.Types.ByteString(new byte[] {
                        0xd4, 0x2d, 0xf8, 0xe6, 0x43, 0x0a, 0x8b, 0xc6, 0x1c, 0x3f, 0xd6, 0x20, 0xf0, 0x96, 0xc0, 0x76,
                        0x96, 0x7e, 0x92, 0x77, 0x88, 0x8a, 0x33, 0x1c, 0x90, 0xc2, 0x48, 0x8a, 0x8a, 0x98, 0xd7, 0x68
                    });
                case 3: // Pattern
                    byte[] pattern = new byte[32];
                    for (int i = 0; i < pattern.Length; i++) pattern[i] = (byte)i;
                    return new VM.Types.ByteString(pattern);
                case 4: // Alternating
                    byte[] alternating = new byte[32];
                    for (int i = 0; i < alternating.Length; i++) alternating[i] = (byte)(i % 2 == 0 ? 0 : 255);
                    return new VM.Types.ByteString(alternating);
                default:
                    byte[] random = new byte[32];
                    _random.NextBytes(random);
                    return new VM.Types.ByteString(random);
            }
        }

        /// <summary>
        /// Generates an edge case public key
        /// </summary>
        /// <returns>A byte string stack item with an edge case public key value</returns>
        private VM.Types.ByteString GenerateEdgeCasePublicKey()
        {
            switch (_random.Next(4))
            {
                case 0: // Valid compressed public key (all zeros except first byte)
                    byte[] zeros = new byte[33];
                    zeros[0] = 0x02; // Compressed public key format
                    return new VM.Types.ByteString(zeros);
                case 1: // Valid compressed public key (all ones except first byte)
                    byte[] ones = new byte[33];
                    for (int i = 0; i < ones.Length; i++) ones[i] = 255;
                    ones[0] = 0x03; // Compressed public key format
                    return new VM.Types.ByteString(ones);
                case 2: // Pattern
                    byte[] pattern = new byte[33];
                    for (int i = 1; i < pattern.Length; i++) pattern[i] = (byte)i;
                    pattern[0] = 0x02; // Compressed public key format
                    return new VM.Types.ByteString(pattern);
                case 3: // Invalid format (wrong first byte)
                    byte[] invalid = new byte[33];
                    _random.NextBytes(invalid);
                    invalid[0] = 0x04; // Uncompressed format (not typically used in Neo)
                    return new VM.Types.ByteString(invalid);
                default:
                    byte[] random = new byte[33];
                    _random.NextBytes(random);
                    random[0] = (byte)(_random.Next(2) == 0 ? 0x02 : 0x03); // Compressed public key format
                    return new VM.Types.ByteString(random);
            }
        }

        /// <summary>
        /// Generates an edge case signature
        /// </summary>
        /// <returns>A byte string stack item with an edge case signature value</returns>
        private VM.Types.ByteString GenerateEdgeCaseSignature()
        {
            switch (_random.Next(4))
            {
                case 0: // All zeros
                    return new VM.Types.ByteString(new byte[64]);
                case 1: // All ones
                    byte[] allOnes = new byte[64];
                    for (int i = 0; i < allOnes.Length; i++) allOnes[i] = 255;
                    return new VM.Types.ByteString(allOnes);
                case 2: // Pattern
                    byte[] pattern = new byte[64];
                    for (int i = 0; i < pattern.Length; i++) pattern[i] = (byte)i;
                    return new VM.Types.ByteString(pattern);
                case 3: // Invalid size (too small)
                    byte[] tooSmall = new byte[63];
                    _random.NextBytes(tooSmall);
                    return new VM.Types.ByteString(tooSmall);
                default:
                    byte[] random = new byte[64];
                    _random.NextBytes(random);
                    return new VM.Types.ByteString(random);
            }
        }
    }
}


