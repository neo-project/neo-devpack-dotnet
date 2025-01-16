using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_NULL : SmartContract.Framework.SmartContract
    {
        public static bool IsNull(object? value)
        {
            return value is null;
        }

        public static bool EqualNullA(object? value)
        {
            return null == value;
        }

        public static bool EqualNullB(object? value)
        {
            return value == null;
        }

        public static bool EqualNotNullA(object? value)
        {
            return null != value;
        }

        public static bool EqualNotNullB(object? value)
        {
            return value != null;
        }

        public static string? NullCoalescing(string code)
        {
            string? myname = code?.Substring(1, 2);
            return myname;
        }

        public static string NullCollation(string code)
        {
            string myname = code ?? "linux";
            return myname;
        }

        public static bool NullPropertyGT(string a)
        {
            return a?.Length > 0;
        }

        public static bool NullPropertyLT(string a)
        {
            return a?.Length < 0;
        }

        public static bool NullPropertyGE(string a)
        {
            return a?.Length >= 0;
        }

        public static bool NullPropertyLE(string a)
        {
            return a?.Length <= 0;
        }

        public static bool NullProperty(string a)
        {
            return a?.Length != 0;
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
            return Storage.Get(context, code) ?? (ByteString)new byte[] { 123 };
        }

        public static object NullCollationAndCollation2(string code)
        {
            var context = Storage.CurrentContext;
            Storage.Put(context, code, "111");
            return Storage.Get(context, code) ?? (ByteString)new byte[] { 123 };
        }

        class TestClass
        {
            public void VoidMethod()
            {
            }
            public int IntProperty => 42;
            public byte? NullableField;
            public byte?[]? NullableArray { get; set; }
            public static bool? StaticNullableField = true;
            public static int? StaticNullableProperty { get; set; } = 0;

            public byte? FieldCoalesce(byte? f) => NullableField ??= f;
            public byte?[]? PropertyCoalesce(byte?[]? p) => NullableArray ??= p;
        }

        public static void NullType()
        {
            TestClass? obj1 = null;
            obj1?.VoidMethod();
        }

        public static void NullCoalescingAssignment(byte? nullableArg)
        {
            nullableArg ??= 1;
            TestClass t = new() { NullableField = nullableArg };
            ExecutionEngine.Assert(t.NullableField == 1);
            t.NullableField = null;
            ExecutionEngine.Assert(t.NullableArray == null);
            t.NullableArray ??= [0, null, 2, 3, t.NullableField];
            t.PropertyCoalesce([null]);
            t.FieldCoalesce(t.NullableArray![0]);
            ExecutionEngine.Assert(t.NullableField == 0);
            t.NullableField ??= t.NullableArray[1];
            ExecutionEngine.Assert(t.NullableField == 0);
            t.NullableArray[1] ??= 1;
            ExecutionEngine.Assert(t.NullableArray[1] == 1);
            t.NullableArray[1] ??= 2;
            ExecutionEngine.Assert(t.NullableArray[1] == 1);
            t.NullableArray[1]++;
            ExecutionEngine.Assert(t.NullableArray[1] == 2);
            t.NullableArray = [null];
            ExecutionEngine.Assert(t.NullableArray[0] == null);
        }

        public static void StaticNullableCoalesceAssignment()
        {
            TestClass.StaticNullableField ??= null;
            ExecutionEngine.Assert(TestClass.StaticNullableField == true);
            TestClass.StaticNullableField = null;
            ExecutionEngine.Assert((TestClass.StaticNullableField ??= false) == false);

            TestClass.StaticNullableProperty ??= null;
            ExecutionEngine.Assert(TestClass.StaticNullableProperty == 0);
            TestClass.StaticNullableProperty = null;
            ExecutionEngine.Assert((TestClass.StaticNullableProperty ??= 1) == 1);
            TestClass.StaticNullableProperty++;
            ExecutionEngine.Assert(TestClass.StaticNullableProperty == 2);
            --TestClass.StaticNullableProperty;
            ExecutionEngine.Assert(TestClass.StaticNullableProperty == 1);
        }
    }
}
