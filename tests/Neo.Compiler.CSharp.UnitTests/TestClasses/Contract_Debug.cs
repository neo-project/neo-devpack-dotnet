using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Debug : SmartContract.Framework.SmartContract
    {
        public static int TestElse()
        {
#if DEBUG
            return 1;
#else
            return 2;
#endif
        }

        public static int TestIf()
        {
#if DEBUG
            return 1;
#endif
            return 2;
        }
    }
}
