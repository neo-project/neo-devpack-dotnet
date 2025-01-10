using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_MemberAccess : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var my = new MyClass();
            Runtime.Log(my.Data1.ToString());
            Runtime.Log(MyClass.Data2);
            //Runtime.Log(MyClass.Data3.ToString());
            Runtime.Log(my.Data4);
            Runtime.Log(my.Method());
        }

        public static void TestStaticComplexAssignment()
        {
            MyClass.Data3 = 0;
            ExecutionEngine.Assert(MyClass.Data3 == 0);
            MyClass.Data3 += 1;
            ExecutionEngine.Assert(MyClass.Data3 == 1);

            ExecutionEngine.Assert(MyClass.Data6 == "6");
            MyClass.Data6 += "233";
            ExecutionEngine.Assert(MyClass.Data6 == "6233");
        }

        public static void TestFieldComplexAssignment()
        {
            var my = new MyClass();
            ExecutionEngine.Assert(my.FieldComplexAssignment() == 6);
            ExecutionEngine.Assert(my.FieldComplexAssignmentString() == "hello2");
            ExecutionEngine.Assert((my.Data4 += "33") == "hello233");
        }

        public class MyClass
        {
            public int Data1 { get; set; }  // non-static IPropertySymbol

            public const string Data2 = "msg";

            public static int Data3 = 3;  // static IFieldSymbol

            public string Data4 = "hello";  // non-static IFieldSymbol

            public int Data5 = 5;  // non-static IFieldSymbol

            public static string Data6 { get; set; } = "6";  // static IPropertySymbol

            public string Method() => "";

            public int FieldComplexAssignment() => Data5 += 1;
            public string FieldComplexAssignmentString() => Data4 += "2";
        }
    }
}
