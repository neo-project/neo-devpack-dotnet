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
        => _instructionsBuilder.AddInstruction(new Instruction
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
    internal Instruction CallContractMethod(UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags = CallFlags.All)
    {
        ushort token = Context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
        return _instructionsBuilder.AddInstruction(new Instruction
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

        if (TryProcessSpecialMethods(model, symbol, null, arguments))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

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
        if (TryProcessSystemMethods(model, symbol, null, null) || TryProcessInlineMethods(model, symbol, null))
            return;

        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, symbol);

        int pc = symbol.Parameters.Length + (symbol.IsStatic ? 0 : 1);
        if (pc > 1 && methodCallingConvention != callingConvention)
            _instructionsBuilder.ReverseStackItems(pc);

        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);

        var parameters = symbol.Parameters;
        parameters.Where(p => Context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in Context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    default:
                        throw new CompilationException(sync, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {sync}");
                }
            }
        });
    }

    private bool TryProcessSpecialMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode> arguments)
    {
        return TryProcessSystemMethods(model, symbol, instanceExpression, arguments) ||
               TryProcessInlineMethods(model, symbol, arguments);
    }

    private void ProcessOutParameters(SemanticModel model, IMethodSymbol symbol, IEnumerable<SyntaxNode> arguments)
    {
        var parameters = DetermineParameterOrder(symbol, CallingConvention.Cdecl);
        foreach (var parameter in parameters.Where(p => p.RefKind == RefKind.Out))
        {
            if (arguments.ElementAtOrDefault(parameter.Ordinal) is not ArgumentSyntax argument) continue;

            ProcessOutArgument(model, symbol, parameter, argument);
        }
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
            default:
                throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {argument}");
        }
    }

    private void ProcessOutDeclaration(SemanticModel model, IMethodSymbol methodSymbol, IParameterSymbol parameter, SingleVariableDesignationSyntax designation)
    {
        var local = (ILocalSymbol)model.GetDeclaredSymbol(designation)!;
        ProcessOutSymbol(parameter, local);
        _instructionsBuilder.PushDefault(local.Type);
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
            case IDiscardSymbol:
                _instructionsBuilder.PushDefault(parameter.Type);
                Context.GetOrAddCapturedStaticField(parameter);
                StArgSlot(parameter);
                break;
            default:
                throw new CompilationException(identifierName, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {identifierName}");
        }
    }

    private void ProcessOutSymbol(IParameterSymbol parameter, ISymbol symbol)
    {
        bool parameterCaptured = Context.TryGetCapturedStaticField(parameter, out var parameterIndex);
        bool symbolCaptured = Context.TryGetCapturedStaticField(symbol, out var symbolIndex);

        if (parameterCaptured && !symbolCaptured)
        {
            Context.AssociateCapturedStaticField(symbol, parameterIndex);
        }
        else if (!parameterCaptured && symbolCaptured)
        {
            Context.AssociateCapturedStaticField(parameter, symbolIndex);
        }
        else if (parameterCaptured && symbolCaptured && parameterIndex != symbolIndex)
        {
            // both values are already captured in different indirectly connected methods,
            // but they are different, thus need to sync value from symbol to parameter
            if (!Context.OutStaticFieldsSync.TryGetValue(parameter, out var syncList))
            {
                syncList = new List<ISymbol>();
                Context.OutStaticFieldsSync[parameter] = syncList;
            }
            syncList.Add(symbol);
        }
        else if (!parameterCaptured && !symbolCaptured)
        {
            var index = Context.GetOrAddCapturedStaticField(symbol);
            Context.AssociateCapturedStaticField(parameter, index);
        }
    }

    private void HandleConstructorDuplication(bool instanceOnStack, CallingConvention methodCallingConvention, IMethodSymbol symbol)
    {
        if (instanceOnStack && methodCallingConvention != CallingConvention.Cdecl && symbol.MethodKind == MethodKind.Constructor)
            _instructionsBuilder.Dup();
    }

    private void HandleInstanceExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
                                        CallingConvention methodCallingConvention, bool beforeArguments)
    {
        if (symbol.IsStatic) return;

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
        parameters.Where(p => Context.OutStaticFieldsSync.ContainsKey(p)).ForEach(p =>
        {
            foreach (var sync in Context.OutStaticFieldsSync[p])
            {
                LdArgSlot(p);
                switch (sync)
                {
                    case IParameterSymbol param:
                        StArgSlot(param);
                        break;
                    case ILocalSymbol local:
                        StLocSlot(local);
                        break;
                    default:
                        throw new CompilationException(sync, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {sync}");
                }
            }
        });
    }

    // Helper method to get MethodConvert and CallingConvention
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol)
    {
        if (symbol.IsVirtualMethod())
            return (null, CallingConvention.Cdecl);

        var convert = Context.ConvertMethod(model, symbol);
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
                if (isConstructor) _instructionsBuilder.Dup();
                break;
            case 1:
                _ = isConstructor ? _instructionsBuilder.Over() : _instructionsBuilder.Swap();
                break;
            default:
                _ = isConstructor ? _instructionsBuilder.Pick(symbol.Parameters.Length) : _instructionsBuilder.Roll(symbol.Parameters.Length);
                break;
        }
    }

    // Helper method to get MethodConvert and CallingConvention for instance expression
    private (MethodConvert? convert, CallingConvention methodCallingConvention) GetMethodConvertAndCallingConvention(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression)
    {
        if (symbol.IsVirtualMethod() && instanceExpression is not BaseExpressionSyntax)
            return (null, CallingConvention.Cdecl);

        var convert = symbol.ReducedFrom is null
            ? Context.ConvertMethod(model, symbol)
            : Context.ConvertMethod(model, symbol.ReducedFrom);
        return (convert, convert._callingConvention);
    }

    // Helper method to convert instance expression
    private void ConvertInstanceExpression(SemanticModel model, ExpressionSyntax? instanceExpression)
    {
        if (instanceExpression is null)
            _instructionsBuilder.LdArg0();
        else
            ConvertExpression(model, instanceExpression);
    }

    private void EmitCall(MethodConvert target)
    {
        if (target._inline && !Context.Options.NoInline)
            EmitInlineInstructions(target);
        else
            _instructionsBuilder.Jump(OpCode.CALL_L, target._startTarget);
    }

    // Helper method to emit inline instructions
    private void EmitInlineInstructions(MethodConvert target)
    {
        for (int i = 0; i < target.Instructions.Count - 1; i++)
            _instructionsBuilder.AddInstruction(target.Instructions[i].Clone());
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
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {symbol.OriginalDefinition}.");

        _instructionsBuilder.Dup();
        _instructionsBuilder.Push(fields.Length);
        _instructionsBuilder.PickItem();
        _instructionsBuilder.Push(index);
        _instructionsBuilder.PickItem();
        _instructionsBuilder.AddInstruction(OpCode.CALLA);
    }

    private void InvokeMethod(SemanticModel model, IMethodSymbol method)
    {
        var convert = Context.ConvertMethod(model, method);
        _instructionsBuilder.Jump(OpCode.PUSHA, convert._startTarget);
    }

    private void InvokeMethod(MethodConvert convert)
        => _instructionsBuilder.Jump(OpCode.PUSHA, convert._startTarget);

    /// <summary>
    /// Attempts to process system constructors. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system constructors are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        switch (symbol.ToString())
        {
            //For the BigInteger(byte[]) constructor, prepares method arguments and changes the return type to integer.
            case "System.Numerics.BigInteger.BigInteger(byte[])":
                PrepareArgumentsForMethod(model, symbol, arguments);
                _instructionsBuilder.ChangeType(VM.Types.StackItemType.Integer);
                return true;
            //For other constructors, such as List<T>(), return processing failure.
            default:
                return false;
        }
    }

    /// <summary>
    /// Attempts to process system methods. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="instanceExpression">The instance expression representing the instance of method invocation, if any.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system methods are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        //If the method belongs to a delegate and the method name is "Invoke",
        //calls the PrepareArgumentsForMethod method with CallingConvention.Cdecl convention and changes the return type to integer.
        //Example: Func<int, int, int>(privateSum).Invoke(a, b);
        //see ~/tests/Neo.Compiler.CSharp.TestContracts/Contract_Delegate.cs
        if (symbol.ContainingType.TypeKind == TypeKind.Delegate && symbol.Name == "Invoke")
        {
            if (arguments is not null)
                PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
            ConvertExpression(model, instanceExpression!);
            _instructionsBuilder.AddInstruction(OpCode.CALLA);
            return true;
        }

        var key = symbol.ToString()!.Replace("out ", "");
        key = (from parameter in symbol.Parameters let parameterType = parameter.Type.ToString() where !parameter.Type.IsValueType && parameterType!.EndsWith('?') select parameterType).Aggregate(key, (current, parameterType) => current.Replace(parameterType, parameterType[..^1]));
        if (key == "string.ToString()") key = "object.ToString()";
        if (!SystemMethods.SystemCallHandlers.TryGetValue(key, out var handler)) return false;
        handler(this, model, symbol, instanceExpression, arguments);
        return true;
    }

    /// <summary>
    /// Attempts to process system operators. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">An array of expression parameters.</param>
    /// <returns>True if system operators are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemOperators(SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
    {
        switch (symbol.ToString())
        {
            //Handles cases of equality operator (==), comparing whether two objects or strings are equal.
            case "object.operator ==(object, object)":
            case "string.operator ==(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                _instructionsBuilder.Equal();
                return true;
            //Handles cases of inequality operator (!=), comparing whether two objects are not equal.
            case "object.operator !=(object, object)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                _instructionsBuilder.NotEqual();
                return true;
            //Handles cases of string concatenation operator (+), concatenating two strings into one.
            case "string.operator +(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                _instructionsBuilder.Cat();
                _instructionsBuilder.ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating a string with an object.
            //Unsupported interpolation: object
            case "string.operator +(string, object)":
                ConvertExpression(model, arguments[0]);
                ConvertObjectToString(model, arguments[1]);
                _instructionsBuilder.Cat();
                _instructionsBuilder.ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating an object with a string.
            //Unsupported interpolation: object
            case "string.operator +(object, string)":
                ConvertObjectToString(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                _instructionsBuilder.Cat();
                _instructionsBuilder.ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            default:
                return false;
        }
    }
}
