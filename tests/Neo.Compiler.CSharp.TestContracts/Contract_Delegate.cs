using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
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

        public delegate int MyDelegate(int x, int y);

        static int CalculateSum(int x, int y)
        {
            return x + y;
        }

        public void TestDelegate()
        {
            MyDelegate myDelegate = CalculateSum;
            int result = myDelegate(5, 6);
            Runtime.Log($"Sum: {result}");
        }
    }
}
