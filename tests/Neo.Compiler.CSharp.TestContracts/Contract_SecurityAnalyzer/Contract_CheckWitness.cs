using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
#pragma warning disable CS8625
    public class Contract_CheckWitness : SmartContract.Framework.SmartContract
    {
        public static void Main(UInt160 u)
        {
            Runtime.CheckWitness(u);
            ExecutionEngine.Assert(Runtime.CheckWitness(u));
        }
#pragma warning restore CS8625
    }
}
