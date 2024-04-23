namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_MultipleA : SmartContract.Framework.SmartContract
    {
        public static bool test() => true;
    }

    public class Contract_MultipleB : SmartContract.Framework.SmartContract
    {
        public static bool test() => false;
    }
}
