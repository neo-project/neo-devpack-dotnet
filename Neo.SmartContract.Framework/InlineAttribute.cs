using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    // OpCode with Extension parameters
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class InlineAttribute : Attribute
    {
        // opcode id
        public OpCode op { get; }
        // extension of opcode
        public string ext { get; }
        // if extension is Hex or Char
        public bool isHex { get; }

        public InlineAttribute(OpCode op, string extension = "", bool isHex = false)
        {
            this.op = op;
            this.ext = extension;
            this.isHex = isHex;
        }
    }
}
