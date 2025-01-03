using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_BinaryExpression : SmartContract.Framework.SmartContract
    {
        public static void BinaryIs()
        {
            ByteString a = "a";
            ExecutionEngine.Assert(a is ByteString);
            string b = $"";
#pragma warning disable CS0184
            ExecutionEngine.Assert(b is ByteString);
#pragma warning restore CS0184
        }

        public static void BinaryAs()
        {
            UInt160 a = UInt160.Zero;
            ExecutionEngine.Assert(a as ByteString == UInt160.Zero);
        }
    }
}
