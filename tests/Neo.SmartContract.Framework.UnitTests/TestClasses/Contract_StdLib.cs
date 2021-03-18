using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Binary : SmartContract.Framework.SmartContract
    {
        public static byte[] base64Decode(string input)
        {
            return (byte[])StdLib.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return StdLib.Base64Encode((ByteString)input);
        }

        public static byte[] base58Decode(string input)
        {
            return (byte[])StdLib.Base58Decode(input);
        }

        public static string base58Encode(byte[] input)
        {
            return StdLib.Base58Encode((ByteString)input);
        }

        public static object atoi(string value, int @base)
        {
            return StdLib.Atoi(value, @base);
        }

        public static string itoa(int value, int @base)
        {
            return StdLib.Itoa(value, @base);
        }
    }
}
