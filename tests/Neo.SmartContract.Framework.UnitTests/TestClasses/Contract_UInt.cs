namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_UInt : SmartContract
    {
        public static bool IsZeroUInt256(UInt256 value)
        {
            return value.IsZero;
        }

        public static bool IsZeroUInt160(UInt160 value)
        {
            return value.IsZero;
        }
    }
}
