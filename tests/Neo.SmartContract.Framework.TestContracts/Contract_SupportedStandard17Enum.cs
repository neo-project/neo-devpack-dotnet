using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [DisplayName(nameof(Contract_SupportedStandard17Enum))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ManifestExtra("Description", "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template")]
    [ContractPermission("*", "*")]
    [SupportedStandards(NEPStandard.NEP17)]
    public class Contract_SupportedStandard17Enum : Nep17Token
    {
        public override string Symbol { [Safe] get; }
        public override byte Decimals { [Safe] get; }
    }
}
