using Neo.Cryptography.ECC;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Types_ECPoint : SmartContract.Framework.SmartContract
    {
        public static bool isValid(ECPoint point) { return point.IsValid; }
    }
}
