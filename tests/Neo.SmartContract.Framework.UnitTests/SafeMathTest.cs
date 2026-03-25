using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
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
public class SafeMathTest
{
    [TestMethod]
    public void SafeMath_UnsignedArithmetic_ReturnsExpectedValues()
    {
        var (nef, manifest, debugInfo) = CompileSafeMathContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SafeMathContractProxy>(nef, manifest);

        Assert.AreEqual(new BigInteger(9), contract.UnsignedAdd(4, 5));
        Assert.AreEqual(new BigInteger(7), contract.UnsignedSub(10, 3));
        Assert.AreEqual(new BigInteger(42), contract.UnsignedMul(6, 7));
        Assert.AreEqual(new BigInteger(5), contract.UnsignedDiv(20, 4));
        Assert.AreEqual(new BigInteger(2), contract.UnsignedMod(20, 6));

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void SafeMath_Rejects_NegativeValues_And_InvalidOperations()
    {
        var (nef, manifest, debugInfo) = CompileSafeMathContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SafeMathContractProxy>(nef, manifest);

        var addNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedAdd(-1, 1));
        StringAssert.Contains(addNegative.InnerException?.Message ?? addNegative.Message, "negative values are not supported");

        var subUnderflow = Assert.ThrowsException<TestException>(() => contract.UnsignedSub(1, 2));
        StringAssert.Contains(subUnderflow.InnerException?.Message ?? subUnderflow.Message, "result would be negative");

        var mulNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedMul(-1, 2));
        StringAssert.Contains(mulNegative.InnerException?.Message ?? mulNegative.Message, "negative values are not supported");

        var divByZero = Assert.ThrowsException<TestException>(() => contract.UnsignedDiv(1, 0));
        StringAssert.Contains(divByZero.InnerException?.Message ?? divByZero.Message, "the divisor must be positive");

        var modByZero = Assert.ThrowsException<TestException>(() => contract.UnsignedMod(1, 0));
        StringAssert.Contains(modByZero.InnerException?.Message ?? modByZero.Message, "the divisor must be positive");

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void SafeMath_Rejects_NegativeRightOperands_WithExpectedMessages()
    {
        var (nef, manifest, debugInfo) = CompileSafeMathContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SafeMathContractProxy>(nef, manifest);

        var addNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedAdd(1, -1));
        StringAssert.Contains(addNegative.InnerException?.Message ?? addNegative.Message, "negative values are not supported");

        var mulNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedMul(2, -1));
        StringAssert.Contains(mulNegative.InnerException?.Message ?? mulNegative.Message, "negative values are not supported");

        var divNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedDiv(2, -1));
        StringAssert.Contains(divNegative.InnerException?.Message ?? divNegative.Message, "the divisor must be positive");

        var modNegative = Assert.ThrowsException<TestException>(() => contract.UnsignedMod(2, -1));
        StringAssert.Contains(modNegative.InnerException?.Message ?? modNegative.Message, "the divisor must be positive");

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileSafeMathContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : SmartContract
{
    public static BigInteger UnsignedAdd(BigInteger left, BigInteger right)
    {
        return SafeMath.UnsignedAdd(left, right);
    }

    public static BigInteger UnsignedSub(BigInteger left, BigInteger right)
    {
        return SafeMath.UnsignedSub(left, right);
    }

    public static BigInteger UnsignedMul(BigInteger left, BigInteger right)
    {
        return SafeMath.UnsignedMul(left, right);
    }

    public static BigInteger UnsignedDiv(BigInteger left, BigInteger right)
    {
        return SafeMath.UnsignedDiv(left, right);
    }

    public static BigInteger UnsignedMod(BigInteger left, BigInteger right)
    {
        return SafeMath.UnsignedMod(left, right);
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

            var (nef, manifest, debugInfoJson) = context.CreateResults(repoRoot);
            return (nef, manifest, NeoDebugInfo.FromDebugInfoJson(debugInfoJson));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    public abstract class SafeMathContractProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("unsignedAdd")]
        public abstract BigInteger? UnsignedAdd(BigInteger? left, BigInteger? right);

        [DisplayName("unsignedSub")]
        public abstract BigInteger? UnsignedSub(BigInteger? left, BigInteger? right);

        [DisplayName("unsignedMul")]
        public abstract BigInteger? UnsignedMul(BigInteger? left, BigInteger? right);

        [DisplayName("unsignedDiv")]
        public abstract BigInteger? UnsignedDiv(BigInteger? left, BigInteger? right);

        [DisplayName("unsignedMod")]
        public abstract BigInteger? UnsignedMod(BigInteger? left, BigInteger? right);
    }
}
