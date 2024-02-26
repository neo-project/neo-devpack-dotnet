using Neo.Persistence;
using Neo.SmartContract.Testing.Native;
using System;
using System.Reflection;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// NativeArtifacts makes it easier to access native contracts
    /// </summary>
    public class NativeArtifacts
    {
        private readonly TestEngine _engine;

        /// <summary>
        /// ContractManagement
        /// </summary>
        public ContractManagement ContractManagement { get; }

        /// <summary>
        /// CryptoLib
        /// </summary>
        public CryptoLib CryptoLib { get; }

        /// <summary>
        /// GasToken
        /// </summary>
        public GasToken GAS { get; }

        /// <summary>
        /// NeoToken
        /// </summary>
        public NeoToken NEO { get; }

        /// <summary>
        /// LedgerContract
        /// </summary>
        public LedgerContract Ledger { get; }

        /// <summary>
        /// OracleContract
        /// </summary>
        public OracleContract Oracle { get; }

        /// <summary>
        /// PolicyContract
        /// </summary>
        public PolicyContract Policy { get; }

        /// <summary>
        /// RoleManagement
        /// </summary>
        public RoleManagement RoleManagement { get; }

        /// <summary>
        /// StdLib
        /// </StdLib>
        public StdLib StdLib { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeArtifacts(TestEngine engine)
        {
            _engine = engine;

            ContractManagement = _engine.FromHash<ContractManagement>(Neo.SmartContract.Native.NativeContract.ContractManagement.Hash, Neo.SmartContract.Native.NativeContract.ContractManagement.Id);
            CryptoLib = _engine.FromHash<CryptoLib>(Neo.SmartContract.Native.NativeContract.CryptoLib.Hash, Neo.SmartContract.Native.NativeContract.CryptoLib.Id);
            GAS = _engine.FromHash<GasToken>(Neo.SmartContract.Native.NativeContract.GAS.Hash, Neo.SmartContract.Native.NativeContract.GAS.Id);
            NEO = _engine.FromHash<NeoToken>(Neo.SmartContract.Native.NativeContract.NEO.Hash, Neo.SmartContract.Native.NativeContract.NEO.Id);
            Ledger = _engine.FromHash<LedgerContract>(Neo.SmartContract.Native.NativeContract.Ledger.Hash, Neo.SmartContract.Native.NativeContract.Ledger.Id);
            Oracle = _engine.FromHash<OracleContract>(Neo.SmartContract.Native.NativeContract.Oracle.Hash, Neo.SmartContract.Native.NativeContract.Oracle.Id);
            Policy = _engine.FromHash<PolicyContract>(Neo.SmartContract.Native.NativeContract.Policy.Hash, Neo.SmartContract.Native.NativeContract.Policy.Id);
            RoleManagement = _engine.FromHash<RoleManagement>(Neo.SmartContract.Native.NativeContract.RoleManagement.Hash, Neo.SmartContract.Native.NativeContract.RoleManagement.Id);
            StdLib = _engine.FromHash<StdLib>(Neo.SmartContract.Native.NativeContract.StdLib.Hash, Neo.SmartContract.Native.NativeContract.StdLib.Id);
        }

        /// <summary>
        /// Initialize native contracts
        /// </summary>
        /// <param name="commit">Initialize native contracts</param>
        public void Initialize(bool commit = false)
        {
            _engine.Transaction.Script = Array.Empty<byte>(); // Store the script in the current transaction

            var genesis = NeoSystem.CreateGenesisBlock(_engine.ProtocolSettings);

            // Attach to static event

            ApplicationEngine.Log += _engine.ApplicationEngineLog;
            ApplicationEngine.Notify += _engine.ApplicationEngineNotify;

            // Process native contracts

            foreach (var native in new Neo.SmartContract.Native.NativeContract[]
                {
                    Neo.SmartContract.Native.NativeContract.ContractManagement,
                    Neo.SmartContract.Native.NativeContract.Ledger,
                    Neo.SmartContract.Native.NativeContract.NEO,
                    Neo.SmartContract.Native.NativeContract.GAS
                }
            )
            {
                // Mock Native.OnPersist

                var method = native.GetType().GetMethod("OnPersist", BindingFlags.NonPublic | BindingFlags.Instance);

                DataCache clonedSnapshot = _engine.Storage.Snapshot.CreateSnapshot();
                using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis))
                {
                    engine.LoadScript(Array.Empty<byte>());
                    if (method!.Invoke(native, new object[] { engine }) is not ContractTask task)
                        throw new Exception($"Error casting {native.Name}.OnPersist to ContractTask");

                    task.GetAwaiter().GetResult();
                    if (engine.Execute() != VM.VMState.HALT)
                        throw new Exception($"Error executing {native.Name}.OnPersist");
                }

                // Mock Native.PostPersist

                method = native.GetType().GetMethod("PostPersist", BindingFlags.NonPublic | BindingFlags.Instance);

                using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis))
                {
                    engine.LoadScript(Array.Empty<byte>());
                    if (method!.Invoke(native, new object[] { engine }) is not ContractTask task)
                        throw new Exception($"Error casting {native.Name}.PostPersist to ContractTask");

                    task.GetAwaiter().GetResult();
                    if (engine.Execute() != VM.VMState.HALT)
                        throw new Exception($"Error executing {native.Name}.PostPersist");
                }

                clonedSnapshot.Commit();
            }

            if (commit)
            {
                _engine.Storage.Commit();
            }

            // Detach to static event

            ApplicationEngine.Log -= _engine.ApplicationEngineLog;
            ApplicationEngine.Notify -= _engine.ApplicationEngineNotify;
        }
    }
}
