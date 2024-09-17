using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Assignment : SmartContract.Framework.SmartContract
    {
        public static void TestAssignment()
        {
            int a = 1;
            ExecutionEngine.Assert(a == 1);
            int b;
            a = b = 2;
            ExecutionEngine.Assert(a == 2);
            ExecutionEngine.Assert(b == 2);
        }

        public static void TestCoalesceAssignment()
        {
            int? a = null;
            a ??= 1;
            ExecutionEngine.Assert(a == 1);
            a ??= 2;
            ExecutionEngine.Assert(a == 1);
        }
    }
}
