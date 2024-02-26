using System.Reflection;

namespace Neo.SmartContract.Testing
{
    internal class CustomMock
    {
        /// <summary>
        /// Mocked contract
        /// </summary>
        public SmartContract Contract { get; }

        /// <summary>
        /// Mocked method
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        public CustomMock(SmartContract contract, MethodInfo method)
        {
            Contract = contract;
            Method = method;
        }
    }
}
