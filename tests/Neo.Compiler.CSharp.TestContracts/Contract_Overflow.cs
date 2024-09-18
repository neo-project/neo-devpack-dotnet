namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Overflow : SmartContract.Framework.SmartContract
    {
        public static int Add(int a, int b) => a + b;
        public static int Mul(int a, int b) => a * b;
    }
}
