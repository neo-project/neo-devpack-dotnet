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
#pragma warning disable CS0067 // Event is never used
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static event Action<byte[], byte[], BigInteger> Transferred;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS0067 // Event is never used

        public static void Main2(string method, object[] args)
        {
            MyStaticVar1 = 1;
            MyStaticVar2 = true;
        }
    }
}
