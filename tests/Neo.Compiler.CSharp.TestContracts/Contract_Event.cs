using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Event : SmartContract.Framework.SmartContract
    {
        public static int MyStaticVar1;
        public static bool MyStaticVar2;

        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static void Main2(string method, object[] args)
        {
            MyStaticVar1 = 1;
            MyStaticVar2 = true;
        }
    }
}
