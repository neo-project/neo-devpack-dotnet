using System;

namespace Neo.Compiler.MSIL.Utils
{
    internal class TestTxOutput
    {
        public byte[] PrevHash { get; } = new byte[] { 1, 23, 44, 44 };

        public ushort PrevIndex => 7;

        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
    }
}
