namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_InvokeCsNef : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// One return
        /// </summary>
        public static int returnInteger()
        {
            return 42;
        }

        public static int Main()
        {
            return 22;
        }

        public static string returnString()
        {
            return "hello world";
        }
    }
}
