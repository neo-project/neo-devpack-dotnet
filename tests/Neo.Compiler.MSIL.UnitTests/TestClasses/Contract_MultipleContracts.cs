using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_a : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return 'a';
        }
    }

    class Contract_b : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return 'b';
        }
    }
}
