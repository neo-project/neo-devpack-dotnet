namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_SwitchValid : SmartContract.Framework.SmartContract
    {
        public static object Main(string method)
        {
            switch (method)
            {
                case "0": return 1;
                case "1": return 2;
                case "2": return 3;
                case "3": return 4;
                case "4": return 5;
                case "5": return 6;

                default: return 99;
            }
        }
    }
}
