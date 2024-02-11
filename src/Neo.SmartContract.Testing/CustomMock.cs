using System.Reflection;

namespace Neo.SmartContract.Testing
{
    internal class CustomMock
    {
        /// <summary>
        /// Contract
        /// </summary>
        public required SmartContract Contract { get; init; }

        /// <summary>
        /// Method
        /// </summary>
        public required MethodInfo Method { get; init; }
    }
}
