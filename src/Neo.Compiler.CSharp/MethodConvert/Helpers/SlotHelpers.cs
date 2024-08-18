// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using scfx::Neo.SmartContract.Framework;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

internal partial class MethodConvert
{

    #region Variables

    private byte AddLocalVariable(ILocalSymbol symbol)
    {
        var index = (byte)(_localVariables.Count + _anonymousVariables.Count);
        _variableSymbols.Add((symbol, index));
        _localVariables.Add(symbol, index);
        if (_localsCount < index + 1)
            _localsCount = index + 1;
        _blockSymbols.Peek().Add(symbol);
        return index;
    }

    private byte AddAnonymousVariable()
    {
        var index = (byte)(_localVariables.Count + _anonymousVariables.Count);
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

    private void RemoveLocalVariable(ILocalSymbol symbol)
    {
        if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
            _localVariables.Remove(symbol);
    }

    #endregion

    #region Helper

    /// <summary>
    /// load parameter value
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private Instruction LdArgSlot(IParameterSymbol parameter)
    {
        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }
        if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.LDSFLD, staticIndex);
        }
        // local parameter in current method
        var index = _parameters[parameter];
        return AccessSlot(OpCode.LDARG, index);
    }

    /// <summary>
    /// store value to parameter
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private Instruction StArgSlot(IParameterSymbol parameter)
    {
        if (_context.TryGetCapturedStaticField(parameter, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }
        if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_parameters.ContainsKey(parameter))
        {
            //create static fields from captured parameter
            var staticIndex = _context.GetOrAddCapturedStaticField(parameter);
            CapturedLocalSymbols.Add(parameter);
            return AccessSlot(OpCode.STSFLD, staticIndex);
        }
        // local parameter in current method
        var index = _parameters[parameter];
        return AccessSlot(OpCode.STARG, index);
    }

    /// <summary>
    /// load local variable value
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    private Instruction LdLocSlot(ILocalSymbol local)
    {
        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.LDSFLD, staticFieldIndex);
        }
        if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local))
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
    /// store value to local variable
    /// </summary>
    /// <param name="local"></param>
    /// <returns></returns>
    private Instruction StLocSlot(ILocalSymbol local)
    {
        if (_context.TryGetCapturedStaticField(local, out var staticFieldIndex))
        {
            //using created static fields
            return AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }
        if (Symbol.MethodKind == MethodKind.AnonymousFunction && !_localVariables.ContainsKey(local))
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

    private bool TryProcessInlineMethods(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments)
    {
        SyntaxNode? syntaxNode = null;
        if (!symbol.DeclaringSyntaxReferences.IsEmpty)
            syntaxNode = symbol.DeclaringSyntaxReferences[0].GetSyntax();

        if (syntaxNode is not BaseMethodDeclarationSyntax syntax) return false;
        if (!symbol.GetAttributesWithInherited().Any(attribute => attribute.ConstructorArguments.Length > 0
                && attribute.AttributeClass?.Name == nameof(MethodImplAttribute)
                && attribute.ConstructorArguments[0].Value is not null
                && (MethodImplOptions)attribute.ConstructorArguments[0].Value! == MethodImplOptions.AggressiveInlining))
            return false;

        _internalInline = true;

        using (InsertSequencePoint(syntax))
        {
            if (arguments is not null) PrepareArgumentsForMethod(model, symbol, arguments);
            if (syntax.Body != null) ConvertStatement(model, syntax.Body);
        }
        return true;
    }

    private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        // 1. Process named arguments
        var namedArguments = ProcessNamedArguments(model, arguments);

        // 2. Determine parameter order based on calling convention
        var parameters = DetermineParameterOrder(symbol, callingConvention);

        // 3. Process each parameter
        foreach (var parameter in parameters)
        {
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
                // c. Regular Arguments
                // Example: MethodCall(42, "Hello")
                // Where method signature is: void MethodCall(int num, string message)
                ProcessRegularArgument(model, arguments, parameter);
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

    private static IEnumerable<IParameterSymbol> DetermineParameterOrder(IMethodSymbol symbol, CallingConvention callingConvention)
    {
        return callingConvention == CallingConvention.Cdecl ? symbol.Parameters.Reverse() : symbol.Parameters;
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

    private void ProcessRegularArgument(SemanticModel model, IReadOnlyList<SyntaxNode> arguments, IParameterSymbol parameter)
    {
        if (arguments.Count > parameter.Ordinal)
        {
            var argument = arguments[parameter.Ordinal];
            switch (argument)
            {
                case ArgumentSyntax { NameColon: null } arg:
                    ConvertExpression(model, arg.Expression);
                    return;
                case ExpressionSyntax ex:
                    ConvertExpression(model, ex);
                    return;
                default:
                    throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {argument}");
            }
        }
        Push(parameter.ExplicitDefaultValue);
    }

    private static ExpressionSyntax ExtractExpression(SyntaxNode node)
    {
        return node switch
        {
            ArgumentSyntax argument => argument.Expression,
            ExpressionSyntax exp => exp,
            _ => throw new CompilationException(node, DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {node}"),
        };
    }

    private Instruction IsType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.ISTYPE,
            Operand = [(byte)type]
        });
    }

    private Instruction ChangeType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.CONVERT,
            Operand = [(byte)type]
        });
    }

    private void InitializeFieldForObject(SemanticModel model, IFieldSymbol field, InitializerExpressionSyntax? initializer)
    {
        ExpressionSyntax? expression = null;
        if (initializer is not null)
        {
            foreach (var e in initializer.Expressions)
            {
                if (e is not AssignmentExpressionSyntax ae)
                    throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
                if (SymbolEqualityComparer.Default.Equals(field, ModelExtensions.GetSymbolInfo(model, ae.Left).Symbol))
                {
                    expression = ae.Right;
                    break;
                }
            }
        }
        if (expression is null)
            PushDefault(field.Type);
        else
            ConvertExpression(model, expression);
    }

    private void CreateObject(SemanticModel model, ITypeSymbol type, InitializerExpressionSyntax? initializer)
    {
        var members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToArray();
        if (fields.Length == 0 || type.IsValueType || type.IsRecord)
        {
            AddInstruction(type.IsValueType || type.IsRecord ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);
            foreach (var field in fields)
            {
                AddInstruction(OpCode.DUP);
                InitializeFieldForObject(model, field, initializer);
                AddInstruction(OpCode.APPEND);
            }
        }
        else
        {
            for (int i = fields.Length - 1; i >= 0; i--)
                InitializeFieldForObject(model, fields[i], initializer);
            Push(fields.Length);
            AddInstruction(OpCode.PACK);
        }
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        if (type.IsRecord || virtualMethods.Length <= 0) return;
        var index = _context.AddVTable(type);
        AddInstruction(OpCode.DUP);
        AccessSlot(OpCode.LDSFLD, index);
        AddInstruction(OpCode.APPEND);
    }

    private Instruction Jump(OpCode opcode, JumpTarget target)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode,
            Target = target
        });
    }

    /// <summary>
    /// Convert a throw expression or throw statement to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the Throw.</param>
    /// <param name="exception">The content of exception</param>
    /// <exception cref="CompilationException">Only a single parameter is supported for exceptions.</exception>
    /// <example>
    /// throw statement:
    /// <code>
    /// if (shapeAmount <= 0)
    /// {
    ///     throw new Exception("Amount of shapes must be positive.");
    /// }
    ///</code>
    /// throw expression:
    /// <code>
    /// string a = null;
    /// var b = a ?? throw new Exception();
    /// </code>
    /// <code>
    /// var first = args.Length >= 1 ? args[0] : throw new Exception();
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-throw-expression">The throw expression</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-catch-statement">Exception-handling statements - throw</seealso>
    private void Throw(SemanticModel model, ExpressionSyntax? exception)
    {
        if (exception is not null)
        {
            var type = ModelExtensions.GetTypeInfo(model, exception).Type!;
            if (type.IsSubclassOf(nameof(UncatchableException), includeThisClass: true))
            {
                AddInstruction(OpCode.ABORT);
                return;
            }
        }
        switch (exception)
        {
            case ObjectCreationExpressionSyntax expression:
                switch (expression.ArgumentList?.Arguments.Count)
                {
                    case null:
                    case 0:
                        Push("exception");
                        break;
                    case 1:
                        ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                        break;
                    default:
                        throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                }
                break;
            case null:
                AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                break;
            default:
                ConvertExpression(model, exception);
                break;
        }
        AddInstruction(OpCode.THROW);
    }
    #endregion
}
