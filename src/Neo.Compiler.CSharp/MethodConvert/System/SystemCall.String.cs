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
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

partial class MethodConvert
{

    private bool HandleStringPickItem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.PICKITEM);
        return true;
    }

    private bool HandleStringLength(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.SIZE);
        return true;
    }

    private bool HandleStringContains(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.SWAP);
        CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        AddInstruction(OpCode.PUSH0);
        AddInstruction(OpCode.GE);
        return true;
    }

    private bool HandleStringIndexOf(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.SWAP);
        CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        return true;
    }

    private bool HandleStringEndsWith(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.SIZE);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.SIZE);
        AddInstruction(OpCode.DUP);
        Push(3);
        AddInstruction(OpCode.ROLL);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(0);
        Jump(OpCode.JMPGT, validCountTarget);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.PUSHF);
        Jump(OpCode.JMP, endTarget);
        validCountTarget.Instruction = AddInstruction(OpCode.NOP);
        Push(3);
        AddInstruction(OpCode.ROLL);
        AddInstruction(OpCode.REVERSE3);
        AddInstruction(OpCode.SUBSTR);
        ChangeType(StackItemType.ByteString);
        AddInstruction(OpCode.EQUAL);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    private bool HandleStringSubstring(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.SUBSTR);
        return true;
    }

    private bool HandleStringSubStringToEnd(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.OVER);
        AddInstruction(OpCode.SIZE);
        AddInstruction(OpCode.OVER);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.SUBSTR);
        return true;
    }

    private bool HandleStringIsNullOrEmpty(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF, nullOrEmptyTarget);
        AddInstruction(OpCode.SIZE);
        Push(0);
        AddInstruction(OpCode.NUMEQUAL);
        Jump(OpCode.JMP, endTarget);
        nullOrEmptyTarget.Instruction = AddInstruction(OpCode.DROP); // drop the duped item
        AddInstruction(OpCode.PUSHT);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    private bool HandleObjectEquals(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.EQUAL);
        return true;
    }

    private bool HandleStringCompare(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.SIGN);
        return true;
    }

    private bool HandleBoolToString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        JumpTarget trueTarget = new(), endTarget = new();
        Jump(OpCode.JMPIF_L, trueTarget);
        Push("False");
        Jump(OpCode.JMP_L, endTarget);
        trueTarget.Instruction = Push("True");
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    private bool HandleCharToString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        ChangeType(StackItemType.ByteString);
        return true;
    }

    // Handler for object.ToString()
    private bool HandleObjectToString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        ChangeType(StackItemType.ByteString);
        return true;
    }

    // Handler for numeric types' ToString() methods
    private bool HandleToString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        return true;
    }

    private bool HandleStringToString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        return true;
    }

    private bool HandleStringConcat(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.CAT);
        return true;
    }
}
