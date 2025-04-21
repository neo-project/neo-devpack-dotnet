using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Persistence.Providers;
using Neo.SmartContract.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Neo.Cryptography;
using Neo.Extensions;
using Neo.Ledger;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Neo.Wallets; // Added using

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Provides a simple blockchain environment for testing smart contracts
    /// </summary>
    public static class TestBlockchain
    {
        private static readonly NeoSystem _neoSystem;
        private static readonly UInt160 _defaultAccount;
        private static readonly ECPoint _defaultAccountECPoint;
        private static DataCache? _persistentSnapshot = null; // Added for persistent state

        /// <summary>
        /// Static constructor to initialize the test blockchain
        /// </summary>
        static TestBlockchain()
        {
            try // Add try-catch for detailed logging
            {
                Console.WriteLine("TestBlockchain static constructor: Starting..."); // Log start
                // Create a memory-based store
                var provider = new MemoryStoreProvider();
                Console.WriteLine("TestBlockchain static constructor: MemoryStoreProvider created.");

                // Create a valid temporary path for storage
                string tempPath = Path.Combine(Path.GetTempPath(), "NeoFuzzerTestChain");
                Directory.CreateDirectory(tempPath); // Ensure the directory exists
                Console.WriteLine($"TestBlockchain static constructor: Using temp path: {tempPath}");

                // Create a default account *before* creating settings
                byte[] privateKey = new byte[32];
                new Random(1234).NextBytes(privateKey);
                _defaultAccountECPoint = new KeyPair(privateKey).PublicKey;
                _defaultAccount = Contract.CreateSignatureRedeemScript(_defaultAccountECPoint).ToScriptHash();
                Console.WriteLine($"TestBlockchain static constructor: Default account created: {_defaultAccount}");

                // Create custom protocol settings using IConfiguration
                var configDict = new Dictionary<string, string?>
                {
                    // Add default values explicitly to ensure they are loaded
                    { "ProtocolConfiguration:Network", "860833102" }, // Neo N3 MainNet Magic
                    { "ProtocolConfiguration:AddressVersion", "53" },    // Neo N3 Address Version (0x35)
                    // Set StandbyCommittee to include only the default account
                    // Note: Configuration keys use ':' as separator
                    { "ProtocolConfiguration:StandbyCommittee:0", _defaultAccountECPoint.ToString() },
                    // Set ValidatorsCount to 1
                    { "ProtocolConfiguration:ValidatorsCount", "1" }
                };

                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(configDict)
                    .Build();

                // Load settings from the specific section
                IConfigurationSection section = configuration.GetSection("ProtocolConfiguration");
                ProtocolSettings settings = ProtocolSettings.Load(section);

                // Check if settings were loaded correctly (Load should handle defaults)
                if (settings.Network == 0) // Basic check, Load should provide defaults
                {
                    throw new InvalidOperationException("Failed to load ProtocolSettings using IConfiguration.");
                }
                Console.WriteLine($"TestBlockchain static constructor: Custom ProtocolSettings loaded via IConfiguration.");

                // Initialize NeoSystem with the loaded settings
                _neoSystem = new NeoSystem(settings, nameof(MemoryStore), tempPath);
                Console.WriteLine("TestBlockchain static constructor: NeoSystem initialized.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"!!! TestBlockchain static constructor FAILED: {ex.GetType().Name} - {ex.Message}");
                Console.Error.WriteLine(ex.ToString()); // Log full exception details to stderr
                // Optionally re-throw or handle as needed, but logging is crucial here
                throw;
            }
            finally
            {
                Console.WriteLine("TestBlockchain static constructor: Finished.");
            }
        }

        /// <summary>
        /// Get a snapshot of the blockchain. This snapshot is isolated and changes are NOT persisted
        /// back to the underlying store unless explicitly committed.
        /// </summary>
        /// <returns>A snapshot of the blockchain</returns>
        public static DataCache GetSnapshot()
        {
            // Return a fresh snapshot cache for isolated execution
            return _neoSystem.GetSnapshotCache();
        }

        /// <summary>
        /// Gets the current persistent snapshot, creating one if it doesn't exist.
        /// Changes made to this snapshot will affect subsequent calls using this method
        /// until ResetPersistentState is called, provided CommitPersistentSnapshot is used.
        /// </summary>
        /// <returns>The persistent snapshot cache</returns>
        public static DataCache GetOrCreatePersistentSnapshot()
        {
            _persistentSnapshot ??= _neoSystem.GetSnapshotCache();
            return _persistentSnapshot;
        }

        /// <summary>
        /// Commits the current persistent snapshot back to the underlying MemoryStoreProvider.
        /// This makes the state changes visible to future snapshots.
        /// </summary>
        public static void CommitPersistentSnapshot()
        {
            _persistentSnapshot?.Commit();
            // We keep the same snapshot instance after commit for subsequent calls
            // until ResetPersistentState is called.
        }

        /// <summary>
        /// Resets the persistent snapshot, ensuring the next call to
        /// GetOrCreatePersistentSnapshot starts from the last committed state
        /// of the underlying store.
        /// </summary>
        public static void ResetPersistentState()
        {
            _persistentSnapshot = null;
        }

        /// <summary>
        /// Get the default account
        /// </summary>
        /// <returns>The default account</returns>
        public static UInt160 GetDefaultAccount()
        {
            return _defaultAccount;
        }

        /// <summary>
        /// A test block for persisting changes
        /// </summary>
        public class TestPersistingBlock
        {
            public Header Header { get; }
            public Transaction[] Transactions { get; }

            public TestPersistingBlock()
            {
                Header = new Header
                {
                    PrevHash = UInt256.Zero,
                    MerkleRoot = UInt256.Zero,
                    Timestamp = 1234567890,
                    Index = 1,
                    NextConsensus = UInt160.Zero,
                    Witness = new Witness { InvocationScript = System.Array.Empty<byte>(), VerificationScript = System.Array.Empty<byte>() }
                };
                Transactions = System.Array.Empty<Transaction>();
            }
        }
    }
}
