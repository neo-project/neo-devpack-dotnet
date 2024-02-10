using Neo.TestEngine.Contracts;

namespace Neo.SmartContract.TestEngine
{
    public class NativeArtifacts
    {
        private readonly TestEngine _engine;
        private ContractManagement? _contractManagement;

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
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeArtifacts(TestEngine engine)
        {
            _engine = engine;
        }
    }
}
