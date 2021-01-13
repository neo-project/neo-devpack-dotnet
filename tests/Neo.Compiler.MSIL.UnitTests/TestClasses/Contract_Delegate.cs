using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
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

        public static string testDynamicCall(byte[] token, string method)
        {
            var result = ((Func<string, object[], string>)token.ToDelegate())(method, new object[0]);
            return result;
        }
    }
}
