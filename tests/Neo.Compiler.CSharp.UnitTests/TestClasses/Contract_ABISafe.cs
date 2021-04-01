using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_ABISafe : SmartContract.Framework.SmartContract
    {
        static int s = 1;

        public static int UnitTest_001()
        {
            return 1;
        }

        [Safe]
        public static int UnitTest_002()
        {
            return 2;
        }

        public static int UnitTest_003()
        {
            return 3;
        }
    }
}
