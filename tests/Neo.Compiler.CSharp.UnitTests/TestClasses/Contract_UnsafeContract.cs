using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public unsafe class Contract_UnsafeContract : SmartContract.Framework.SmartContract
    {
        public static object throwcall()
        {
            throw new System.Exception();
        }
    }
}
