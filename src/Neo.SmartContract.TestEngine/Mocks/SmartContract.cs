namespace Neo.SmartContract.TestEngine.Mocks
{
    public class SmartContract
    {
        private readonly TestEngine _engine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testEngine">TestEngine</param>
        protected SmartContract(TestEngine testEngine)
        {
            _engine = testEngine;
        }
    }
}
