namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_NULL : SmartContract.Framework.SmartContract
    {
        public static bool IsNull(byte[] value)
        {
            return value is null;
        }

        public static bool EqualNull(byte[] value)
        {
            return value == null;
        }
    }
}
