using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Extends the parameter generator with Neo N3-specific type generation capabilities
    /// </summary>
    public class NeoParameterGenerator
    {
        // Neo N3 native contract hashes (TestNet values)
        private static readonly byte[] NeoTokenHash = new byte[] { 0xed, 0xa6, 0x2c, 0xa0, 0x7c, 0xb2, 0x62, 0x38, 0x97, 0xa8, 0xcc, 0x52, 0x6e, 0x90, 0x58, 0x3f, 0x9d, 0x5a, 0xf4, 0x25 };
        private static readonly byte[] GasTokenHash = new byte[] { 0x35, 0xc0, 0x79, 0x28, 0x44, 0x4c, 0xdd, 0x7c, 0x56, 0x37, 0x94, 0xd4, 0xf8, 0x0f, 0xf9, 0xb1, 0xb1, 0xd6, 0x32, 0x39 };
        private static readonly byte[] ContractManagementHash = new byte[] { 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        private static readonly byte[] OracleContractHash = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        private static readonly byte[] PolicyContractHash = new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        private static readonly byte[] RoleManagementHash = new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };

        // Common user account formats
        private static readonly byte[][] CommonUserAddressFormats = {
            // Standard 20-byte address format
            new byte[20], 
            // All zeros - special case
            new byte[20], 
            // All ones - special case
            Enumerable.Repeat((byte)0xFF, 20).ToArray(),
            // Account with single byte - for edge testing
            new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
        };

        // Common storage key prefixes used in Neo contracts
        private static readonly byte[][] CommonStoragePrefixes = {
            new byte[] { 0x01 },     // Single byte prefix
            new byte[] { 0x02 },     // Alternative single byte
            new byte[] { 0x09 },     // Common balance prefix
            new byte[] { 0x0a },     // Common account prefix
            new byte[] { 0x11 },     // Contract storage
            new byte[] { 0x70 },     // Contract property 
            Encoding.UTF8.GetBytes("BALANCE"), // Text-based prefix
            Encoding.UTF8.GetBytes("TOTAL"),   // Total supply
            Encoding.UTF8.GetBytes("STATE")    // Contract state
        };

        private readonly Random _random;
        private const int MaxRecursionDepth = 3;
        private const int EdgeCaseProbability = 20; // Chance (out of 100) to generate an edge case
        private const int StorageMaxKeySize = 64; // Maximum Neo storage key size

        /// <summary>
        /// Initialize a new instance of the NeoParameterGenerator class
        /// </summary>
        /// <param name="seed">Random seed for parameter generation</param>
        public NeoParameterGenerator(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Generate a random Hash160 value (Neo address or contract hash)
        /// </summary>
        /// <returns>A Hash160 stack item</returns>
        public StackItem GenerateHash160()
        {
            // Decide whether to generate a special case or random Hash160
            if (_random.Next(100) < EdgeCaseProbability)
            {
                // Select a predefined contract hash or address format
                byte[] hash160;
                int specialCase = _random.Next(10);

                switch (specialCase)
                {
                    case 0:
                        // Neo native token
                        hash160 = NeoTokenHash;
                        break;
                    case 1:
                        // Gas native token
                        hash160 = GasTokenHash;
                        break;
                    case 2:
                        // Contract Management
                        hash160 = ContractManagementHash;
                        break;
                    case 3:
                        // Oracle
                        hash160 = OracleContractHash;
                        break;
                    case 4:
                        // Policy
                        hash160 = PolicyContractHash;
                        break;
                    case 5:
                        // Role Management
                        hash160 = RoleManagementHash;
                        break;
                    default:
                        // Use common user address formats
                        int addrIndex = _random.Next(CommonUserAddressFormats.Length);
                        hash160 = CommonUserAddressFormats[addrIndex].ToArray();
                        // Fill with random data if not special format (all zeros/ones)
                        if (addrIndex == 0)
                        {
                            _random.NextBytes(hash160);
                        }
                        break;
                }

                return new ByteString(hash160);
            }

            // Standard random Hash160
            var generatedBytes = new byte[20];
            _random.NextBytes(generatedBytes);
            return new ByteString(generatedBytes);
        }

        /// <summary>
        /// Generate a random Hash256 value (transaction hash, block hash)
        /// </summary>
        /// <returns>A Hash256 stack item</returns>
        public StackItem GenerateHash256()
        {
            // Decide whether to generate a special case or random Hash256
            if (_random.Next(100) < EdgeCaseProbability)
            {
                // Select a special case
                int specialCase = _random.Next(4);
                byte[] hash256;

                switch (specialCase)
                {
                    case 0:
                        // Zero hash (all zeros)
                        hash256 = new byte[32];
                        break;
                    case 1:
                        // Max value hash (all 0xFF)
                        hash256 = Enumerable.Repeat((byte)0xFF, 32).ToArray();
                        break;
                    case 2:
                        // Genesis block hash pattern
                        hash256 = new byte[32];
                        hash256[0] = 0x01;
                        break;
                    default:
                        // Random hash
                        hash256 = new byte[32];
                        _random.NextBytes(hash256);
                        break;
                }

                return new ByteString(hash256);
            }

            // Standard random Hash256
            var generatedBytes = new byte[32];
            _random.NextBytes(generatedBytes);
            return new ByteString(generatedBytes);
        }

        /// <summary>
        /// Generate a random public key with valid EC point format
        /// </summary>
        /// <returns>A public key stack item</returns>
        public StackItem GeneratePublicKey()
        {
            // Decide whether to generate a valid or invalid public key
            bool generateValidFormat = _random.Next(100) < 80; // 80% valid keys

            if (generateValidFormat)
            {
                // Generate a valid compressed public key (always 33 bytes)
                var pubKeyBytes = new byte[33];
                _random.NextBytes(pubKeyBytes);

                // Set the prefix byte to either 0x02 or 0x03 for compressed format
                pubKeyBytes[0] = (byte)(_random.Next(2) == 0 ? 0x02 : 0x03);

                return new ByteString(pubKeyBytes);
            }
            else
            {
                // Generate an invalid public key for fuzzing
                // Various invalid formats: wrong length, invalid prefix, etc.
                int invalidCase = _random.Next(4);
                byte[] invalidPubKey;

                switch (invalidCase)
                {
                    case 0:
                        // Wrong length (not 33 bytes)
                        invalidPubKey = new byte[_random.Next(20, 40)];
                        _random.NextBytes(invalidPubKey);
                        break;
                    case 1:
                        // Invalid prefix (not 0x02 or 0x03)
                        invalidPubKey = new byte[33];
                        _random.NextBytes(invalidPubKey);
                        invalidPubKey[0] = (byte)(_random.Next(0x04, 0xFF)); // Not 0x02 or 0x03
                        break;
                    case 2:
                        // Uncompressed format attempt (65 bytes with 0x04 prefix)
                        invalidPubKey = new byte[65];
                        _random.NextBytes(invalidPubKey);
                        invalidPubKey[0] = 0x04; // Uncompressed format
                        break;
                    default:
                        // All zeros or other special pattern
                        invalidPubKey = new byte[33];
                        // Leave as zeros or set all to 0xFF
                        if (_random.Next(2) == 0)
                        {
                            for (int i = 0; i < invalidPubKey.Length; i++)
                                invalidPubKey[i] = 0xFF;
                        }
                        break;
                }

                return new ByteString(invalidPubKey);
            }
        }

        /// <summary>
        /// Generate a valid ECDSA signature or an edge case
        /// </summary>
        /// <returns>A signature stack item</returns>
        public StackItem GenerateSignature()
        {
            // Decide whether to generate a standard signature or an edge case
            if (_random.Next(100) < EdgeCaseProbability)
            {
                int edgeCase = _random.Next(5);
                byte[] sigBytes;

                switch (edgeCase)
                {
                    case 0:
                        // Empty signature
                        sigBytes = System.Array.Empty<byte>();
                        break;
                    case 1:
                        // Invalid length signature (not 64 bytes)
                        int invalidLength = _random.Next(1, 128);
                        if (invalidLength == 64) invalidLength++; // Ensure it's not 64
                        sigBytes = new byte[invalidLength];
                        _random.NextBytes(sigBytes);
                        break;
                    case 2:
                        // All zeros signature
                        sigBytes = new byte[64];
                        break;
                    case 3:
                        // All ones signature
                        sigBytes = Enumerable.Repeat((byte)0xFF, 64).ToArray();
                        break;
                    default:
                        // Valid length, but potentially invalid format or values
                        sigBytes = new byte[64];
                        _random.NextBytes(sigBytes);
                        // Potentially set R or S to invalid range for ECDSA
                        if (_random.Next(2) == 0)
                        {
                            // Set part of R or S to extreme values
                            int startIdx = _random.Next(2) == 0 ? 0 : 32; // R or S
                            for (int i = 0; i < 32; i++)
                            {
                                sigBytes[startIdx + i] = (byte)(_random.Next(2) == 0 ? 0 : 0xFF);
                            }
                        }
                        break;
                }

                return new ByteString(sigBytes);
            }

            // Standard random signature (64 bytes for ECDSA R+S components)
            byte[] signature = new byte[64];
            _random.NextBytes(signature);
            return new ByteString(signature);
        }

        /// <summary>
        /// Generate a storage key suitable for Neo smart contract storage operations
        /// </summary>
        /// <returns>A storage key as a byte string</returns>
        public StackItem GenerateStorageKey()
        {
            // Decide whether to use a known storage prefix or generate random
            bool useKnownPrefix = _random.Next(100) < 70; // 70% use known prefixes
            byte[] storageKey;

            if (useKnownPrefix)
            {
                // Select a common prefix
                int prefixIndex = _random.Next(CommonStoragePrefixes.Length);
                byte[] prefix = CommonStoragePrefixes[prefixIndex];

                // Generate the remaining part of the key
                int remainingLength = _random.Next(1, StorageMaxKeySize - prefix.Length + 1);
                storageKey = new byte[prefix.Length + remainingLength];

                // Copy the prefix
                System.Buffer.BlockCopy(prefix, 0, storageKey, 0, prefix.Length);

                // Fill the rest with random data
                for (int i = prefix.Length; i < storageKey.Length; i++)
                {
                    storageKey[i] = (byte)_random.Next(256);
                }

                // Special cases for known prefixes
                if (prefixIndex >= 6) // Text-based prefixes
                {
                    // For text prefixes, possibly append a UInt160 hash or number
                    if (_random.Next(2) == 0)
                    {
                        byte[] appendData;
                        if (_random.Next(2) == 0)
                        {
                            // Append a UInt160 hash
                            appendData = new byte[20];
                            _random.NextBytes(appendData);
                        }
                        else
                        {
                            // Append a number as string
                            string numberStr = _random.Next(1000000).ToString();
                            appendData = Encoding.UTF8.GetBytes(numberStr);
                        }

                        // Create new array with the appended data
                        byte[] combinedKey = new byte[prefix.Length + appendData.Length];
                        System.Buffer.BlockCopy(prefix, 0, combinedKey, 0, prefix.Length);
                        System.Buffer.BlockCopy(appendData, 0, combinedKey, prefix.Length, appendData.Length);
                        storageKey = combinedKey;
                    }
                }
            }
            else
            {
                // Generate a completely random storage key
                int keyLength = _random.Next(1, StorageMaxKeySize + 1);
                storageKey = new byte[keyLength];
                _random.NextBytes(storageKey);
            }

            return new ByteString(storageKey);
        }
    }
}
