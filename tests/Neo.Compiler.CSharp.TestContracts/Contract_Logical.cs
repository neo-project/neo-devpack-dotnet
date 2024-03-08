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

        public static byte TestLogicalAnd(byte x, byte y)
        {
            return x & y;
        }

        public static byte TestLogicalOr(byte x, byte y)
        {
            return x | y;
        }

        public static byte TestLogicalNegation(bool x)
        {
            return !x;
        }
    }
}
