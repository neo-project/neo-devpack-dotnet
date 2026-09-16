using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_DefaultNullable : SmartContract.Framework.SmartContract
    {
        public static bool TestDefaultLiteral()
        {
            int? x = default;
            return x == null;
        }

        public static bool TestDefaultExpression()
        {
            int? y = default(int?);
            return y == null;
        }

        public static int TestNullCoalescing()
        {
            int? z = default;
            return z ?? 42;
        }

        public static bool TestHasValue()
        {
            int? x = default;
            return x.HasValue;
        }

        public static int? TestReturnDefault()
        {
            return default;
        }

        public static bool TestComparison()
        {
            int? x = default;
            int? y = null;
            return x == y;
        }
    }
}
