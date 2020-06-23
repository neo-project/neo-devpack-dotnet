using System;

namespace Neo.SmartContract.Framework.Services.System
{
    public class Binary
    {
        [Syscall("System.Binary.Base64Decode")]
        public extern byte[] Base64Decode(string input);

        [Syscall("System.Binary.Base64Encode")]
        public extern string Base64Encode(byte[] input);
    }
}
