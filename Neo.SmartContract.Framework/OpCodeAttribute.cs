using Neo.VM;
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

        /// <summary>
        /// if opcode data is Hex (default) or ascii
        /// </summary>
        public bool IsHex { get; }

        public OpCodeAttribute(OpCode opCode, string opData = "", bool isHex = true)
        {
            OpCode = opCode;
            OpData = opData;
            IsHex = isHex;
        }
    }
}
