using System;

namespace AntShares.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ScriptOpAttribute : Attribute
    {
        public ScriptOp ScriptOp { get; }

        public ScriptOpAttribute(ScriptOp opcode)
        {
            this.ScriptOp = opcode;
        }
    }
}
