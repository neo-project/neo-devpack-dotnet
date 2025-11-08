using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MiddleEnd.Lowering;
using System.Numerics;

namespace Neo.Compiler.CSharp.IRTests.Lowering;

[TestClass]
public sealed class HirToMirLowererTests
{
    private static readonly SourceSpan GeneratedSpan = new("<generated>", 0, 0, 0, 0);

    [TestMethod]
    public void Lowerer_LowersNullConstant()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.NullType, Array.Empty<HirAttribute>());
        var function = new HirFunction("ReturnNull", signature);
        var entry = function.Entry;

        var nullValue = new HirConstNull() { Span = GeneratedSpan };
        entry.Append(nullValue);
        entry.SetTerminator(new HirReturn(nullValue) { Span = GeneratedSpan });

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        var mirReturn = (MirReturn)mirFunction.Entry.Terminator!;
        Assert.IsInstanceOfType(mirReturn.Value, typeof(MirConstNull));
    }

    [TestMethod]
    public void Lowerer_LowersThrowToAbortMessage()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.VoidType, Array.Empty<HirAttribute>());
        var function = new HirFunction("Thrower", signature);
        var entry = function.Entry;

        var message = new HirConstByteString(new byte[] { 0x01, 0x02 }) { Span = GeneratedSpan };
        entry.Append(message);
        entry.SetTerminator(new HirThrow(message) { Span = GeneratedSpan });

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        Assert.IsInstanceOfType(mirFunction.Entry.Terminator, typeof(MirAbortMsg));
        var abortMsg = (MirAbortMsg)mirFunction.Entry.Terminator!;
        Assert.IsInstanceOfType(abortMsg.Message, typeof(MirConstByteString));
    }

    [TestMethod]
    public void Lowerer_LowersUnreachable()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.VoidType, Array.Empty<HirAttribute>());
        var function = new HirFunction("Unreachable", signature);
        function.Entry.SetTerminator(new HirUnreachable() { Span = GeneratedSpan });

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        Assert.IsInstanceOfType(mirFunction.Entry.Terminator, typeof(MirUnreachable));
    }

    [TestMethod]
    public void Lowerer_EmitsPointerCallAndGuard()
    {
        var signature = new HirSignature(new[] { HirType.IntType }, HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("PointerGuard", signature);
        var entry = function.Entry;

        var argument = new HirArgument("index", HirType.IntType, 0);
        var loadIndex = new HirLoadArgument(argument) { Span = GeneratedSpan };
        entry.Append(loadIndex);

        var pointer = new HirConstInt(new BigInteger(0x1234)) { Span = GeneratedSpan };
        entry.Append(pointer);

        var pointerCall = new HirPointerCall(pointer, new HirValue[] { loadIndex }, HirType.IntType, HirCallSemantics.Effectful, isTailCall: false, callTableIndex: null)
        {
            Span = GeneratedSpan
        };
        entry.Append(pointerCall);

        var length = new HirConstInt(new BigInteger(16)) { Span = GeneratedSpan };
        entry.Append(length);

        var bounds = new HirBoundsCheck(loadIndex, length, HirFailPolicy.Abort) { Span = GeneratedSpan };
        entry.Append(bounds);

        entry.SetTerminator(new HirReturn(pointerCall) { Span = GeneratedSpan });

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        var instructions = mirFunction.Blocks.SelectMany(b => b.Instructions).ToArray();
        var mirPointer = instructions.OfType<MirPointerCall>().Single();
        Assert.IsFalse(mirPointer.IsPure, "Pointer call should be treated as effectful.");
        Assert.AreEqual(1, mirPointer.Arguments.Count, "Pointer call should forward HIR argument.");

        Assert.IsTrue(instructions.OfType<MirGuardBounds>().Any(), "Bounds guard should lower to MirGuardBounds.");
        var ret = mirFunction.Blocks.Single(b => b.Terminator is MirReturn).Terminator as MirReturn;
        Assert.IsNotNull(ret);
        Assert.AreSame(mirPointer, ret!.Value, "Return should forward pointer-call result.");
    }

    [TestMethod]
    public void Lowerer_TranslatesLoopPhi()
    {
        var signature = new HirSignature(new[] { HirType.IntType }, HirType.IntType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("Loop", signature);
        var entry = function.Entry;
        var loop = function.AddBlock("loop");
        var body = function.AddBlock("body");
        var exit = function.AddBlock("exit");

        var argument = new HirArgument("limit", HirType.IntType, 0);
        var limit = new HirLoadArgument(argument) { Span = GeneratedSpan };
        entry.Append(limit);
        var zero = new HirConstInt(BigInteger.Zero) { Span = GeneratedSpan };
        entry.Append(zero);
        entry.SetTerminator(new HirBranch(loop) { Span = GeneratedSpan });

        var phi = new HirPhi(HirType.IntType) { Span = GeneratedSpan };
        loop.AppendPhi(phi);
        var compare = new HirCompare(HirCmpKind.Lt, phi, limit) { Span = GeneratedSpan };
        loop.Append(compare);
        loop.SetTerminator(new HirConditionalBranch(compare, body, exit) { Span = GeneratedSpan });

        var one = new HirConstInt(BigInteger.One) { Span = GeneratedSpan };
        body.Append(one);
        var next = new HirAdd(phi, one, HirType.IntType) { Span = GeneratedSpan };
        body.Append(next);
        body.SetTerminator(new HirBranch(loop) { Span = GeneratedSpan });

        exit.SetTerminator(new HirReturn(phi) { Span = GeneratedSpan });

        phi.AddIncoming(entry, zero);
        phi.AddIncoming(body, next);

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        var loopBlock = mirFunction.Blocks.Single(b => b.Label == "loop");
        Assert.IsTrue(loopBlock.Phis.Any(p => p.Type is MirTokenType), "Loop header should materialise a token phi.");
        var mirPhi = loopBlock.Phis.Single(p => p.Type != MirType.TToken);
        Assert.AreEqual(MirType.TInt, mirPhi.Type, "Loop phi should lower to MIR int phi.");

        Assert.IsInstanceOfType(loopBlock.Terminator, typeof(MirCondBranch), "Loop header should lower to conditional branch.");
        Assert.IsTrue(mirFunction.Blocks.SelectMany(b => b.Instructions).OfType<MirBinary>().Any(bin => bin.OpCode == MirBinary.Op.Add),
            "Loop body should emit addition in MIR.");
    }

    [TestMethod]
    public void Lowerer_MaterialisesNestedExpressions()
    {
        var signature = new HirSignature(System.Array.Empty<HirType>(), HirType.ByteStringType, System.Array.Empty<HirAttribute>());
        var function = new HirFunction("Nested", signature);
        var entry = function.Entry;
        var exit = function.AddBlock("exit");

        var resultLocal = new HirLocal("result", HirType.ByteStringType);

        var zero = new HirConstInt(BigInteger.Zero) { Span = GeneratedSpan };
        var one = new HirConstInt(BigInteger.One) { Span = GeneratedSpan };
        var pointerValue = new HirConstInt(new BigInteger(0x1234)) { Span = GeneratedSpan };
        var bytesA = new HirConstByteString(new byte[] { 0x01 }) { Span = GeneratedSpan };
        var bytesB = new HirConstByteString(new byte[] { 0x02 }) { Span = GeneratedSpan };

        entry.Append(zero);
        entry.Append(one);
        entry.Append(pointerValue);
        entry.Append(bytesA);
        entry.Append(bytesB);

        var arrayNew = new HirArrayNew(one, HirType.IntType) { Span = GeneratedSpan };
        var arrayGet = new HirArrayGet(arrayNew, zero, HirType.IntType) { Span = GeneratedSpan };
        var arrayLen = new HirArrayLen(arrayNew) { Span = GeneratedSpan };

        var mapNew = new HirMapNew(HirType.IntType, HirType.ByteStringType) { Span = GeneratedSpan };
        var pointerCall = new HirPointerCall(pointerValue, new HirValue[] { zero }, HirType.IntType, HirCallSemantics.Pure) { Span = GeneratedSpan };
        var mapGet = new HirMapGet(mapNew, pointerCall, HirType.ByteStringType) { Span = GeneratedSpan };
        var mapLen = new HirMapLen(mapNew) { Span = GeneratedSpan };
        var mapHas = new HirMapHas(mapNew, pointerCall) { Span = GeneratedSpan };

        var concat = new HirConcat(bytesA, bytesB) { Span = GeneratedSpan };
        var slice = new HirSlice(concat, zero, one, isBufferSlice: false) { Span = GeneratedSpan };
        var checkedAdd = new HirCheckedBinary(HirCheckedOp.Add, arrayGet, mapLen, HirType.IntType, HirFailPolicy.Abort) { Span = GeneratedSpan };
        var compare = new HirCompare(HirCmpKind.Gt, checkedAdd, arrayLen) { Span = GeneratedSpan };
        var convert = new HirConvert(HirConvKind.ToBool, compare, HirType.BoolType) { Span = GeneratedSpan };
        var notHas = new HirNot(mapHas) { Span = GeneratedSpan };

        var pureCall = new HirCall("PureCall", new HirValue[] { slice, notHas }, HirType.ByteStringType, isStatic: true, HirCallSemantics.Pure) { Span = GeneratedSpan };
        var intrinsicMetadata = new HirIntrinsicMetadata("Runtime", "PureIntrinsic", HirEffect.None, HirType.ByteStringType, new[] { HirType.ByteStringType }, true, true, false, null);
        var intrinsic = new HirIntrinsicCall("Runtime", "PureIntrinsic", new HirValue[] { pureCall }, intrinsicMetadata) { Span = GeneratedSpan };

        var structType = new HirStructType(new[]
        {
            new HirField("data", HirType.ByteStringType, false),
            new HirField("flag", HirType.BoolType, false)
        });
        var newStruct = new HirNewStruct(new HirValue[] { intrinsic, convert }, structType) { Span = GeneratedSpan };
        var structGet = new HirStructGet(newStruct, 0, HirType.ByteStringType) { Span = GeneratedSpan };

        var finalConcat = new HirConcat(structGet, mapGet) { Span = GeneratedSpan };
        var store = new HirStoreLocal(resultLocal, finalConcat) { Span = GeneratedSpan };
        entry.Append(store);

        entry.SetTerminator(new HirConditionalBranch(convert, exit, exit) { Span = GeneratedSpan });

        var exitLoad = new HirLoadLocal(resultLocal) { Span = GeneratedSpan };
        exit.Append(exitLoad);
        exit.SetTerminator(new HirReturn(exitLoad) { Span = GeneratedSpan });

        var lowerer = new HirToMirLowerer();
        var module = new MirModule();
        var mirFunction = lowerer.Lower(function, module);

        var instructions = mirFunction.Blocks.SelectMany(b => b.Instructions).ToArray();

        Assert.IsTrue(instructions.OfType<MirArrayNew>().Any(), "Array creation should materialise via MirArrayNew.");
        Assert.IsTrue(instructions.OfType<MirArrayGet>().Any(), "Array element reads should lower inline.");
        Assert.IsTrue(instructions.OfType<MirArrayLen>().Any(), "Array length should lower inline.");
        Assert.IsTrue(instructions.OfType<MirMapNew>().Any(), "Map creation should materialise inline.");
        Assert.IsTrue(instructions.OfType<MirMapGet>().Any(), "Map get should lower inline.");
        Assert.IsTrue(instructions.OfType<MirMapLen>().Any(), "Map len should lower inline.");
        Assert.IsTrue(instructions.OfType<MirMapHas>().Any(), "Map has should lower inline.");
        Assert.IsTrue(instructions.OfType<MirPointerCall>().Any(call => call.IsPure), "Pure pointer call should lower inline.");
        Assert.IsTrue(instructions.OfType<MirCall>().Any(call => call.IsPure), "Pure call should lower inline.");
        Assert.IsTrue(instructions.OfType<MirSyscall>().Any(), "Pure intrinsic should lower inline.");
        Assert.IsTrue(instructions.OfType<MirStructPack>().Any(), "Struct pack should materialise inline.");
        Assert.IsTrue(instructions.OfType<MirStructGet>().Any(), "Struct field access should lower inline.");
        Assert.IsTrue(instructions.OfType<MirConcat>().Count() >= 2, "Concat should be emitted for nested expressions.");
        Assert.IsTrue(instructions.OfType<MirSlice>().Any(), "Slice should lower inline.");
        Assert.IsTrue(instructions.OfType<MirCheckedAdd>().Any(), "Checked arithmetic should lower inline.");
        Assert.IsTrue(instructions.OfType<MirCompare>().Any(), "Compare should lower inline.");
        Assert.IsTrue(instructions.OfType<MirConvert>().Any(), "Convert should lower inline.");
    }
}
