using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class OpCodesAttribute : Attribute
    {
        public OpCode[] OpCodes { get; }

        public OpCodesAttribute(params OpCode[] opcodes)
        {
            this.OpCodes = opcodes;
        }
    }
}
