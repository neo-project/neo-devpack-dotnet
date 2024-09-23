namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Overflow : SmartContract.Framework.SmartContract
    {
        public static int AddInt(int a, int b) => a + b;
        public static int MulInt(int a, int b) => a * b;
        public static uint AddUInt(uint a, uint b) => a + b;
        public static uint MulUInt(uint a, uint b) => a * b;
    }
}
