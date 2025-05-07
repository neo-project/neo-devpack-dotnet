using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Generates random parameters for contract method calls
    /// </summary>
    public class ParameterGenerator
    {
        private readonly Random _random;
        private readonly NeoParameterGenerator _neoGenerator;
        private const int MaxRecursionDepth = 3; // Add max depth for recursive types
        private const int EdgeCaseProbability = 20; // Chance (out of 100) to generate an edge case

        // Predefined edge case values
        private static readonly BigInteger[] IntegerEdgeCases = {
            0, 1, -1, BigInteger.Parse("340282366920938463463374607431768211455"), // ulong.MaxValue approx
            BigInteger.Parse("-340282366920938463463374607431768211456"), // -ulong.MaxValue approx
            int.MaxValue, int.MinValue, long.MaxValue, long.MinValue, 1024, 65536
        };

        private static readonly Func<Random, byte[]>[] ByteArrayEdgeCases = {
            // Empty array
            rnd => System.Array.Empty<byte>(),
            // Array of zeros
            rnd => new byte[rnd.Next(1, 65)],
            // Array of 0xFF
            rnd => Enumerable.Repeat((byte)0xFF, rnd.Next(1, 65)).ToArray(),
            // Long array (adjust max size as needed for performance)
            rnd => { byte[] b = new byte[rnd.Next(256, 513)]; rnd.NextBytes(b); return b; },
             // Specific lengths (Hash160, Hash256, PublicKey, Signature)
            rnd => { byte[] b = new byte[20]; rnd.NextBytes(b); return b; },
            rnd => { byte[] b = new byte[32]; rnd.NextBytes(b); return b; },
            rnd => { byte[] b = new byte[33]; rnd.NextBytes(b); b[0] = (byte)(rnd.Next(2) == 0 ? 0x02 : 0x03); return b; }, // Basic Pk validation
            rnd => { byte[] b = new byte[64]; rnd.NextBytes(b); return b; },
        };

        private static readonly Func<Random, string>[] StringEdgeCases = {
             // Empty string
            rnd => "",
            // Long string (adjust max size as needed)
            rnd => GenerateRandomStringInternal(rnd, rnd.Next(256, 513)),
            // String with control characters
            rnd => GenerateRandomStringInternal(rnd, rnd.Next(10, 30), includeControlChars: true),
            // String with non-UTF8 sequences (by generating random bytes)
            rnd => Encoding.UTF8.GetString(ByteArrayEdgeCases.Last()(rnd)), // Re-use random byte generation
            // Basic SQL injection attempt
            rnd => "' OR '1'='1",
            // Basic script injection attempt
            rnd => "<script>alert('fuzz')</script>"
        };


        /// <summary>
        /// Initialize a new instance of the ParameterGenerator class
        /// </summary>
        /// <param name="seed">Random seed for parameter generation</param>
        public ParameterGenerator(int seed)
        {
            _random = new Random(seed);
            _neoGenerator = new NeoParameterGenerator(seed);
        }

        /// <summary>
        /// Generate a random parameter based on the parameter type with recursion depth tracking
        /// </summary>
        /// <param name="type">The parameter type</param>
        /// <param name="currentDepth">Current recursion depth</param>
        /// <returns>A stack item representing the parameter</returns>
        public StackItem GenerateParameter(ContractParameterType type, int currentDepth = 0)
        {
            // Prevent excessive recursion
            if (currentDepth > MaxRecursionDepth)
            {
                // Return a simple type if max depth is reached
                int primitiveTypeIndex = _random.Next(5); // Boolean, Integer, ByteArray, String, Hash160
                switch (primitiveTypeIndex)
                {
                    case 0: return GenerateBoolean();
                    case 1: return GenerateInteger();
                    case 2: return GenerateByteArray();
                    case 3: return GenerateString();
                    case 4: return GenerateHash160(); // Use Hash160 as a representative byte type
                    default: return StackItem.Null; // Should not happen
                }
            }

            switch (type)
            {
                case ContractParameterType.Boolean:
                    return GenerateBoolean();
                case ContractParameterType.Integer:
                    return GenerateInteger();
                case ContractParameterType.ByteArray:
                    return GenerateByteArray();
                case ContractParameterType.String:
                    return GenerateString();
                case ContractParameterType.Hash160:
                    return _neoGenerator.GenerateHash160();
                case ContractParameterType.Hash256:
                    return _neoGenerator.GenerateHash256();
                case ContractParameterType.PublicKey:
                    return _neoGenerator.GeneratePublicKey();
                case ContractParameterType.Signature:
                    return _neoGenerator.GenerateSignature();
                case ContractParameterType.Array:
                    return GenerateArray(currentDepth + 1);
                case ContractParameterType.Map:
                    return GenerateMap(currentDepth + 1);
                case ContractParameterType.Any:
                    return GenerateAny(currentDepth + 1);
                case ContractParameterType.Void:
                    return StackItem.Null; // Void parameters are not expected
                case ContractParameterType.InteropInterface:
                    // Interfaces cannot be easily generated randomly. Return Null.
                    return StackItem.Null;
                default:
                    // For unknown/unsupported types, return null
                    Console.WriteLine($"Warning: Unsupported parameter type encountered: {type}. Returning Null.");
                    return StackItem.Null;
            }
        }

        /// <summary>
        /// Generate a random boolean value
        /// </summary>
        /// <returns>A boolean stack item</returns>
        private StackItem GenerateBoolean()
        {
            return _random.Next(2) == 0 ? StackItem.False : StackItem.True;
        }

        /// <summary>
        /// Generate a random integer value, possibly an edge case
        /// </summary>
        /// <returns>An integer stack item</returns>
        private StackItem GenerateInteger()
        {
            // Occasionally generate an edge case value
            if (_random.Next(100) < EdgeCaseProbability)
            {
                int index = _random.Next(IntegerEdgeCases.Length);
                return new Integer(IntegerEdgeCases[index]);
            }

            // Otherwise, generate a random integer within a typical range
            // Use bit manipulation for safer random long generation
            int lower = _random.Next(int.MinValue, int.MaxValue); // Generate 32 random bits
            int upper = _random.Next(int.MinValue, int.MaxValue); // Generate another 32 random bits
            long randomLong = ((long)upper << 32) | (uint)lower; // Combine into a 64-bit long

            if (_random.Next(10) == 0) // 10% chance for a very large/small number
            {
                // Generate BigInteger around common large values (e.g., near ulong max/min)
                var largeMagnitudeBytes = new byte[32]; // Up to 256 bits
                _random.NextBytes(largeMagnitudeBytes);
                // Ensure non-zero for BigInteger constructor if needed by adding 1 randomly
                if (largeMagnitudeBytes.All(b => b == 0) && _random.Next(2) == 0) largeMagnitudeBytes[0] = 1;
                var largeMagnitude = new BigInteger(largeMagnitudeBytes);
                return new Integer(_random.Next(2) == 0 ? largeMagnitude : -largeMagnitude);
            }
            return new Integer(randomLong);
        }

        /// <summary>
        /// Generate a random byte array, possibly an edge case
        /// </summary>
        /// <param name="fixedLength">Optional fixed length (less likely to hit edge cases if specified)</param>
        /// <returns>A byte array stack item</returns>
        private StackItem GenerateByteArray(int? fixedLength = null)
        {
            // Occasionally generate an edge case value, less likely if fixedLength is specified
            if (fixedLength == null && _random.Next(100) < EdgeCaseProbability)
            {
                int index = _random.Next(ByteArrayEdgeCases.Length);
                byte[] edgeBytes = ByteArrayEdgeCases[index](_random);
                return new ByteString(edgeBytes);
            }

            // Generate a random byte array of length 1-32, unless fixedLength is provided
            int length = fixedLength ?? _random.Next(1, 33);
            if (length < 0) length = 0; // Allow zero length if randomly generated near min int
            byte[] bytes = new byte[length];
            if (length > 0) _random.NextBytes(bytes);
            return new ByteString(bytes);
        }


        /// <summary>
        /// Generate a random string, possibly an edge case
        /// </summary>
        /// <returns>A string stack item</returns>
        private StackItem GenerateString()
        {
            // Occasionally generate an edge case value
            if (_random.Next(100) < EdgeCaseProbability)
            {
                int index = _random.Next(StringEdgeCases.Length);
                string edgeString = StringEdgeCases[index](_random);
                return new ByteString(Encoding.UTF8.GetBytes(edgeString)); // Store as ByteString
            }

            // Generate a random string of length 1-20
            int length = _random.Next(1, 21);
            string randomString = GenerateRandomStringInternal(_random, length);
            return new ByteString(Encoding.UTF8.GetBytes(randomString));
        }

        /// <summary>
        /// Helper to generate random string content
        /// </summary>
        private static string GenerateRandomStringInternal(Random rnd, int length, bool includeControlChars = false)
        {
            if (length <= 0) return "";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:',.<>/?`~ ";
            const string controlChars = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F" + // C0 controls
                                        "\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F\x7F"; // DEL
            string charSet = includeControlChars ? chars + controlChars : chars;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(charSet[rnd.Next(charSet.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate a random Hash160 value (delegates to NeoParameterGenerator)
        /// </summary>
        /// <returns>A Hash160 stack item</returns>
        private StackItem GenerateHash160()
        {
            // Delegate to the Neo-specific generator for comprehensive Neo-specific generation
            return _neoGenerator.GenerateHash160();
        }

        /// <summary>
        /// Generate a random Hash256 value (potentially an edge case byte array)
        /// </summary>
        /// <returns>A Hash256 stack item</returns>
        [Obsolete("Use the NeoParameterGenerator.GenerateHash256() method instead for comprehensive Neo-specific generation")]
        private StackItem GenerateHash256()
        {
            // Delegate to the Neo-specific generator
            return _neoGenerator.GenerateHash256();
        }

        /// <summary>
        /// Generate a random public key (potentially an edge case byte array)
        /// </summary>
        /// <returns>A public key stack item</returns>
        [Obsolete("Use the NeoParameterGenerator.GeneratePublicKey() method instead for comprehensive Neo-specific generation")]
        private StackItem GeneratePublicKey()
        {
            // Delegate to the Neo-specific generator
            return _neoGenerator.GeneratePublicKey();
        }

        /// <summary>
        /// Generate a random array
        /// </summary>
        /// <param name="currentDepth">Current recursion depth</param>
        /// <returns>An array stack item</returns>
        private StackItem GenerateArray(int currentDepth)
        {
            // Generate a random array of size 0-5
            int size = _random.Next(6);
            var array = new VM.Types.Array();
            for (int i = 0; i < size; i++)
            {
                // Generate elements using GenerateAny with increased depth
                array.Add(GenerateAny(currentDepth + 1));
            }
            return array;
        }

        /// <summary>
        /// Generate a random map
        /// </summary>
        /// <param name="currentDepth">Current recursion depth</param>
        /// <returns>A map stack item</returns>
        private StackItem GenerateMap(int currentDepth)
        {
            // Generate a random map of size 0-5
            int size = _random.Next(6);
            var map = new Map();
            for (int i = 0; i < size; i++)
            {
                // Generate a key that's either a string or integer (PrimitiveType)
                PrimitiveType key;
                if (_random.Next(2) == 0)
                {
                    // Use GenerateString directly to get ByteString, then cast
                    key = (PrimitiveType)GenerateString();
                }
                else
                {
                    key = (PrimitiveType)GenerateInteger();
                }

                // Generate value using GenerateAny with increased depth
                StackItem value = GenerateAny(currentDepth + 1);

                // Avoid duplicate keys by trying a few times or skipping
                if (!map.ContainsKey(key))
                {
                    map[key] = value;
                }
                // Optional: Add logic here to retry with a different key if collision occurs
            }
            return map;
        }

        /// <summary>
        /// Generate a random stack item of any type
        /// </summary>
        /// <param name="currentDepth">Current recursion depth</param>
        /// <returns>A random stack item</returns>
        private StackItem GenerateAny(int currentDepth)
        {
            // Prevent excessive recursion directly within GenerateAny as well
            if (currentDepth > MaxRecursionDepth)
            {
                // Return a simple type if max depth is reached
                int primitiveTypeIndex = _random.Next(5); // Boolean, Integer, ByteArray, String, Hash160
                switch (primitiveTypeIndex)
                {
                    case 0: return GenerateBoolean();
                    case 1: return GenerateInteger();
                    case 2: return GenerateByteArray();
                    case 3: return GenerateString();
                    case 4: return _neoGenerator.GenerateHash160();
                    default: return StackItem.Null;
                }
            }

            // Generate a random parameter type index (0-10 inclusive now)
            // Boolean, Integer, ByteArray, String, Hash160, Hash256, PublicKey, Array, Map, Signature, StorageKey
            int typeIndex = _random.Next(11);
            switch (typeIndex)
            {
                case 0:
                    return GenerateBoolean();
                case 1:
                    return GenerateInteger();
                case 2:
                    return GenerateByteArray();
                case 3:
                    return GenerateString();
                case 4:
                    return _neoGenerator.GenerateHash160();
                case 5:
                    return _neoGenerator.GenerateHash256();
                case 6:
                    return _neoGenerator.GeneratePublicKey();
                case 7:
                    // Generate Array recursively
                    return GenerateArray(currentDepth + 1);
                case 8:
                    // Generate Map recursively
                    return GenerateMap(currentDepth + 1);
                case 9:
                    // Generate Signature
                    return _neoGenerator.GenerateSignature();
                case 10:
                    // Generate Storage Key
                    return _neoGenerator.GenerateStorageKey();
                default:
                    // Fallback to a simple type
                    return GenerateInteger();
            }
        }
    }
}
