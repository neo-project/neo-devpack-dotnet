namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_UIntTypes : SmartContract.Framework.SmartContract
    {
        static readonly UInt160 Owner = SmartContract.Framework.Helper.ToScriptHash("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");

        public static bool checkOwner(UInt160 owner) { return owner == Owner; }

        public static bool checkZeroStatic(UInt160 owner) { return owner == UInt160.Zero; }

        public static UInt160 constructUInt160(byte[] bytes) { return (UInt160)bytes; }

        public static bool validateAddress(UInt160 address) => address.IsValid && !address.IsZero;
    }
}
