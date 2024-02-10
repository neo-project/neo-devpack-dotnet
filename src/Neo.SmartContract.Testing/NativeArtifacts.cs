namespace Neo.SmartContract.Testing
{
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
                _contractManagement ??= _engine.FromHash<ContractManagement>(Native.NativeContract.ContractManagement.Hash);
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
                _cryptoLib ??= _engine.FromHash<CryptoLib>(Native.NativeContract.CryptoLib.Hash);
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
                _gas ??= _engine.FromHash<GasToken>(Native.NativeContract.GAS.Hash);
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
                _neo ??= _engine.FromHash<NeoToken>(Native.NativeContract.NEO.Hash);
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
                _ledger ??= _engine.FromHash<LedgerContract>(Native.NativeContract.Ledger.Hash);
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
                _oracle ??= _engine.FromHash<OracleContract>(Native.NativeContract.Oracle.Hash);
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
                _policy ??= _engine.FromHash<PolicyContract>(Native.NativeContract.Policy.Hash);
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
                _roleManagement ??= _engine.FromHash<RoleManagement>(Native.NativeContract.RoleManagement.Hash);
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
                _stdLib ??= _engine.FromHash<StdLib>(Native.NativeContract.StdLib.Hash);
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
        public void Initialize()
        {
            // TODO: Initialize them
        }
    }
}
