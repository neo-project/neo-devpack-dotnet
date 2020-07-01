using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Base64 : SmartContract.Framework.SmartContract
    {
        public static byte[] base64Decode(string input)
        {
            return Binary.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return Binary.Base64Encode(input);
        }
    }
}
