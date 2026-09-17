using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_InitialValue : SmartContract.Framework.SmartContract
    {
        // These fields are initialized by the Neo compiler from attributes, not C# initializers.
#pragma warning disable CS8618, CS0649
        // Test single-parameter constructor with inferred String type
        [InitialValue("DefaultName")]
        private static readonly string DefaultName;

        // Test dual-parameter constructor with explicit Integer type
        [InitialValue("18", ContractParameterType.Integer)]
        private static readonly int DefaultAge;

        // Test single-parameter constructor with inferred Integer type
        [InitialValue("42")]
        private static readonly int InferredInteger;

        // Test single-parameter constructor with inferred ByteArray type
        [InitialValue("00112233")]
        private static readonly byte[] InferredBytes;

        // Test dual-parameter constructor with explicit Hash160 type
        [InitialValue("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq", ContractParameterType.Hash160)]
        private static readonly UInt160 DefaultOwner;
#pragma warning restore CS8618, CS0649

        public static string GetDefaultName()
        {
            return DefaultName;
        }

        public static int GetDefaultAge()
        {
            return DefaultAge;
        }

        public static int GetInferredInteger() => InferredInteger;

        public static byte[] GetInferredBytes() => InferredBytes;

        public static UInt160 GetDefaultOwner()
        {
            return DefaultOwner;
        }
    }
}
