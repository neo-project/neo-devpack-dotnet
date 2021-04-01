namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Concat : SmartContract.Framework.SmartContract
    {
        public static string TestStringAdd1(string a)
        {
            return a + "hello";
        }

        public static string TestStringAdd2(string a, string b)
        {
            return a + b + "hello";
        }

        public static string TestStringAdd3(string a, string b, string c)
        {
            return a + b + c + "hello";
        }

        public static string TestStringAdd4(string a, string b, string c, string d)
        {
            return a + b + c + d + "hello";
        }
    }
}
