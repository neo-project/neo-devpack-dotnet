namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        static int a1 = 1;

        public static object Main()
        {
            testadd();
            testmulti();
            return a1;
        }

        static void testadd()
        {
            a1 += 5;
        }
        static void testmulti()
        {
            a1 *= 7;
        }
    }
}
