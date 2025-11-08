using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Import;

/// <summary>
/// Entry point for Roslyn-to-HIR lowering. The heavy lifting lives in partial files that focus on
/// initialisation, statement lowering, expressions, and helpers; this file keeps the shared state and
/// high-level entry methods.
/// </summary>
internal sealed partial class HirMethodImporter
{
    private readonly CompilationContext _context;
    private readonly IMethodSymbol _symbol;
    private readonly SemanticModel _model;
    private HirBuilder? _hirBuilder;
    private HirArgument? _hirThisArgument;
    private INamedTypeSymbol? _systemExceptionType;
    private readonly Dictionary<ILocalSymbol, HirLocal> _hirLocals = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<IParameterSymbol, HirArgument> _hirArguments = new(SymbolEqualityComparer.Default);
    private readonly Stack<SourceSpan?> _hirSequencePoints = new();
    private readonly Stack<BranchTarget> _hirBreakTargets = new();
    private readonly Stack<BranchTarget> _hirContinueTargets = new();
    private readonly Stack<HirTryScopeContext> _hirTryScopes = new();
    private readonly Dictionary<ISymbol, LambdaInfo> _lambdaAssignments = new(SymbolEqualityComparer.Default);
    private readonly Stack<Dictionary<IParameterSymbol, HirValue>> _lambdaParameterScopes = new();
    private readonly Dictionary<string, int> _hirBlockCounters = new(StringComparer.Ordinal);
    private readonly Dictionary<ITypeSymbol, HirType> _hirTypeCache = new(SymbolEqualityComparer.Default);
    private int _hirTempCounter;
    private HirSsaState? _hirCurrentState;
    private HirBlock? _hirReturnBlock;
    private HirLocal? _hirReturnValueSlot;
    private readonly Dictionary<HirValue, HirLocal> _hirLocalValueMap = new(ReferenceEqualityComparer.Instance);
    private static readonly bool s_traceHirLocals =
        string.Equals(Environment.GetEnvironmentVariable("NEO_HIR_TRACE_LOCALS"), "1", StringComparison.OrdinalIgnoreCase);
    private static readonly bool s_dumpHirPhiDebug =
        string.Equals(Environment.GetEnvironmentVariable("NEO_IR_DUMP_HIR_PHI"), "1", StringComparison.OrdinalIgnoreCase);

    internal HirMethodImporter(CompilationContext context, IMethodSymbol symbol, SemanticModel model)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public HirBuilder Import()
    {
        InitializeHirBuilder();
        if (_hirBuilder is null)
            throw new InvalidOperationException("Failed to initialize HIR builder for method import.");

        ConvertToHir(_model);
        return _hirBuilder;
    }

    private sealed class HirSsaState
    {
        private readonly Dictionary<ISymbol, HirValue> _values;
        private readonly Dictionary<HirLocal, HirValue> _locals;

        public HirSsaState()
            : this(new Dictionary<ISymbol, HirValue>(SymbolEqualityComparer.Default), new Dictionary<HirLocal, HirValue>(ReferenceEqualityComparer.Instance))
        {
        }

        private HirSsaState(
            Dictionary<ISymbol, HirValue> values,
            Dictionary<HirLocal, HirValue> locals)
        {
            _values = values;
            _locals = locals;
        }

        public HirSsaState Clone()
        {
            var copy = new Dictionary<ISymbol, HirValue>(_values.Count, SymbolEqualityComparer.Default);
            foreach (var kvp in _values)
                copy[kvp.Key] = kvp.Value;
            var localsCopy = new Dictionary<HirLocal, HirValue>(_locals, ReferenceEqualityComparer.Instance);
            return new HirSsaState(copy, localsCopy);
        }

        public bool TryGetValue(ISymbol symbol, out HirValue value)
            => _values.TryGetValue(symbol, out value!);

        public void Assign(ISymbol symbol, HirValue value)
            => _values[symbol] = value;

        public bool TryGetValue(HirLocal local, out HirValue value)
            => _locals.TryGetValue(local, out value!);

        public void Assign(HirLocal local, HirValue value)
            => _locals[local] = value;

        public IEnumerable<ISymbol> Symbols => _values.Keys;
        public IEnumerable<HirLocal> LocalSymbols => _locals.Keys;

    }

    private bool TryGetLambdaParameterValue(IParameterSymbol symbol, out HirValue value)
    {
        foreach (var scope in _lambdaParameterScopes)
        {
            if (scope.TryGetValue(symbol, out value))
                return true;
        }

        value = null!;
        return false;
    }

    private readonly record struct BranchTarget(HirBlock Block, int TryDepth);
    private sealed record LambdaInfo(
        LambdaExpressionSyntax Syntax,
        System.Collections.Immutable.ImmutableArray<IParameterSymbol> Parameters,
        IMethodSymbol DelegateSignature);

    private sealed class HirTryScopeContext
    {
        internal HirTryScopeContext(HirTryFinallyScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        internal HirTryFinallyScope Scope { get; }
        internal Dictionary<HirBlock, HirBlock> LeaveDispatch { get; } = new();
        internal Dictionary<ISymbol, HirLocal> StateSlots { get; } = new(SymbolEqualityComparer.Default);
        internal HirBlock[] CatchBlocks { get; set; } = Array.Empty<HirBlock>();
        internal CatchDispatchInfo[] CatchMetadata { get; set; } = Array.Empty<CatchDispatchInfo>();
    }

    private readonly record struct CatchDispatchInfo(HirCatchClause Clause, HirBlock Block, ITypeSymbol? Type);

    private sealed class HirClosureContext
    {
        internal HirClosureContext(HirStructType closureStruct, IReadOnlyDictionary<ISymbol, HirField> captures)
        {
            ClosureStruct = closureStruct;
            CapturedFields = captures;
        }

        internal HirStructType ClosureStruct { get; }
        internal IReadOnlyDictionary<ISymbol, HirField> CapturedFields { get; }
    }
}
