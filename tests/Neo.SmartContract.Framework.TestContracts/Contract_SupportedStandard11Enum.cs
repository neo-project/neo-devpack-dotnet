using System.Numerics;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NepStandard.Nep11)]
    public class Contract_SupportedStandard11Enum : Nep11Token<Nep11TokenState>, INep11Payable
    {
        public static bool TestStandard()
        {
            return true;
        }

        public override string Symbol { [Safe] get; }

        public void OnNEP11Payment(UInt160 from, BigInteger amount, object? data = null)
        {
        }
    }
}
