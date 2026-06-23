// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StandardsEnforcement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StandardsEnforcement
    {
        private const string NepStandardDiagnosticId = "NC3006";

        [TestMethod]
        public void DeclaredStandard_NonCompliant_FailsCompilation()
        {
            // Declares NEP-17 but the symbol getter is not marked [Safe], so the
            // contract does not correctly implement the standard it advertises.
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[SupportedStandards(NepStandard.Nep17)]
public class Contract : Nep17Token
{
    public override string Symbol { get => ""TKN""; }
    public override byte Decimals { [Safe] get => 8; }
}";

            var context = TestHelper.CompileSingleContract(source);

            Assert.IsFalse(context.Success, "A contract declaring a standard it does not implement must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == NepStandardDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected a NEP standard compliance error diagnostic.");
        }

        [TestMethod]
        public void DeclaredStandard_Compliant_Compiles()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[SupportedStandards(NepStandard.Nep17)]
public class Contract : Nep17Token
{
    public override string Symbol { [Safe] get => ""TKN""; }
    public override byte Decimals { [Safe] get => 8; }
}";

            var context = TestHelper.CompileSingleContract(source);

            Assert.IsTrue(context.Success, "A compliant contract should compile successfully.");
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == NepStandardDiagnosticId),
                "A compliant contract must not produce a NEP standard compliance diagnostic.");
        }
    }
}
