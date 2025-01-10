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

            MyClass.Data3 = 0;
            ExecutionEngine.Assert(MyClass.Data3 == 0);
            MyClass.Data3 += 1;
            ExecutionEngine.Assert(MyClass.Data3 == 1);
        }

        public class MyClass
        {
            public int Data1 { get; set; }

            public const string Data2 = "msg";

            public static int Data3 = 3;

            public string Data4 = "hello";

            public string Method() => "";
        }
    }
}
