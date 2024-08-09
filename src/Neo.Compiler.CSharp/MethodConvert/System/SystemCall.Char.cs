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

    private bool HandleCharParse(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        AddInstruction(OpCode.DUP);
        Push(char.MinValue);
        Push(char.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for equality methods (Equals)
    private bool HandleEquals(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    // Handler for Array.Length and string.Length properties
    private bool HandleLength(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        AddInstruction(OpCode.SIZE);
        return true;
    }

    private bool HandleCharIsDigit(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.DUP);
        Push((ushort)'0');
        Push((ushort)'9' + 1);
        AddInstruction(OpCode.WITHIN);
        return true;
    }

    private bool HandleCharIsLetter(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.DUP);
        Push((ushort)'A');
        Push((ushort)'Z' + 1);
        AddInstruction(OpCode.WITHIN);
        AddInstruction(OpCode.SWAP);
        Push((ushort)'a');
        Push((ushort)'z' + 1);
        AddInstruction(OpCode.WITHIN);
        AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private bool HandleCharIsWhiteSpace(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.DUP);
        Push((ushort)'\t');
        Push((ushort)'\r' + 1);
        AddInstruction(OpCode.WITHIN);
        AddInstruction(OpCode.SWAP);
        Push((ushort)'\n');
        Push((ushort)' ' + 1);
        AddInstruction(OpCode.WITHIN);
        AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private bool HandleCharIsLower(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.DUP);
        Push((ushort)'a');
        Push((ushort)'z' + 1);
        AddInstruction(OpCode.WITHIN);
        return true;
    }

    private bool HandleCharIsUpper(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.DUP);
        Push((ushort)'A');
        Push((ushort)'Z' + 1);
        AddInstruction(OpCode.WITHIN);
        return true;
    }
}
