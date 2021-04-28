using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract_CheckWitness : SmartContract.Framework.SmartContract
    {
        public static bool testWitness(UInt160 signature)
        {
            return Runtime.CheckWitness(signature);
        }
    }
}
