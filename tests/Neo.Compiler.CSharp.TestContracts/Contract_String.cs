using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_String : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var firstName = "Mark";
            var lastName = $"";
            var timestamp = Ledger.GetBlock(Ledger.CurrentHash).Timestamp;
            Runtime.Log($"Hello, {firstName} {lastName}! Current timestamp is {timestamp}.");
        }
    }
}
