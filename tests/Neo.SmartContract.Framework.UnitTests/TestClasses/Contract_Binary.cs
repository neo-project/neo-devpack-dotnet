using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Binary : SmartContract.Framework.SmartContract
    {
        public static byte[] base64Decode(string input)
        {
            return (byte[])Binary.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return Binary.Base64Encode((ByteString)input);
        }

        public static byte[] base58Decode(string input)
        {
            return (byte[])Binary.Base58Decode(input);
        }

        public static string base58Encode(byte[] input)
        {
            return Binary.Base58Encode((ByteString)input);
        }

        public static object atoi(string value, int @base)
        {
            return Binary.Atoi(value, @base);
        }

        public static string itoa(int value, int @base)
        {
            return Binary.Itoa(value, @base);
        }
    }
}
