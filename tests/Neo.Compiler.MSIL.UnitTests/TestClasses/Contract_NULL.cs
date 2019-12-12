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

        public static string NullCoalescing(string code)
        {
            string myname = code?.Substring(1, 2);
            return myname;
        }
        public static string NullCollation(string code)
        {
            string myname = code ?? "linux";
            return myname;
        }
    }
}
