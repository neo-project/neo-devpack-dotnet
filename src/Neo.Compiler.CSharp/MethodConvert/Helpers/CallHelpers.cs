// Copyright (C) 2015-2025 The Neo Project.
//
// CallHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Akka.Util.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Creates an instruction to call an interop method using the given descriptor.
    /// </summary>
    /// <param name="descriptor">The interop descriptor representing the method to call.</param>
    /// <returns>The instruction to perform the interop call.</returns>
    private Instruction CallInteropMethod(InteropDescriptor descriptor)
        => AddInstruction(new Instruction
        {
            OpCode = OpCode.SYSCALL,
            Operand = BitConverter.GetBytes(descriptor)
        });

    /// <summary>
    /// Creates an instruction to call a method in another smart contract identified by its hash.
    /// </summary>
    /// <param name="hash">The hash of the contract containing the method.</param>
    /// <param name="method">The name of the method to call.</param>
    /// <param name="parametersCount">The number of parameters the method takes.</param>
    /// <param name="hasReturnValue">Whether the method returns a value.</param>
    /// <param name="callFlags">The call flags to use for the method call.</param>
    /// <returns>The instruction to perform the contract method call.</returns>
    private Instruction CallContractMethod(UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags = CallFlags.All)
    {
        ushort token = _context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.CALLT,
            Operand = BitConverter.GetBytes(token)
        });
    }

    /// <summary>
    /// Creates instructions to call an instance method, handling the instance on the stack and preparing the arguments.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="instanceOnStack">Whether the instance is on the stack.</param>
    /// <param name="arguments">The list of arguments for the method call.</param>
    private void CallInstanceMethod(SemanticModel model, IMethodSymbol symbol, bool instanceOnStack, IReadOnlyList<ArgumentSyntax> arguments)
    {
        ProcessOutParameters(model, symbol, arguments);

        if (TryProcessSpecialMethods(model, symbol, null, arguments, instanceOnStack))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

        if (convert != null && convert.Instructions.Count == 1 && convert.Instructions[0].OpCode == OpCode.RET && !symbol.IsExtern)
            return;  // Do not call meaningless contructors
        if (NeedInstanceConstructor(symbol) && convert != null && convert.Instructions.Count >= 2)
        {
            Instruction initslot = convert.Instructions[0];
            Instruction ret = convert.Instructions[1];
            if (initslot.OpCode == OpCode.INITSLOT && initslot.Operand?[0] == 0 && initslot.Operand[1] == 1
             && ret.OpCode == OpCode.RET)
                return;  // Do not call meaningless contructors
        }

        HandleConstructorDuplication(instanceOnStack, methodCallingConvention, symbol);

        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);

        HandleInstanceOnStack(symbol, instanceOnStack, methodCallingConvention);

        EmitMethodCall(convert, symbol);
    }

    /// <summary>
    /// Creates instructions to call a method with an optional instance expression, handling both instance and static methods.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="instanceExpression">The optional instance expression for the method call.</param>
    /// <param name="arguments">The list of arguments for the method call.</param>
    private void CallMethodWithInstanceExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, params SyntaxNode[] arguments)
    {
        ProcessOutParameters(model, symbol, arguments);

        if (TryProcessSpecialMethods(model, symbol, instanceExpression, arguments))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol, instanceExpression);

        if (convert != null && convert.Instructions.Count == 1 && convert.Instructions[0].OpCode == OpCode.RET && !symbol.IsExtern)
            return;  // Do not call meaningless contructors
        if (NeedInstanceConstructor(symbol) && convert != null && convert.Instructions.Count >= 2)
        {
            Instruction initslot = convert.Instructions[0];
            Instruction ret = convert.Instructions[1];
            if (initslot.OpCode == OpCode.INITSLOT && initslot.Operand?[0] == 0 && initslot.Operand[1] == 1
             && ret.OpCode == OpCode.RET)
                return;  // Do not call meaningless contructors
        }

        HandleInstanceExpression(model, symbol, instanceExpression, methodCallingConvention, beforeArguments: true);

        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);

        HandleInstanceExpression(model, symbol, instanceExpression, methodCallingConvention, beforeArguments: false);

        EmitMethodCall(convert, symbol);
    }

    /// <summary>
    /// Creates instructions to call a method with a specified calling convention.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="callingConvention">The calling convention to use for the method call.</param>
    private void CallMethodWithConvention(SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        if (TryProcessSystemMethods(model, symbol, null, null) || TryProcessInlineMethods(model, symbol, null, null))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

        int pc = symbol.Parameters.Length + (!NeedInstanceConstructor(symbol) ? 0 : 1);
        if (pc > 1 && methodCallingConvention != callingConvention)
            ReverseStackItems(pc);

        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);

        var parameters = symbol.Parameters;
        parameters.Where(p => _context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in _context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync.Symbol)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    case IFieldSymbol fieldSync:
                        StoreOutFieldValue(fieldSync, sync.InstanceSlot);
                        break;
                    default:
                        throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported symbol type '{sync.Symbol.GetType().Name}' for parameter synchronization. Only parameters, local variables, and fields are supported.");
                }
            }
        });
    }

    private bool TryProcessSpecialMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode> arguments, bool instanceOnStack = false)
    {
        return TryProcessSystemMethods(model, symbol, instanceExpression, arguments) ||
               TryProcessInlineMethods(model, symbol, instanceExpression, arguments, instanceOnStack);
    }

    private void ProcessOutParameters(SemanticModel model, IMethodSymbol symbol, IEnumerable<SyntaxNode> arguments)
    {
        var argumentMap = MapArgumentsToParameters(model, symbol, arguments);
        var parameters = DetermineParameterOrder(symbol, CallingConvention.Cdecl);
        foreach (var parameter in parameters.Where(p => p.RefKind == RefKind.Out))
        {
            if (!argumentMap.TryGetValue(parameter, out var argument)) continue;

            ProcessOutArgument(model, symbol, parameter, argument);
        }
    }

    private static Dictionary<IParameterSymbol, ArgumentSyntax> MapArgumentsToParameters(SemanticModel model, IMethodSymbol symbol, IEnumerable<SyntaxNode> arguments)
    {
        Dictionary<IParameterSymbol, ArgumentSyntax> map = new(SymbolEqualityComparer.Default);
        int positionalIndex = 0;
        foreach (SyntaxNode node in arguments)
        {
            if (node is not ArgumentSyntax argument) continue;

            IParameterSymbol? targetParameter;
            if (argument.NameColon is null)
            {
                targetParameter = symbol.Parameters.ElementAtOrDefault(positionalIndex++);
            }
            else
            {
                string parameterName = argument.NameColon.Name.Identifier.ValueText;
                targetParameter = symbol.Parameters.FirstOrDefault(p => p.Name == parameterName);
            }

            if (targetParameter is null) continue;
            if (!map.ContainsKey(targetParameter))
                map[targetParameter] = argument;
        }
        return map;
    }

    private void ProcessOutArgument(SemanticModel model, IMethodSymbol methodSymbol, IParameterSymbol parameter, ArgumentSyntax argument)
    {
        switch (argument.Expression)
        {
            case DeclarationExpressionSyntax { Designation: SingleVariableDesignationSyntax designation }:
                ProcessOutDeclaration(model, methodSymbol, parameter, designation);
                break;
            case IdentifierNameSyntax identifierName:
                ProcessOutIdentifier(model, parameter, identifierName);
                break;
            case MemberAccessExpressionSyntax memberAccess:
                ProcessOutMemberAccess(model, parameter, memberAccess);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(argument, $"Unsupported out parameter syntax '{argument.GetType().Name}'. Use 'out var variable' or 'out existingVariable'.");
        }
    }

    private void ProcessOutDeclaration(SemanticModel model, IMethodSymbol methodSymbol, IParameterSymbol parameter, SingleVariableDesignationSyntax designation)
    {
        var local = (ILocalSymbol)model.GetDeclaredSymbol(designation)!;
        ProcessOutSymbol(parameter, local);
        PushDefault(local.Type);
        StLocSlot(local); // initialize the local variable with default value
    }

    private void ProcessOutIdentifier(SemanticModel model, IParameterSymbol parameter, IdentifierNameSyntax identifierName)
    {
        var symbol = model.GetSymbolInfo(identifierName).Symbol!;
        switch (symbol)
        {
            case ILocalSymbol local:
                LdLocSlot(local);
                ProcessOutSymbol(parameter, local);
                StLocSlot(local);
                break;
            case IParameterSymbol param:
                LdArgSlot(param);
                ProcessOutSymbol(parameter, param);
                StArgSlot(param);
                break;
            case IFieldSymbol field:
                ProcessOutField(model, parameter, field, instanceExpression: null, identifierName);
                break;
            case IDiscardSymbol:
                PushDefault(parameter.Type);
                _context.GetOrAddCapturedStaticField(parameter);
                StArgSlot(parameter);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(identifierName, $"Unsupported identifier '{identifierName.Identifier.ValueText}' in out parameter. Use a variable name or discard ('_').");
        }
    }

    private void ProcessOutMemberAccess(SemanticModel model, IParameterSymbol parameter, MemberAccessExpressionSyntax memberAccess)
    {
        var symbol = model.GetSymbolInfo(memberAccess).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ProcessOutField(model, parameter, field, memberAccess.Expression, memberAccess);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(memberAccess, $"Unsupported member access '{memberAccess}' in out parameter. Only fields are supported.");
        }
    }

    private void ProcessOutField(SemanticModel model, IParameterSymbol parameter, IFieldSymbol field, ExpressionSyntax? instanceExpression, SyntaxNode syntaxNode)
    {
        if (field.IsStatic)
        {
            byte fieldIndex = _context.AddStaticField(field);
            AccessSlot(OpCode.LDSFLD, fieldIndex);
            ProcessOutSymbol(parameter, field);
            AccessSlot(OpCode.STSFLD, fieldIndex);
            return;
        }

        byte instanceSlot = _context.AddAnonymousStaticField();
        if (instanceExpression is null)
            AddInstruction(OpCode.LDARG0);
        else
            ConvertExpression(model, instanceExpression);
        AccessSlot(OpCode.STSFLD, instanceSlot);

        AccessSlot(OpCode.LDSFLD, instanceSlot);
        int fieldOffset = Array.IndexOf(field.ContainingType.GetFields(), field);
        Push(fieldOffset);
        AddInstruction(OpCode.PICKITEM);

        ProcessOutSymbol(parameter, field, instanceSlot);
        if (!_context.TryGetCapturedStaticField(field, out var fieldStorageIndex))
            fieldStorageIndex = _context.GetOrAddCapturedStaticField(field);
        AccessSlot(OpCode.STSFLD, fieldStorageIndex);
    }

    private void ProcessOutSymbol(IParameterSymbol parameter, ISymbol symbol, byte? instanceSlot = null)
    {
        if (symbol is IFieldSymbol fieldSymbol)
        {
            ProcessOutFieldSymbol(parameter, fieldSymbol, instanceSlot);
            return;
        }

        bool parameterCaptured = _context.TryGetCapturedStaticField(parameter, out var parameterIndex);
        bool symbolCaptured = _context.TryGetCapturedStaticField(symbol, out var symbolIndex);

        if (parameterCaptured && !symbolCaptured)
        {
            _context.AssociateCapturedStaticField(symbol, parameterIndex);
        }
        else if (!parameterCaptured && symbolCaptured)
        {
            _context.AssociateCapturedStaticField(parameter, symbolIndex);
        }
        else if (parameterCaptured && symbolCaptured && parameterIndex != symbolIndex)
        {
            if (!_context.OutStaticFieldsSync.TryGetValue(parameter, out var syncList))
            {
                syncList = new List<CompilationContext.OutSyncTarget>();
                _context.OutStaticFieldsSync[parameter] = syncList;
            }
            syncList.Add(new CompilationContext.OutSyncTarget(symbol));
        }
        else if (!parameterCaptured && !symbolCaptured)
        {
            var index = _context.GetOrAddCapturedStaticField(symbol);
            _context.AssociateCapturedStaticField(parameter, index);
        }
    }

    private void ProcessOutFieldSymbol(IParameterSymbol parameter, IFieldSymbol field, byte? instanceSlot)
    {
        bool parameterCaptured = _context.TryGetCapturedStaticField(parameter, out var parameterIndex);
        byte fieldIndex = field.IsStatic
            ? _context.AddStaticField(field)
            : _context.GetOrAddCapturedStaticField(field);

        if (field.IsStatic)
            _context.AssociateCapturedStaticField(field, fieldIndex);

        if (!parameterCaptured)
        {
            _context.AssociateCapturedStaticField(parameter, fieldIndex);
            parameterIndex = fieldIndex;
        }

        bool requireSync = !field.IsStatic || parameterIndex != fieldIndex;
        if (requireSync)
        {
            if (!field.IsStatic && instanceSlot is null)
                throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Missing instance context for field '{field.Name}' in out parameter synchronization.");

            if (!_context.OutStaticFieldsSync.TryGetValue(parameter, out var syncList))
            {
                syncList = new List<CompilationContext.OutSyncTarget>();
                _context.OutStaticFieldsSync[parameter] = syncList;
            }
            syncList.Add(new CompilationContext.OutSyncTarget(field, instanceSlot));
        }
    }

    private void StoreOutFieldValue(IFieldSymbol field, byte? instanceSlot)
    {
        if (field.IsStatic)
        {
            byte index = _context.AddStaticField(field);
            AccessSlot(OpCode.STSFLD, index);
            return;
        }

        if (instanceSlot is null)
            throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Missing instance context for field '{field.Name}' in out argument synchronization.");

        AccessSlot(OpCode.LDSFLD, instanceSlot.Value);
        int fieldOffset = Array.IndexOf(field.ContainingType.GetFields(), field);
        Push(fieldOffset);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SETITEM);
    }

    private void HandleConstructorDuplication(bool instanceOnStack, CallingConvention methodCallingConvention, IMethodSymbol symbol)
    {
        if (instanceOnStack && methodCallingConvention != CallingConvention.Cdecl && symbol.MethodKind == MethodKind.Constructor)
            AddInstruction(OpCode.DUP);
    }

    private void HandleInstanceExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
                                        CallingConvention methodCallingConvention, bool beforeArguments)
    {
        if (!NeedInstanceConstructor(symbol)) return;

        bool shouldConvert = beforeArguments ? (methodCallingConvention != CallingConvention.Cdecl)
                                             : (methodCallingConvention == CallingConvention.Cdecl);

        if (shouldConvert)
            ConvertInstanceExpression(model, instanceExpression);
    }

    private void EmitMethodCall(MethodConvert? convert, IMethodSymbol symbol)
    {
        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);

        var parameters = symbol.Parameters;
        parameters.Where(p => _context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in _context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync.Symbol)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    case IFieldSymbol fieldSync:
                        StoreOutFieldValue(fieldSync, sync.InstanceSlot);
                        break;
                    default:
                        throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported symbol type '{sync.Symbol.GetType().Name}' for parameter synchronization. Only parameters, local variables, and fields are supported.");
                }
            }
        });
    }

    // Helper method to get MethodConvert and CallingConvention
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol)
    {
        if (symbol.IsVirtualMethod())
            return (null, CallingConvention.Cdecl);

        var convert = _context.ConvertMethod(model, symbol);
        return (convert, convert._callingConvention);
    }

    // Helper method to handle instance on stack
    private void HandleInstanceOnStack(IMethodSymbol symbol, bool instanceOnStack, CallingConvention methodCallingConvention)
    {
        if (!instanceOnStack || methodCallingConvention != CallingConvention.Cdecl)
            return;

        bool isConstructor = symbol.MethodKind == MethodKind.Constructor;

        switch (symbol.Parameters.Length)
        {
            case 0:
                if (isConstructor && NeedInstanceConstructor(symbol))
                    AddInstruction(OpCode.DUP);
                break;
            case 1:
                AddInstruction(isConstructor ? OpCode.OVER : OpCode.SWAP);
                break;
            default:
                Push(symbol.Parameters.Length);
                AddInstruction(isConstructor ? OpCode.PICK : OpCode.ROLL);
                break;
        }
    }

    // Helper method to get MethodConvert and CallingConvention for instance expression
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression)
    {
        if (symbol.IsVirtualMethod() && instanceExpression is not BaseExpressionSyntax)
            return (null, CallingConvention.Cdecl);

        var convert = symbol.ReducedFrom is null
            ? _context.ConvertMethod(model, symbol)
            : _context.ConvertMethod(model, symbol.ReducedFrom);
        return (convert, convert._callingConvention);
    }

    // Helper method to convert instance expression
    private void ConvertInstanceExpression(SemanticModel model, ExpressionSyntax? instanceExpression)
    {
        if (instanceExpression is null)
            AddInstruction(OpCode.LDARG0);
        else
            ConvertExpression(model, instanceExpression);
    }

    private void EmitCall(MethodConvert target)
    {
        if (target._inline && !_context.Options.NoInline)
            EmitInlineInstructions(target);
        else
            Jump(OpCode.CALL_L, target._startTarget);
    }

    // Helper method to emit inline instructions
    private void EmitInlineInstructions(MethodConvert target)
    {
        for (int i = 0; i < target._instructions.Count - 1; i++)
            AddInstruction(target._instructions[i].Clone());
    }

    private void CallVirtual(IMethodSymbol symbol)
    {
        if (symbol.ContainingType.TypeKind == TypeKind.Interface)
            throw new CompilationException(symbol.ContainingType, DiagnosticId.InterfaceCall, "Interfaces are not supported.");

        var members = symbol.ContainingType.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToArray();
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();

        int index = Array.IndexOf(virtualMethods, symbol);
        if (index < 0)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Virtual method '{symbol.Name}' not found in type '{symbol.ContainingType.Name}'. Ensure the method is properly declared as virtual or override.");

        AddInstruction(OpCode.DUP);
        Push(fields.Length);
        AddInstruction(OpCode.PICKITEM);
        Push(index);
        AddInstruction(OpCode.PICKITEM);
        AddInstruction(OpCode.CALLA);
    }

    private void InvokeMethod(SemanticModel model, IMethodSymbol method)
    {
        var convert = _context.ConvertMethod(model, method);
        Jump(OpCode.PUSHA, convert._startTarget);
    }

    private void InvokeMethod(MethodConvert convert)
        => Jump(OpCode.PUSHA, convert._startTarget);
}
