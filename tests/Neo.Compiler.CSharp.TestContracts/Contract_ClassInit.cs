using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public struct IntInit
    {
        public int A;
        public BigInteger B;
    }

    public class Contract_ClassInit : SmartContract.Framework.SmartContract
    {
        public static IntInit testInitInt()
        {
            return new IntInit();
        }
    }
}
