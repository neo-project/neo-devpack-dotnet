namespace Neo.SmartContract.Testing
{
    public class NativeArtifacts
    {
        private readonly TestEngine _engine;
        private ContractManagement? _contractManagement;
        private CryptoLib? _cryptoLib;

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
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeArtifacts(TestEngine engine)
        {
            _engine = engine;
        }
    }
}
