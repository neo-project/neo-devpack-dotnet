using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class Nep11MintUniquenessTest
{
    [TestMethod]
    public void Nep11Mint_RejectsDuplicateTokenIds()
    {
        var (nef, manifest) = CompileContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Nep11MintUniquenessContract>(nef, manifest);

        var firstOwner = TestEngine.GetNewSigner().Account;
        var secondOwner = TestEngine.GetNewSigner().Account;
        var tokenId = new byte[] { 0x42 };

        contract.Mint(tokenId, firstOwner);

        Assert.AreEqual(BigInteger.One, contract.TotalSupply);
        Assert.AreEqual(BigInteger.One, contract.BalanceOf(firstOwner));
        Assert.AreEqual(firstOwner, contract.OwnerOf(tokenId));

        Assert.ThrowsException<TestException>(() => contract.Mint(tokenId, secondOwner));

        Assert.AreEqual(BigInteger.One, contract.TotalSupply);
        Assert.AreEqual(BigInteger.One, contract.BalanceOf(firstOwner));
        Assert.AreEqual(BigInteger.Zero, contract.BalanceOf(secondOwner));
        Assert.AreEqual(firstOwner, contract.OwnerOf(tokenId));
    }

    private static (NefFile nef, ContractManifest manifest) CompileContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : Nep11Token<TestTokenState>
{
    public override string Symbol => ""TEST"";

    [DisplayName(""mint"")]
    public static void Mint(byte[] tokenId, UInt160 owner)
    {
        Nep11Token<TestTokenState>.Mint((ByteString)tokenId, new TestTokenState
        {
            Owner = owner,
            Name = ""token""
        });
    }
}

public class TestTokenState : Nep11TokenState
{
}";

        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            var context = contexts[0];
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            return (context.CreateExecutable(), context.CreateManifest());
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    public abstract class Nep11MintUniquenessContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

        [DisplayName("balanceOf")]
        public abstract BigInteger? BalanceOf(UInt160 owner);

        [DisplayName("ownerOf")]
        public abstract UInt160? OwnerOf(byte[] tokenId);

        [DisplayName("mint")]
        public abstract void Mint(byte[] tokenId, UInt160 owner);
    }
}
