namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Boolean : SmartContract.Framework.SmartContract
    {
        public static bool TestBooleanOr()
        {
            return true || false;
        }
    }
}
