// Copyright (C) 2015-2026 The Neo Project.
//
// RoyaltyNep11TokenTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class RoyaltyNep11TokenTest
{
    private static readonly Signer Alice = TestEngine.Alice;
    private static readonly Signer Bob = TestEngine.Bob;
    private static readonly Signer Charlie = TestEngine.Charlie;
    private static readonly UInt160 Gas = UInt160.Parse("0xd2a4cff31913016155e38e474a2c06d08be276cf");

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static RoyaltyProxy Deploy(TestEngine engine)
        => engine.Deploy<RoyaltyProxy>(_compiled.nef, _compiled.manifest, null);

    private static void Merge(RoyaltyProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    private static (RoyaltyProxy c, byte[] id) DeployAndMint()
    {
        var c = Deploy(CreateEngine());
        var id = c.MintToken(Alice.Account, "art");
        return (c, id!);
    }

    [TestMethod]
    public void DefaultRoyalty_ComputesAmountFromBasisPoints()
    {
        var (c, id) = DeployAndMint();
        c.SetDefault(Bob.Account, 500); // 5%

        Assert.AreEqual(Bob.Account, c.RoyaltyRecipient(id, Gas, 10_000));
        Assert.AreEqual(new BigInteger(500), c.RoyaltyAmount(id, Gas, 10_000));
        // Integer division floors.
        Assert.AreEqual(new BigInteger(5), c.RoyaltyAmount(id, Gas, 100));
        Merge(c);
    }

    [TestMethod]
    public void PerTokenRoyalty_OverridesDefault()
    {
        var (c, id) = DeployAndMint();
        c.SetDefault(Bob.Account, 500);
        c.SetToken(id, Charlie.Account, 1000); // 10%

        Assert.AreEqual(Charlie.Account, c.RoyaltyRecipient(id, Gas, 10_000));
        Assert.AreEqual(new BigInteger(1000), c.RoyaltyAmount(id, Gas, 10_000));
        Merge(c);
    }

    [TestMethod]
    public void NoRoyalty_ReturnsEmpty()
    {
        var (c, id) = DeployAndMint();
        Assert.AreEqual(BigInteger.Zero, c.RoyaltyCount(id, Gas, 10_000));
        Merge(c);
    }

    [TestMethod]
    public void RoyaltyInfo_NonexistentToken_Reverts()
    {
        var c = Deploy(CreateEngine());
        Assert.ThrowsException<TestException>(() => c.RoyaltyCount(new byte[] { 0xde, 0xad }, Gas, 10_000));
        Merge(c);
    }

    [TestMethod]
    public void SetRoyalty_BasisPointsAboveDenominator_Reverts()
    {
        var (c, _) = DeployAndMint();
        Assert.ThrowsException<TestException>(() => c.SetDefault(Bob.Account, 10_001));
        Merge(c);
    }

    [TestMethod]
    public void SetRoyalty_ZeroRecipient_Reverts()
    {
        var (c, _) = DeployAndMint();
        Assert.ThrowsException<TestException>(() => c.SetDefault(UInt160.Zero, 500));
        Merge(c);
    }

    [TestMethod]
    public void DeleteTokenRoyalty_FallsBackToDefault()
    {
        var (c, id) = DeployAndMint();
        c.SetDefault(Bob.Account, 500);
        c.SetToken(id, Charlie.Account, 1000);
        c.DelToken(id);

        Assert.AreEqual(Bob.Account, c.RoyaltyRecipient(id, Gas, 10_000));
        Assert.AreEqual(new BigInteger(500), c.RoyaltyAmount(id, Gas, 10_000));
        Merge(c);
    }

    [TestMethod]
    public void DeleteDefaultRoyalty_RemovesRoyalty()
    {
        var (c, id) = DeployAndMint();
        c.SetDefault(Bob.Account, 500);
        c.DelDefault();
        Assert.AreEqual(BigInteger.Zero, c.RoyaltyCount(id, Gas, 10_000));
        Merge(c);
    }

    [TestMethod]
    public void Manifest_Nep24Conformance()
    {
        var method = _compiled.manifest.Abi.GetMethod("royaltyInfo", 3);
        Assert.IsNotNull(method, "royaltyInfo must be present in the ABI");
        Assert.IsTrue(method.Safe, "royaltyInfo must be Safe");
        Assert.AreEqual(ContractParameterType.Array, method.ReturnType);
        Assert.AreEqual(ContractParameterType.ByteArray, method.Parameters[0].Type);
        Assert.AreEqual(ContractParameterType.Hash160, method.Parameters[1].Type);
        Assert.AreEqual(ContractParameterType.Integer, method.Parameters[2].Type);

        // The contract declares NEP-11 and NEP-24; compilation succeeding proves it satisfies both
        // standards' compliance checks.
        Assert.IsTrue(_compiled.manifest.SupportedStandards.Contains("NEP-24"));
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System.Numerics;

public class Contract : RoyaltyNep11Token<Nep11TokenState>
{
    public override string Symbol { [Safe] get => ""ART""; }

    public static ByteString MintToken(UInt160 owner, string name)
    {
        ByteString id = NewTokenId();
        Mint(id, new Nep11TokenState { Owner = owner, Name = name });
        return id;
    }

    public static void SetDefault(UInt160 recipient, BigInteger basisPoints) => SetDefaultRoyalty(recipient, basisPoints);
    public static void SetToken(ByteString tokenId, UInt160 recipient, BigInteger basisPoints) => SetTokenRoyalty(tokenId, recipient, basisPoints);
    public static void DelDefault() => DeleteDefaultRoyalty();
    public static void DelToken(ByteString tokenId) => DeleteTokenRoyalty(tokenId);

    public static UInt160 RoyaltyRecipient(ByteString tokenId, UInt160 token, BigInteger price)
    {
        Map<string, object>[] info = RoyaltyInfo(tokenId, token, price);
        if (info.Length == 0) return UInt160.Zero;
        return (UInt160)info[0][""royaltyRecipient""];
    }

    public static BigInteger RoyaltyAmount(ByteString tokenId, UInt160 token, BigInteger price)
    {
        Map<string, object>[] info = RoyaltyInfo(tokenId, token, price);
        if (info.Length == 0) return -1;
        return (BigInteger)info[0][""royaltyAmount""];
    }

    public static int RoyaltyCount(ByteString tokenId, UInt160 token, BigInteger price)
    {
        return RoyaltyInfo(tokenId, token, price).Length;
    }
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

            var contexts = engine.CompileSources(new CompilationSourceReferences { Projects = new[] { frameworkProject } }, tempFile);
            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            var context = contexts[0];
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var (nef, manifest, debugInfoJson) = context.CreateResults(repoRoot);
            return (nef, manifest, NeoDebugInfo.FromDebugInfoJson(debugInfoJson));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    public abstract class RoyaltyProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("mintToken")]
        public abstract byte[]? MintToken(UInt160 owner, string name);

        [DisplayName("setDefault")]
        public abstract void SetDefault(UInt160 recipient, BigInteger basisPoints);

        [DisplayName("setToken")]
        public abstract void SetToken(byte[] tokenId, UInt160 recipient, BigInteger basisPoints);

        [DisplayName("delDefault")]
        public abstract void DelDefault();

        [DisplayName("delToken")]
        public abstract void DelToken(byte[] tokenId);

        [DisplayName("royaltyRecipient")]
        public abstract UInt160? RoyaltyRecipient(byte[] tokenId, UInt160 token, BigInteger price);

        [DisplayName("royaltyAmount")]
        public abstract BigInteger RoyaltyAmount(byte[] tokenId, UInt160 token, BigInteger price);

        [DisplayName("royaltyCount")]
        public abstract BigInteger RoyaltyCount(byte[] tokenId, UInt160 token, BigInteger price);
    }
}
