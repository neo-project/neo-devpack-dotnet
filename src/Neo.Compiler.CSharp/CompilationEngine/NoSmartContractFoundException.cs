using System;

namespace Neo.Compiler
{
    public sealed class NoSmartContractFoundException : FormatException
    {
        public const string DefaultMessage = "No valid neo SmartContract found. Please make sure your contract is subclass of SmartContract and is not abstract.";

        public NoSmartContractFoundException()
            : base(DefaultMessage)
        {
        }
    }
}
