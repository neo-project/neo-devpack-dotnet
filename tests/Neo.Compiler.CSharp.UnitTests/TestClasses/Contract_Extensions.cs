namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public static class Ext
    {
        public static int sum(this int a, int b)
        {
            return a + b;
        }
    }

    public class Contract_Extensions : SmartContract.Framework.SmartContract
    {
        public static int TestSum(int a, int b)
        {
            return a.sum(b);
        }
    }
}
