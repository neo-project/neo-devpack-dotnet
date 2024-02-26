namespace Neo.SmartContract.Testing
{
    public class SmartContractInitialize
    {
        /// <summary>
        /// Engine
        /// </summary>
        public TestEngine Engine { get; }

        /// <summary>
        /// Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// ContractId
        /// </summary>
        internal int? ContractId { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="hash">Hash</param>
        /// <param name="contractId">Contract Id</param>
        internal SmartContractInitialize(TestEngine engine, UInt160 hash, int? contractId = null)
        {
            Engine = engine;
            Hash = hash;
            ContractId = contractId;
        }
    }
}
