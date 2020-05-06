using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Event : SmartContract.Framework.SmartContract
    {
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        [Neo.SmartContract.Framework.Appcall("1234567890abcdef1234567890abcdef12345678")]
        public static extern string DynamicTest(string arg);

        public static void Main(string method, object[] args) { }
    }
}
