using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    [WildcardContractPermission("a", "b")]
    [ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "b")]
    public class Contract_ABIContractPermission : SmartContract.Framework.SmartContract
    {
        public static int test() => 0;
    }
}
