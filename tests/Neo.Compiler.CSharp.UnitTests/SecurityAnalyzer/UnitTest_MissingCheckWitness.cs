// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_MissingCheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class MissingCheckWitnessTests : DebugAndTestBase<Contract_MissingCheckWitness>
    {
        [TestMethod]
        public void Test_MissingCheckWitness()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            // UnsafeUpdate writes storage without CheckWitness - should be flagged
            Assert.IsTrue(result.vulnerableMethodNames.Contains("unsafeUpdate"));
            // UnsafeLocalUpdate writes local storage without CheckWitness - should be flagged
            Assert.IsTrue(result.vulnerableMethodNames.Contains("unsafeLocalUpdate"));
            // SafeUpdate has CheckWitness - should NOT be flagged
            Assert.IsFalse(result.vulnerableMethodNames.Contains("safeUpdate"));
            // SafeUpdateViaHelper delegates CheckWitness to helper - should NOT be flagged
            Assert.IsFalse(result.vulnerableMethodNames.Contains("safeUpdateViaHelper"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_WarningInfo()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SECURITY]"));
            Assert.IsTrue(warning.Contains("unsafeUpdate"));
            Assert.IsTrue(warning.Contains("unsafeLocalUpdate"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_DoesNotSkip_UnderscorePrefixedPublicAbiMethods()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""_admin_transfer"")]
    public static void AdminTransfer()
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }
}";

            var context = CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);

            Assert.IsTrue(result.vulnerableMethodNames.Contains("_admin_transfer"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_Skips_Deploy_And_Initialize_Callbacks()
        {
            const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static void _deploy(object data, bool update)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
    }

    public static void _initialize()
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x02 }, 1);
    }
}";

            var context = CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);

            Assert.IsFalse(result.vulnerableMethodNames.Contains("_deploy"));
            Assert.IsFalse(result.vulnerableMethodNames.Contains("_initialize"));
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
                    Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
                    SkipRestoreIfAssetsPresent = true
                };

                var engine = new CompilationEngine(options);
                var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
                var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

                var contexts = engine.CompileSources(new CompilationSourceReferences
                {
                    Projects = new[] { frameworkProject }
                }, tempFile);

                Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
                return contexts[0];
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
