using Neo.VM;
using System;

namespace Neo2.Compiler.MSIL.Utils
{
    internal class TestCrypto : ICrypto
    {
        public byte[] Hash160(byte[] message)
        {
            return new byte[] { 4, 56, 66, 5 };
        }

        public byte[] Hash256(byte[] message)
        {
            throw new NotImplementedException();
        }

        public bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey)
        {
            return true;
        }
    }
}
