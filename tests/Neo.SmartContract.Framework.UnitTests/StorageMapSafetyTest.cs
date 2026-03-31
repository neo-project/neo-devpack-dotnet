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
public class StorageMapSafetyTest
{
    [TestMethod]
    public void StorageMap_RejectsNegativeIncrease_And_UnderflowingDecrease()
    {
        var (nef, manifest) = CompileStorageMapContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<StorageMapSafetyContract>(nef, manifest);

        var key = new byte[] { 0x01 };

        Assert.ThrowsException<TestException>(() => contract.StorageIncrease(key, -1));
        var underflow = Assert.ThrowsException<TestException>(() => contract.StorageDecrease(key, 1));
        StringAssert.Contains(underflow.Message, "result would be negative");
    }

    [TestMethod]
    public void LocalStorageMap_RejectsNegativeIncrease_And_UnderflowingDecrease()
    {
        var (nef, manifest) = CompileStorageMapContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<StorageMapSafetyContract>(nef, manifest);

        var key = new byte[] { 0x02 };

        Assert.ThrowsException<TestException>(() => contract.LocalIncrease(key, -1));
        var underflow = Assert.ThrowsException<TestException>(() => contract.LocalDecrease(key, 1));
        StringAssert.Contains(underflow.Message, "result would be negative");
    }

    private static (NefFile nef, ContractManifest manifest) CompileStorageMapContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    [DisplayName(""storageIncrease"")]
    public static BigInteger StorageIncrease(byte[] key, BigInteger amount)
    {
        var map = new StorageMap(Storage.CurrentContext, (byte)0xA1);
        return map.Increase(key, amount);
    }

    [DisplayName(""storageDecrease"")]
    public static BigInteger StorageDecrease(byte[] key, BigInteger amount)
    {
        var map = new StorageMap(Storage.CurrentContext, (byte)0xA2);
        return map.Decrease(key, amount);
    }

    [DisplayName(""localIncrease"")]
    public static BigInteger LocalIncrease(byte[] key, BigInteger amount)
    {
        var map = new LocalStorageMap(new byte[] { 0xB1 });
        return map.Increase(key, amount);
    }

    [DisplayName(""localDecrease"")]
    public static BigInteger LocalDecrease(byte[] key, BigInteger amount)
    {
        var map = new LocalStorageMap(new byte[] { 0xB2 });
        return map.Decrease(key, amount);
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

    public abstract class StorageMapSafetyContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("storageIncrease")]
        public abstract BigInteger? StorageIncrease(byte[] key, BigInteger amount);

        [DisplayName("storageDecrease")]
        public abstract BigInteger? StorageDecrease(byte[] key, BigInteger amount);

        [DisplayName("localIncrease")]
        public abstract BigInteger? LocalIncrease(byte[] key, BigInteger amount);

        [DisplayName("localDecrease")]
        public abstract BigInteger? LocalDecrease(byte[] key, BigInteger amount);
    }
}
