using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;

namespace Neo.Compiler.CSharp.UnitTests.Mir;

[TestClass]
public sealed class MirVerifierTests
{
    [TestMethod]
    public void Syscall_Resolved_From_Catalog_Is_Accepted()
    {
        var signature = new HirSignature(
            new[] { HirType.ByteStringType },
            HirType.VoidType,
            System.Array.Empty<HirAttribute>());
        var hirFunction = new HirFunction("test", signature);
        var mirFunction = new MirFunction(hirFunction);
        var entry = mirFunction.Entry;

        var scriptHash = new MirConstByteString(new byte[] { 0x01 });
        entry.Append(scriptHash);
        var methodName = new MirConstByteString(System.Text.Encoding.ASCII.GetBytes("Main"));
        entry.Append(methodName);
        var callFlags = new MirConstInt(BigInteger.Zero);
        entry.Append(callFlags);
        var argCount = new MirConstInt(BigInteger.Zero);
        entry.Append(argCount);
        var argArray = new MirArrayNew(argCount, MirType.TUnknown);
        entry.Append(argArray);

        var syscall = new MirSyscall(
            "Contract",
            "Call",
            new MirValue[] { scriptHash, methodName, callFlags, argArray },
            MirType.TUnknown,
            MirEffect.Interop);
        syscall.AttachMemoryToken(mirFunction.EntryToken);
        entry.Append(syscall);
        entry.Terminator = new MirReturn(null);

        var verifier = new MirVerifier();
        var errors = verifier.Verify(mirFunction);
        Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
    }

    [TestMethod]
    public void MirVerifier_Flags_Leave_With_Unknown_Scope()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.VoidType, Array.Empty<HirAttribute>());
        var hirFunction = new HirFunction("invalid_leave", signature);
        var mirFunction = new MirFunction(hirFunction);

        var tryBlock = mirFunction.CreateBlock("try_block");
        var finallyBlock = mirFunction.CreateBlock("finally_block");
        var mergeBlock = mirFunction.CreateBlock("merge_block");

        tryBlock.Terminator = new MirReturn(null);
        finallyBlock.Terminator = new MirReturn(null);
        mergeBlock.Terminator = new MirReturn(null);

        var orphanScope = new MirTry(tryBlock, finallyBlock, mergeBlock, Array.Empty<MirCatchHandler>());
        mirFunction.Entry.Terminator = new MirLeave(orphanScope, mergeBlock);

        var verifier = new MirVerifier();
        var errors = verifier.Verify(mirFunction);

        Assert.IsTrue(errors.Any(e => e.Contains("unknown try scope", StringComparison.OrdinalIgnoreCase)), "Verifier should report leaves that reference unknown try scopes.");
    }
}
