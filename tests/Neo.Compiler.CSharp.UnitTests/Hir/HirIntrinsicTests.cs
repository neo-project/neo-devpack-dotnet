using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirIntrinsicTests
{
    [TestMethod]
    public void StoragePut_Lowers_To_Intrinsic()
    {
        const string source = @"
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public sealed class C
{
    public static void Put(ByteString key, ByteString value)
    {
        Storage.Put(Storage.CurrentContext, key, value);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var intrinsic = function.Blocks.SelectMany(b => b.Instructions).OfType<HirIntrinsicCall>().Single();

        Assert.AreEqual("Storage", intrinsic.Category);
        Assert.AreEqual("Put", intrinsic.Name);
        Assert.AreEqual(HirEffect.StorageWrite, intrinsic.Metadata.Effect);
        Assert.AreEqual(3, intrinsic.Arguments.Count, "Storage.Put should receive context, key, and value arguments.");
    }

    [TestMethod]
    public void RuntimeNotify_Is_Emitted_For_EventInvocation()
    {
        const string source = @"
using System;
using Neo;
using Neo.SmartContract.Framework;

public sealed class C : SmartContract
{
    public static event Action<ByteString, ByteString>? OnTransfer;

    public static void Raise(ByteString from, ByteString to)
    {
        OnTransfer?.Invoke(from, to);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var intrinsic = function.Blocks.SelectMany(b => b.Instructions).OfType<HirIntrinsicCall>().Single();

        Assert.AreEqual("Runtime", intrinsic.Category);
        Assert.AreEqual("Notify", intrinsic.Name);
        Assert.AreEqual(2, intrinsic.Arguments.Count, "Notify expects event name and payload.");
    }

    [TestMethod]
    public void CryptoSha256_Maps_To_PureIntrinsic()
    {
        const string source = @"
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public sealed class C
{
    public static ByteString Hash(ByteString value)
    {
        return Crypto.Sha256(value);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var intrinsic = function.Blocks.SelectMany(b => b.Instructions).OfType<HirIntrinsicCall>().Single();

        Assert.AreEqual("Crypto", intrinsic.Category);
        Assert.AreEqual("Sha256", intrinsic.Name);
        Assert.AreEqual(HirEffect.Crypto, intrinsic.Metadata.Effect);
        Assert.IsFalse(intrinsic.Metadata.RequiresMemoryToken, "Pure intrinsic should not require memory token.");
    }

    [TestMethod]
    public void ContractCall_Uses_InteropIntrinsic()
    {
        const string source = @"
using System;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public sealed class C : SmartContract
{
    public static object? Call(ByteString hash, string method, params object[] args)
    {
        return Contract.Call(hash, method, CallFlags.All, args);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var intrinsic = function.Blocks.SelectMany(b => b.Instructions).OfType<HirIntrinsicCall>().Single();

        Assert.AreEqual("Contract", intrinsic.Category);
        Assert.AreEqual("Call", intrinsic.Name);
        Assert.AreEqual(HirEffect.Interop, intrinsic.Metadata.Effect);
        Assert.AreEqual(4, intrinsic.Metadata.ParameterTypes.Count, "Contract.Call signature should surface four arguments.");
    }
}
