using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo2.Compiler.MSIL.Utils
{
    internal class TestTransaction : IScriptContainer
    {
        public readonly byte[] Hash = new byte[] { 26, 05, 01 };
        public readonly List<TestTxInput> Inputs = new List<TestTxInput> { new TestTxInput() };
        public readonly List<TestTxOutput> Outputs = new List<TestTxOutput> { new TestTxOutput() };

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
