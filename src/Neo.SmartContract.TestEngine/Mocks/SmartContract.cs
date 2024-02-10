namespace Neo.SmartContract.TestEngine.Mocks
{
    public class SmartContract
    {
        private readonly Engine _engine;

        /// <summary>
        /// Contract hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testEngine">TestEngine</param>
        /// <param name="hash">Contract hash</param>
        protected SmartContract(Engine testEngine, UInt160 hash)
        {
            _engine = testEngine;
            Hash = hash;
        }
    }
}
