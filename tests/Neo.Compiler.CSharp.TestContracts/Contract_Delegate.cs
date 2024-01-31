using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Delegate : SmartContract.Framework.SmartContract
    {
        public static int sumFunc(int a, int b)
        {
            return new Func<int, int, int>(privateSum).Invoke(a, b);
        }

        private static int privateSum(int a, int b)
        {
            return a + b;
        }
    }
}
