using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.Compiler.SecurityAnalyzer;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer;

[TestClass]
public class TokenCallbackAuthorizationAnalyzerTests
{
    [TestMethod]
    public void TokenCallbacks_WritingStorageWithoutCallingScriptHashValidation_AreFlagged()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class Contract : SmartContract
{
    public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, amount);
    }

    public static void onNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x02 }, amount);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var result = TokenCallbackAuthorizationAnalyzer.AnalyzeTokenCallbacks(
            context.CreateExecutable(),
            context.CreateManifest(),
            null);

        CollectionAssert.AreEquivalent(
            new[] { "onNEP17Payment", "onNEP11Payment" },
            result.vulnerableMethodNames.ToArray());
    }

    [TestMethod]
    public void TokenCallbacks_WithCallingScriptHashValidation_AreNotFlagged()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

public class Contract : SmartContract
{
    public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        if (Runtime.CallingScriptHash != NEO.Hash) throw new Exception();
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, amount);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var result = TokenCallbackAuthorizationAnalyzer.AnalyzeTokenCallbacks(
            context.CreateExecutable(),
            context.CreateManifest(),
            null);

        Assert.AreEqual(0, result.vulnerableMethodNames.Count);
    }

    [TestMethod]
    public void TokenCallbacks_FetchedAndDroppedCallingScriptHash_AreStillFlagged()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class Contract : SmartContract
{
    public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        _ = Runtime.CallingScriptHash;
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, amount);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var result = TokenCallbackAuthorizationAnalyzer.AnalyzeTokenCallbacks(
            context.CreateExecutable(),
            context.CreateManifest(),
            null);

        CollectionAssert.AreEquivalent(new[] { "onNEP17Payment" }, result.vulnerableMethodNames.ToArray());
    }

    [TestMethod]
    public void TokenCallbacks_RequireExactCallbackCasing()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    [DisplayName(""OnNEP17Payment"")]
    public static void Payment(UInt160 from, BigInteger amount, object data)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, amount);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var result = TokenCallbackAuthorizationAnalyzer.AnalyzeTokenCallbacks(
            context.CreateExecutable(),
            context.CreateManifest(),
            null);

        Assert.AreEqual(0, result.vulnerableMethodNames.Count);
    }

    [TestMethod]
    public void SecurityAnalyzer_AnalyzeWithPrint_IncludesTokenCallbackWarnings()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class Contract : SmartContract
{
    public static void onNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, amount);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        using var stdout = new StringWriter();
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetOut(stdout);
            Neo.Compiler.SecurityAnalyzer.SecurityAnalyzer.AnalyzeWithPrint(
                context.CreateExecutable(),
                context.CreateManifest(),
                null);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        StringAssert.Contains(stdout.ToString(), "onNEP17Payment");
        StringAssert.Contains(stdout.ToString(), "Runtime.CallingScriptHash");
    }
}
