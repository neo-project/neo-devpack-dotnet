using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_InterfaceSupportedStandards
{
    [TestMethod]
    public void Nep17Interface_ContributesSupportedStandardToManifest()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using System.Numerics;

public class Contract : SmartContract, INEP17
{
    public string Symbol => ""TKN"";
    public byte Decimals => 8;

    [Safe]
    public static BigInteger TotalSupply => 0;

    [Safe]
    public static BigInteger BalanceOf(UInt160 owner) => 0;

    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null) => true;
}";

        var manifest = CompileSingleContract(source).CreateManifest();

        CollectionAssert.Contains(manifest.SupportedStandards, "NEP-17");
    }

    [TestMethod]
    public void Nep11Interface_ContributesSupportedStandardToManifest()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class Contract : SmartContract, INEP11
{
    public string Symbol => ""NFT"";
    public byte Decimals => 0;

    [Safe]
    public static BigInteger TotalSupply => 0;

    [Safe]
    public static BigInteger BalanceOf(UInt160 owner) => 0;

    [Safe]
    public static UInt160 OwnerOf(ByteString tokenId) => UInt160.Zero;

    public Map<string, object> Properties(ByteString tokenId) => new();

    [Safe]
    public static Iterator Tokens() => Storage.Find(Storage.CurrentContext, new byte[] { });

    [Safe]
    public static Iterator TokensOf(UInt160 owner) => Storage.Find(Storage.CurrentContext, owner);

    public static bool Transfer(UInt160 to, ByteString tokenId, object? data = null) => true;
}";

        var manifest = CompileSingleContract(source).CreateManifest();

        CollectionAssert.Contains(manifest.SupportedStandards, "NEP-11");
    }

    private static CompilationContext CompileSingleContract(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            var context = contexts[0];
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
            return context;
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
