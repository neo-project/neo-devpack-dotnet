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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Creates an instruction to call an interop method using the given descriptor.
    /// </summary>
    /// <param name="descriptor">The interop descriptor representing the method to call.</param>
    /// <returns>The instruction to perform the interop call.</returns>
    private Instruction CallInteropMethod(InteropDescriptor descriptor)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.SYSCALL,
            Operand = BitConverter.GetBytes(descriptor)
        });
    }

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
        if (TryProcessSystemMethods(model, symbol, null, arguments))
            return;
        if (TryProcessInlineMethods(model, symbol, arguments))
            return;
        MethodConvert? convert;
        CallingConvention methodCallingConvention;
        if (symbol.IsVirtualMethod())
        {
            convert = null;
            methodCallingConvention = CallingConvention.Cdecl;
        }
        else
        {
            convert = _context.ConvertMethod(model, symbol);
            methodCallingConvention = convert._callingConvention;
        }
        bool isConstructor = symbol.MethodKind == MethodKind.Constructor;
        if (instanceOnStack && methodCallingConvention != CallingConvention.Cdecl && isConstructor)
            AddInstruction(OpCode.DUP);
        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);
        if (instanceOnStack && methodCallingConvention == CallingConvention.Cdecl)
        {
            switch (symbol.Parameters.Length)
            {
                case 0:
                    if (isConstructor) AddInstruction(OpCode.DUP);
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
        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);
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
        if (TryProcessSystemMethods(model, symbol, instanceExpression, arguments))
            return;
        if (TryProcessInlineMethods(model, symbol, arguments))
            return;
        MethodConvert? convert;
        CallingConvention methodCallingConvention;
        if (symbol.IsVirtualMethod() && instanceExpression is not BaseExpressionSyntax)
        {
            convert = null;
            methodCallingConvention = CallingConvention.Cdecl;
        }
        else
        {
            convert = symbol.ReducedFrom is null
                ? _context.ConvertMethod(model, symbol)
                : _context.ConvertMethod(model, symbol.ReducedFrom);
            methodCallingConvention = convert._callingConvention;
        }
        if (!symbol.IsStatic && methodCallingConvention != CallingConvention.Cdecl)
        {
            if (instanceExpression is null)
                AddInstruction(OpCode.LDARG0);
            else
                ConvertExpression(model, instanceExpression);
        }
        PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);
        if (!symbol.IsStatic && methodCallingConvention == CallingConvention.Cdecl)
        {
            if (instanceExpression is null)
                AddInstruction(OpCode.LDARG0);
            else
                ConvertExpression(model, instanceExpression);
        }
        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);
    }

    /// <summary>
    /// Creates instructions to call a method with a specified calling convention.
    /// </summary>
    /// <param name="model">The semantic model of the code.</param>
    /// <param name="symbol">The method symbol representing the method to call.</param>
    /// <param name="callingConvention">The calling convention to use for the method call.</param>
    private void CallMethodWithConvention(SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
    {
        if (TryProcessSystemMethods(model, symbol, null, null))
            return;
        if (TryProcessInlineMethods(model, symbol, null))
            return;
        MethodConvert? convert;
        CallingConvention methodCallingConvention;
        if (symbol.IsVirtualMethod())
        {
            convert = null;
            methodCallingConvention = CallingConvention.Cdecl;
        }
        else
        {
            convert = _context.ConvertMethod(model, symbol);
            methodCallingConvention = convert._callingConvention;
        }
        int pc = symbol.Parameters.Length;
        if (!symbol.IsStatic) pc++;
        if (pc > 1 && methodCallingConvention != callingConvention)
        {
            switch (pc)
            {
                case 2:
                    AddInstruction(OpCode.SWAP);
                    break;
                case 3:
                    AddInstruction(OpCode.REVERSE3);
                    break;
                case 4:
                    AddInstruction(OpCode.REVERSE4);
                    break;
                default:
                    Push(pc);
                    AddInstruction(OpCode.REVERSEN);
                    break;
            }
        }
        if (convert is null)
            CallVirtual(symbol);
        else
            EmitCall(convert);
    }

    private void EmitCall(MethodConvert target)
    {
        if (target._inline && !_context.Options.NoInline)
            for (int i = 0; i < target._instructions.Count - 1; i++)
                AddInstruction(target._instructions[i].Clone());
        else
            Jump(OpCode.CALL_L, target._startTarget);
    }

    private void CallVirtual(IMethodSymbol symbol)
    {
        if (symbol.ContainingType.TypeKind == TypeKind.Interface)
            throw new CompilationException(symbol.ContainingType, DiagnosticId.InterfaceCall, "Interfaces are not supported.");
        ISymbol[] members = symbol.ContainingType.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        IFieldSymbol[] fields = members.OfType<IFieldSymbol>().ToArray();
        IMethodSymbol[] virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        int index = Array.IndexOf(virtualMethods, symbol);
        if (index < 0)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {symbol.OriginalDefinition}.");
        AddInstruction(OpCode.DUP);
        Push(fields.Length);
        AddInstruction(OpCode.PICKITEM);
        Push(index);
        AddInstruction(OpCode.PICKITEM);
        AddInstruction(OpCode.CALLA);
    }
}
