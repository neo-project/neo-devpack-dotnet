using Neo.SmartContract;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_UIntTypes : SmartContract.Framework.SmartContract
    {
        [InitialValue("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB", ContractParameterType.Hash160)]
        static readonly UInt160 Owner = default;

        public static bool checkOwner(UInt160 owner) { return owner == Owner; }

        public static bool checkZeroStatic(UInt160 owner) { return owner == UInt160.Zero; }

        public static UInt160 constructUInt160(byte[] bytes) { return (UInt160)bytes; }

        public static bool validateAddress(UInt160 address) => address.IsValid && !address.IsZero;
    }
}
