using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.HIR;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using Neo.Compiler.MIR;
using System.Numerics;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;

namespace Neo.Compiler.CSharp.IRTests.Pipeline;

[TestClass]
public sealed class PointerCallPipelineTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void TailPointerOperand_PreservesTailFlagsThroughPipeline()
    {
        var signature = new HirSignature(new[] { HirType.IntType }, HirType.IntType, System.Array.Empty<HirAttribute>());
        var mirFunction = new MirFunction(new HirFunction("TailPointerOperand", signature));
        var entry = mirFunction.Entry;

        var argument = new MirArg(0, MirType.TInt) { Span = GeneratedSpan };
        entry.Append(argument);

        var pointer = new MirConstInt(new BigInteger(0x1234)) { Span = GeneratedSpan };
        entry.Append(pointer);

        var pointerCall = new MirPointerCall(pointer, new MirValue[] { argument }, MirType.TInt, isPure: false, isTailCall: true, callTableIndex: null)
        {
            Span = GeneratedSpan
        };
        entry.Append(pointerCall);
        entry.Terminator = new MirReturn(pointerCall) { Span = GeneratedSpan };

        var selector = new InstructionSelector();
        var vFunction = selector.Select(mirFunction);

        var vEntryBlock = vFunction.Blocks.Single(b => b.Label == entry.Label);
        var vPointerCall = vEntryBlock.Nodes.OfType<VPointerCall>().Single();
        Assert.IsTrue(vPointerCall.IsTailCall, "Tail call flag should survive MIR -> VReg lowering.");
        Assert.IsNull(vPointerCall.CallTableIndex, "Pointer operand tail call should not encode a call-table index.");
        Assert.IsNotNull(vPointerCall.Pointer, "Pointer operand should flow into the VPointerCall node.");

        var scheduler = new StackScheduler();
        var schedule = scheduler.Lower(vFunction);
        var lirEntry = schedule.Function.Blocks.Single(b => b.Label == entry.Label);
        var callInst = lirEntry.Instructions.Single(i => i.Op == LirOpcode.CALLT);
        Assert.AreEqual(2, callInst.PopOverride, "CALLT should pop the pointer plus the single argument.");
        Assert.IsNull(callInst.Immediate, "CALLT should not carry an immediate for pointer operand calls.");
    }

    [TestMethod]
    public void PurePointerCall_ProducesCallaAndPushesResult()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.IntType, Array.Empty<HirAttribute>());
        var mirFunction = new MirFunction(new HirFunction("PurePointerCall", signature));
        var entry = mirFunction.Entry;

        var pointer = new MirConstInt(new BigInteger(0x123456)) { Span = GeneratedSpan };
        entry.Append(pointer);

        var pointerCall = new MirPointerCall(pointer, Array.Empty<MirValue>(), MirType.TInt, isPure: true, isTailCall: false, callTableIndex: null)
        {
            Span = GeneratedSpan
        };
        entry.Append(pointerCall);
        entry.Terminator = new MirReturn(pointerCall) { Span = GeneratedSpan };

        var selector = new InstructionSelector();
        var vFunction = selector.Select(mirFunction);
        var vPointer = vFunction.Blocks.Single(b => b.Label == entry.Label).Nodes.OfType<VPointerCall>().Single();
        Assert.IsFalse(vPointer.IsTailCall, "Pure pointer call should not emit tail call opcode.");

        var scheduler = new StackScheduler();
        var schedule = scheduler.Lower(vFunction);
        var entryBlock = schedule.Function.Blocks.Single(b => b.Label == entry.Label);
        var callInst = entryBlock.Instructions.Single(i => i.Op == LirOpcode.CALLA);

        Assert.AreEqual(1, callInst.PopOverride, "CALLA should pop the pointer operand.");
        Assert.AreEqual(1, callInst.PushOverride, "CALLA should push the return value onto the stack.");
        Assert.IsNull(callInst.Immediate, "Pointer-based CALLA should not encode an immediate payload.");
    }

    [TestMethod]
    public void TailCallTableIndex_PreservesImmediateThroughPipeline()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.VoidType, System.Array.Empty<HirAttribute>());
        var mirFunction = new MirFunction(new HirFunction("TailCallTableIndex", signature));
        var entry = mirFunction.Entry;

        var pointerCall = new MirPointerCall(pointer: null, new MirValue[0], MirType.TVoid, isPure: false, isTailCall: true, callTableIndex: 7)
        {
            Span = GeneratedSpan
        };
        entry.Append(pointerCall);
        entry.Terminator = new MirReturn(null) { Span = GeneratedSpan };

        var selector = new InstructionSelector();
        var vFunction = selector.Select(mirFunction);

        var vEntryBlock = vFunction.Blocks.Single(b => b.Label == entry.Label);
        var vPointerCall = vEntryBlock.Nodes.OfType<VPointerCall>().Single();
        Assert.IsTrue(vPointerCall.IsTailCall, "Tail call flag should survive MIR -> VReg lowering.");
        Assert.AreEqual((byte)7, vPointerCall.CallTableIndex, "Call-table index should round-trip through VPointerCall.");
        Assert.IsNull(vPointerCall.Pointer, "Call-table invocation should not keep a pointer operand.");

        var scheduler = new StackScheduler();
        var schedule = scheduler.Lower(vFunction);
        var lirEntry = schedule.Function.Blocks.Single(b => b.Label == entry.Label);
        var callInst = lirEntry.Instructions.Single(i => i.Op == LirOpcode.CALLT);
        Assert.AreEqual(0, callInst.PopOverride, "CALLT should pop zero operands when invoked via call-table index with no arguments.");
        Assert.IsNotNull(callInst.Immediate, "CALLT should encode the call-table index as an immediate.");
        Assert.AreEqual(1, callInst.Immediate!.Length, "Call-table index should encode as a single byte.");
        Assert.AreEqual(7, callInst.Immediate![0], "CALLT immediate should match the MIR call-table index.");
    }

    [TestMethod]
    public void ExternContractTailCall_CompilesToCallTablePointer()
    {
        const string source = """
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0xb6ae1662a8228ed73e372b0d0ea11716445a4281")]
    public static class ExternalContract
    {
        public static extern BigInteger Echo(BigInteger value);
    }

    public class TailCallContract : SmartContract.Framework.SmartContract
    {
        public static BigInteger Invoke(BigInteger value) => ExternalContract.Echo(value);
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.EnableHir = true;
                options.Optimize = CompilationOptions.OptimizationType.All;
            });

            var context = contexts.Single();
            Assert.IsNotNull(context.MirModule, "MIR module should be produced with IR pipeline enabled.");
            Assert.IsNotNull(context.LirModule, "LIR module should be produced with IR pipeline enabled.");

            var invokeKey = context.MirModule!.Functions.Keys.Single(k => k.Contains("Invoke", StringComparison.Ordinal));
            var mirFunction = context.MirModule.Functions[invokeKey];
            var pointerCall = mirFunction.Blocks
                .SelectMany(b => b.Instructions)
                .OfType<MirPointerCall>()
                .Single();

            Assert.IsTrue(pointerCall.IsTailCall, "Extern contract invocation should lower as a tail CALLT.");
            Assert.IsNull(pointerCall.Pointer, "Extern contract invocation should use the call-table index path.");
            Assert.IsTrue(pointerCall.CallTableIndex.HasValue, "Call-table index should be assigned to the pointer call.");
            Assert.AreEqual(1, pointerCall.Arguments.Count, "Exactly one argument should flow into the call.");

            Assert.IsTrue(context.LirModule!.TryGetCompilation(invokeKey, out var compilation), "LIR compilation for Invoke should be present.");
            var callInst = compilation.StackFunction.Blocks
                .SelectMany(b => b.Instructions)
                .Single(i => i.Op == LirOpcode.CALLT);

            Assert.AreEqual(pointerCall.Arguments.Count, callInst.PopOverride, "CALLT should pop the single argument.");
            Assert.IsNotNull(callInst.Immediate, "CALLT should encode the call-table index as an immediate.");
            Assert.AreEqual(1, callInst.Immediate!.Length, "CALLT should emit a single-byte immediate for the table index.");
            Assert.AreEqual(pointerCall.CallTableIndex!.Value, callInst.Immediate[0], "Immediate must match MIR call-table index.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void ExternContractTailCall_WithMultipleArguments_PopsAllArguments()
    {
        const string source = """
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0xaabbccddeeff00112233445566778899aabbccdd")]
    public static class ExternalCalculator
    {
        public static extern BigInteger Sum(BigInteger left, BigInteger right);
    }

    public class Aggregator : SmartContract.Framework.SmartContract
    {
        public static BigInteger Invoke(BigInteger left, BigInteger right) => ExternalCalculator.Sum(left, right);
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.EnableHir = true;
                options.Optimize = CompilationOptions.OptimizationType.All;
            });

            var context = contexts.Single();
            var invokeKey = context.MirModule!.Functions.Keys.Single(k => k.Contains("Invoke", StringComparison.Ordinal));
            var mirFunction = context.MirModule.Functions[invokeKey];
            var pointerCall = mirFunction.Blocks
                .SelectMany(b => b.Instructions)
                .OfType<MirPointerCall>()
                .Single();

            Assert.IsTrue(pointerCall.IsTailCall, "Extern contract invocation should remain a tail call.");
            Assert.IsNull(pointerCall.Pointer, "Extern contract tail call should rely on call-table indices.");
            Assert.AreEqual(2, pointerCall.Arguments.Count, "Both method arguments must flow into the pointer call.");

            Assert.IsTrue(context.LirModule!.TryGetCompilation(invokeKey, out var compilation), "LIR compilation for Invoke should be available.");
            var callInst = compilation.StackFunction.Blocks
                .SelectMany(b => b.Instructions)
                .Single(i => i.Op == LirOpcode.CALLT);

            Assert.AreEqual(pointerCall.Arguments.Count, callInst.PopOverride, "CALLT should pop both arguments.");
            Assert.IsNotNull(callInst.Immediate, "CALLT should encode the call-table index.");
            Assert.AreEqual(pointerCall.CallTableIndex!.Value, callInst.Immediate![0], "Immediate payload should match MIR call-table index.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
