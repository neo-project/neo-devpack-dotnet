using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_OptimizationTest : SmartContract.Framework.SmartContract
    {
        private static byte[] Owner = "Ne9ipxm2sPUaetvh3ZvjhyCzRZqP355dTZ".ToScriptHash();

        public static bool Verify()
        {
            return Runtime.CheckWitness(Owner);
        }
    }
}
