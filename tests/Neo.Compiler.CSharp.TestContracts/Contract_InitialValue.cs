using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_InitialValue : SmartContract.Framework.SmartContract
    {
        // Test single-parameter constructor (defaults to String type)
        [InitialValue("DefaultName")]
        private static readonly string DefaultName;

        // Test dual-parameter constructor with explicit Integer type
        [InitialValue("18", ContractParameterType.Integer)]
        private static readonly int DefaultAge;

        // Test dual-parameter constructor with Hash160 type
        [InitialValue("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq", ContractParameterType.Hash160)]
        private static readonly UInt160 DefaultOwner;

        public static string GetDefaultName()
        {
            return DefaultName;
        }

        public static int GetDefaultAge()
        {
            return DefaultAge;
        }

        public static UInt160 GetDefaultOwner()
        {
            return DefaultOwner;
        }
    }
}
