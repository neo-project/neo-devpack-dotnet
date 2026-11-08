// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StorageMap.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    /// <summary>
    /// Covers contracts that construct <see cref="Neo.SmartContract.Framework.Services.StorageMap"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>StorageMap</c> constructors are declared <c>extern</c> and describe their
    /// lowering with <c>[OpCode]</c> attributes, which the compiler expands through
    /// <c>ConvertExtern</c>. A constructor that is never expanded leaves its local slot
    /// unassigned, so these tests assert the value that is written and read back rather
    /// than only asserting that compilation succeeds.
    /// </para>
    /// <para>
    /// The framework reaches the compiler in two shapes and they behave differently: a
    /// project reference becomes a <c>CompilationReference</c>, which still exposes the
    /// syntax of every member, while a NuGet package is a compiled assembly with no syntax
    /// at all. Both shapes are covered, because a contract built against the published
    /// package has to work the same way as one built inside the repository.
    /// </para>
    /// </remarks>
    [TestClass]
    public class UnitTest_StorageMap
    {
        private const string Source = """
            using System.Numerics;
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Services;

            public class Contract_StorageMap : SmartContract
            {
                public static int PutAndGet(int value)
                {
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x01 });
                    map.Put(new byte[] { 0x02 }, value);
                    var stored = map.Get(new byte[] { 0x02 });
                    return stored == null ? -1 : (int)(BigInteger)stored;
                }

                public static int GetMissingKey(int fallback)
                {
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x03 });
                    var stored = map.Get(new byte[] { 0x99 });
                    return stored == null ? fallback : (int)(BigInteger)stored;
                }

                public static int PutAndGetSeparateMaps(int first, int second)
                {
                    var a = new StorageMap(Storage.CurrentContext, new byte[] { 0x04 });
                    var b = new StorageMap(Storage.CurrentContext, new byte[] { 0x05 });
                    a.Put(new byte[] { 0x01 }, first);
                    b.Put(new byte[] { 0x01 }, second);

                    var storedA = a.Get(new byte[] { 0x01 });
                    var storedB = b.Get(new byte[] { 0x01 });
                    if (storedA == null || storedB == null) return -1;
                    return (int)(BigInteger)storedA + (int)(BigInteger)storedB;
                }

                public static int DeleteRemovesKey(int value)
                {
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x06 });
                    map.Put(new byte[] { 0x01 }, value);
                    map.Delete(new byte[] { 0x01 });
                    var stored = map.Get(new byte[] { 0x01 });
                    return stored == null ? 0 : (int)(BigInteger)stored;
                }
            }
            """;

        /// <summary>
        /// Compiles the sample through a project reference to the framework and runs it.
        /// </summary>
        [TestMethod]
        public void StorageMap_WithProjectReference_Works()
        {
            Neo.Compiler.CompilationContext context = TestHelper.CompileSingleContract(Source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

            AssertBehaviour(Deploy(context));
        }

        /// <summary>
        /// Compiles the sample against the framework as a compiled assembly and runs it.
        /// </summary>
        /// <remarks>
        /// This is the shape a contract sees when the framework comes from the NuGet
        /// package rather than from a project reference. The assembly carries no syntax, so
        /// the lowering has to work from metadata alone.
        /// </remarks>
        [TestMethod]
        public void StorageMap_WithAssemblyReference_Works()
        {
            Neo.Compiler.CompilationContext context = CompileAgainstFrameworkAssembly(Source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

            AssertBehaviour(Deploy(context));
        }

        private static void AssertBehaviour(ContractStorageMap contract)
        {
            Assert.AreEqual(new BigInteger(42), contract.PutAndGet(42), "a stored value must be read back");
            Assert.AreEqual(new BigInteger(-7), contract.PutAndGet(-7), "negative values must round-trip");
            Assert.AreEqual(BigInteger.Zero, contract.PutAndGet(0), "zero must round-trip");

            Assert.AreEqual(new BigInteger(1234), contract.GetMissingKey(1234), "a missing key must read as null");
            Assert.AreEqual(new BigInteger(30), contract.PutAndGetSeparateMaps(10, 20), "distinct prefixes must not collide");
            Assert.AreEqual(BigInteger.Zero, contract.DeleteRemovesKey(99), "a deleted key must read as null");
        }

        private static ContractStorageMap Deploy(Neo.Compiler.CompilationContext context)
        {
            var engine = new TestEngine(true);
            return engine.Deploy<ContractStorageMap>(context.CreateExecutable(), context.CreateManifest());
        }

        /// <summary>
        /// Compiles a source against the framework assembly rather than its project.
        /// </summary>
        private static Neo.Compiler.CompilationContext CompileAgainstFrameworkAssembly(string source)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
            File.WriteAllText(tempFile, source);

            try
            {
                var references = new List<MetadataReference>();

                if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa)
                {
                    foreach (string path in tpa.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!File.Exists(path))
                        {
                            continue;
                        }

                        // The framework defines its own UInt160 and ByteString, so the Neo
                        // core assemblies have to stay out of the reference set; otherwise
                        // "using Neo;" would resolve those names to the core types.
                        string fileName = Path.GetFileName(path);
                        if (fileName.StartsWith("Neo.", StringComparison.OrdinalIgnoreCase) &&
                            !fileName.StartsWith("Neo.SmartContract.", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        references.Add(MetadataReference.CreateFromFile(path));
                    }
                }

                references.Add(MetadataReference.CreateFromFile(
                    typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly.Location));

                var engine = new CompilationEngine(TestHelper.CreateDefaultOptions());
                return engine.Compile([tempFile], references).First();
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        public abstract class ContractStorageMap(SmartContractInitialize initialize)
            : Neo.SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("putAndGet")]
            public abstract BigInteger? PutAndGet(BigInteger value);

            [DisplayName("getMissingKey")]
            public abstract BigInteger? GetMissingKey(BigInteger fallback);

            [DisplayName("putAndGetSeparateMaps")]
            public abstract BigInteger? PutAndGetSeparateMaps(BigInteger first, BigInteger second);

            [DisplayName("deleteRemovesKey")]
            public abstract BigInteger? DeleteRemovesKey(BigInteger value);
        }
    }
}
