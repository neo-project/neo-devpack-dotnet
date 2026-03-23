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

        Assert.AreEqual(new BigInteger(9), contract.Add(4, 5));
        Assert.AreEqual(new BigInteger(7), contract.Sub(10, 3));
        Assert.AreEqual(new BigInteger(42), contract.Mul(6, 7));
        Assert.AreEqual(new BigInteger(5), contract.Div(20, 4));
        Assert.AreEqual(new BigInteger(2), contract.Mod(20, 6));

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void SafeMath_Rejects_NegativeValues_And_InvalidOperations()
    {
        var (nef, manifest, debugInfo) = CompileSafeMathContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SafeMathContractProxy>(nef, manifest);

        var addNegative = Assert.ThrowsException<TestException>(() => contract.Add(-1, 1));
        StringAssert.Contains(addNegative.InnerException?.Message ?? addNegative.Message, "negative values are not supported");

        var subUnderflow = Assert.ThrowsException<TestException>(() => contract.Sub(1, 2));
        StringAssert.Contains(subUnderflow.InnerException?.Message ?? subUnderflow.Message, "result would be negative");

        var mulNegative = Assert.ThrowsException<TestException>(() => contract.Mul(-1, 2));
        StringAssert.Contains(mulNegative.InnerException?.Message ?? mulNegative.Message, "negative values are not supported");

        var divByZero = Assert.ThrowsException<TestException>(() => contract.Div(1, 0));
        StringAssert.Contains(divByZero.InnerException?.Message ?? divByZero.Message, "division by zero");

        var modByZero = Assert.ThrowsException<TestException>(() => contract.Mod(1, 0));
        StringAssert.Contains(modByZero.InnerException?.Message ?? modByZero.Message, "modulo by zero");

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void SafeMath_Rejects_NegativeRightOperands_WithExpectedMessages()
    {
        var (nef, manifest, debugInfo) = CompileSafeMathContract();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SafeMathContractProxy>(nef, manifest);

        var addNegative = Assert.ThrowsException<TestException>(() => contract.Add(1, -1));
        StringAssert.Contains(addNegative.InnerException?.Message ?? addNegative.Message, "negative values are not supported");

        var mulNegative = Assert.ThrowsException<TestException>(() => contract.Mul(2, -1));
        StringAssert.Contains(mulNegative.InnerException?.Message ?? mulNegative.Message, "negative values are not supported");

        var divNegative = Assert.ThrowsException<TestException>(() => contract.Div(2, -1));
        StringAssert.Contains(divNegative.InnerException?.Message ?? divNegative.Message, "negative values are not supported");

        var modNegative = Assert.ThrowsException<TestException>(() => contract.Mod(2, -1));
        StringAssert.Contains(modNegative.InnerException?.Message ?? modNegative.Message, "negative values are not supported");

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileSafeMathContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : SmartContract
{
    public static BigInteger Add(BigInteger left, BigInteger right)
    {
        return SafeMath.Add(left, right);
    }

    public static BigInteger Sub(BigInteger left, BigInteger right)
    {
        return SafeMath.Sub(left, right);
    }

    public static BigInteger Mul(BigInteger left, BigInteger right)
    {
        return SafeMath.Mul(left, right);
    }

    public static BigInteger Div(BigInteger left, BigInteger right)
    {
        return SafeMath.Div(left, right);
    }

    public static BigInteger Mod(BigInteger left, BigInteger right)
    {
        return SafeMath.Mod(left, right);
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
        [DisplayName("add")]
        public abstract BigInteger? Add(BigInteger? left, BigInteger? right);

        [DisplayName("sub")]
        public abstract BigInteger? Sub(BigInteger? left, BigInteger? right);

        [DisplayName("mul")]
        public abstract BigInteger? Mul(BigInteger? left, BigInteger? right);

        [DisplayName("div")]
        public abstract BigInteger? Div(BigInteger? left, BigInteger? right);

        [DisplayName("mod")]
        public abstract BigInteger? Mod(BigInteger? left, BigInteger? right);
    }
}
