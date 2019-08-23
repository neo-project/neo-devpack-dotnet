using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Abi : SmartContract.Framework.SmartContract
    {
        [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static void Main(string method, object[] args) { }

        [ReadOnly(true)]
        public static void readOnlyTrue() { }

        [ReadOnly(false)]
        public static void readOnlyFalse1() { }

        public static void readOnlyFalse2() { }
    }
}
