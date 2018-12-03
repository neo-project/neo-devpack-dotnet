using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// OpCode with data in hex or ascii
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class InlineAttribute : Attribute
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
        /// if opcode data is Hex or ascii
        /// </summary>
        public bool IsHex { get; }

        public InlineAttribute(OpCode opCode, string opData = "", bool isHex = false)
        {
            OpCode = opCode;
            OpData = opData;
            IsHex = isHex;
        }
    }
}
