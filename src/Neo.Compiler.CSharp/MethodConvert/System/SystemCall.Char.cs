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
        methodConvert. Push((ushort)'0');
        methodConvert. Push((ushort)'9' + 1);
        methodConvert. AddInstruction(OpCode.WITHIN);
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
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsPunctuation(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'!');
        methodConvert.Push((ushort)'/' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)':');
        methodConvert.Push((ushort)'@' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'[');
        methodConvert.Push((ushort)'`' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'{');
        methodConvert.Push((ushort)'~' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharIsSymbol(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'$');
        methodConvert.Push((ushort)'+' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'<');
        methodConvert.Push((ushort)'=' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'>');
        methodConvert.Push((ushort)'@' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'[');
        methodConvert.Push((ushort)'`' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'{');
        methodConvert.Push((ushort)'~' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharIsControl(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'\0');
        methodConvert.Push((ushort)'\x1F' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)'\x7F');
        methodConvert.Push((ushort)'\x9F' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private static bool HandleCharIsSurrogate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)0xD800);
        methodConvert.Push((ushort)0xDBFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((ushort)0xDC00);
        methodConvert.Push((ushort)0xDFFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.AddInstruction(OpCode.BOOLOR);
        return true;
    }

    private static bool HandleCharIsHighSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)0xD800);
        methodConvert.Push((ushort)0xDBFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsLowSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push((ushort)0xDC00);
        methodConvert.Push((ushort)0xDFFF + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        return true;
    }

    private static bool HandleCharIsLetterOrDigit(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharIsBetween(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget validTarget = new();
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.GE);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Jump(OpCode.JMPIFNOT, validTarget);
        methodConvert.AddInstruction(OpCode.REVERSE3);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endTarget);
        validTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.LT);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharGetNumericValue(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget validTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'0');
        methodConvert.Push((ushort)'9' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, validTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.PUSHM1);
        methodConvert.Jump(OpCode.JMP, endTarget);
        validTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'0');
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'A');
        methodConvert.Push((ushort)'Z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.Push((ushort)'A');
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push((ushort)'a');
        methodConvert.AddInstruction(OpCode.ADD);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

    private static bool HandleCharToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'a');
        methodConvert.Push((ushort)'z' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.Push((ushort)'a');
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push((ushort)'A');
        methodConvert.AddInstruction(OpCode.ADD);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        return true;
    }

}
