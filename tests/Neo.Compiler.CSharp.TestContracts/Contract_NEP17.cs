using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [SupportedStandards(NepStandard.Nep17)]
    public class Contract_NEP17 : Nep17Token
    {
        public override byte Decimals { [Safe] get => 8; }

        public override string Symbol { [Safe] get => "TEST"; }
    }
}
