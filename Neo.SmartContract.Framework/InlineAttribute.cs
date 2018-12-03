using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    // OpCode with data in hex or ascii
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class InlineAttribute : Attribute
    {
        // opcode
        public OpCode opCode { get; }
        // opcode data (can be hex "ab01ab" or ascii "Runtime.Notify")
        public string opData { get; }
        // if opcode data is Hex or ascii
        public bool isHex { get; }

        public InlineAttribute(OpCode opCode, string opData = "", bool isHex = false)
        {
            this.opCode = opCode;
            this.opData = opData;
            this.isHex = isHex;
        }
    }
}
