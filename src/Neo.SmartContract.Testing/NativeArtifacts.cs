using Neo.Persistence;
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

        #region Native contracts

        private ContractManagement? _contractManagement;
        private CryptoLib? _cryptoLib;
        private GasToken? _gas;
        private NeoToken? _neo;
        private LedgerContract? _ledger;
        private OracleContract? _oracle;
        private PolicyContract? _policy;
        private RoleManagement? _roleManagement;
        private StdLib? _stdLib;

        #endregion

        /// <summary>
        /// ContractManagement
        /// </summary>
        public ContractManagement ContractManagement
        {
            get
            {
                _contractManagement ??= _engine.FromHash<ContractManagement>(Native.NativeContract.ContractManagement.Hash, Native.NativeContract.ContractManagement.Id);
                return _contractManagement;
            }
        }

        /// <summary>
        /// CryptoLib
        /// </summary>
        public CryptoLib CryptoLib
        {
            get
            {
                _cryptoLib ??= _engine.FromHash<CryptoLib>(Native.NativeContract.CryptoLib.Hash, Native.NativeContract.CryptoLib.Id);
                return _cryptoLib;
            }
        }

        /// <summary>
        /// GasToken
        /// </summary>
        public GasToken GAS
        {
            get
            {
                _gas ??= _engine.FromHash<GasToken>(Native.NativeContract.GAS.Hash, Native.NativeContract.GAS.Id);
                return _gas;
            }
        }

        /// <summary>
        /// NeoToken
        /// </summary>
        public NeoToken NEO
        {
            get
            {
                _neo ??= _engine.FromHash<NeoToken>(Native.NativeContract.NEO.Hash, Native.NativeContract.NEO.Id);
                return _neo;
            }
        }

        /// <summary>
        /// LedgerContract
        /// </summary>
        public LedgerContract Ledger
        {
            get
            {
                _ledger ??= _engine.FromHash<LedgerContract>(Native.NativeContract.Ledger.Hash, Native.NativeContract.Ledger.Id);
                return _ledger;
            }
        }

        /// <summary>
        /// OracleContract
        /// </summary>
        public OracleContract Oracle
        {
            get
            {
                _oracle ??= _engine.FromHash<OracleContract>(Native.NativeContract.Oracle.Hash, Native.NativeContract.Oracle.Id);
                return _oracle;
            }
        }

        /// <summary>
        /// PolicyContract
        /// </summary>
        public PolicyContract Policy
        {
            get
            {
                _policy ??= _engine.FromHash<PolicyContract>(Native.NativeContract.Policy.Hash, Native.NativeContract.Policy.Id);
                return _policy;
            }
        }

        /// <summary>
        /// RoleManagement
        /// </summary>
        public RoleManagement RoleManagement
        {
            get
            {
                _roleManagement ??= _engine.FromHash<RoleManagement>(Native.NativeContract.RoleManagement.Hash, Native.NativeContract.RoleManagement.Id);
                return _roleManagement;
            }
        }

        /// <summary>
        /// OracleContract
        /// </StdLib>
        public StdLib StdLib
        {
            get
            {
                _stdLib ??= _engine.FromHash<StdLib>(Native.NativeContract.StdLib.Hash, Native.NativeContract.StdLib.Id);
                return _stdLib;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeArtifacts(TestEngine engine)
        {
            _engine = engine;
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

            foreach (var native in new Native.NativeContract[]
                {
                    Native.NativeContract.ContractManagement,
                    Native.NativeContract.Ledger,
                    Native.NativeContract.NEO,
                    Native.NativeContract.GAS
                }
            )
            {
                // Mock Native.OnPersist

                var method = native.GetType().GetMethod("OnPersist", BindingFlags.NonPublic | BindingFlags.Instance);

                DataCache clonedSnapshot = _engine.Storage.Snapshot.CreateSnapshot();
                using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis, _engine.ProtocolSettings, _engine.Gas))
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

                using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis, _engine.ProtocolSettings, _engine.Gas))
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
