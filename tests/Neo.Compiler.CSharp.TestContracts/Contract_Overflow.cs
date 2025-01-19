namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Overflow : SmartContract.Framework.SmartContract
    {
        public static int AddInt(int a, int b) => a + b;
        public static int MulInt(int a, int b) => a * b;
        public static uint AddUInt(uint a, uint b) => a + b;
        public static uint MulUInt(uint a, uint b) => a * b;

        public static int NegateIntChecked(int a) => checked(-a);
        public static int NegateInt(int a) => -a;

        public static long NegateLongChecked(long a) => checked(-a);
        public static long NegateLong(long a) => -a;

        public static int NegateShortChecked(short a) => checked(-a);
        public static int NegateShort(short a) => -a;
    }
}
