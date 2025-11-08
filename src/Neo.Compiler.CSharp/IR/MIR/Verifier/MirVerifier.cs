using System;
using System.Collections.Generic;
using System.Linq;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR.Optimization;

namespace Neo.Compiler.MIR;

/// <summary>
/// Performs structural validation on MIR to guarantee basic SSA invariants, effect sequencing, and type consistency.
/// This verifier focuses on invariants required before applying optimisation or lowering passes; additional checks can
/// be layered on once the pipeline grows.
/// </summary>
internal sealed partial class MirVerifier
{

    private readonly List<string> _errors = new();
    private readonly Dictionary<MirBlock, HashSet<MirBlock>> _predecessors = new();
    private readonly Dictionary<MirBlock, HashSet<MirBlock>> _dominators = new();
    private MirFunction? _function;

    private static bool TryGetSyscallSignature(MirSyscall syscall, out SyscallSignature signature)
    {
        if (syscall is null)
            throw new ArgumentNullException(nameof(syscall));

        var syscallName = $"System.{syscall.Category}.{syscall.Name}";
        if (!HirIntrinsicCatalog.TryResolveSyscall(syscallName, out var metadata))
        {
            signature = null!;
            return false;
        }

        var parameterTypes = metadata.ParameterTypes.Select(MirTypeMapper.FromHirType).ToArray();
        signature = new SyscallSignature(
            (MirEffect)metadata.Effect,
            MirTypeMapper.FromHirType(metadata.ReturnType),
            parameterTypes,
            metadata.IsDeterministic);
        return true;
    }

    internal IReadOnlyList<string> Verify(MirFunction function)
    {
        _function = function ?? throw new ArgumentNullException(nameof(function));
        _errors.Clear();
        _predecessors.Clear();
        _dominators.Clear();

        BuildPredecessors(function);
        ComputeDominators(function);
        CheckBlocks(function);
        CheckTerminators(function);
        CheckPhis(function);
        CheckInstructions(function);
        CheckDominance(function);
        CheckTokenFlow(function);

        return _errors.ToArray();
    }

    private void BuildPredecessors(MirFunction function)
    {
        foreach (var block in function.Blocks)
            _predecessors[block] = new HashSet<MirBlock>();

        foreach (var block in function.Blocks)
        {
            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (_predecessors.TryGetValue(successor, out var set))
                    set.Add(block);
            }
        }
    }

    private void ComputeDominators(MirFunction function)
    {
        var blocks = function.Blocks.ToArray();
        if (blocks.Length == 0)
            return;

        foreach (var block in blocks)
        {
            if (ReferenceEquals(block, function.Entry))
            {
                _dominators[block] = new HashSet<MirBlock> { block };
            }
            else
            {
                _dominators[block] = new HashSet<MirBlock>(blocks);
            }
        }

        var changed = true;
        while (changed)
        {
            changed = false;

            foreach (var block in blocks)
            {
                if (ReferenceEquals(block, function.Entry))
                    continue;

                var preds = _predecessors[block];
                HashSet<MirBlock> newSet;

                if (preds.Count == 0)
                {
                    newSet = new HashSet<MirBlock> { block };
                }
                else
                {
                    using var enumerator = preds.GetEnumerator();
                    enumerator.MoveNext();
                    newSet = new HashSet<MirBlock>(_dominators[enumerator.Current]);
                    while (enumerator.MoveNext())
                        newSet.IntersectWith(_dominators[enumerator.Current]);
                    newSet.Add(block);
                }

                var domSet = _dominators[block];
                if (!domSet.SetEquals(newSet))
                {
                    domSet.Clear();
                    domSet.UnionWith(newSet);
                    changed = true;
                }
            }
        }
    }

    private void CheckBlocks(MirFunction function)
    {
        if (function.Blocks.Count == 0)
        {
            _errors.Add($"Function '{function.Name}' contains no blocks.");
            return;
        }

        if (!ReferenceEquals(function.Blocks[0], function.Entry))
            _errors.Add($"Function '{function.Name}' entry block is not first in the block list.");

        foreach (var block in function.Blocks)
        {
            if (block.Terminator is null)
                _errors.Add($"Block '{block.Label}' is missing a terminator.");

            foreach (var phi in block.Phis)
            {
                if (phi is null)
                {
                    _errors.Add($"Block '{block.Label}' contains a null phi.");
                    continue;
                }

                if (!ReferenceEquals(block, _function!.Blocks.FirstOrDefault(b => b.Phis.Contains(phi))))
                    _errors.Add($"Phi found in block '{block.Label}' is not associated with that block.");
            }

            foreach (var inst in block.Instructions)
            {
                if (inst is null)
                    _errors.Add($"Block '{block.Label}' contains a null instruction.");
            }
        }
    }

    private void CheckTerminators(MirFunction function)
    {
        var blockSet = new HashSet<MirBlock>(function.Blocks);
        var tryScopes = new HashSet<MirTry>(function.Blocks.SelectMany(b => b.Instructions).OfType<MirTry>());

        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                if (inst is MirTry mirTry)
                {
                    foreach (var handler in mirTry.CatchHandlers)
                        RequireBlock(handler.Block, blockSet, block, "try catch");
                }

                if (inst is MirCatch mirCatch && !ReferenceEquals(mirCatch.Handler.Block, block))
                    _errors.Add($"MirCatch for try scope '{mirCatch.Parent.TryBlock.Label}' appears outside its handler block '{mirCatch.Handler.Block.Label}'.");
            }

            switch (block.Terminator)
            {
                case MirBranch branch:
                    RequireBlock(branch.Target, blockSet, block, "branch");
                    break;

                case MirCondBranch cond:
                    RequireBlock(cond.TrueTarget, blockSet, block, "conditional true branch");
                    RequireBlock(cond.FalseTarget, blockSet, block, "conditional false branch");
                    break;

                case MirCompareBranch cmpBranch:
                    RequireBlock(cmpBranch.TrueTarget, blockSet, block, "compare true branch");
                    RequireBlock(cmpBranch.FalseTarget, blockSet, block, "compare false branch");
                    break;

                case MirSwitch @switch:
                    foreach (var (_, target) in @switch.Cases)
                        RequireBlock(target, blockSet, block, "switch case");
                    RequireBlock(@switch.DefaultTarget, blockSet, block, "switch default");
                    break;

                case MirLeave leave:
                    if (!tryScopes.Contains(leave.Scope))
                        _errors.Add($"Block '{block.Label}' references unknown try scope in MirLeave terminator.");
                    RequireBlock(leave.Target, blockSet, block, "leave target");
                    break;

                case MirEndFinally endFinally:
                    if (!tryScopes.Contains(endFinally.Scope))
                        _errors.Add($"Block '{block.Label}' references unknown try scope in MirEndFinally terminator.");
                    RequireBlock(endFinally.Target, blockSet, block, "endfinally target");
                    break;

                case MirReturn ret:
                    if (ret.Value is null)
                    {
                        var expected = _function!.Source.Signature.ReturnType;
                        if (!ReferenceEquals(expected, Neo.Compiler.HIR.HirType.VoidType))
                            _errors.Add($"Block '{block.Label}' returns void but function '{_function!.Name}' is non-void.");
                    }
                    else
                    {
                        var expected = MirTypeMapper.FromHirType(_function!.Source.Signature.ReturnType);
                        if (!TypesCompatible(expected, ret.Value.Type))
                            _errors.Add($"Return value type mismatch in function '{_function!.Name}': expected {expected}, actual {ret.Value.Type}.");
                    }
                    break;

                case MirAbortMsg abortMsg:
                    if (abortMsg.Message.Type is not MirByteStringType)
                        _errors.Add($"MirAbortMsg in block '{block.Label}' requires byte string message but found {abortMsg.Message.Type}.");
                    break;
                case MirAbort:
                case MirUnreachable:
                    break;

                case null:
                    break;

                default:
                    _errors.Add($"Terminator '{block.Terminator.GetType().Name}' in block '{block.Label}' is not recognised.");
                    break;
            }
        }
    }

    private void CheckPhis(MirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                if (phi.Inputs.Count == 0)
                {
                    _errors.Add($"Phi in block '{block.Label}' has no incoming values.");
                    continue;
                }

                var preds = _predecessors[block];
                foreach (var (incomingBlock, value) in phi.Inputs)
                {
                    if (!preds.Contains(incomingBlock))
                        _errors.Add($"Phi in block '{block.Label}' receives input from non-predecessor '{incomingBlock.Label}'.");

                    if (value is null)
                        _errors.Add($"Phi in block '{block.Label}' has null incoming value.");
                }
            }
        }
    }

    private void CheckInstructions(MirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                switch (inst)
                {
                    case MirStructPack pack:
                        if (pack.Type is MirStructType structType)
                        {
                            if (structType.Fields.Count != pack.Fields.Count)
                                _errors.Add($"StructPack in block '{block.Label}' has {pack.Fields.Count} fields but type expects {structType.Fields.Count}.");
                        }
                        else
                        {
                            _errors.Add($"StructPack in block '{block.Label}' does not produce a struct type.");
                        }
                        break;

                    case MirStructSet set:
                        if (set.Type is MirStructType setStructType)
                        {
                            if (set.Index < 0 || set.Index >= setStructType.Fields.Count)
                                _errors.Add($"StructSet in block '{block.Label}' references invalid field index {set.Index}.");
                        }
                        break;

                    case MirArrayGet arrayGet:
                        if (arrayGet.Array.Type is not MirArrayType)
                            _errors.Add($"ArrayGet in block '{block.Label}' operates on non-array value.");
                        break;

                    case MirArraySet arraySet:
                        if (arraySet.Array.Type is not MirArrayType)
                            _errors.Add($"ArraySet in block '{block.Label}' operates on non-array value.");
                        break;

                    case MirMapGet mapGet:
                        if (mapGet.Map.Type is not MirMapType)
                            _errors.Add($"MapGet in block '{block.Label}' operates on non-map value.");
                        break;

                    case MirMapSet mapSet:
                        if (mapSet.Map.Type is not MirMapType)
                            _errors.Add($"MapSet in block '{block.Label}' operates on non-map value.");
                        break;

                    case MirStaticFieldStore staticStore:
                        if (!TypesCompatible(staticStore.FieldType, staticStore.Value.Type))
                            _errors.Add($"StaticFieldStore in block '{block.Label}' stores value of incompatible type.");
                        break;

                    case MirMapDelete mapDel:
                        if (mapDel.Map.Type is not MirMapType)
                            _errors.Add($"MapDelete in block '{block.Label}' operates on non-map value.");
                        break;

                    case MirGuardNull guardNull when guardNull.Fail == MirGuardFail.Branch && guardNull.FailTarget is null:
                        _errors.Add($"GuardNull in block '{block.Label}' uses branch failure but has no target block.");
                        break;

                    case MirGuardBounds guardBounds when guardBounds.Fail == MirGuardFail.Branch && guardBounds.FailTarget is null:
                        _errors.Add($"GuardBounds in block '{block.Label}' uses branch failure but has no target block.");
                        break;

                    case MirCheckedAdd checkedAdd when checkedAdd.Fail == MirGuardFail.Branch && checkedAdd.FailTarget is null:
                        _errors.Add($"CheckedAdd in block '{block.Label}' uses branch failure but has no target block.");
                        break;

                    case MirCheckedSub checkedSub when checkedSub.Fail == MirGuardFail.Branch && checkedSub.FailTarget is null:
                        _errors.Add($"CheckedSub in block '{block.Label}' uses branch failure but has no target block.");
                        break;

                    case MirCheckedMul checkedMul when checkedMul.Fail == MirGuardFail.Branch && checkedMul.FailTarget is null:
                        _errors.Add($"CheckedMul in block '{block.Label}' uses branch failure but has no target block.");
                        break;

                    case MirSyscall syscall:
                        if (!TryGetSyscallSignature(syscall, out var signature))
                        {
                            _errors.Add($"Syscall {syscall.Category}.{syscall.Name} in block '{block.Label}' is not recognised.");
                            break;
                        }

                        if (syscall.Arguments.Count != signature.Parameters.Count)
                        {
                            _errors.Add($"Syscall {syscall.Category}.{syscall.Name} expects {signature.Parameters.Count} arguments but block '{block.Label}' provides {syscall.Arguments.Count}.");
                        }
                        else
                        {
                            for (int i = 0; i < signature.Parameters.Count; i++)
                            {
                                if (!TypesCompatible(signature.Parameters[i], syscall.Arguments[i].Type))
                                    _errors.Add($"Argument {i} for syscall {syscall.Category}.{syscall.Name} in block '{block.Label}' has incompatible type.");
                            }
                        }

                        if (!TypesCompatible(signature.ReturnType, syscall.Type))
                            _errors.Add($"Syscall {syscall.Category}.{syscall.Name} in block '{block.Label}' returns incompatible type.");

                        if (syscall.Effect != signature.Effect)
                            _errors.Add($"Syscall {syscall.Category}.{syscall.Name} in block '{block.Label}' carries effect '{syscall.Effect}' but catalog expects '{signature.Effect}'.");
                        break;
                }

                if (inst is MirTokenSeed)
                {
                    if (!ReferenceEquals(block, function.Entry))
                        _errors.Add($"Token seed may only appear in the entry block but was found in '{block.Label}'.");
                    continue;
                }

                // Enforce token discipline according to effect classification.
                var effect = inst.Effect;
                var requiresToken = (effect & MirEffect.Memory) != 0 || effect.HasFlag(MirEffect.Abort);

                if (requiresToken)
                {
                    if (!inst.ConsumesMemoryToken || !inst.ProducesMemoryToken)
                        _errors.Add($"Instruction '{inst.GetType().Name}' in block '{block.Label}' has effect '{effect}' but is not linked to the memory token chain.");
                }
                else if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
                {
                    _errors.Add($"Instruction '{inst.GetType().Name}' in block '{block.Label}' is tokenised despite having no observable side effects.");
                }
            }
        }
    }

    private void CheckDominance(MirFunction function)
    {
        var definitions = BuildDefinitionIndex(function);

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
            {
                foreach (var (incomingBlock, value) in phi.Inputs)
                {
                    if (!ValidateValueDefined(value, block, definitions))
                        continue;

                    if (IsConstant(value))
                        continue;

                    var defBlock = definitions[value];
                    if (!_dominators.TryGetValue(incomingBlock, out var dominators) || !dominators.Contains(defBlock))
                        _errors.Add($"Value defined in '{defBlock.Label}' feeding phi in block '{block.Label}' does not dominate predecessor '{incomingBlock.Label}'.");
                }
            }

            foreach (var inst in block.Instructions)
            {
                foreach (var operand in EnumerateOperands(inst))
                    ValidateValueDominates(operand, block, definitions);
            }

            foreach (var operand in EnumerateTerminatorOperands(block.Terminator))
                ValidateValueDominates(operand, block, definitions);
        }
    }

    private void CheckTokenFlow(MirFunction function)
    {
        if (function.Entry.Instructions.Count == 0 || function.Entry.Instructions[0] is not MirTokenSeed seed)
        {
            _errors.Add($"Entry block for function '{function.Name}' must begin with a token seed instruction.");
            return;
        }

        if (!ReferenceEquals(seed.Token, function.EntryToken))
            _errors.Add($"Entry token seed for '{function.Name}' is not wired to the function's EntryToken property.");

        var incomingTokens = new Dictionary<MirBlock, List<MirValue>>();
        foreach (var block in function.Blocks)
            incomingTokens[block] = new List<MirValue>();

        incomingTokens[function.Entry].Add(function.EntryToken);

        var tokenPhis = new Dictionary<MirBlock, MirPhi>();
        var queue = new Queue<MirBlock>();
        var inQueue = new HashSet<MirBlock> { function.Entry };
        queue.Enqueue(function.Entry);

        while (queue.Count > 0)
        {
            var block = queue.Dequeue();
            inQueue.Remove(block);

            var incoming = incomingTokens[block];
            MirValue currentToken;

            var tokenPhi = block.Phis.FirstOrDefault(p => p.Type is MirTokenType);
            if (tokenPhi is not null)
            {
                tokenPhis[block] = tokenPhi;
                currentToken = tokenPhi;
            }
            else
            {
                if (incoming.Count == 0)
                {
                    _errors.Add($"Block '{block.Label}' has no incoming memory token.");
                    currentToken = function.EntryToken;
                }
                else if (incoming.Count > 1)
                {
                    _errors.Add($"Block '{block.Label}' receives multiple memory tokens but lacks a token phi.");
                    currentToken = incoming[0];
                }
                else
                {
                    currentToken = incoming[0];
                }
            }

            foreach (var inst in block.Instructions)
            {
                if (inst.ConsumesMemoryToken)
                {
                    if (inst.TokenInput is null)
                        _errors.Add($"Instruction '{inst.GetType().Name}' in block '{block.Label}' consumes a token but TokenInput is null.");
                    else if (!ReferenceEquals(inst.TokenInput, currentToken))
                        _errors.Add($"Instruction '{inst.GetType().Name}' in block '{block.Label}' consumes unexpected memory token.");
                }

                if (inst.ProducesMemoryToken)
                {
                    if (inst.TokenOutput is null)
                        _errors.Add($"Instruction '{inst.GetType().Name}' in block '{block.Label}' produces a token but TokenOutput is null.");
                    else
                        currentToken = inst.TokenOutput;
                }
            }

            foreach (var successor in MirControlFlow.GetSuccessors(block))
            {
                if (!incomingTokens.ContainsKey(successor))
                {
                    Console.WriteLine($"[MIR VERIFY] Unknown successor '{successor?.Label ?? "<null>"}' from block '{block.Label}' in function '{function.Name}'.");
                    foreach (var b in function.Blocks)
                        Console.WriteLine($"    FUN BLOCK {b.Label}");
                    throw new KeyNotFoundException("Successor block missing from incoming token map.");
                }
                if (!ContainsReference(incomingTokens[successor], currentToken))
                {
                    incomingTokens[successor].Add(currentToken);
                    if (!inQueue.Contains(successor))
                    {
                        queue.Enqueue(successor);
                        inQueue.Add(successor);
                    }
                }
            }
        }

        foreach (var (block, tokenPhi) in tokenPhis)
        {
            var incoming = incomingTokens[block];
            var expectedTokens = new List<MirValue>();
            foreach (var token in incoming)
            {
                if (!ContainsReference(expectedTokens, token))
                    expectedTokens.Add(token);
            }

            foreach (var (_, value) in tokenPhi.Inputs)
            {
                if (!ContainsReference(incoming, value))
                    _errors.Add($"Token phi in block '{block.Label}' has input not provided by predecessors.");
            }

            if (expectedTokens.Count != tokenPhi.Inputs.Count)
                _errors.Add($"Token phi in block '{block.Label}' does not match predecessor count.");
        }
    }

    private static bool ContainsReference(List<MirValue> values, MirValue value)
    {
        foreach (var entry in values)
        {
            if (ReferenceEquals(entry, value))
                return true;
        }

        return false;
    }

    private Dictionary<MirValue, MirBlock> BuildDefinitionIndex(MirFunction function)
    {
        var definitions = new Dictionary<MirValue, MirBlock>();

        definitions[function.EntryToken] = function.Entry;

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
                definitions[phi] = block;

            foreach (var inst in block.Instructions)
            {
                definitions[inst] = block;

                if (inst is MirTokenSeed seed)
                    definitions[seed.Token] = block;

                if (inst.TokenOutput is not null)
                    definitions[inst.TokenOutput] = block;
            }
        }

        return definitions;
    }

    private bool ValidateValueDominates(MirValue? value, MirBlock useBlock, Dictionary<MirValue, MirBlock> definitions)
    {
        if (value is null)
            return true;

        if (IsConstant(value))
            return true;

        if (!ValidateValueDefined(value, useBlock, definitions))
            return false;

        var defBlock = definitions[value];
        if (!_dominators.TryGetValue(useBlock, out var domSet) || !domSet.Contains(defBlock))
        {
            _errors.Add($"Value defined in '{defBlock.Label}' does not dominate its use in block '{useBlock.Label}'.");
            return false;
        }

        return true;
    }

    private bool ValidateValueDefined(MirValue value, MirBlock useBlock, Dictionary<MirValue, MirBlock> definitions)
    {
        if (IsConstant(value))
            return true;

        if (!definitions.TryGetValue(value, out var defBlock))
        {
            _errors.Add($"Value '{value.GetType().Name}' used in block '{useBlock.Label}' has no defining block.");
            return false;
        }

        return true;
    }

    private static bool IsConstant(MirValue value)
        => value is MirConstInt or MirConstBool or MirConstByteString or MirConstBuffer;

    private static IEnumerable<MirValue> EnumerateOperands(MirInst inst)
    {
        switch (inst)
        {
            case MirUnary unary:
                yield return unary.Operand;
                break;
            case MirConvert convert:
                yield return convert.Value;
                break;
            case MirBinary binary:
                yield return binary.Left;
                yield return binary.Right;
                break;
            case MirCompare compare:
                yield return compare.Left;
                yield return compare.Right;
                break;
            case MirStructPack pack:
                foreach (var field in pack.Fields)
                    yield return field;
                break;
            case MirStructGet get:
                yield return get.Object;
                break;
            case MirStructSet set:
                yield return set.Object;
                yield return set.Value;
                break;
            case MirArrayNew arrayNew:
                yield return arrayNew.Length;
                break;
            case MirArrayLen arrayLen:
                yield return arrayLen.Array;
                break;
            case MirArrayGet arrayGet:
                yield return arrayGet.Array;
                yield return arrayGet.Index;
                break;
            case MirArraySet arraySet:
                yield return arraySet.Array;
                yield return arraySet.Index;
                yield return arraySet.Value;
                break;
            case MirMapNew:
                break;
            case MirMapLen mapLen:
                yield return mapLen.Map;
                break;
            case MirMapGet mapGet:
                yield return mapGet.Map;
                yield return mapGet.Key;
                break;
            case MirMapSet mapSet:
                yield return mapSet.Map;
                yield return mapSet.Key;
                yield return mapSet.Value;
                break;
            case MirMapHas mapHas:
                yield return mapHas.Map;
                yield return mapHas.Key;
                break;
            case MirMapDelete mapDelete:
                yield return mapDelete.Map;
                yield return mapDelete.Key;
                break;
            case MirConcat concat:
                yield return concat.Left;
                yield return concat.Right;
                break;
            case MirSlice slice:
                yield return slice.Value;
                yield return slice.Start;
                yield return slice.Length;
                break;
            case MirBufferNew bufferNew:
                yield return bufferNew.Length;
                break;
            case MirBufferSet bufferSet:
                yield return bufferSet.Buffer;
                yield return bufferSet.Index;
                yield return bufferSet.Byte;
                break;
            case MirBufferCopy bufferCopy:
                yield return bufferCopy.Destination;
                yield return bufferCopy.Source;
                yield return bufferCopy.DestinationOffset;
                yield return bufferCopy.SourceOffset;
                yield return bufferCopy.Length;
                break;
            case MirModMul modMul:
                yield return modMul.Left;
                yield return modMul.Right;
                yield return modMul.Modulus;
                break;
            case MirModPow modPow:
                yield return modPow.Value;
                yield return modPow.Exponent;
                yield return modPow.Modulus;
                break;
            case MirPointerCall pointerCall:
                if (pointerCall.Pointer is not null)
                    yield return pointerCall.Pointer;
                foreach (var arg in pointerCall.Arguments)
                    yield return arg;
                break;
            case MirTry:
            case MirFinally:
                break;
            case MirCall call:
                foreach (var arg in call.Arguments)
                    yield return arg;
                break;
            case MirSyscall syscall:
                foreach (var arg in syscall.Arguments)
                    yield return arg;
                break;
            case MirGuardNull guardNull:
                yield return guardNull.Reference;
                break;
            case MirGuardBounds guardBounds:
                yield return guardBounds.Index;
                yield return guardBounds.Length;
                break;
            case MirCheckedAdd checkedAdd:
                yield return checkedAdd.Left;
                yield return checkedAdd.Right;
                break;
            case MirCheckedSub checkedSub:
                yield return checkedSub.Left;
                yield return checkedSub.Right;
                break;
            case MirCheckedMul checkedMul:
                yield return checkedMul.Left;
                yield return checkedMul.Right;
                break;
        }

        if (inst.ConsumesMemoryToken && inst.TokenInput is not null)
            yield return inst.TokenInput;
    }

    private static IEnumerable<MirValue> EnumerateTerminatorOperands(MirTerminator? terminator)
    {
        switch (terminator)
        {
            case MirReturn ret when ret.Value is not null:
                yield return ret.Value;
                break;
            case MirCondBranch cond:
                yield return cond.Condition;
                break;
            case MirSwitch @switch:
                yield return @switch.Key;
                break;
            case MirCompareBranch cmpBranch:
                yield return cmpBranch.Left;
                yield return cmpBranch.Right;
                break;
            case MirAbortMsg abortMsg:
                yield return abortMsg.Message;
                break;
        }
    }

    private static bool TypesCompatible(MirType expected, MirType actual)
    {
        if (expected is MirUnknownType || actual is MirUnknownType)
            return true;

        if (expected.GetType() != actual.GetType())
            return false;

        return expected switch
        {
            MirIntType expectedInt when actual is MirIntType actualInt
                => expectedInt.IsSigned == actualInt.IsSigned,
            MirArrayType expectedArray when actual is MirArrayType actualArray
                => TypesCompatible(expectedArray.ElementType, actualArray.ElementType),
            MirMapType expectedMap when actual is MirMapType actualMap
                => TypesCompatible(expectedMap.KeyType, actualMap.KeyType) && TypesCompatible(expectedMap.ValueType, actualMap.ValueType),
            MirStructType expectedStruct when actual is MirStructType actualStruct
                => expectedStruct.Fields.Count == actualStruct.Fields.Count
                   && expectedStruct.Fields.Zip(actualStruct.Fields, (e, a) => (e, a)).All(pair => TypesCompatible(pair.e, pair.a)),
            _ => true
        };
    }

    private void RequireBlock(MirBlock target, HashSet<MirBlock> knownBlocks, MirBlock source, string context)
    {
        if (!knownBlocks.Contains(target))
            _errors.Add($"Block '{source.Label}' references unknown block '{target.Label}' in {context}.");
    }

}
