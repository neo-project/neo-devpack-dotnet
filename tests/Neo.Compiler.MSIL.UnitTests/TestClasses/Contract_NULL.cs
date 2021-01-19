using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
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

        public static bool EqualNotNullA(byte[] value)
        {
            return null != value;
        }

        public static bool EqualNotNullB(byte[] value)
        {
            return value != null;
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

        public static bool IfNull(object obj)
        {
            if ((bool)obj)
            {
                return true;
            }

            return false;
        }

        public static object NullCollationAndCollation(string code)
        {
            var context = Storage.CurrentContext;
            return Storage.Get(context, code) ?? (ByteString) new byte[] { 123 };
        }

        public static object NullCollationAndCollation2(string code)
        {
            var context = Storage.CurrentContext;
            Storage.Put(context, code, "111");
            return Storage.Get(context, code) ?? (ByteString) new byte[] { 123 };
        }
    }
}
