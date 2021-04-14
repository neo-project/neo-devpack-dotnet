using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class OpCodeAttribute : Attribute
    {
        public OpCodeAttribute(OpCode opCode, string opData = "")
        {
        }
    }
}
