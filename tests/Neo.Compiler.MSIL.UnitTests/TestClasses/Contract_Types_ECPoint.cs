using Neo.Cryptography.ECC;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_ECPoint : SmartContract.Framework.SmartContract
    {
        public static bool isValid(ECPoint point) { return point.IsValid; }
    }
}
