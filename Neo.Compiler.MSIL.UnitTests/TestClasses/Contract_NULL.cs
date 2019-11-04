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

        public static object Main(string method, object[] args)
        {
            if (method == "IsNull")
                return IsNull((byte[])args[0]);
            if (method == "EqualNullA")
                return EqualNullA((byte[])args[0]);
            if (method == "EqualNullB")
                return EqualNullB((byte[])args[0]);
            return null;
        }
    }
}
