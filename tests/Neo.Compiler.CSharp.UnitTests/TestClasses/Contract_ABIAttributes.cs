using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    [ContractPermission("*", "c")]
    [ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "b")]
    [ContractTrust("0x0a0b00ff00ff00ff00ff00ff00ff00ff00ff00a4")]
    public class Contract_ABIAttributes : SmartContract.Framework.SmartContract
    {
        public static int test() => 0;
    }
}
