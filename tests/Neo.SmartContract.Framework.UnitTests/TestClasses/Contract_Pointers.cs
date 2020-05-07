using Neo.SmartContract.Framework.Services.Neo;
using System;

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
    }
}
