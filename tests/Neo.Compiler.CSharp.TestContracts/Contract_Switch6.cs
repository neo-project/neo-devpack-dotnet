using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Switch6 : SmartContract.Framework.SmartContract
    {
        public static object TestMain(string method)
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

        public static object Main2(string method)
        {
            return method switch
            {
                "0" => 1,
                "1" => 2,
                "2" => 3,
                "3" => 4,
                "4" => 5,
                "5" => 6,
                _ => 99
            };
        }
    }
}
