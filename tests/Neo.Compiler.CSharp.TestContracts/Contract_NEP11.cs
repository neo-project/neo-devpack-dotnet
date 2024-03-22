using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    [SupportedStandards("NEP-11")]
    public class Contract_NEP11 : Nep11Token<TokenState>
    {
        public override string Symbol { [Safe] get => "TEST"; }
    }

    public class TokenState : Nep11TokenState
    {

    }
}
