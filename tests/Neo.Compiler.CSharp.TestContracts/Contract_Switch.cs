namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Switch : SmartContract.Framework.SmartContract
    {
        public static object SwitchLong(string method)
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

        public static object Switch6(string method)
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

        public static object Switch6Inline(string method)
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

        public static object SwitchInteger(int b)
        {
            int a = 1;
            switch (b)
            {
                case 1:
                    a = 2;
                    break;
                case 2:
                    a = 3;
                    break;
                case 3:
                    a = 6;
                    break;
                default:
                    a = 0;
                    break;
            }
            return a;
        }

        public static object SwitchLongLong(string test)
        {
            int a = 1;
            switch (test)
            {
                case "a":
                    a++;
                    break;
                case "c":
                    a = a * 2;
                    break;
                case "b":
                    a--;
                    break;
                case "d":
                    a = a * -1;
                    break;
                case "e":
                    a = a * a;
                    break;
                case "f":
                    a = a * 3;
                    break;
                case "g":
                    a = a + 2;
                    break;
                /* 您可以有任意数量的 case 语句 */
                default: /* 可选的 */
                    a = a / 1;
                    break;
            }
            return a;
        }
    }
}
