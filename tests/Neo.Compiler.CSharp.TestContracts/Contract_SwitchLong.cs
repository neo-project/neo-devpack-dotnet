namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_SwitchInvalid : SmartContract.Framework.SmartContract
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
                case "6": return 7;
                case "7": return 8;
                case "8": return 9;
                case "9": return 10;
                case "10": return 11;
                case "11": return 12;
                case "12": return 13;
                case "13": return 14;
                case "14": return 15;
                case "15": return 16;
                case "16": return 17;
                case "17": return 18;
                case "18": return 19;
                case "19": return 20;
                case "20": return 21;

                default: return 99;
            }
        }
    }
}
