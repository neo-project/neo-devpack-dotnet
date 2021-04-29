using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Math : SmartContract.Framework.SmartContract
    {
        public static int max(int a, int b)
        {
            return Math.Max(a, b);
        }

        public static int min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static int sign(int a)
        {
            return Math.Sign(a);
        }
    }
}
