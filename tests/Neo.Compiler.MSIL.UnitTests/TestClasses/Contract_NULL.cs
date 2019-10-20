namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_NULL : SmartContract.Framework.SmartContract
    {
        public static bool IsNull(byte[] value)
        {
            return value is null;
        }

        public static bool EqualNullA(byte[] value)
        {
            return null == value;
        }

        public static bool EqualNullB(byte[] value)
        {
            return value == null;
        }
    }
}
