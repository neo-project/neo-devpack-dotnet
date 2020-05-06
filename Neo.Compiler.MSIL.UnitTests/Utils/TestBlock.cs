using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.MSIL.Utils
{
    internal class TestBlock : IScriptContainer
    {
        public readonly int TxCount = 4;
        public readonly List<TestTxInput> Inputs = new List<TestTxInput>();
        public readonly List<TestTxOutput> Outputs = new List<TestTxOutput>();

        public byte[] GetMessage()
        {
            return new byte[] { 1 };
        }

        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
    }
}
