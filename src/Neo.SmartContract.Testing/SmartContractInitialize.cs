namespace Neo.SmartContract.Testing
{
    public class SmartContractInitialize
    {
        /// <summary>
        /// Engine
        /// </summary>
        public required TestEngine Engine { get; init; }

        /// <summary>
        /// Hash
        /// </summary>
        public required UInt160 Hash { get; init; }

        /// <summary>
        /// ContractId
        /// </summary>
        internal int? ContractId { get; init; }
    }
}
