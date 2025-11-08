using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Import;

internal sealed partial class HirMethodImporter
{
    private readonly struct SequencePointScope : IDisposable
    {
        private readonly HirMethodImporter _owner;
        private readonly bool _applied;

        public SequencePointScope(HirMethodImporter owner, bool applied)
        {
            _owner = owner;
            _applied = applied;
        }

        public void Dispose()
        {
            if (_applied)
                _owner.PopSequencePoint();
        }
    }

    private SequencePointScope InsertSequencePoint(SyntaxNode node)
    {
        if (_hirBuilder is null)
            return default;

        var location = node.GetLocation();
        if (!location.IsInSource)
            return new SequencePointScope(this, false);

        var span = location.GetLineSpan();
        var sourceSpan = new SourceSpan(
            span.Path,
            span.StartLinePosition.Line + 1,
            span.StartLinePosition.Character + 1,
            span.EndLinePosition.Line + 1,
            span.EndLinePosition.Character + 1);

        _hirSequencePoints.Push(sourceSpan);
        _hirBuilder.MarkLocation(sourceSpan);
        return new SequencePointScope(this, true);
    }

    private void PopSequencePoint()
    {
        if (_hirSequencePoints.Count > 0)
            _hirSequencePoints.Pop();

        _hirBuilder?.MarkLocation(_hirSequencePoints.Count > 0 ? _hirSequencePoints.Peek() : null);
    }

    private void AssignLocalSymbol(ILocalSymbol symbol, HirValue value)
    {
        if (_hirCurrentState is null)
            return;

        if (!_hirLocals.ContainsKey(symbol))
        {
            var localType = MapType(symbol.Type);
            _hirLocals[symbol] = new HirLocal(symbol.Name, localType);
        }

        var prepared = _hirTryScopes.Count > 0 ? EnsureLocalisedValue(value) : value;
        _hirCurrentState.Assign(symbol, prepared);
    }

    private void AssignParameterSymbol(IParameterSymbol symbol, HirValue value)
    {
        if (_hirCurrentState is null)
            return;

        var prepared = _hirTryScopes.Count > 0 ? EnsureLocalisedValue(value) : value;
        _hirCurrentState.Assign(symbol, prepared);
    }

    private HirArgument? TryGetHirArgument(IParameterSymbol symbol)
        => _hirArguments.TryGetValue(symbol, out var argument) ? argument : null;

    private bool TryGetSymbolValue(ISymbol symbol, out HirValue value)
    {
        if (_hirCurrentState is not null && _hirCurrentState.TryGetValue(symbol, out value))
            return true;

        value = null!;
        return false;
    }

    private HirValue GetSymbolValue(ISymbol symbol)
    {
        if (TryGetSymbolValue(symbol, out var value))
            return value;

        throw new NotSupportedException($"Value for symbol '{symbol.Name}' is not available in SSA state.");
    }

    private HirSsaState MergeStates(
        HirBlock mergeBlock,
        (HirBlock Block, HirSsaState State) left,
        (HirBlock Block, HirSsaState State) right,
        HirSsaState baseline)
        => MergeStates(mergeBlock, new[] { left, right }, baseline);

    private HirSsaState MergeStates(
        HirBlock mergeBlock,
        IReadOnlyList<(HirBlock Block, HirSsaState State)> states,
        HirSsaState baseline)
    {
        var builder = _hirBuilder ?? throw new InvalidOperationException("HIR builder is not initialized.");
        if (states is null || states.Count == 0)
            return baseline.Clone();

        if (states.Count == 1)
        {
            var copy = baseline.Clone();
            foreach (var symbol in states[0].State.Symbols)
            {
                if (states[0].State.TryGetValue(symbol, out var value))
                    copy.Assign(symbol, value!);
            }
            return copy;
        }

        var merged = baseline.Clone();
        var symbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        foreach (var symbol in baseline.Symbols)
            symbols.Add(symbol);
        foreach (var state in states)
        {
            foreach (var symbol in state.State.Symbols)
                symbols.Add(symbol);
        }

        foreach (var symbol in symbols)
        {
            var incoming = new List<(HirBlock Block, HirValue Value)>();
            foreach (var state in states)
            {
                if (state.State.TryGetValue(symbol, out var value))
                    incoming.Add((state.Block, value!));
            }

            if (incoming.Count == 0)
                continue;

            bool allSame = true;
            var representative = incoming[0].Value;
            for (int i = 1; i < incoming.Count; i++)
            {
                if (!ReferenceEquals(representative, incoming[i].Value))
                {
                    allSame = false;
                    break;
                }
            }

            if (allSame)
            {
                merged.Assign(symbol, MaterialiseValue(representative));
                continue;
            }

            var phi = new HirPhi(representative.Type);
            if (symbol is ILocalSymbol localSymbol && _hirLocals.TryGetValue(localSymbol, out var hirLocal))
            {
                phi.IsLocalPhi = true;
                phi.Local = hirLocal;
            }
            foreach (var (block, value) in incoming)
            {
                var materialised = MaterialiseValue(value);
                phi.AddIncoming(block, materialised);
            }

            builder.SetCurrentBlock(mergeBlock);
            builder.AppendPhi(phi);
            if (!phi.IsLocalPhi)
            {
                if (s_dumpHirPhiDebug)
                {
                    Console.WriteLine($"[HIR-PHI] Merge block={mergeBlock.Label} symbol={symbol?.Name ?? "<null>"} initial local flag={phi.IsLocalPhi}");
                }
                TryPromoteLocalPhi(phi);
            }
            else if (s_dumpHirPhiDebug)
            {
                Console.WriteLine($"[HIR-PHI] Already local block={mergeBlock.Label} local={phi.Local?.Name}");
            }
            if (phi.IsLocalPhi && phi.Local is not null)
                TrackLocalValue(phi.Local, phi);
            merged.Assign(symbol, phi);
        }

        var localSet = new HashSet<HirLocal>(ReferenceEqualityComparer.Instance);
        foreach (var local in baseline.LocalSymbols)
            localSet.Add(local);
        foreach (var state in states)
        {
            foreach (var local in state.State.LocalSymbols)
                localSet.Add(local);
        }

        foreach (var local in localSet)
        {
            var incoming = new List<(HirBlock Block, HirValue Value)>();
            foreach (var state in states)
            {
                if (state.State.TryGetValue(local, out var value))
                    incoming.Add((state.Block, value!));
            }
            if (s_dumpHirPhiDebug)
                Console.WriteLine($"[HIR-PHI] Local candidate block={mergeBlock.Label} local={local.Name} incoming={incoming.Count}");

            if (incoming.Count == 0)
                continue;

            bool allSame = true;
            var representative = incoming[0].Value;
            for (int i = 1; i < incoming.Count; i++)
            {
                if (!ReferenceEquals(representative, incoming[i].Value))
                {
                    allSame = false;
                    break;
                }
            }

            if (incoming.Count == 1 && allSame)
            {
                merged.Assign(local, MaterialiseValue(representative));
                continue;
            }

            var phi = new HirPhi(local.Type)
            {
                IsLocalPhi = true,
                Local = local
            };
            if (s_dumpHirPhiDebug)
                Console.WriteLine($"[HIR-PHI] Creating local phi block={mergeBlock.Label} local={local.Name} incoming={incoming.Count}");
            foreach (var (block, value) in incoming)
            {
                var materialised = MaterialiseValue(value);
                phi.AddIncoming(block, materialised);
            }

            builder.SetCurrentBlock(mergeBlock);
            builder.AppendPhi(phi);
            TrackLocalValue(local, phi);
            merged.Assign(local, phi);
        }

        return merged;
    }

    private void TryPromoteLocalPhi(HirPhi phi)
    {
        if (s_dumpHirPhiDebug)
        {
            var details = string.Join(
                ", ",
                phi.Inputs.Select(input =>
                {
                    var hasLocal = _hirLocalValueMap.TryGetValue(input.Value, out var mapped)
                        ? mapped.Name
                        : "<none>";
                    return $"{input.Value.GetType().Name}->{hasLocal}";
                }));
            Console.WriteLine($"[HIR-PHI] Inspect Block={_hirBuilder?.CurrentBlock?.Label ?? "<unknown>"} Inputs={details}");
        }

        HirLocal? candidate = null;
        foreach (var (_, value) in phi.Inputs)
        {
            if (!_hirLocalValueMap.TryGetValue(value, out var local))
            {
                if (s_dumpHirPhiDebug)
                    Console.WriteLine($"[HIR-PHI] Input {value.GetType().Name} has no local mapping.");
                return;
            }
            if (candidate is null)
            {
                candidate = local;
                continue;
            }

            if (!ReferenceEquals(candidate, local))
                return;
        }

        if (candidate is null)
            return;

        phi.IsLocalPhi = true;
        phi.Local = candidate;
        _hirLocalValueMap[phi] = candidate;
    }

    private static int GetFieldIndex(HirStructType structType, string fieldName)
    {
        for (int i = 0; i < structType.Fields.Count; i++)
        {
            if (string.Equals(structType.Fields[i].Name, fieldName, StringComparison.Ordinal))
                return i;
        }

        throw new NotSupportedException($"Field '{fieldName}' not found in struct type.");
    }

    private HirValue EmitStaticFieldLoad(IFieldSymbol field)
    {
        ArgumentNullException.ThrowIfNull(field);
        var builder = _hirBuilder ?? throw new InvalidOperationException("HIR builder is not initialized.");

        var slot = _context.AddStaticField(field);
        var fieldType = MapType(field.Type);
        var load = new HirLoadStaticField(slot, fieldType, GetStaticFieldDisplayName(field));
        builder.Append(load);
        return load;
    }

    private HirValue EnsureLocalisedValue(HirValue value)
    {
        if (_hirTryScopes.Count == 0)
            return value;

        if (_hirBuilder is null)
            return value;

        switch (value)
        {
            case HirLoadLocal:
            case HirLoadArgument:
            case HirConstInt:
            case HirConstBool:
            case HirConstByteString:
            case HirConstBuffer:
            case HirConstNull:
                return value;
            case HirArrayGet arrayGet:
                {
                    var array = EnsureLocalisedValue(arrayGet.Array);
                    var index = EnsureLocalisedValue(arrayGet.Index);
                    var clone = new HirArrayGet(array, index, arrayGet.Type);
                    _hirBuilder.Append(clone);
                    var captured = CreateSyntheticLocal("arrget_capture", clone.Type);
                    AppendStoreLocal(new HirStoreLocal(captured, clone));
                    return LoadLocal(captured);
                }
            case HirArrayNew arrayNew:
                {
                    var length = EnsureLocalisedValue(arrayNew.Length);
                    var clone = new HirArrayNew(length, arrayNew.Type);
                    _hirBuilder.Append(clone);
                    var captured = CreateSyntheticLocal("arrnew_capture", clone.Type);
                    AppendStoreLocal(new HirStoreLocal(captured, clone));
                    return LoadLocal(captured);
                }
        }

        var scopeCapture = CreateSyntheticLocal("scope_capture", value.Type);
        AppendStoreLocal(new HirStoreLocal(scopeCapture, value));
        return LoadLocal(scopeCapture);
    }

    private HirValue EmitStaticFieldStore(IFieldSymbol field, HirValue value)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);

        var builder = _hirBuilder ?? throw new InvalidOperationException("HIR builder is not initialized.");
        var fieldType = MapType(field.Type);
        var coerced = EnsureType(value, fieldType);

        var slot = _context.AddStaticField(field);
        var store = new HirStoreStaticField(slot, coerced, fieldType, GetStaticFieldDisplayName(field));
        builder.Append(store);
        return coerced;
    }

    private void StoreScopeState(HirTryScopeContext scope, HirSsaState? state)
    {
        if (scope is null || state is null || _hirBuilder is null)
            return;

        foreach (var symbol in state.Symbols)
        {
            if (!state.TryGetValue(symbol, out var value) || value is null)
                continue;

            var prepared = EnsureLocalisedValue(value);
            var slot = EnsureScopeSlot(scope, symbol, prepared.Type);
            if (prepared is HirLoadLocal loadLocal && ReferenceEquals(loadLocal.Local, slot))
                continue;
            var store = new HirStoreLocal(slot, prepared);
            AppendStoreLocal(store);
        }
    }

    private HirSsaState LoadScopeState(HirTryScopeContext scope, HirSsaState? baseline)
    {
        var loaded = baseline?.Clone() ?? new HirSsaState();
        if (scope is null || _hirBuilder is null)
            return loaded;

        foreach (var (symbol, slot) in scope.StateSlots)
        {
            if (slot.Name.StartsWith("try_state_", StringComparison.Ordinal))
                continue;

            var load = LoadLocal(slot);
            loaded.Assign(symbol, load);
        }

        return loaded;
    }

    private HirLocal EnsureScopeSlot(HirTryScopeContext scope, ISymbol symbol, HirType type)
    {
        if (scope.StateSlots.TryGetValue(symbol, out var existing))
            return existing;

        var resolved = type ?? HirType.VoidType;
        if (resolved.Equals(HirType.VoidType))
            resolved = HirType.UnknownType;

        var slot = CreateSyntheticLocal($"try_state_{symbol.Name}", resolved);
        scope.StateSlots[symbol] = slot;
        return slot;
    }

    private HirValue MaterialiseValue(HirValue value)
    {
        if (_hirBuilder is null)
            return value;

        if (value is HirPhi)
        {
            var temp = CreateSyntheticLocal("phi_capture", value.Type);
            var store = new HirStoreLocal(temp, value);
            AppendStoreLocal(store);
            return LoadLocal(temp);
        }

        return value;
    }

    private static string GetStaticFieldDisplayName(IFieldSymbol field)
    {
        ArgumentNullException.ThrowIfNull(field);
        return TrimGlobal(field.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }

    private HirClosureContext CreateClosureContext(
        IReadOnlyList<ISymbol> capturedSymbols,
        string closureName)
    {
        if (capturedSymbols.Count == 0)
            return new HirClosureContext(new HirStructType(Array.Empty<HirField>()), new Dictionary<ISymbol, HirField>(SymbolEqualityComparer.Default));

        var fields = new List<HirField>(capturedSymbols.Count);
        var mapping = new Dictionary<ISymbol, HirField>(SymbolEqualityComparer.Default);

        foreach (var symbol in capturedSymbols)
        {
            var type = symbol switch
            {
                ILocalSymbol local => MapType(local.Type),
                IParameterSymbol parameter => MapType(parameter.Type),
                _ => HirType.UnknownType
            };

            var field = new HirField(symbol.Name, type, type.Kind is HirTypeKind.Struct or HirTypeKind.Array or HirTypeKind.Map);
            fields.Add(field);
            mapping[symbol] = field;
        }

        var closureStruct = new HirStructType(fields);
        return new HirClosureContext(closureStruct, mapping);
    }

    private void EnsureNonNull(HirValue value)
    {
        if (_hirBuilder is null || value is null)
            return;

        var kind = value.Type?.Kind ?? HirTypeKind.Unknown;
        switch (kind)
        {
            case HirTypeKind.Int:
            case HirTypeKind.Bool:
            case HirTypeKind.Void:
            case HirTypeKind.Unknown:
                return;
        }

        var check = new HirNullCheck(value, HirFailPolicy.Abort);
        _hirBuilder.Append(check);
    }

    private HirValue EmitHirDefault(HirType type)
        => type switch
        {
            HirBoolType => EmitConstant(false),
            HirIntType => EmitConstant(BigInteger.Zero),
            HirByteStringType => EmitConstant(string.Empty),
            HirBufferType => EmitConstant(Array.Empty<byte>()),
            _ => EmitConstant(null)
        };

    private HirValue GetThisValue()
    {
        if (_hirThisArgument is null)
            throw new InvalidOperationException("Attempted to access 'this' in a static context.");

        return _hirThisArgument!;
    }

    private HirValue EnsureType(HirValue? value, HirType targetType)
    {
        if (value is null)
            return EmitHirDefault(targetType);

        if (_hirBuilder is null || targetType is null)
            return value;

        if (Equals(value.Type, targetType))
            return value;

        if (value.Type is HirIntType && targetType is HirIntType intTarget)
        {
            value.SetType(intTarget);
            return value;
        }

        var convert = new HirConvert(HirConvKind.Narrow, value, targetType);
        _hirBuilder.Append(convert);
        return convert;
    }

    private HirValue EmitEventInvocation(IEventSymbol eventSymbol, IReadOnlyList<HirValue> arguments, SyntaxNode syntax)
    {
        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder is not initialized.");

        if (!HirIntrinsicCatalog.TryResolve("Neo.SmartContract.Framework.Services.Runtime.Notify", out var metadata))
            throw new NotSupportedException("Runtime.Notify intrinsic metadata is unavailable.");

        var span = GetSourceSpan(syntax);

        if (span is not null)
            _hirBuilder.MarkLocation(span);
        var eventName = EnsureType(EmitConstant(eventSymbol.GetDisplayName()), HirType.ByteStringType);

        var payload = BuildArgumentArray(arguments, span);

        var notifyCall = new HirIntrinsicCall(
            metadata.Category,
            metadata.Name,
            new HirValue[] { eventName, payload },
            metadata)
        {
            Span = span
        };

        if (span is not null)
            _hirBuilder.MarkLocation(span);
        _hirBuilder.Append(notifyCall);
        return EmitHirDefault(HirType.VoidType);
    }

    private HirValue BuildArgumentArray(IReadOnlyList<HirValue> arguments, SourceSpan? span)
    {
        if (span is not null)
            _hirBuilder!.MarkLocation(span);
        var lengthValue = EnsureType(EmitConstant(arguments.Count), HirType.IntType);

        var array = new HirArrayNew(lengthValue, HirType.UnknownType)
        {
            Span = span
        };
        _hirBuilder!.Append(array);

        for (int i = 0; i < arguments.Count; i++)
        {
            if (span is not null)
                _hirBuilder.MarkLocation(span);
            var indexValue = EnsureType(EmitConstant(i), HirType.IntType);

            if (span is not null)
                _hirBuilder.MarkLocation(span);
            var element = EnsureType(arguments[i], HirType.UnknownType);
            var set = new HirArraySet(array, indexValue, element)
            {
                Span = span
            };
            _hirBuilder.Append(set);
        }

        return array;
    }

    private static SourceSpan? GetSourceSpan(SyntaxNode node)
    {
        var location = node.GetLocation();
        if (!location.IsInSource)
            return null;

        var span = location.GetLineSpan();
        return new SourceSpan(
            span.Path,
            span.StartLinePosition.Line + 1,
            span.StartLinePosition.Character + 1,
            span.EndLinePosition.Line + 1,
            span.EndLinePosition.Character + 1);
    }

    private void EmitExpressionBodyReturn(SemanticModel model, ExpressionSyntax expression, bool returnsVoid, HirType? expectedType)
    {
        var value = LowerExpression(model, expression);
        if (_hirBuilder is null)
            return;

        if (returnsVoid)
        {
            EmitReturn(null);
            return;
        }

        var ensured = EnsureType(value, expectedType ?? MapType(_symbol.ReturnType));
        EmitReturn(ensured);
    }

    private void EnsureBlockTerminated(bool terminated, bool returnsVoid, HirType? expectedType)
    {
        if (terminated || _hirBuilder is null)
            return;

        if (returnsVoid)
        {
            EmitReturn(null);
            return;
        }

        throw new NotSupportedException($"Non-void method '{_symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}' is missing a terminating return statement.");
    }

    private void EmitReturn(HirValue? value)
    {
        if (_hirBuilder is null)
            return;

        if (_hirTryScopes.Count == 0)
        {
            _hirBuilder.AppendTerminator(new HirReturn(value));
            return;
        }

        if (!_symbol.ReturnsVoid)
        {
            if (value is null)
                throw new NotSupportedException("Return value cannot be null for non-void method.");

            var slot = EnsureReturnValueSlot();
            var store = new HirStoreLocal(slot, value);
            AppendStoreLocal(store);
        }

        var returnTarget = EnsureReturnDispatchBlock();
        _ = EmitLeaveTo(returnTarget, targetTryDepth: 0);
    }

    private HirBlock EnsureReturnDispatchBlock()
    {
        if (_hirReturnBlock is not null)
            return _hirReturnBlock;

        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder not initialised.");

        var current = _hirBuilder.CurrentBlock;
        var dispatch = _hirBuilder.CreateBlock(NewBlockLabel("return_dispatch"));
        _hirBuilder.SetCurrentBlock(dispatch);

        if (_symbol.ReturnsVoid)
        {
            _hirBuilder.AppendTerminator(new HirReturn(null));
        }
        else
        {
            var slot = EnsureReturnValueSlot();
            var load = new HirLoadLocal(slot);
            _hirBuilder.Append(load);
            _hirBuilder.AppendTerminator(new HirReturn(load));
        }

        _hirBuilder.SetCurrentBlock(current);
        _hirReturnBlock = dispatch;
        return dispatch;
    }

    private HirLocal EnsureReturnValueSlot()
    {
        if (_hirReturnValueSlot is not null)
            return _hirReturnValueSlot;

        var type = MapType(_symbol.ReturnType);
        _hirReturnValueSlot = CreateSyntheticLocal("ret", type);
        return _hirReturnValueSlot;
    }

    private HirBlock EmitLeaveTo(HirBlock target, int targetTryDepth)
    {
        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder not initialised.");

        var scopesToExit = _hirTryScopes.Count - targetTryDepth;
        if (scopesToExit <= 0)
        {
            AppendBranch(target);
            return _hirBuilder.CurrentBlock;
        }

        var contexts = _hirTryScopes.ToArray();
        if (scopesToExit > contexts.Length)
            throw new InvalidOperationException("Cannot leave more try scopes than currently active.");

        var dispatchTarget = target;
        for (int i = 1; i < scopesToExit; i++)
        {
            dispatchTarget = EnsureLeaveDispatch(contexts[i], dispatchTarget);
        }

        var currentScope = contexts[0];
        StoreScopeState(currentScope, _hirCurrentState);
        var currentBlock = _hirBuilder.CurrentBlock;
        _hirBuilder.AppendTerminator(new HirLeave(currentScope.Scope, dispatchTarget));
        return currentBlock;
    }

    private HirBlock EnsureLeaveDispatch(HirTryScopeContext scope, HirBlock target)
    {
        if (scope.LeaveDispatch.TryGetValue(target, out var dispatch))
            return dispatch;

        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder not initialised.");

        var current = _hirBuilder.CurrentBlock;
        var dispatchBlock = _hirBuilder.CreateBlock(NewBlockLabel("leave_scope"));
        _hirBuilder.SetCurrentBlock(dispatchBlock);
        _hirBuilder.AppendTerminator(new HirLeave(scope.Scope, target));
        _hirBuilder.SetCurrentBlock(current);
        scope.LeaveDispatch[target] = dispatchBlock;
        return dispatchBlock;
    }

    private string NewBlockLabel(string prefix)
    {
        if (!_hirBlockCounters.TryGetValue(prefix, out var count))
        {
            _hirBlockCounters[prefix] = 1;
            return prefix;
        }

        var label = $"{prefix}_{count}";
        _hirBlockCounters[prefix] = count + 1;
        return label;
    }

    private HirLocal CreateSyntheticLocal(string prefix, HirType type)
        => new($"{prefix}_{_hirTempCounter++}", type);

    private HirValue LoadLocal(HirLocal local)
    {
        var load = new HirLoadLocal(local);
        if (s_traceHirLocals && _hirBuilder is not null)
        {
            var blockLabel = _hirBuilder.CurrentBlock?.Label ?? "<null>";
            Console.WriteLine($"[HIR-LOAD] Block={blockLabel} Local={local.Name}");
        }
        _hirBuilder!.Append(load);
        TrackLocalValue(local, load);
        return load;
    }

    private void TrackLocalValue(HirLocal local, HirValue value)
    {
        if (value is null)
            return;
        _hirLocalValueMap[value] = local;
    }

    private void AppendStoreLocal(HirStoreLocal store)
    {
        _hirBuilder!.Append(store);
        TrackLocalValue(store.Local, store.Value);
        AssignSyntheticLocal(store.Local, store.Value);
    }

    private void AssignSyntheticLocal(HirLocal local, HirValue value)
    {
        if (_hirCurrentState is null)
            return;
        _hirCurrentState.Assign(local, value);
        if (s_traceHirLocals)
        {
            var blockLabel = _hirBuilder?.CurrentBlock?.Label ?? "<null>";
            Console.WriteLine($"[HIR-LOCAL] Assign {local.Name} in block {blockLabel}");
        }
    }

    private void LowerLocalDeclaration(SemanticModel model, LocalDeclarationStatementSyntax declaration)
        => LowerVariableDeclaration(model, declaration.Declaration);

    private void LowerVariableDeclaration(SemanticModel model, VariableDeclarationSyntax declaration)
    {
        foreach (var variable in declaration.Variables)
        {
            if (model.GetDeclaredSymbol(variable) is not ILocalSymbol symbol)
                continue;

            HirValue initializer;
            if (variable.Initializer?.Value is LambdaExpressionSyntax lambdaInitializer)
            {
                if (!TryRegisterLambda(symbol, lambdaInitializer))
                    throw new NotSupportedException("Lambda expressions with captures or block bodies are not yet supported in HIR conversion.");

                initializer = EmitHirDefault(MapType(symbol.Type));
            }
            else
            {
                _lambdaAssignments.Remove(symbol);
                initializer = variable.Initializer is null
                    ? EmitHirDefault(MapType(symbol.Type))
                    : LowerExpression(model, variable.Initializer.Value);
            }

            AssignLocalSymbol(symbol, initializer);
        }
    }

    private BigInteger ConstantToBigInteger(object? value) => value switch
    {
        null => BigInteger.Zero,
        sbyte b => new BigInteger(b),
        byte b => new BigInteger(b),
        short s => new BigInteger(s),
        ushort s => new BigInteger(s),
        int i => new BigInteger(i),
        uint i => new BigInteger(i),
        long l => new BigInteger(l),
        ulong l => new BigInteger(l),
        BigInteger big => big,
        bool boolValue => boolValue ? BigInteger.One : BigInteger.Zero,
        char ch => new BigInteger(ch),
        _ => throw new NotSupportedException($"Unsupported switch constant value '{value}'.")
    };

    private void AppendBranch(HirBlock target)
        => _hirBuilder!.AppendTerminator(new HirBranch(target));

    private void AppendConditional(HirValue condition, HirBlock trueTarget, HirBlock falseTarget)
        => _hirBuilder!.AppendTerminator(new HirConditionalBranch(condition, trueTarget, falseTarget));

    private bool TryRegisterLambda(ISymbol target, LambdaExpressionSyntax lambda)
    {
        var parameters = GetLambdaParameters(lambda);
        if (parameters.IsDefault)
            return false;

        if (lambda.Body is not ExpressionSyntax)
            throw CompilationException.UnsupportedSyntax(lambda.Body, "Block-bodied lambdas are not yet supported in HIR conversion. Please refactor the lambda to use an expression body.");

        if (!IsCaptureFreeLambda(lambda, parameters))
            throw CompilationException.UnsupportedSyntax(lambda, "Lambdas that capture local state are not yet supported in HIR conversion.");

        ITypeSymbol? delegateType = target switch
        {
            ILocalSymbol local => local.Type,
            IParameterSymbol parameter => parameter.Type,
            _ => null
        };

        if (delegateType is not INamedTypeSymbol namedDelegate || namedDelegate.DelegateInvokeMethod is not IMethodSymbol invokeSignature)
            throw new NotSupportedException("Lambda expressions require a delegate-typed target.");

        if (invokeSignature.Parameters.Length != parameters.Length)
            throw new NotSupportedException("Lambda parameter count does not match delegate signature.");

        for (int i = 0; i < parameters.Length; i++)
        {
            if (!SymbolEqualityComparer.Default.Equals(parameters[i].Type, invokeSignature.Parameters[i].Type))
                throw new NotSupportedException("Lambda parameter types must match the delegate signature exactly.");
        }

        _lambdaAssignments[target] = new LambdaInfo(lambda, parameters, invokeSignature);
        return true;
    }

    private ImmutableArray<IParameterSymbol> GetLambdaParameters(LambdaExpressionSyntax lambda)
    {
        var builder = ImmutableArray.CreateBuilder<IParameterSymbol>();

        switch (lambda)
        {
            case SimpleLambdaExpressionSyntax simple:
                if (_model.GetDeclaredSymbol(simple.Parameter) is not IParameterSymbol simpleParameter)
                    return default;
                builder.Add(simpleParameter);
                break;

            case ParenthesizedLambdaExpressionSyntax parenthesized:
                foreach (var parameterSyntax in parenthesized.ParameterList.Parameters)
                {
                    if (_model.GetDeclaredSymbol(parameterSyntax) is not IParameterSymbol parameterSymbol)
                        return default;
                    builder.Add(parameterSymbol);
                }
                break;
        }

        return builder.ToImmutable();
    }

    private bool IsCaptureFreeLambda(LambdaExpressionSyntax lambda, ImmutableArray<IParameterSymbol> parameters)
    {
        if (lambda.Body is not ExpressionSyntax body)
            return false;

        var dataFlow = _model.AnalyzeDataFlow(body);
        if (dataFlow.Succeeded is false)
            return false;

        if (dataFlow.Captured.Length > 0 || dataFlow.CapturedInside.Length > 0)
            return false;

        var parameterSet = parameters.ToImmutableHashSet(SymbolEqualityComparer.Default);
        foreach (var symbol in dataFlow.DataFlowsIn)
        {
            if (symbol is IParameterSymbol parameter && parameterSet.Contains(parameter))
                continue;

            return false;
        }

        return true;
    }

    private HirValue LowerLambdaInvocation(LambdaInfo info, IReadOnlyList<HirValue> arguments)
    {
        if (info.Parameters.Length != arguments.Count)
            throw new NotSupportedException("Lambda invocation argument count mismatch.");

        if (info.Syntax.Body is not ExpressionSyntax bodyExpression)
            throw new NotSupportedException("Block-bodied lambdas are not yet supported in HIR conversion.");

        var mapping = new Dictionary<IParameterSymbol, HirValue>(SymbolEqualityComparer.Default);
        for (int i = 0; i < info.Parameters.Length; i++)
        {
            var expectedType = MapType(info.DelegateSignature.Parameters[i].Type);
            var coerced = EnsureType(arguments[i], expectedType);
            mapping[info.Parameters[i]] = coerced;
        }

        _lambdaParameterScopes.Push(mapping);
        try
        {
            var loweredBody = LowerExpression(_model, bodyExpression);
            return EnsureType(loweredBody, MapType(info.DelegateSignature.ReturnType));
        }
        finally
        {
            _lambdaParameterScopes.Pop();
        }
    }
}
