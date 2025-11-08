using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo.Compiler.MIR;
using Neo.Cryptography;

namespace Neo.Compiler.LIR.Backend;

/// <summary>
/// Lowers MIR into VReg-LIR (SSA value form) suitable for stack scheduling.
/// </summary>
internal sealed class InstructionSelector
{
    private readonly Dictionary<MirValue, VNode> _valueCache = new();
    private readonly Dictionary<MirBlock, VBlock> _blockMap = new();
    private readonly Dictionary<MirType, LirType> _typeCache = new();
    private readonly Dictionary<MirTry, VTry> _tryScopeMap = new();

    internal VFunction Select(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        _valueCache.Clear();
        _blockMap.Clear();
        _typeCache.Clear();
        _tryScopeMap.Clear();

        var vFunction = new VFunction(function.Name);
        vFunction.ParameterCount = function.Source.Signature.ParameterTypes.Count;
        foreach (var mirBlock in function.Blocks)
        {
            var vBlock = new VBlock(mirBlock.Label);
            vFunction.Blocks.Add(vBlock);
            _blockMap.Add(mirBlock, vBlock);
        }

        foreach (var mirBlock in function.Blocks)
        {
            var vBlock = _blockMap[mirBlock];

            // Handle phi nodes first so downstream instructions can reference them.
            foreach (var phi in mirBlock.Phis)
            {
                if (phi.Type is MirTokenType)
                {
                    _valueCache[phi] = new VConstInt(0); // placeholder; tokens do not lower to V-Reg
                    continue;
                }

                var vPhi = new VPhi(MapType(phi.Type))
                {
                    Span = phi.Span
                };
                vBlock.Nodes.Add(vPhi);
                _valueCache[phi] = vPhi;
            }

            foreach (var inst in mirBlock.Instructions)
                LowerInstruction(inst, mirBlock, vBlock);

            vBlock.Terminator = LowerTerminator(mirBlock.Terminator, mirBlock);
        }

        // Wire phi inputs now that all values exist.
        foreach (var mirBlock in function.Blocks)
        {
            for (int i = 0; i < mirBlock.Phis.Count; i++)
            {
                var mirPhi = mirBlock.Phis[i];
                if (_valueCache.TryGetValue(mirPhi, out var node) && node is VPhi vPhi)
                {
                    foreach (var incoming in mirPhi.Inputs)
                        vPhi.AddIncoming(_blockMap[incoming.Block], GetValue(incoming.Value));
                }
            }
        }

        return vFunction;
    }

    private void LowerInstruction(MirInst inst, MirBlock owningBlock, VBlock target)
    {
        switch (inst)
        {
            case MirTokenSeed seed:
                _valueCache[seed] = new VConstInt(0); // dummy anchor, tokens are not lowered.
                break;

            case MirConstInt constInt:
                EmitValue(constInt, new VConstInt(constInt.Value), target);
                break;

            case MirConstBool constBool:
                EmitValue(constBool, new VConstBool(constBool.Value), target);
                break;

            case MirConstByteString constByteString:
                EmitValue(constByteString, new VConstByteString(constByteString.Value), target);
                break;

            case MirConstBuffer constBuffer:
                EmitValue(constBuffer, new VConstBuffer(constBuffer.Value), target);
                break;

            case MirConstNull constNull:
                EmitValue(constNull, new VConstNull(), target);
                break;

            case MirArg arg:
                EmitValue(arg, new VParam(arg.Index, MapType(arg.Type)), target);
                break;

            case MirUnary unary:
                EmitValue(unary, new VUnary(MapUnaryOp(unary.OpCode), GetValue(unary.Operand), MapType(unary.Type)), target);
                break;

            case MirBinary binary:
                EmitValue(binary, new VBinary(MapBinaryOp(binary.OpCode), GetValue(binary.Left), GetValue(binary.Right), MapType(binary.Type)), target);
                break;

            case MirCompare compare:
                EmitValue(compare, new VCompare(MapCompareOp(compare.OpCode), compare.Unsigned, GetValue(compare.Left), GetValue(compare.Right)), target);
                break;

            case MirConvert convert:
                EmitValue(convert, new VConvert(MapConvertOp(convert.ConversionKind), GetValue(convert.Value), MapType(convert.Type)), target);
                break;

            case MirPhi phi:
                // Already emitted.
                break;

            case MirStructPack pack:
                EmitValue(pack, new VStructPack(pack.Fields.Select(GetValue).ToArray(), (LirStructType)MapType(pack.Type)), target);
                break;

            case MirStructGet get:
                EmitValue(get, new VStructGet(GetValue(get.Object), get.Index, MapType(get.Type)), target);
                break;

            case MirStructSet set:
                EmitValue(set, new VStructSet(GetValue(set.Object), set.Index, GetValue(set.Value), (LirStructType)MapType(set.Type)), target);
                break;

            case MirStaticFieldLoad loadStatic:
                EmitValue(loadStatic, new VStaticLoad(loadStatic.Slot, MapType(loadStatic.Type)), target);
                break;

            case MirStaticFieldStore storeStatic:
                EmitValue(storeStatic, new VStaticStore(storeStatic.Slot, GetValue(storeStatic.Value), MapType(storeStatic.FieldType)), target);
                break;

            case MirArrayNew arrayNew:
                EmitValue(arrayNew, new VArrayNew(GetValue(arrayNew.Length), MapType(arrayNew.ElementType)), target);
                break;

            case MirArrayLen arrayLen:
                EmitValue(arrayLen, new VArrayLen(GetValue(arrayLen.Array)), target);
                break;

            case MirArrayGet arrayGet:
                EmitValue(arrayGet, new VArrayGet(GetValue(arrayGet.Array), GetValue(arrayGet.Index), MapType(arrayGet.Type)), target);
                break;

            case MirArraySet arraySet:
                EmitValue(arraySet, new VArraySet(GetValue(arraySet.Array), GetValue(arraySet.Index), GetValue(arraySet.Value)), target);
                break;

            case MirMapNew mapNew:
                EmitValue(mapNew, new VMapNew(MapType(mapNew.KeyType), MapType(mapNew.ValueType)), target);
                break;

            case MirMapGet mapGet:
                EmitValue(mapGet, new VMapGet(GetValue(mapGet.Map), GetValue(mapGet.Key), MapType(mapGet.Type)), target);
                break;

            case MirMapSet mapSet:
                EmitValue(mapSet, new VMapSet(GetValue(mapSet.Map), GetValue(mapSet.Key), GetValue(mapSet.Value)), target);
                break;

            case MirMapDelete mapDelete:
                EmitValue(mapDelete, new VMapDelete(GetValue(mapDelete.Map), GetValue(mapDelete.Key)), target);
                break;

            case MirMapHas mapHas:
                EmitValue(mapHas, new VMapHas(GetValue(mapHas.Map), GetValue(mapHas.Key)), target);
                break;

            case MirMapLen mapLen:
                EmitValue(mapLen, new VMapLen(GetValue(mapLen.Map)), target);
                break;

            case MirConcat concat:
                EmitValue(concat, new VConcat(GetValue(concat.Left), GetValue(concat.Right)), target);
                break;

            case MirSlice slice:
                EmitValue(slice, new VSlice(GetValue(slice.Value), GetValue(slice.Start), GetValue(slice.Length), slice.IsBufferSlice), target);
                break;

            case MirBufferNew bufferNew:
                EmitValue(bufferNew, new VBufferNew(GetValue(bufferNew.Length)), target);
                break;

            case MirBufferSet bufferSet:
                EmitValue(bufferSet, new VBufferSet(GetValue(bufferSet.Buffer), GetValue(bufferSet.Index), GetValue(bufferSet.Byte)), target);
                break;

            case MirBufferCopy bufferCopy:
                EmitValue(bufferCopy, new VBufferCopy(GetValue(bufferCopy.Destination), GetValue(bufferCopy.Source), GetValue(bufferCopy.DestinationOffset), GetValue(bufferCopy.SourceOffset), GetValue(bufferCopy.Length)), target);
                break;

            case MirModMul modMul:
                EmitValue(modMul, new VModMul(GetValue(modMul.Left), GetValue(modMul.Right), GetValue(modMul.Modulus), MapType(modMul.Type)), target);
                break;

            case MirModPow modPow:
                EmitValue(modPow, new VModPow(GetValue(modPow.Value), GetValue(modPow.Exponent), GetValue(modPow.Modulus), MapType(modPow.Type)), target);
                break;

            case MirGuardNull guardNull:
                target.Nodes.Add(new VGuardNull(GetValue(guardNull.Reference), MapGuardFail(guardNull.Fail), guardNull.FailTarget is null ? null : _blockMap[guardNull.FailTarget]) { Span = guardNull.Span });
                break;

            case MirGuardBounds guardBounds:
                target.Nodes.Add(new VGuardBounds(GetValue(guardBounds.Index), GetValue(guardBounds.Length), MapGuardFail(guardBounds.Fail), guardBounds.FailTarget is null ? null : _blockMap[guardBounds.FailTarget]) { Span = guardBounds.Span });
                break;

            case MirCheckedAdd checkedAdd:
                EmitValue(checkedAdd, new VCheckedArithmetic(VCheckedOp.Add, GetValue(checkedAdd.Left), GetValue(checkedAdd.Right), MapType(checkedAdd.Type), MapGuardFail(checkedAdd.Fail), checkedAdd.FailTarget is null ? null : _blockMap[checkedAdd.FailTarget]), target);
                break;

            case MirCheckedSub checkedSub:
                EmitValue(checkedSub, new VCheckedArithmetic(VCheckedOp.Sub, GetValue(checkedSub.Left), GetValue(checkedSub.Right), MapType(checkedSub.Type), MapGuardFail(checkedSub.Fail), checkedSub.FailTarget is null ? null : _blockMap[checkedSub.FailTarget]), target);
                break;

            case MirCheckedMul checkedMul:
                EmitValue(checkedMul, new VCheckedArithmetic(VCheckedOp.Mul, GetValue(checkedMul.Left), GetValue(checkedMul.Right), MapType(checkedMul.Type), MapGuardFail(checkedMul.Fail), checkedMul.FailTarget is null ? null : _blockMap[checkedMul.FailTarget]), target);
                break;

            case MirCall call:
                {
                    var args = call.Arguments.Select(GetValue).ToArray();
                    var node = new VCall(call.Callee, args, MapType(call.Type), call.IsPure)
                    {
                        Span = call.Span
                    };
                    if (!call.IsPure)
                        target.Nodes.Add(node);
                    EmitValue(call, node, target, emitToBlock: call.IsPure);
                    break;
                }

            case MirPointerCall pointerCall:
                {
                    var args = pointerCall.Arguments.Select(GetValue).ToArray();
                    var pointerValue = pointerCall.Pointer is null ? null : GetValue(pointerCall.Pointer);
                    var node = new VPointerCall(pointerValue, args, MapType(pointerCall.Type), pointerCall.IsPure, pointerCall.IsTailCall, pointerCall.CallTableIndex)
                    {
                        Span = pointerCall.Span
                    };
                    if (!pointerCall.IsPure)
                        target.Nodes.Add(node);
                    EmitValue(pointerCall, node, target, emitToBlock: pointerCall.IsPure);
                    break;
                }

            case MirTry mirTry:
                {
                    var catchBlocks = mirTry.CatchHandlers.Select(handler => _blockMap[handler.Block]).ToArray();
                    var node = new VTry(_blockMap[mirTry.TryBlock], _blockMap[mirTry.FinallyBlock], _blockMap[mirTry.MergeBlock], catchBlocks)
                    {
                        Span = mirTry.Span
                    };
                    target.Nodes.Add(node);
                    _tryScopeMap[mirTry] = node;
                    break;
                }

            case MirCatch mirCatch:
                {
                    if (!_tryScopeMap.TryGetValue(mirCatch.Parent, out var scope))
                        throw new NotSupportedException("Encountered MirCatch without associated VTry scope.");
                    var node = new VCatch(scope)
                    {
                        Span = mirCatch.Span
                    };
                    target.Nodes.Add(node);
                    break;
                }

            case MirFinally mirFinally:
                {
                    if (!_tryScopeMap.TryGetValue(mirFinally.Parent, out var scope))
                        throw new NotSupportedException("Encountered MirFinally without associated MirTry.");
                    var node = new VFinally(scope)
                    {
                        Span = mirFinally.Span
                    };
                    target.Nodes.Add(node);
                    break;
                }

            case MirSyscall syscall:
                {
                    var args = syscall.Arguments.Select(GetValue).ToArray();
                    var opcode = new VSyscall(ComputeSyscallId(syscall), args, MapType(syscall.Type))
                    {
                        Span = syscall.Span
                    };
                    EmitValue(syscall, opcode, target);
                    break;
                }
        }
    }

    private VTerminator? LowerTerminator(MirTerminator? terminator, MirBlock owningBlock)
    {
        switch (terminator)
        {
            case MirBranch branch:
                return new VJmp(_blockMap[branch.Target]);

            case MirCondBranch cond:
                return new VJmpIf(GetValue(cond.Condition), _blockMap[cond.TrueTarget], _blockMap[cond.FalseTarget]);

            case MirCompareBranch cmp:
                return new VCompareBranch(
                    MapCompareOp(cmp.Operation),
                    cmp.Unsigned,
                    GetValue(cmp.Left),
                    GetValue(cmp.Right),
                    _blockMap[cmp.TrueTarget],
                    _blockMap[cmp.FalseTarget])
                { Span = cmp.Span };

            case MirSwitch @switch:
                var cases = @switch.Cases.Select(c => (c.Case, _blockMap[c.Target])).ToArray();
                return new VSwitch(GetValue(@switch.Key), cases, _blockMap[@switch.DefaultTarget]);

            case MirReturn ret:
                return new VRet(ret.Value is null ? null : GetValue(ret.Value));

            case MirAbort:
                return new VAbort();

            case MirAbortMsg abortMsg:
                return new VAbortMsg(GetValue(abortMsg.Message));

            case MirUnreachable:
                return new VUnreachable();

            case MirLeave leave:
                if (!_tryScopeMap.TryGetValue(leave.Scope, out var leaveScope))
                    throw new NotSupportedException("Encountered MirLeave without associated VTry scope.");
                return new VLeave(leaveScope, _blockMap[leave.Target]) { Span = leave.Span };

            case MirEndFinally endFinally:
                if (!_tryScopeMap.TryGetValue(endFinally.Scope, out var finallyScope))
                    throw new NotSupportedException("Encountered MirEndFinally without associated VTry scope.");
                return new VEndFinally(finallyScope, _blockMap[endFinally.Target]) { Span = endFinally.Span };

            case null:
                return null;

            default:
                throw new NotSupportedException($"Unsupported MIR terminator '{terminator.GetType().Name}'.");
        }
    }

    private void EmitValue(MirInst source, VNode node, VBlock target, bool emitToBlock = true)
    {
        node.Span = source.Span;
        if (emitToBlock)
            target.Nodes.Add(node);
        _valueCache[source] = node;
    }

    private VNode GetValue(MirValue value)
    {
        if (_valueCache.TryGetValue(value, out var node))
            return node;

        switch (value)
        {
            case MirConstInt constInt:
                node = new VConstInt(constInt.Value) { Span = constInt.Span };
                _valueCache[value] = node;
                return node;

            case MirConstBool constBool:
                node = new VConstBool(constBool.Value) { Span = constBool.Span };
                _valueCache[value] = node;
                return node;

            case MirConstByteString constByteString:
                node = new VConstByteString(constByteString.Value) { Span = constByteString.Span };
                _valueCache[value] = node;
                return node;

            case MirConstBuffer constBuffer:
                node = new VConstBuffer(constBuffer.Value) { Span = constBuffer.Span };
                _valueCache[value] = node;
                return node;

            case MirConstNull constNull:
                node = new VConstNull { Span = constNull.Span };
                _valueCache[value] = node;
                return node;
        }

        throw new NotSupportedException($"Value '{value.GetType().Name}' not lowered to LIR.");
    }

    private static VUnaryOp MapUnaryOp(MirUnary.Op op) => op switch
    {
        MirUnary.Op.Neg => VUnaryOp.Negate,
        MirUnary.Op.Not => VUnaryOp.Not,
        MirUnary.Op.Abs => VUnaryOp.Abs,
        MirUnary.Op.Sign => VUnaryOp.Sign,
        MirUnary.Op.Inc => VUnaryOp.Inc,
        MirUnary.Op.Dec => VUnaryOp.Dec,
        MirUnary.Op.Sqrt => VUnaryOp.Sqrt,
        _ => throw new NotSupportedException($"Unsupported unary op {op}.")
    };

    private static VBinaryOp MapBinaryOp(MirBinary.Op op) => op switch
    {
        MirBinary.Op.Add => VBinaryOp.Add,
        MirBinary.Op.Sub => VBinaryOp.Sub,
        MirBinary.Op.Mul => VBinaryOp.Mul,
        MirBinary.Op.Div => VBinaryOp.Div,
        MirBinary.Op.Mod => VBinaryOp.Mod,
        MirBinary.Op.And => VBinaryOp.And,
        MirBinary.Op.Or => VBinaryOp.Or,
        MirBinary.Op.Xor => VBinaryOp.Xor,
        MirBinary.Op.Shl => VBinaryOp.Shl,
        MirBinary.Op.Shr => VBinaryOp.Shr,
        MirBinary.Op.Max => VBinaryOp.Max,
        MirBinary.Op.Min => VBinaryOp.Min,
        MirBinary.Op.Pow => VBinaryOp.Pow,
        _ => throw new NotSupportedException($"Unsupported binary op {op}.")
    };

    private static VCompareOp MapCompareOp(MirCompare.Op op) => op switch
    {
        MirCompare.Op.Eq => VCompareOp.Eq,
        MirCompare.Op.Ne => VCompareOp.Ne,
        MirCompare.Op.Lt => VCompareOp.Lt,
        MirCompare.Op.Le => VCompareOp.Le,
        MirCompare.Op.Gt => VCompareOp.Gt,
        MirCompare.Op.Ge => VCompareOp.Ge,
        _ => throw new NotSupportedException($"Unsupported compare op {op}.")
    };

    private static VConvertOp MapConvertOp(MirConvert.Kind kind) => kind switch
    {
        MirConvert.Kind.SignExtend => VConvertOp.SignExtend,
        MirConvert.Kind.ZeroExtend => VConvertOp.ZeroExtend,
        MirConvert.Kind.Narrow => VConvertOp.Narrow,
        MirConvert.Kind.ToBool => VConvertOp.ToBool,
        MirConvert.Kind.ToByteString => VConvertOp.ToByteString,
        MirConvert.Kind.ToBuffer => VConvertOp.ToBuffer,
        _ => throw new NotSupportedException($"Unsupported convert op {kind}.")
    };

    private static VGuardFailKind MapGuardFail(MirGuardFail fail) => fail switch
    {
        MirGuardFail.Branch => VGuardFailKind.Branch,
        _ => VGuardFailKind.Abort
    };

    private LirType MapType(MirType type)
    {
        if (_typeCache.TryGetValue(type, out var cached))
            return cached;

        LirType mapped = type switch
        {
            MirBoolType => LirType.TBool,
            MirVoidType => LirType.TVoid,
            MirUnknownType => new LirAnyType(),
            MirByteStringType => LirType.TByteString,
            MirBufferType => LirType.TBuffer,
            MirIntType mirInt => new LirIntType(mirInt.WidthHintBits, mirInt.IsSigned),
            MirArrayType arr => new LirArrayType(MapType(arr.ElementType)),
            MirStructType st => new LirStructType(st.Fields.Select(fieldType => MapType(fieldType)).ToArray()),
            MirMapType map => new LirMapType(MapType(map.KeyType), MapType(map.ValueType)),
            MirHandleType => new LirAnyType(),
            MirTokenType => new LirAnyType(),
            _ => new LirAnyType()
        };

        _typeCache[type] = mapped;
        return mapped;
    }

    private static uint ComputeSyscallId(MirSyscall syscall)
    {
        var name = $"System.{syscall.Category}.{syscall.Name}";
        var bytes = Encoding.ASCII.GetBytes(name);
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes.Sha256());
    }
}
