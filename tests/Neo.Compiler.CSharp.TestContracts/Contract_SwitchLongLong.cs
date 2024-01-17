namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_SwitchInvalid : SmartContract.Framework.SmartContract
    {
        public static object Main(string test)
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
                    a = a * (-1);
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
