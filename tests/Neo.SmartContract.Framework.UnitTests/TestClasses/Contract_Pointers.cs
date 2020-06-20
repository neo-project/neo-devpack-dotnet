using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;
using System;
using System.Numerics;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Pointers : SmartContract.Framework.SmartContract
    {
        public static Func<int> CreateFuncPointer()
        {
            return new Func<int>(MyMethod);
        }

        public static int MyMethod()
        {
            return 123;
        }

        public static int CallFuncPointer()
        {
            var pointer = CreateFuncPointer();
            return pointer.Invoke();
        }

        public static Func<byte[], BigInteger> CreateFuncPointerWithArg()
        {
            return new Func<byte[], BigInteger>(MyMethodWithArg);
        }

        public static BigInteger MyMethodWithArg(byte[] num)
        {
            return num.ToBigInteger();
        }

        public static BigInteger CallFuncPointerWithArg()
        {
            var pointer = CreateFuncPointerWithArg();

            return pointer.Invoke(new byte[] { 11, 22, 33 });
        }
    }
}
