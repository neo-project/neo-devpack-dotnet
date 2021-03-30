using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_OptimizationTest : SmartContract.Framework.SmartContract
    {
        private static Neo.UInt160 Owner = "Ne9ipxm2sPUaetvh3ZvjhyCzRZqP355dTZ".ToScriptHash();

        public static bool DummyMethod() { return true; }

        public static bool Verify()
        {
            return Runtime.CheckWitness(Owner);
        }
    }
}
