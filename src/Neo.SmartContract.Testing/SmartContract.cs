namespace Neo.SmartContract.Testing
{
    public class SmartContract
    {
        private readonly TestEngine _engine;

        /// <summary>
        /// Contract hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testEngine">TestEngine</param>
        /// <param name="hash">Contract hash</param>
        protected SmartContract(TestEngine testEngine, UInt160 hash)
        {
            _engine = testEngine;
            Hash = hash;
        }
    }
}
