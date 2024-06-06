namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_StaticConstruct : SmartContract.Framework.SmartContract
    {
        static int a;
        //define and staticvar and initit with a runtime code.
        static Contract_StaticConstruct()
        {
            int b = 3;
            a = b + 1;
        }

        public static int TestStatic()
        {
            return a;
        }
    }
}
