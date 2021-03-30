namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_a : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return 'a';
        }
    }

    public class Contract_b : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return 'b';
        }
    }
}
