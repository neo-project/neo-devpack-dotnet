using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Logical : SmartContract.Framework.SmartContract
    {
        public static bool TestConditionalLogicalAnd(bool x, bool y)
        {
            return x && y;
        }

        public static bool TestConditionalLogicalOr(bool x, bool y)
        {
            return x || y;
        }

        public static bool TestLogicalExclusiveOr(bool x, bool y)
        {
            return x ^ y;
        }

        public static int TestLogicalAnd(byte x, byte y)
        {
            return x & y;
        }

        public static int TestLogicalOr(byte x, byte y)
        {
            return x | y;
        }

        public static bool TestLogicalNegation(bool x)
        {
            return !x;
        }
    }
}
