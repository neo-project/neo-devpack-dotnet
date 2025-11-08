using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Import;

internal sealed partial class HirMethodImporter
{
    private (HirValue? Value, bool Terminated) ConvertBlock(SemanticModel model, BlockSyntax block)
    {
        var builder = _hirBuilder!;

        if (builder.CurrentBlock == builder.Function.Entry &&
            builder.CurrentBlock.Phis.Count == 0 &&
            builder.CurrentBlock.Instructions.Count == 0 &&
            builder.CurrentBlock.Terminator is null)
        {
            var bodyBlock = builder.CreateBlock(NewBlockLabel("entry_body"));
            AppendBranch(bodyBlock);
            builder.SetCurrentBlock(bodyBlock);
        }

        HirValue? last = null;
        bool terminated = false;

        for (int i = 0; i < block.Statements.Count; i++)
        {
            if (terminated)
                break;

            var statement = block.Statements[i];

            if (statement is LocalDeclarationStatementSyntax localDecl && (localDecl.UsingKeyword != default || localDecl.AwaitKeyword != default))
            {
                var remainder = block.Statements.Skip(i + 1).ToList();
                terminated = LowerUsingDeclaration(model, localDecl, remainder, ref last);
                return (last, terminated);
            }

            terminated = LowerStatement(model, statement, ref last);
        }

        return (last, terminated);
    }

    private bool LowerStatement(SemanticModel model, StatementSyntax statement, ref HirValue? lastValue)
    {
        using var seq = InsertSequencePoint(statement);

        switch (statement)
        {
            case LocalDeclarationStatementSyntax localDeclaration:
                LowerLocalDeclaration(model, localDeclaration);
                lastValue = null;
                return false;

            case ExpressionStatementSyntax expressionStatement:
                lastValue = LowerExpression(model, expressionStatement.Expression);
                return _hirBuilder?.CurrentBlock.Terminator is not null;

            case ReturnStatementSyntax returnStatement:
                var value = returnStatement.Expression is null ? null : LowerExpression(model, returnStatement.Expression);
                if (value is not null)
                    value = EnsureType(value, MapType(_symbol.ReturnType));
                EmitReturn(value);
                lastValue = value;
                return true;

            case BlockSyntax blockSyntax:
                var (blockValue, blockTerminated) = ConvertBlock(model, blockSyntax);
                lastValue = blockValue;
                return blockTerminated;

            case IfStatementSyntax ifStatement:
                lastValue = null;
                return LowerIfStatement(model, ifStatement);

            case ForEachStatementSyntax foreachStatement:
                LowerForEachStatement(model, foreachStatement);
                lastValue = null;
                return false;

            case ForEachVariableStatementSyntax:
                throw new NotSupportedException("Foreach variable patterns are not yet supported in HIR conversion.");

            case WhileStatementSyntax whileStatement:
                LowerWhileStatement(model, whileStatement);
                lastValue = null;
                return false;

            case DoStatementSyntax doStatement:
                LowerDoWhileStatement(model, doStatement);
                lastValue = null;
                return false;

            case ForStatementSyntax forStatement:
                LowerForStatement(model, forStatement);
                lastValue = null;
                return false;

            case BreakStatementSyntax:
                LowerBreakStatement();
                lastValue = null;
                return true;

            case ContinueStatementSyntax:
                LowerContinueStatement();
                lastValue = null;
                return true;

            case EmptyStatementSyntax:
                lastValue = null;
                return false;

            case SwitchStatementSyntax switchStatement:
                var fallsThrough = LowerSwitchStatement(model, switchStatement);
                lastValue = null;
                return !fallsThrough;

            case TryStatementSyntax tryStatement:
                LowerTryStatement(model, tryStatement);
                lastValue = null;
                return false;

            case UsingStatementSyntax usingStatement:
                return LowerUsingStatement(model, usingStatement, ref lastValue);

            case ThrowStatementSyntax throwStatement:
                if (throwStatement.Expression is null)
                    throw CompilationException.UnsupportedSyntax(throwStatement, "Rethrow statements inside try blocks are not yet supported in HIR conversion.");

                var loweredException = LowerExpression(model, throwStatement.Expression);

                if (_hirTryScopes.Count == 0)
                {
                    _hirBuilder!.AppendTerminator(new HirThrow(loweredException));
                }
                else
                {
                    var currentScope = _hirTryScopes.Peek();
                    StoreScopeState(currentScope, _hirCurrentState);

                    var thrownType = model.GetTypeInfo(throwStatement.Expression).ConvertedType;
                    var targetBlock = ResolveCatchTarget(currentScope, thrownType) ?? currentScope.Scope.MergeBlock;
                    var targetDepth = Math.Max(0, _hirTryScopes.Count - 1);
                    _ = EmitLeaveTo(targetBlock, targetDepth);
                }

                lastValue = null;
                return true;

            default:
                throw new NotSupportedException($"HIR conversion does not yet support statement '{statement.Kind()}'.");
        }
    }

    private bool LowerIfStatement(SemanticModel model, IfStatementSyntax ifStatement)
    {
        var builder = _hirBuilder!;
        var condition = LowerExpression(model, ifStatement.Condition);
        var trueBlock = builder.CreateBlock(NewBlockLabel("if_true"));
        var mergeBlock = builder.CreateBlock(NewBlockLabel("if_merge"));
        HirBlock? falseBlock = null;

        if (ifStatement.Else is not null)
            falseBlock = builder.CreateBlock(NewBlockLabel("if_false"));

        var falseTarget = falseBlock ?? mergeBlock;
        AppendConditional(condition, trueBlock, falseTarget);

        var incomingState = _hirCurrentState?.Clone();

        builder.SetCurrentBlock(trueBlock);
        _hirCurrentState = incomingState?.Clone();
        HirValue? branchValue = null;
        var trueTerminated = LowerStatement(model, ifStatement.Statement, ref branchValue);
        var trueState = _hirCurrentState?.Clone();
        if (!trueTerminated)
            AppendBranch(mergeBlock);

        bool falseTerminated = false;
        HirSsaState? falseState = null;
        if (ifStatement.Else is not null)
        {
            builder.SetCurrentBlock(falseBlock!);
            _hirCurrentState = incomingState?.Clone();
            branchValue = null;
            falseTerminated = LowerStatement(model, ifStatement.Else.Statement, ref branchValue);
            falseState = _hirCurrentState?.Clone();
            if (!falseTerminated)
                AppendBranch(mergeBlock);
        }
        else
        {
            falseState = incomingState?.Clone();
        }

        builder.SetCurrentBlock(mergeBlock);
        if (trueTerminated && falseTerminated && ifStatement.Else is not null)
        {
            _hirCurrentState = incomingState;
            return true;
        }

        if (trueTerminated)
        {
            _hirCurrentState = falseState;
            return false;
        }

        if (falseTerminated)
        {
            _hirCurrentState = trueState;
            return false;
        }

        if (incomingState is not null && trueState is not null && falseState is not null)
            _hirCurrentState = MergeStates(mergeBlock, (trueBlock, trueState), (falseBlock ?? mergeBlock, falseState), incomingState);

        return false;
    }

    private void LowerWhileStatement(SemanticModel model, WhileStatementSyntax syntax)
    {
        var builder = _hirBuilder!;
        var preLoopState = _hirCurrentState?.Clone();
        var conditionBlock = builder.CreateBlock(NewBlockLabel("while_cond"));
        var bodyBlock = builder.CreateBlock(NewBlockLabel("while_body"));
        var exitBlock = builder.CreateBlock(NewBlockLabel("while_exit"));

        AppendBranch(conditionBlock);
        builder.SetCurrentBlock(conditionBlock);
        _hirCurrentState = preLoopState?.Clone();

        var condition = LowerExpression(model, syntax.Condition);
        AppendConditional(condition, bodyBlock, exitBlock);

        _hirContinueTargets.Push(new BranchTarget(conditionBlock, _hirTryScopes.Count));
        _hirBreakTargets.Push(new BranchTarget(exitBlock, _hirTryScopes.Count));

        builder.SetCurrentBlock(bodyBlock);
        HirValue? value = null;
        var terminated = LowerStatement(model, syntax.Statement, ref value);
        if (!terminated)
            AppendBranch(conditionBlock);

        _hirBreakTargets.Pop();
        _hirContinueTargets.Pop();

        builder.SetCurrentBlock(exitBlock);
        var bodyState = _hirCurrentState?.Clone();
        if (preLoopState is not null && bodyState is not null)
            _hirCurrentState = MergeStates(exitBlock, (bodyBlock, bodyState), (conditionBlock, preLoopState), preLoopState);
        else
            _hirCurrentState = bodyState ?? preLoopState;
    }

    private void LowerDoWhileStatement(SemanticModel model, DoStatementSyntax syntax)
    {
        var builder = _hirBuilder!;
        var preLoopState = _hirCurrentState?.Clone();
        var bodyBlock = builder.CreateBlock(NewBlockLabel("do_body"));
        var conditionBlock = builder.CreateBlock(NewBlockLabel("do_cond"));
        var exitBlock = builder.CreateBlock(NewBlockLabel("do_exit"));

        AppendBranch(bodyBlock);
        builder.SetCurrentBlock(bodyBlock);

        _hirContinueTargets.Push(new BranchTarget(conditionBlock, _hirTryScopes.Count));
        _hirBreakTargets.Push(new BranchTarget(exitBlock, _hirTryScopes.Count));

        HirValue? value = null;
        var terminated = LowerStatement(model, syntax.Statement, ref value);
        if (!terminated)
            AppendBranch(conditionBlock);

        builder.SetCurrentBlock(conditionBlock);
        var condition = LowerExpression(model, syntax.Condition);
        AppendConditional(condition, bodyBlock, exitBlock);

        _hirBreakTargets.Pop();
        _hirContinueTargets.Pop();

        builder.SetCurrentBlock(exitBlock);
        var bodyState = _hirCurrentState?.Clone();
        if (preLoopState is not null && bodyState is not null)
            _hirCurrentState = MergeStates(exitBlock, (conditionBlock, preLoopState), (bodyBlock, bodyState), preLoopState);
        else
            _hirCurrentState = bodyState ?? preLoopState;
    }

    private void LowerForStatement(SemanticModel model, ForStatementSyntax syntax)
    {
        var builder = _hirBuilder!;
        var preLoopState = _hirCurrentState?.Clone();
        var initBlock = builder.CreateBlock(NewBlockLabel("for_init"));
        var conditionBlock = builder.CreateBlock(NewBlockLabel("for_cond"));
        var bodyBlock = builder.CreateBlock(NewBlockLabel("for_body"));
        var incrementBlock = builder.CreateBlock(NewBlockLabel("for_incr"));
        var exitBlock = builder.CreateBlock(NewBlockLabel("for_exit"));

        AppendBranch(initBlock);
        builder.SetCurrentBlock(initBlock);

        if (syntax.Declaration is not null)
            LowerVariableDeclaration(model, syntax.Declaration);
        foreach (var initializer in syntax.Initializers)
            LowerExpression(model, initializer);

        AppendBranch(conditionBlock);

        builder.SetCurrentBlock(conditionBlock);
        _hirCurrentState = _hirCurrentState?.Clone();
        HirValue? conditionValue = syntax.Condition is null ? EmitConstant(true) : LowerExpression(model, syntax.Condition);
        AppendConditional(conditionValue, bodyBlock, exitBlock);

        _hirContinueTargets.Push(new BranchTarget(incrementBlock, _hirTryScopes.Count));
        _hirBreakTargets.Push(new BranchTarget(exitBlock, _hirTryScopes.Count));

        builder.SetCurrentBlock(bodyBlock);
        _hirCurrentState = _hirCurrentState?.Clone();
        HirValue? bodyValue = null;
        var bodyTerminated = LowerStatement(model, syntax.Statement, ref bodyValue);
        var postBodyState = _hirCurrentState?.Clone();
        HirSsaState? loopState = null;
        if (!bodyTerminated)
        {
            AppendBranch(incrementBlock);

            builder.SetCurrentBlock(incrementBlock);
            _hirCurrentState = postBodyState?.Clone();
            foreach (var incrementor in syntax.Incrementors)
                LowerExpression(model, incrementor);
            loopState = _hirCurrentState?.Clone();
            AppendBranch(conditionBlock);
        }
        else
        {
            loopState = postBodyState;
        }

        _hirBreakTargets.Pop();
        _hirContinueTargets.Pop();

        builder.SetCurrentBlock(exitBlock);
        if (preLoopState is not null && loopState is not null)
            _hirCurrentState = MergeStates(exitBlock, (bodyBlock, loopState), (conditionBlock, preLoopState), preLoopState);
        else
            _hirCurrentState = loopState ?? preLoopState;
    }

    private void LowerBreakStatement()
    {
        if (_hirBuilder is null)
            return;

        if (_hirBreakTargets.Count == 0)
            throw new InvalidOperationException("break statement without surrounding loop/switch.");

        var target = _hirBreakTargets.Peek();
        _ = EmitLeaveTo(target.Block, target.TryDepth);
    }

    private void LowerContinueStatement()
    {
        if (_hirBuilder is null)
            return;

        if (_hirContinueTargets.Count == 0)
            throw new InvalidOperationException("continue statement without surrounding loop.");

        var target = _hirContinueTargets.Peek();
        _ = EmitLeaveTo(target.Block, target.TryDepth);
    }

    private void LowerForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
    {
        var builder = _hirBuilder!;
        var preLoopState = _hirCurrentState?.Clone();
        var collectionValue = LowerExpression(model, syntax.Expression);

        var enumeratorInfo = model.GetForEachStatementInfo(syntax);
        var enumeratorType = enumeratorInfo.GetEnumeratorMethod?.ReturnType;
        var enumeratorLocal = CreateSyntheticLocal("enumerator", MapType(enumeratorType));
        var enumeratorValue = EmitInvocation(enumeratorInfo.GetEnumeratorMethod!, collectionValue, Array.Empty<HirValue>());
        var storeEnumerator = new HirStoreLocal(enumeratorLocal, enumeratorValue);
        AppendStoreLocal(storeEnumerator);
        preLoopState = _hirCurrentState?.Clone();

        var conditionBlock = builder.CreateBlock(NewBlockLabel("foreach_cond"));
        var bodyBlock = builder.CreateBlock(NewBlockLabel("foreach_body"));
        var exitBlock = builder.CreateBlock(NewBlockLabel("foreach_exit"));

        AppendBranch(conditionBlock);
        builder.SetCurrentBlock(conditionBlock);
        _hirCurrentState = _hirCurrentState?.Clone();

        var enumeratorForCondition = LoadLocal(enumeratorLocal);
        var moveNext = EmitInvocation(enumeratorInfo.MoveNextMethod!, enumeratorForCondition, Array.Empty<HirValue>());
        AppendConditional(moveNext, bodyBlock, exitBlock);

        _hirBreakTargets.Push(new BranchTarget(exitBlock, _hirTryScopes.Count));
        _hirContinueTargets.Push(new BranchTarget(conditionBlock, _hirTryScopes.Count));

        builder.SetCurrentBlock(bodyBlock);
        var enumeratorForBody = LoadLocal(enumeratorLocal);
        var current = EmitInvocation(enumeratorInfo.CurrentProperty!.GetMethod!, enumeratorForBody, Array.Empty<HirValue>());
        AssignForeachTarget(model, syntax, current);

        HirValue? bodyValue = null;
        var terminated = LowerStatement(model, syntax.Statement, ref bodyValue);
        if (!terminated)
            AppendBranch(conditionBlock);

        _hirContinueTargets.Pop();
        _hirBreakTargets.Pop();
        builder.SetCurrentBlock(exitBlock);
        var bodyState = _hirCurrentState?.Clone();
        if (preLoopState is not null && bodyState is not null)
            _hirCurrentState = MergeStates(exitBlock, (conditionBlock, preLoopState), (bodyBlock, bodyState), preLoopState);
        else
            _hirCurrentState = bodyState ?? preLoopState;
    }

    private void AssignForeachTarget(SemanticModel model, ForEachStatementSyntax syntax, HirValue current)
    {
        if (model.GetDeclaredSymbol(syntax) is not ILocalSymbol localSymbol)
            return;

        AssignLocalSymbol(localSymbol, current);
    }

    private bool LowerSwitchStatement(SemanticModel model, SwitchStatementSyntax syntax)
    {
        var builder = _hirBuilder!;
        var value = LowerExpression(model, syntax.Expression);
        var mergeBlock = builder.CreateBlock(NewBlockLabel("switch_merge"));
        HirBlock? defaultBlock = null;
        var sections = new List<(SwitchSectionSyntax Section, HirBlock Block)>();

        foreach (var section in syntax.Sections)
        {
            var block = builder.CreateBlock(NewBlockLabel("switch_case"));
            sections.Add((section, block));
            if (defaultBlock is null && section.Labels.Any(l => l.IsKind(SyntaxKind.DefaultSwitchLabel)))
                defaultBlock = block;
        }

        defaultBlock ??= mergeBlock;

        var cases = new List<(BigInteger Case, HirBlock Target)>();
        var fallsThrough = ReferenceEquals(defaultBlock, mergeBlock);
        foreach (var (section, block) in sections)
        {
            foreach (var label in section.Labels.OfType<CaseSwitchLabelSyntax>())
            {
                var constantValue = model.GetConstantValue(label.Value);
                if (!constantValue.HasValue)
                    throw new NotSupportedException("Switch case without constant value is not supported.");
                cases.Add((ConstantToBigInteger(constantValue.Value), block));
            }
        }

        var switchTerminator = new HirSwitch(value, cases, defaultBlock);
        builder.AppendTerminator(switchTerminator);

        _hirBreakTargets.Push(new BranchTarget(mergeBlock, _hirTryScopes.Count));

        for (int i = 0; i < sections.Count; i++)
        {
            var (section, block) = sections[i];
            builder.SetCurrentBlock(block);

            HirValue? sectionValue = null;
            var terminated = false;
            foreach (var statement in section.Statements)
            {
                terminated = LowerStatement(model, statement, ref sectionValue);
                if (terminated)
                    break;
            }

            if (!terminated)
            {
                fallsThrough = true;
                var target = i + 1 < sections.Count ? sections[i + 1].Block : mergeBlock;
                AppendBranch(target);
            }
            else if (block.Terminator is HirBranch branch && ReferenceEquals(branch.Target, mergeBlock))
            {
                fallsThrough = true;
            }
        }

        _hirBreakTargets.Pop();
        builder.SetCurrentBlock(mergeBlock);

        if (!fallsThrough)
            builder.AppendTerminator(new HirUnreachable());

        return fallsThrough;
    }

    private void LowerTryStatement(SemanticModel model, TryStatementSyntax syntax)
    {
        if (syntax.Catches.Count == 0 && syntax.Finally is null)
            throw new NotSupportedException("Try statement requires at least one catch or finally clause.");

        var builder = _hirBuilder ?? throw new InvalidOperationException("HIR builder not initialised.");
        var incomingState = _hirCurrentState?.Clone();

        var tryBlock = builder.CreateBlock(NewBlockLabel("try_body"));
        var mergeBlock = builder.CreateBlock(NewBlockLabel("try_merge"));

        var catchInfos = new List<(CatchClauseSyntax Syntax, HirBlock Block, HirCatchClause Clause, ITypeSymbol? CatchType)>();
        foreach (var catchClause in syntax.Catches)
        {
            ValidateCatchClause(model, catchClause);

            var catchBlock = builder.CreateBlock(NewBlockLabel("try_catch"));
            var clause = new HirCatchClause(catchBlock);
            var catchType = catchClause.Declaration is { Type: { } typeSyntax }
                ? model.GetTypeInfo(typeSyntax).ConvertedType ?? GetSystemExceptionType()
                : GetSystemExceptionType();
            catchInfos.Add((catchClause, catchBlock, clause, catchType));
        }

        var finallyBlock = builder.CreateBlock(NewBlockLabel("try_finally"));

        var tryScopeMarker = new HirTryFinallyScope(
            tryBlock,
            finallyBlock,
            mergeBlock,
            catchInfos.Select(info => info.Clause).ToArray());

        builder.Append(tryScopeMarker);
        var tryContext = new HirTryScopeContext(tryScopeMarker);
        tryContext.CatchBlocks = catchInfos.Select(info => info.Block).ToArray();
        tryContext.CatchMetadata = catchInfos
            .Select(info => new CatchDispatchInfo(info.Clause, info.Block, info.CatchType))
            .ToArray();
        _hirTryScopes.Push(tryContext);

        AppendBranch(tryBlock);
        builder.SetCurrentBlock(tryBlock);
        _hirCurrentState = incomingState?.Clone() ?? new HirSsaState();

        HirValue? tryValue = null;
        var tryTerminated = LowerStatement(model, syntax.Block, ref tryValue);
        var tryState = _hirCurrentState?.Clone();
        HirBlock? tryExitBlock = null;

        if (!tryTerminated)
        {
            var targetDepth = Math.Max(0, _hirTryScopes.Count - 1);
            tryExitBlock = EmitLeaveTo(mergeBlock, targetDepth);
        }

        var catchStates = new List<(HirBlock Block, HirSsaState State)>();
        foreach (var info in catchInfos)
        {
            builder.SetCurrentBlock(info.Block);
            builder.Append(new HirCatchScope(tryScopeMarker, info.Clause));
            _hirCurrentState = incomingState?.Clone() ?? new HirSsaState();

            if (info.Syntax.Declaration is not null)
                LowerCatchDeclaration(model, info.Syntax.Declaration);

            HirValue? catchValue = null;
            var catchTerminated = LowerStatement(model, info.Syntax.Block, ref catchValue);
            var catchState = _hirCurrentState?.Clone();

            if (!catchTerminated)
            {
                var targetDepth = Math.Max(0, _hirTryScopes.Count - 1);
                var exitBlock = EmitLeaveTo(mergeBlock, targetDepth);
                if (catchState is not null)
                    catchStates.Add((exitBlock, catchState));
            }
        }

        builder.SetCurrentBlock(finallyBlock);

        var hasFinally = syntax.Finally is not null;
        HirSsaState? finallyState = null;
        HirBlock? finallyExitBlock = null;
        bool finallyTerminated = false;

        if (hasFinally)
        {
            builder.Append(new HirFinallyScope(tryScopeMarker));
            _hirCurrentState = LoadScopeState(tryContext, incomingState);

            HirValue? finallyValue = null;
            finallyTerminated = LowerStatement(model, syntax.Finally!.Block, ref finallyValue);
            finallyState = _hirCurrentState?.Clone();

            if (!finallyTerminated)
            {
                finallyExitBlock = builder.CurrentBlock;
                StoreScopeState(tryContext, _hirCurrentState);
                builder.AppendTerminator(new HirEndFinally(tryScopeMarker, mergeBlock));
            }
        }
        else
        {
            builder.AppendTerminator(new HirBranch(mergeBlock));
        }

        _hirTryScopes.Pop();

        builder.SetCurrentBlock(mergeBlock);

        var baseline = incomingState?.Clone();
        var predecessorStates = new List<(HirBlock Block, HirSsaState State)>();

        if (!tryTerminated && tryState is not null && tryExitBlock is not null)
            predecessorStates.Add((tryExitBlock, tryState));

        predecessorStates.AddRange(catchStates);

        if (hasFinally && !finallyTerminated && finallyState is not null)
        {
            var exitBlock = finallyExitBlock ?? finallyBlock;
            predecessorStates.Add((exitBlock, finallyState));
        }

        if (baseline is null)
        {
            _hirCurrentState = predecessorStates.Count > 0 ? predecessorStates[^1].State : null;
            return;
        }

        if (predecessorStates.Count == 0)
        {
            _hirCurrentState = baseline;
            return;
        }

        _hirCurrentState = MergeStates(mergeBlock, predecessorStates, baseline);
    }

    private void LowerCatchDeclaration(SemanticModel model, CatchDeclarationSyntax declaration)
    {
        if (model.GetDeclaredSymbol(declaration) is not ILocalSymbol symbol)
            return;

        var type = MapType(symbol.Type);
        var initial = EmitHirDefault(type);
        AssignLocalSymbol(symbol, initial);
    }

    private bool LowerUsingDeclaration(SemanticModel model, LocalDeclarationStatementSyntax declaration, IReadOnlyList<StatementSyntax> remainder, ref HirValue? lastValue)
    {
        var body = remainder.Count switch
        {
            0 => (StatementSyntax)SyntaxFactory.Block(),
            1 => remainder[0],
            _ => SyntaxFactory.Block(SyntaxFactory.List(remainder))
        };

        return LowerUsingCore(model, declaration.AwaitKeyword, declaration.Declaration, expression: null, body, ref lastValue);
    }

    private bool LowerUsingStatement(SemanticModel model, UsingStatementSyntax syntax, ref HirValue? lastValue)
        => LowerUsingCore(model, syntax.AwaitKeyword, syntax.Declaration, syntax.Expression, syntax.Statement, ref lastValue);

    private bool LowerUsingCore(
        SemanticModel model,
        SyntaxToken awaitKeyword,
        VariableDeclarationSyntax? declaration,
        ExpressionSyntax? expression,
        StatementSyntax body,
        ref HirValue? lastValue)
    {
        var builder = _hirBuilder!;
        var resources = new List<(ILocalSymbol? Symbol, HirLocal? Synthetic, ITypeSymbol Type)>();

        if (declaration is not null)
        {
            LowerVariableDeclaration(model, declaration);

            foreach (var variable in declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is ILocalSymbol localSymbol)
                {
                    resources.Add((localSymbol, null, localSymbol.Type));
                }
            }
        }
        else if (expression is not null)
        {
            var expressionValue = LowerExpression(model, expression);
            var expressionTypeSymbol = model.GetTypeInfo(expression).ConvertedType
                ?? model.Compilation.GetTypeByMetadataName("System.IDisposable")
                ?? model.Compilation.GetSpecialType(SpecialType.System_Object);
            var local = CreateSyntheticLocal("using_resource", MapType(expressionTypeSymbol));
            var store = new HirStoreLocal(local, expressionValue);
            AppendStoreLocal(store);
            resources.Add((null, local, expressionTypeSymbol));
        }
        else
        {
            throw new NotSupportedException("Using statement requires a resource declaration or expression.");
        }

        var incomingState = _hirCurrentState?.Clone();

        var tryBlock = builder.CreateBlock(NewBlockLabel("using_body"));
        var finallyBlock = builder.CreateBlock(NewBlockLabel("using_dispose"));
        var mergeBlock = builder.CreateBlock(NewBlockLabel("using_merge"));

        var tryScopeMarker = new HirTryFinallyScope(tryBlock, finallyBlock, mergeBlock, Array.Empty<HirCatchClause>());
        builder.Append(tryScopeMarker);

        var tryContext = new HirTryScopeContext(tryScopeMarker)
        {
            CatchBlocks = Array.Empty<HirBlock>()
        };
        _hirTryScopes.Push(tryContext);

        AppendBranch(tryBlock);
        builder.SetCurrentBlock(tryBlock);
        _hirCurrentState = incomingState?.Clone() ?? new HirSsaState();

        HirValue? bodyValue = null;
        var bodyTerminated = LowerStatement(model, body, ref bodyValue);
        var bodyState = _hirCurrentState?.Clone();
        HirBlock? bodyExitBlock = null;

        if (!bodyTerminated)
        {
            var targetDepth = Math.Max(0, _hirTryScopes.Count - 1);
            bodyExitBlock = EmitLeaveTo(mergeBlock, targetDepth);
        }

        builder.SetCurrentBlock(finallyBlock);
        builder.Append(new HirFinallyScope(tryScopeMarker));
        _hirCurrentState = LoadScopeState(tryContext, incomingState);

        var isAsync = awaitKeyword != default && awaitKeyword.IsKind(SyntaxKind.AwaitKeyword);
        foreach (var resource in resources)
        {
            HirValue loaded = resource.Symbol is not null
                ? GetSymbolValue(resource.Symbol)
                : LoadLocal(resource.Synthetic!);
            EmitDisposeCall(model, resource.Type, loaded, isAsync);
        }

        var finallyState = _hirCurrentState?.Clone();
        StoreScopeState(tryContext, _hirCurrentState);
        builder.AppendTerminator(new HirEndFinally(tryScopeMarker, mergeBlock));

        _hirTryScopes.Pop();

        builder.SetCurrentBlock(mergeBlock);

        var baseline = incomingState?.Clone();
        var predecessorStates = new List<(HirBlock Block, HirSsaState State)>();

        if (!bodyTerminated && bodyState is not null && bodyExitBlock is not null)
            predecessorStates.Add((bodyExitBlock, bodyState));

        if (finallyState is not null)
            predecessorStates.Add((finallyBlock, finallyState));

        if (baseline is null)
        {
            _hirCurrentState = predecessorStates.Count > 0 ? predecessorStates[^1].State : null;
        }
        else if (predecessorStates.Count == 0)
        {
            _hirCurrentState = baseline;
        }
        else
        {
            _hirCurrentState = MergeStates(mergeBlock, predecessorStates, baseline);
        }

        lastValue = null;
        return bodyTerminated;
    }

    private void ValidateCatchClause(SemanticModel model, CatchClauseSyntax catchClause)
    {
        if (catchClause.Filter is not null)
            throw CompilationException.UnsupportedSyntax(catchClause.Filter, "Catch filters are not supported by the HIR importer. Remove the filter or refactor the control flow.");

        if (catchClause.Declaration is { Type: { } typeSyntax })
        {
            var typeSymbol = model.GetTypeInfo(typeSyntax).ConvertedType;
            if (typeSymbol is null || !IsSystemExceptionType(typeSymbol))
                throw CompilationException.UnsupportedSyntax(typeSyntax, "Typed catch clauses other than 'catch (System.Exception)' are not supported by the HIR importer.");
        }
    }

    private INamedTypeSymbol GetSystemExceptionType()
    {
        if (_systemExceptionType is not null)
            return _systemExceptionType;

        var symbol = _model.Compilation.GetTypeByMetadataName("System.Exception")
            ?? throw new NotSupportedException("System.Exception type could not be resolved in the current compilation.");
        _systemExceptionType = symbol;
        return symbol;
    }

    private bool IsSystemExceptionType(ITypeSymbol type)
    {
        var exceptionType = GetSystemExceptionType();
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, exceptionType))
                return true;
        }

        return false;
    }

    private HirBlock? ResolveCatchTarget(HirTryScopeContext scope, ITypeSymbol? thrownType)
    {
        if (scope.CatchMetadata.Length == 0)
            return null;

        var compilation = _model.Compilation;
        foreach (var entry in scope.CatchMetadata)
        {
            var catchType = entry.Type ?? GetSystemExceptionType();
            if (thrownType is null)
                return entry.Block;

            var conversion = compilation.ClassifyConversion(thrownType, catchType);
            if (conversion.Exists && conversion.IsImplicit)
                return entry.Block;
        }

        return null;
    }
}
