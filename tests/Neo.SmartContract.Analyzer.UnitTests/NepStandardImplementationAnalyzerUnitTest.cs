// Copyright (C) 2015-2026 The Neo Project.
//
// NepStandardImplementationAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.NepStandardImplementationAnalyzer,
    Neo.SmartContract.Analyzer.NepStandardImplementationCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class NepStandardImplementationAnalyzerUnitTest
{
    private const string CommonSource = """
                                         using System;
                                         using System.Numerics;

                                         namespace Neo.SmartContract.Framework
                                         {
                                             public enum NepStandard
                                             {
                                                 Nep9,
                                                 Nep11,
                                                 Nep17,
                                                 Nep24,
                                                 Nep26,
                                                 Nep27,
                                                 Nep29,
                                                 Nep30
                                             }

                                             public class UInt160 { }

                                             public class ByteString { }

                                             public class Map<TKey, TValue> { }
                                         }

                                         namespace Neo.SmartContract.Framework.Attributes
                                         {
                                             [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
                                             public sealed class SupportedStandardsAttribute : Attribute
                                             {
                                                 public SupportedStandardsAttribute(params string[] supportedStandards) { }

                                                 public SupportedStandardsAttribute(params Neo.SmartContract.Framework.NepStandard[] supportedStandards) { }
                                             }
                                         }

                                         namespace Neo.SmartContract.Framework.Services
                                         {
                                             public class Iterator { }
                                         }

                                         namespace Neo.SmartContract.Framework.Interfaces
                                         {
                                             public interface INep24 { }
                                             public interface INEP26 { }
                                             public interface INEP27 { }
                                             public interface INEP29 { }
                                             public interface INEP30 { }
                                         }
                                         """;

    [TestMethod]
    public async Task Nep17_WithMissingMembers_ShouldReportDiagnostic()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                 }
                                             }
                                             """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyAnalyzerAsync(source, expectedDiagnostic).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep17_CodeFix_AddsMissingMembers()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                     public class SampleToken
                                                     {
                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep11_WithMissingMembers_ShouldReportDiagnostic()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep11)]
                                                 public class SampleNft
                                                 {
                                                 }
                                             }
                                             """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-11", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer, OwnerOf, Properties, Tokens, TokensOf");

        await Verifier.VerifyAnalyzerAsync(source, expectedDiagnostic).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep11_CodeFix_AddsMissingMembers()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep11)]
                                                 public class SampleNft
                                                 {
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep11)]
                                                     public class SampleNft
                                                     {
                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 to, Neo.SmartContract.Framework.ByteString tokenId, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static Neo.SmartContract.Framework.UInt160 OwnerOf(Neo.SmartContract.Framework.ByteString tokenId)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static Neo.SmartContract.Framework.Map<string, object> Properties(Neo.SmartContract.Framework.ByteString tokenId)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static Neo.SmartContract.Framework.Services.Iterator Tokens()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static Neo.SmartContract.Framework.Services.Iterator TokensOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-11", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer, OwnerOf, Properties, Tokens, TokensOf");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep11_DivisibleSurface_ShouldNotReportDiagnostic()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep11)]
                                                 public class DivisibleNft
                                                 {
                                                     public static string Symbol() => "DNFT";

                                                     public static byte Decimals() => 8;

                                                     public static System.Numerics.BigInteger TotalSupply() => 0;

                                                     public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner) => 0;

                                                     public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner, Neo.SmartContract.Framework.ByteString tokenId) => 0;

                                                     public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, Neo.SmartContract.Framework.ByteString tokenId, object data) => true;

                                                     public static Neo.SmartContract.Framework.Services.Iterator OwnerOf(Neo.SmartContract.Framework.ByteString tokenId) => null;

                                                     public static Neo.SmartContract.Framework.Map<string, object> Properties(Neo.SmartContract.Framework.ByteString tokenId) => null;

                                                     public static Neo.SmartContract.Framework.Services.Iterator Tokens() => null;

                                                     public static Neo.SmartContract.Framework.Services.Iterator TokensOf(Neo.SmartContract.Framework.UInt160 owner) => null;
                                                 }
                                             }
                                             """;

        await Verifier.VerifyAnalyzerAsync(source).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep11_DivisibleSpecificBalanceOnly_ShouldReportMissingCommonBalanceOf()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep11)]
                                                 public class DivisibleNft
                                                 {
                                                     public static string Symbol() => "DNFT";

                                                     public static byte Decimals() => 8;

                                                     public static System.Numerics.BigInteger TotalSupply() => 0;

                                                     public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner, Neo.SmartContract.Framework.ByteString tokenId) => 0;

                                                     public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, Neo.SmartContract.Framework.ByteString tokenId, object data) => true;

                                                     public static Neo.SmartContract.Framework.Services.Iterator OwnerOf(Neo.SmartContract.Framework.ByteString tokenId) => null;

                                                     public static Neo.SmartContract.Framework.Map<string, object> Properties(Neo.SmartContract.Framework.ByteString tokenId) => null;

                                                     public static Neo.SmartContract.Framework.Services.Iterator Tokens() => null;

                                                     public static Neo.SmartContract.Framework.Services.Iterator TokensOf(Neo.SmartContract.Framework.UInt160 owner) => null;
                                                 }
                                             }
                                             """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-11", "BalanceOf");

        await Verifier.VerifyAnalyzerAsync(source, expectedDiagnostic).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep24_MissingInterface_ShouldReportDiagnostic()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep24)]
                                                 public class SampleRoyalty
                                                 {
                                                 }
                                             }
                                             """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.InterfaceDiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-24", "Neo.SmartContract.Framework.Interfaces.INep24");

        await Verifier.VerifyAnalyzerAsync(source, expectedDiagnostic).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep24_CodeFix_AddsInterface()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep24)]
                                                 public class SampleRoyalty
                                                 {
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep24)]
                                                     public class SampleRoyalty : Neo.SmartContract.Framework.Interfaces.INep24
                                                     {
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.InterfaceDiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-24", "Neo.SmartContract.Framework.Interfaces.INep24");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Nep26_WithInterface_ShouldNotReportDiagnostic()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep26)]
                                                 public class SampleNep26 : Neo.SmartContract.Framework.Interfaces.INEP26
                                                 {
                                                     public void OnNEP11Payment(Neo.SmartContract.Framework.UInt160 from, System.Numerics.BigInteger amount, Neo.SmartContract.Framework.ByteString tokenId, object data = null)
                                                     {
                                                     }
                                                 }
                                             }
                                             """;

        await Verifier.VerifyAnalyzerAsync(source).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // Line-ending preservation tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that when the source uses CRLF line endings, the code fix only
    /// introduces CRLF in the generated members and does not rewrite unrelated
    /// lines in the document.
    /// </summary>
    [TestMethod]
    public async Task Nep17_CodeFix_PreservesCrlfLineEndings()
    {
        // Build source with explicit CRLF throughout
        var source = BuildWithLineEnding(CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                 }
                                             }
                                             """, "\r\n");

        var fixedSource = BuildWithLineEnding(CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                     public class SampleToken
                                                     {
                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """, "\r\n");

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that when the source uses LF line endings, the code fix only
    /// introduces LF in the generated members and does not rewrite unrelated
    /// lines in the document.
    /// </summary>
    [TestMethod]
    public async Task Nep17_CodeFix_PreservesLfLineEndings()
    {
        var source = BuildWithLineEnding(CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                 }
                                             }
                                             """, "\n");

        var fixedSource = BuildWithLineEnding(CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                     public class SampleToken
                                                     {
                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """, "\n");

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the code fix does not alter existing members (including those
    /// with string expressions) that are already present in the class body.
    /// Only the generated stub methods should be added; nothing else should change.
    /// </summary>
    [TestMethod]
    public async Task Nep17_CodeFix_DoesNotAlterExistingMembers()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                     public static string Description() => "A token";
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                     public class SampleToken
                                                     {
                                                         public static string Description() => "A token";

                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the interface code fix does not reformat the whole class,
    /// only the base list entry.
    /// </summary>
    [TestMethod]
    public async Task Nep24_CodeFix_DoesNotReformatUnrelatedClassMembers()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep24)]
                                                 public class SampleRoyalty
                                                 {
                                                     public static string Name() => "Royalty";
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep24)]
                                                     public class SampleRoyalty : Neo.SmartContract.Framework.Interfaces.INep24
                                                     {
                                                         public static string Name() => "Royalty";
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.InterfaceDiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-24", "Neo.SmartContract.Framework.Interfaces.INep24");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // Multiline-literal preservation tests
    // (line-ending normalization lives only here in the verifier/test, never in
    //  the production code fix)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that a verbatim string literal already present in the class is
    /// left intact after the code fix runs.  The code fix must not rewrite content
    /// inside string literals.
    /// </summary>
    [TestMethod]
    public async Task Nep17_CodeFix_DoesNotAlterVerbatimStringLiteral()
    {
        const string source = CommonSource + """

                                             namespace Contracts
                                             {
                                                 [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                 public class SampleToken
                                                 {
                                                     public static string GetDescription() => @"hello\nworld";
                                                 }
                                             }
                                             """;

        const string fixedSource = CommonSource + """

                                                 namespace Contracts
                                                 {
                                                     [Neo.SmartContract.Framework.Attributes.SupportedStandards(Neo.SmartContract.Framework.NepStandard.Nep17)]
                                                     public class SampleToken
                                                     {
                                                         public static string GetDescription() => @"hello\nworld";

                                                         public static string Symbol()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static byte Decimals()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger TotalSupply()
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static System.Numerics.BigInteger BalanceOf(Neo.SmartContract.Framework.UInt160 owner)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }

                                                         public static bool Transfer(Neo.SmartContract.Framework.UInt160 from, Neo.SmartContract.Framework.UInt160 to, System.Numerics.BigInteger amount, object data)
                                                         {
                                                             throw new System.NotImplementedException();
                                                         }
                                                     }
                                                 }
                                                 """;

        var expectedDiagnostic = Verifier.Diagnostic(NepStandardImplementationAnalyzer.DiagnosticId)
            .WithSpan(51, 6, 51, 110)
            .WithArguments("NEP-17", "Symbol, Decimals, TotalSupply, BalanceOf, Transfer");

        await Verifier.VerifyCodeFixAsync(source, expectedDiagnostic, fixedSource).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // Helper
    // -------------------------------------------------------------------------

    /// <summary>
    /// Replaces all line endings in <paramref name="text"/> with
    /// <paramref name="eol"/> so tests can exercise CRLF or LF explicitly.
    /// Line-ending normalization is intentionally kept here in the test helper
    /// and is never performed by the production code fix.
    /// </summary>
    private static string BuildWithLineEnding(string text, string eol)
    {
        // Normalise first to LF, then replace with target EOL
        var lf = text.Replace("\r\n", "\n");
        return eol == "\n" ? lf : lf.Replace("\n", eol);
    }
}
