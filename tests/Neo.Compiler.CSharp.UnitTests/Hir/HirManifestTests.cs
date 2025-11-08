// Copyright (C) 2015-2025 The Neo Project.
//
// HirManifestTests.cs is part of the neo-devpack-dotnet test suite and is
// distributed under the MIT license; see LICENSE for details.

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Manifest;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirManifestTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Manifest_Uses_HirAttributes_For_Name_And_Safety()
    {
        const string source = """
using Neo;
using Neo;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class HirManifestContract : SmartContract.Framework.SmartContract
    {
        [Safe]
        [DisplayName("balanceOf")]
        public static int BalanceOf(int owner)
        {
            return owner;
        }

        [NoReentrant]
        [DisplayName("setOwner")]
        public static void SetOwner(int newOwner)
        {
            return;
        }

        [NoReentrantMethod]
        public static void Update()
        {
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Inline(int value)
        {
            return value;
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);
        TestContext?.WriteLine($"[Manifest_Uses_HirAttributes_For_Name_And_Safety] Temp file written to {tempFile}");

        try
        {
            var testContext = TestContext;
            var engine = new CompilationEngine(new CompilationOptions
            {
                EnableHir = true,
                Nullable = NullableContextOptions.Enable,
                Logger = message => testContext?.WriteLine(message)
            });
            TestContext?.WriteLine("[Manifest_Uses_HirAttributes_For_Name_And_Safety] Compilation engine created.");

            var contexts = engine.CompileSources(tempFile);
            TestContext?.WriteLine($"[Manifest_Uses_HirAttributes_For_Name_And_Safety] CompileSources returned {contexts.Count} context(s).");
            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var manifest = contexts[0].CreateManifest();
            TestContext?.WriteLine("[Manifest_Uses_HirAttributes_For_Name_And_Safety] Manifest created.");
            var balanceOf = manifest.Abi.Methods.Single(m => m.Name == "balanceOf");
            Assert.IsTrue(balanceOf.Safe, "balanceOf should be marked safe via HIR metadata.");

            var setOwner = manifest.Abi.Methods.Single(m => m.Name == "setOwner");
            Assert.IsFalse(setOwner.Safe, "setOwner should not be considered safe.");

            Assert.IsNotNull(manifest.Extra, "Manifest extras should exist when reentrancy guards are emitted.");
            var guards = manifest.Extra!["reentrancyGuards"] as Neo.Json.JObject;
            Assert.IsNotNull(guards, "Reentrancy guards should be recorded in manifest extra.");
            Assert.IsTrue(guards!.ContainsProperty("setOwner"), "Manifest guards should include setOwner.");
            var guardEntry = guards["setOwner"] as Neo.Json.JObject;
            Assert.IsNotNull(guardEntry, "Guard metadata should expose prefix and key.");
            Assert.AreEqual(0xFF, guardEntry!["prefix"]!.GetInt32(), "Guard prefix should match default.");
            Assert.AreEqual("noReentrant", guardEntry["key"]!.GetString(), "Guard key should match default.");

            Assert.IsTrue(guards.ContainsProperty("update"), "Manifest guards should include update.");
            var methodGuard = guards["update"] as Neo.Json.JObject;
            Assert.IsNotNull(methodGuard, "NoReentrantMethod guard should expose prefix and key.");
            Assert.AreEqual("Update", methodGuard!["key"]!.GetString(), "NoReentrantMethod default key should reflect method name.");

            var inlineHints = manifest.Extra!["inlineHints"] as Neo.Json.JObject;
            Assert.IsNotNull(inlineHints, "Inline hints map should exist in manifest extra.");
            Assert.IsTrue(inlineHints!.ContainsProperty("inline"), "Inline hints should include inline method alias.");

            // Ensure canonical C# names are not leaked when display names are provided.
            CollectionAssert.DoesNotContain(
                manifest.Abi.Methods.Select(m => m.Name).ToArray(),
                "BalanceOf",
                "Manifest method names should respect the HIR alias when available.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Manifest_Exports_Public_Methods_And_Events()
    {
        const string source = """
using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class InterfaceContract : SmartContract.Framework.SmartContract
    {
        public static event Action<ByteString, ByteString, BigInteger>? OnTransfer;

        public static void Ping()
        {
        }

        public static void Raise(ByteString from, ByteString to, BigInteger amount)
        {
            OnTransfer?.Invoke(from, to, amount);
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);
        TestContext?.WriteLine($"[Manifest_Exports_Public_Methods_And_Events] Temp file written to {tempFile}");

        try
        {
            var testContext = TestContext;
            var engine = new CompilationEngine(new CompilationOptions
            {
                EnableHir = true,
                Nullable = NullableContextOptions.Enable,
                Logger = message => testContext?.WriteLine(message)
            });
            TestContext?.WriteLine("[Manifest_Exports_Public_Methods_And_Events] Compilation engine created.");

            var contexts = engine.CompileSources(tempFile);
            TestContext?.WriteLine($"[Manifest_Exports_Public_Methods_And_Events] CompileSources returned {contexts.Count} context(s).");
            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var manifest = contexts[0].CreateManifest();
            TestContext?.WriteLine("[Manifest_Exports_Public_Methods_And_Events] Manifest created.");
            var methodNames = manifest.Abi.Methods.Select(m => m.Name).OrderBy(n => n).ToArray();
            CollectionAssert.AreEqual(new[] { "ping", "raise" }, methodNames, "Only public contract methods should appear in the manifest ABI.");

            var eventNames = manifest.Abi.Events.Select(e => e.Name).OrderBy(n => n).ToArray();
            CollectionAssert.AreEqual(new[] { "OnTransfer" }, eventNames, "Smart-contract events should be surfaced from C# event declarations.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Manifest_Reflects_Contract_Metadata_Attributes()
    {
        const string source = """
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0xd0c5b0a68ee37c9f93d8af57c3bd9fd5b3c2f3f2")]
    [SupportedStandard("NEP-17", "NEP-51")]
    [ContractPermission("*", "*")]
    [ContractPermission("0xfe924b7cfe89ddd271abaf7210a80a7e11178758", "balanceOf")]
    [Trust("0xfe924b7cfe89ddd271abaf7210a80a7e11178758")]
    [ContractSource("https://example.org/contracts/decorated")]
    public class DecoratedContract : SmartContract.Framework.SmartContract
    {
        [DisplayName("balanceOf")]
        [Safe]
        public static int BalanceOf(UInt160 owner) => 0;

        public static void Update(ByteString key, ByteString value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);
        TestContext?.WriteLine($"[Manifest_Reflects_Contract_Metadata_Attributes] Temp file written to {tempFile}");

        try
        {
            var testContext = TestContext;
            var engine = new CompilationEngine(new CompilationOptions
            {
                EnableHir = true,
                Nullable = NullableContextOptions.Enable,
                Logger = message => testContext?.WriteLine(message)
            });
            TestContext?.WriteLine("[Manifest_Reflects_Contract_Metadata_Attributes] Compilation engine created.");

            var contexts = engine.CompileSources(tempFile);
            TestContext?.WriteLine($"[Manifest_Reflects_Contract_Metadata_Attributes] CompileSources returned {contexts.Count} context(s).");
            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var manifest = contexts[0].CreateManifest();
            TestContext?.WriteLine("[Manifest_Reflects_Contract_Metadata_Attributes] Manifest created.");
            Assert.AreEqual("DecoratedContract", manifest.Name, "Contract attribute should keep class name when no display name provided.");

            CollectionAssert.AreEquivalent(new[] { "NEP-17", "NEP-51" }, manifest.SupportedStandards.ToArray());

            Assert.AreEqual(2, manifest.Permissions.Length, "Permissions should originate from attributes.");
            Assert.IsTrue(manifest.Permissions.Any(p => p.Contract.IsWildcard), "Wildcard permission should exist.");
            var specificPermission = manifest.Permissions.Single(p => p.Contract.IsHash &&
                p.Contract.Hash.ToString().Equals("0xfe924b7cfe89ddd271abaf7210a80a7e11178758", StringComparison.OrdinalIgnoreCase));
            CollectionAssert.AreEquivalent(new[] { "balanceOf" }, specificPermission.Methods.ToArray());

            CollectionAssert.AreEquivalent(
                new[] { "0xfe924b7cfe89ddd271abaf7210a80a7e11178758" },
                manifest.Trusts.ToArray(),
                "Trust attribute should propagate to manifest.");

            var sourceUrl = manifest.Extra?["source"]?.GetString();
            Assert.AreEqual("https://example.org/contracts/decorated", sourceUrl, "ContractSource should persist into manifest extra.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
