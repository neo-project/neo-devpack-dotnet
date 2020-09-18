namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_UIntTypes : SmartContract.Framework.SmartContract
    {
        static readonly UInt160 Owner = SmartContract.Framework.Helper.ToScriptHash("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB");

        public static bool checkOwner(UInt160 owner) { return owner == Owner; }

        public static bool checkZeroLocal(UInt160 owner) { return owner == new UInt160(); }

        // public static bool checkZeroStatic(UInt160 owner) { return owner == UInt160.Zero; }

    }
}
