using System.Reflection;

namespace Neo.SmartContract.Testing
{
    internal class CustomMock
    {
        /// <summary>
        /// Mocked contract
        /// </summary>
        public required SmartContract Contract { get; init; }

        /// <summary>
        /// Mocked method
        /// </summary>
        public required MethodInfo Method { get; init; }
    }
}
