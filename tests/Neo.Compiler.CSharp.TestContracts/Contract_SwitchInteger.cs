namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_SwitchValid : SmartContract.Framework.SmartContract
    {
        public static object Main(int b)
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
    }
}
