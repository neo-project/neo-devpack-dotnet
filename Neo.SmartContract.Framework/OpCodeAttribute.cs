using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class OpCodeAttribute : Attribute
    {
        /// <summary>
        /// opcode
        /// </summary>
        public OpCode OpCode { get; }

        /// <summary>
        /// opcode data (can be hex "ab01ab" or ascii "Runtime.Notify")
        /// </summary>
        public string OpData { get; }

        public OpCodeAttribute(OpCode opCode, string opData = "")
        {
            OpCode = opCode;
            OpData = opData;
        }
    }
}
