// Copyright (C) 2015-2026 The Neo Project.
//
// SlotHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    #region Variables

    /// <summary>
    /// Adds a local variable to the method's scope.
    /// </summary>
    /// <remarks>
    /// ILocalSymbols defined by the 'out' parameter will not be processed here
    /// but are considered as static fields.
    /// </remarks>
    /// <param name="symbol">The ILocalSymbol representing the local variable to be added.</param>
    /// <returns>The index of the newly added local variable.</returns>
    private byte AddLocalVariable(ILocalSymbol symbol)
    {
        int indexValue = _localVariables.Count + _anonymousVariables.Count;
        RequireByteSizedSlotCount(Symbol, indexValue + 1, "local slots");
        var index = (byte)indexValue;
        _variableSymbols.Add((symbol, index));
        _localVariables.Add(symbol, index);
        if (_localsCount < index + 1)
            _localsCount = index + 1;
        _blockSymbols.Peek().Add(symbol);
        return index;
    }

    private byte AddAnonymousVariable()
    {
        int indexValue = _localVariables.Count + _anonymousVariables.Count;
        RequireByteSizedSlotCount(Symbol, indexValue + 1, "local slots");
        var index = (byte)indexValue;
        _anonymousVariables.Add(index);
        if (_localsCount < index + 1)
            _localsCount = index + 1;
        return index;
    }

    private void RemoveAnonymousVariable(byte index)
    {
        if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
            _anonymousVariables.Remove(index);
    }

    private AnonymousVariableScope PreserveAnonymousVariables()
        => new(this);

    private readonly struct AnonymousVariableScope(MethodConvert convert) : IDisposable
    {
        private readonly MethodConvert _convert = convert;
        private readonly int _count = convert._anonymousVariables.Count;

        public void Dispose()
        {
            if (!_convert._context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                return;

            while (_convert._anonymousVariables.Count > _count)
                _convert._anonymousVariables.RemoveAt(_convert._anonymousVariables.Count - 1);
        }
    }

    private void RemoveLocalVariable(ILocalSymbol symbol)
    {
        if (HasRefBinding(symbol))
            RemoveRefBinding(symbol);
        if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
            _localVariables.Remove(symbol);
    }

    #endregion

    #region Helper

    /// <summary>
    /// Loads the value of a parameter onto the evaluation stack.
    /// </summary>
    /// <param name="parameter">The parameter symbol to load.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the load operation.</returns>
    private Instruction LdArgSlot(IParameterSymbol parameter)
    {
        if (TryGetInlineParameterSlot(parameter, out byte inlineSlot))
            return AccessSlot(OpCode.LDLOC, inlineSlot);

        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && parameter.RefKind == RefKind.Out))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.LDSFLD, staticIndex);
        }
        // local parameter in current method
        // Roslyn may hand us reduced symbols (e.g. through record synthesized members) that differ from
        // the ones we stored when we registered the signature. Fall back to the original definition before
        // giving up so record-generated accessors continue to function.
        if (!_parameters.TryGetValue(parameter, out var index) &&
            !_parameters.TryGetValue(parameter.OriginalDefinition, out index))
            throw new KeyNotFoundException(parameter.ToDisplayString());
        return AccessSlot(OpCode.LDARG, index);
    }

    /// <summary>
    /// Stores the value from the evaluation stack to a parameter.
    /// </summary>
    /// <param name="parameter">The parameter symbol to store the value to.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the store operation.</returns>
    private Instruction StArgSlot(IParameterSymbol parameter)
    {
        if (TryGetInlineParameterSlot(parameter, out byte inlineSlot))
            return AccessSlot(OpCode.STLOC, inlineSlot);

        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && parameter.RefKind == RefKind.Out))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.STSFLD, staticIndex);
        }
        // local parameter in current method
        // Same rationale as above: prefer the current symbol, but allow the original definition as a
        // fallback for record-generated methods where the parameter symbol instance differs.
        if (!_parameters.TryGetValue(parameter, out var index) &&
            !_parameters.TryGetValue(parameter.OriginalDefinition, out index))
            throw new KeyNotFoundException(parameter.ToDisplayString());
        return AccessSlot(OpCode.STARG, index);
    }

    private bool TryGetInlineParameterSlot(IParameterSymbol parameter, out byte index)
    {
        foreach (Dictionary<IParameterSymbol, byte> scope in _inlineParameterScopes)
        {
            if (scope.TryGetValue(parameter, out index) ||
                scope.TryGetValue(parameter.OriginalDefinition, out index))
                return true;
        }

        index = 0;
        return false;
    }

    /// <summary>
    /// Loads the value of a local variable onto the evaluation stack.
    /// </summary>
    /// <param name="local">The local variable symbol to load.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the load operation.</returns>
    private Instruction LdLocSlot(ILocalSymbol local)
    {
        if (TryGetRefBinding(local, out RefBinding binding))
            return LoadRefBinding(binding);

        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && local.RefKind == RefKind.Out))
        {
            //create static fields from captured local
            var staticIndex = _context.GetOrAddCapturedStaticField(local);
            CapturedLocalSymbols.Add(local);
            return AccessSlot(OpCode.LDSFLD, staticIndex);
        }
        // local variables in current method
        var index = _localVariables[local];
        return AccessSlot(OpCode.LDLOC, index);
    }

    /// <summary>
    /// Stores the value from the evaluation stack into a local variable.
    /// </summary>
    /// <param name="local">The local variable symbol to store the value into.</param>
    /// <param name="method">Optional method symbol, used for determining if it's a built-in type method.</param>
    /// <returns>An instruction representing the store operation.</returns>
    private Instruction StLocSlot(ILocalSymbol local)
    {
        if (TryGetRefBinding(local, out RefBinding binding))
            return StoreRefBinding(binding);

        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }

        if ((Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local)) ||
            (Symbol.MethodKind == MethodKind.Ordinary && local.RefKind == RefKind.Out))
        {
            //create static fields from captured local
            var staticIndex = _context.GetOrAddCapturedStaticField(local);
            CapturedLocalSymbols.Add(local);
            return AccessSlot(OpCode.STSFLD, staticIndex);
        }
        var index = _localVariables[local];
        return AccessSlot(OpCode.STLOC, index);
    }

    private Instruction AccessSlot(OpCode opcode, byte index)
    {
        return index >= 7
            ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
            : AddInstruction(opcode - 7 + index);
    }

    private void EmitPackedItemsLeftToRight<T>(IReadOnlyList<T> items, Action<T> emitItem, OpCode packOpCode, Func<T, bool>? canDeferEmission = null)
    {
        byte?[] slots = new byte?[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            if (canDeferEmission?.Invoke(items[i]) == true)
                continue;

            emitItem(items[i]);
            byte slot = AddAnonymousVariable();
            slots[i] = slot;
            AccessSlot(OpCode.STLOC, slot);
        }

        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (slots[i] is byte slot)
                AccessSlot(OpCode.LDLOC, slot);
            else
                emitItem(items[i]);
        }

        Push(items.Count);
        AddInstruction(packOpCode);

        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (slots[i] is byte slot)
                RemoveAnonymousVariable(slot);
        }
    }

    private static bool CanDeferExpressionEmission(SemanticModel model, ExpressionSyntax expression)
    {
        return expression switch
        {
            InitializerExpressionSyntax initializer =>
                initializer.Expressions.All(child => CanDeferExpressionEmission(model, child)),
            ArrayCreationExpressionSyntax { Initializer: { } initializer } =>
                CanDeferExpressionEmission(model, initializer),
            ImplicitArrayCreationExpressionSyntax { Initializer: { } initializer } =>
                CanDeferExpressionEmission(model, initializer),
            CollectionExpressionSyntax collection =>
                collection.Elements.All(element => element is ExpressionElementSyntax expressionElement &&
                    CanDeferExpressionEmission(model, expressionElement.Expression)),
            TupleExpressionSyntax tuple =>
                tuple.Arguments.All(argument => CanDeferExpressionEmission(model, argument.Expression)),
            AnonymousObjectCreationExpressionSyntax anonymousObject =>
                anonymousObject.Initializers.All(initializer => CanDeferExpressionEmission(model, initializer.Expression)),
            _ => model.GetConstantValue(expression).HasValue,
        };
    }

    /// <summary>
    /// Prepares arguments for a method call, handling various parameter types and calling conventions.
    /// </summary>
    /// <param name="model">The semantic model used for analysis.</param>
    /// <param name="symbol">The method symbol being called.</param>
    /// <param name="arguments">The list of argument syntax nodes.</param>
    /// <param name="callingConvention">The calling convention to use (default is Cdecl).</param>
    /// <param name="isSysCall">Indicates whether the method is a system call.</param>
    /// <remarks>
    /// This method processes named arguments, determines parameter order based on calling convention,
    /// and handles regular, params, and out arguments. It also takes into account system calls for
    /// special processing of out parameters.
    /// </remarks>
    private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        // 1. Process named arguments
        var namedArguments = ProcessNamedArguments(model, arguments);
        var argumentMap = MapArgumentsToParameters(model, symbol, arguments);
        var argumentExpressions = MapArgumentExpressionsToParameters(symbol, arguments);

        if (ShouldPreserveArgumentEvaluationOrder(model, symbol, arguments, callingConvention))
        {
            PrepareArgumentsPreservingEvaluationOrder(model, symbol, arguments, callingConvention);
            return;
        }

        // 2. Determine parameter order based on calling convention
        var parameters = DetermineParameterOrder(symbol, callingConvention);

        // 3. Process each parameter
        foreach (var parameter in parameters)
        {
            if (IsByRef(parameter.RefKind))
            {
                if (!argumentMap.TryGetValue(parameter, out var argument))
                    throw new CompilationException(DiagnosticId.SyntaxNotSupported,
                        $"Missing argument for by-ref parameter '{parameter.Name}'.");

                ProcessByRefArgument(model, symbol, parameter, argument);
                continue;
            }

            // a. Named Arguments
            // Example: MethodCall(paramName: value)
            if (TryProcessNamedArgument(model, namedArguments, parameter))
                continue;

            if (parameter.IsParams)
            {
                // b. Params Arguments
                // Example: MethodCall(1, 2, 3, 4, 5)
                // Where method signature is: void MethodCall(params int[] numbers)
                ProcessParamsArgument(model, arguments, parameter);
            }
            else
            {
                // d. Regular Arguments
                // Example: MethodCall(42, "Hello")
                // Where method signature is: void MethodCall(int num, string message)
                ProcessRegularArgument(model, argumentExpressions, parameter);
            }
        }
    }

    // Helper methods
    private static Dictionary<IParameterSymbol, ExpressionSyntax> ProcessNamedArguments(SemanticModel model, IReadOnlyList<SyntaxNode> arguments)
    {
        // NameColon is not null means the argument is a named argument
        // so we need to get the parameter symbol and the expression
        // and put them into a dictionary
        return arguments.OfType<ArgumentSyntax>()
             .Where(p => p.NameColon is not null)
             .Select(p => (Symbol: (IParameterSymbol)ModelExtensions.GetSymbolInfo(model, p.NameColon!.Name).Symbol!, p.Expression))
             .ToDictionary(p =>
                 p.Symbol,
                 p => p.Expression, (IEqualityComparer<IParameterSymbol>)SymbolEqualityComparer.Default);
    }

    private static Dictionary<IParameterSymbol, ExpressionSyntax> MapArgumentExpressionsToParameters(IMethodSymbol symbol, IEnumerable<SyntaxNode> arguments)
    {
        Dictionary<IParameterSymbol, ExpressionSyntax> map = new(SymbolEqualityComparer.Default);
        int positionalIndex = 0;

        foreach (SyntaxNode node in arguments)
        {
            IParameterSymbol? targetParameter;
            ExpressionSyntax expression;

            switch (node)
            {
                case ArgumentSyntax argument:
                    expression = argument.Expression;
                    targetParameter = argument.NameColon is null
                        ? symbol.Parameters.ElementAtOrDefault(positionalIndex++)
                        : symbol.Parameters.FirstOrDefault(p => p.Name == argument.NameColon.Name.Identifier.ValueText);
                    break;
                case ExpressionSyntax exp:
                    expression = exp;
                    targetParameter = symbol.Parameters.ElementAtOrDefault(positionalIndex++);
                    break;
                default:
                    continue;
            }

            if (targetParameter is not null && !map.ContainsKey(targetParameter))
                map[targetParameter] = expression;
        }

        return map;
    }

    private static IEnumerable<IParameterSymbol> DetermineParameterOrder(IMethodSymbol symbol, CallingConvention callingConvention)
    {
        return callingConvention == CallingConvention.Cdecl ? symbol.Parameters.Reverse() : symbol.Parameters;
    }

    private void PrepareArgumentsPreservingEvaluationOrder(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention)
    {
        if (TryPrepareArgumentsWithStackReversal(model, symbol, arguments, callingConvention))
            return;

        Dictionary<IParameterSymbol, byte> slots = new(SymbolEqualityComparer.Default);
        List<byte> anonymousSlots = [];
        int positionalIndex = 0;

        foreach (SyntaxNode argument in arguments)
        {
            if (!TryGetRegularArgumentExpression(symbol, argument, ref positionalIndex, out IParameterSymbol? parameter, out ExpressionSyntax? expression) ||
                parameter is null ||
                expression is null)
                continue;

            ConvertExpression(model, expression);
            byte slot = AddAnonymousVariable();
            AccessSlot(OpCode.STLOC, slot);
            anonymousSlots.Add(slot);
            slots[parameter] = slot;
        }

        foreach (IParameterSymbol parameter in DetermineParameterOrder(symbol, callingConvention))
        {
            if (slots.TryGetValue(parameter, out byte slot))
                AccessSlot(OpCode.LDLOC, slot);
            else
                Push(parameter.ExplicitDefaultValue);
        }

        foreach (byte slot in anonymousSlots)
            RemoveAnonymousVariable(slot);
    }

    private bool TryPrepareArgumentsWithStackReversal(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention)
    {
        if (callingConvention != CallingConvention.Cdecl ||
            !TryMapArgumentsInParameterOrder(symbol, arguments, out ExpressionSyntax?[] orderedExpressions))
            return false;

        for (int i = 0; i < symbol.Parameters.Length; i++)
        {
            if (orderedExpressions[i] is { } expression)
                ConvertExpression(model, expression);
            else
                Push(symbol.Parameters[i].ExplicitDefaultValue);
        }

        if (symbol.Parameters.Length > 1)
            ReverseStackItems(symbol.Parameters.Length);

        return true;
    }

    private static bool TryMapArgumentsInParameterOrder(IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, out ExpressionSyntax?[] orderedExpressions)
    {
        orderedExpressions = new ExpressionSyntax?[symbol.Parameters.Length];
        int positionalIndex = 0;
        int lastOrdinal = -1;

        foreach (SyntaxNode argument in arguments)
        {
            if (!TryGetRegularArgumentExpression(symbol, argument, ref positionalIndex, out IParameterSymbol? parameter, out ExpressionSyntax? expression) ||
                parameter is null ||
                expression is null ||
                parameter.Ordinal < lastOrdinal ||
                orderedExpressions[parameter.Ordinal] is not null)
                return false;

            orderedExpressions[parameter.Ordinal] = expression;
            lastOrdinal = parameter.Ordinal;
        }

        return true;
    }

    private static bool TryGetRegularArgumentExpression(IMethodSymbol symbol, SyntaxNode argument, ref int positionalIndex, out IParameterSymbol? parameter, out ExpressionSyntax? expression)
    {
        switch (argument)
        {
            case ArgumentSyntax syntax:
                expression = syntax.Expression;
                parameter = syntax.NameColon is null
                    ? symbol.Parameters.ElementAtOrDefault(positionalIndex++)
                    : symbol.Parameters.FirstOrDefault(p => p.Name == syntax.NameColon.Name.Identifier.ValueText);
                return parameter is not null;
            case ExpressionSyntax syntax:
                expression = syntax;
                parameter = symbol.Parameters.ElementAtOrDefault(positionalIndex++);
                return parameter is not null;
            default:
                expression = null;
                parameter = null;
                return false;
        }
    }

    private static bool ShouldPreserveArgumentEvaluationOrder(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention)
    {
        if (callingConvention != CallingConvention.Cdecl || symbol.Parameters.Length <= 1)
            return false;

        if (symbol.Parameters.Any(parameter => parameter.IsParams || IsByRef(parameter.RefKind)))
            return false;

        foreach (SyntaxNode argument in arguments)
        {
            if (argument is ArgumentSyntax { RefKindKeyword.RawKind: not 0 })
                return false;
            if (argument is not ArgumentSyntax and not ExpressionSyntax)
                return false;
        }

        return arguments
            .Select(ExtractExpression)
            .Any(expression => HasObservableEvaluationOrder(model, expression));
    }

    private static bool HasObservableEvaluationOrder(SemanticModel model, ExpressionSyntax expression)
    {
        foreach (SyntaxNode node in expression.DescendantNodesAndSelf(descendIntoChildren: node =>
            node is not LambdaExpressionSyntax and not AnonymousMethodExpressionSyntax &&
            (node is not ExpressionSyntax childExpression || !model.GetConstantValue(childExpression).HasValue)))
        {
            switch (node)
            {
                case InvocationExpressionSyntax invocation when !model.GetConstantValue(invocation).HasValue:
                case AssignmentExpressionSyntax:
                case AwaitExpressionSyntax:
                case ThrowExpressionSyntax:
                case ObjectCreationExpressionSyntax:
                case ImplicitObjectCreationExpressionSyntax:
                case PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression):
                case PrefixUnaryExpressionSyntax prefix when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                    return true;
                case MemberAccessExpressionSyntax memberAccess when model.GetSymbolInfo(memberAccess).Symbol is IPropertySymbol:
                case ElementAccessExpressionSyntax elementAccess when model.GetSymbolInfo(elementAccess).Symbol is IPropertySymbol:
                    return true;
            }
        }

        return false;
    }

    private bool TryProcessNamedArgument(SemanticModel model, Dictionary<IParameterSymbol, ExpressionSyntax> namedArguments, IParameterSymbol parameter)
    {
        if (!namedArguments.TryGetValue(parameter, out var expression)) return false;
        ConvertExpression(model, expression);
        return true;
    }

    private void ProcessParamsArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count > parameter.Ordinal)
        {
            if (TryProcessSingleParamsArgument(model, arguments, parameter))
                return;

            ProcessMultipleParamsArguments(model, arguments, parameter);
        }
        else
        {
            AddInstruction(OpCode.NEWARRAY0);
        }
    }

    private bool TryProcessSingleParamsArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count != parameter.Ordinal + 1) return false;
        var expression = ExtractExpression(arguments[parameter.Ordinal]);
        var conversion = model.ClassifyConversion(expression, parameter.Type);
        if (!conversion.Exists) return false;
        ConvertExpression(model, expression);
        return true;
    }

    private void ProcessMultipleParamsArguments(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        for (int i = arguments.Count - 1; i >= parameter.Ordinal; i--)
        {
            var expression = ExtractExpression(arguments[i]);
            ConvertExpression(model, expression);
        }
        Push(arguments.Count - parameter.Ordinal);
        AddInstruction(OpCode.PACK);
    }

    private void ProcessRegularArgument(SemanticModel model, IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> argumentExpressions, IParameterSymbol parameter)
    {
        if (argumentExpressions.TryGetValue(parameter, out var expression))
            ConvertExpression(model, expression);
        else
            Push(parameter.ExplicitDefaultValue);
    }

    private static ExpressionSyntax ExtractExpression(SyntaxNode node)
    {
        return node switch
        {
            ArgumentSyntax argument => argument.Expression,
            ExpressionSyntax exp => exp,
            _ => throw CompilationException.UnsupportedSyntax(node, $"Unsupported argument node type '{node.GetType().Name}'. Expected ArgumentSyntax or ExpressionSyntax."),
        };
    }
    #endregion
}
