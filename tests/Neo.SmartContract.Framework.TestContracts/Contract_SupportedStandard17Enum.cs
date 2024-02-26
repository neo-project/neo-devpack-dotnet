using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [DisplayName(nameof(Contract_SupportedStandard17Enum))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ContractDescription("<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template")]
    [ContractPermission(Permission.WildCard, Method.WildCard)]
    [SupportedStandards(NepStandard.Nep17)]
    public class Contract_SupportedStandard17Enum : Nep17Token, INep17Payment
    {
        public override string Symbol { [Safe] get; }
        public override byte Decimals { [Safe] get; }

        public void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null)
        {
        }
    }
}
