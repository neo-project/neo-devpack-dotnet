namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_UInt : SmartContract.Framework.SmartContract
    {
        public static bool IsEmptyUInt256(UInt256 value)
        {
            return value.IsEmpty;
        }

        public static bool IsEmptyUInt160(UInt160 value)
        {
            return value.IsEmpty;
        }
    }
}
