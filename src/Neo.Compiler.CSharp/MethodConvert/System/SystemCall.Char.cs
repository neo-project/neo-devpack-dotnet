// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{

    private static bool HandleCharParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(char.MinValue);
        methodConvert.Push(char.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for equality methods (Equals)
    private static bool HandleEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    // Handler for Array.Length and string.Length properties
    private static bool HandleLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.SIZE);
        return true;
    }

    private static bool HandleCharIsDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private static bool HandleCharIsWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'\t');
        methodConvert.Push((ushort)'\r' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'\n');
        methodConvert.Push((ushort)' ' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private static bool HandleCharIsLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }
}
