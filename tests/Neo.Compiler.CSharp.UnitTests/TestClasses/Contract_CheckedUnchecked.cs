namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_CheckedUnchecked : SmartContract.Framework.SmartContract
    {
        public static int AddChecked(int a, int b)
        {
            return checked(a + b);
        }

        public static int AddUnchecked(int a, int b)
        {
            return unchecked(a + b);
        }

        public static uint CastChecked(int a)
        {
            return checked((uint)a);
        }

        public static uint CastUnchecked(int a)
        {
            return unchecked((uint)a);
        }
    }
}
