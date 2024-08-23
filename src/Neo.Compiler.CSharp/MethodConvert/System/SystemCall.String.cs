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

internal partial class MethodConvert
{
    private static void HandleStringPickItem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.PICKITEM);
    }

    private static void HandleStringLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIZE);
    }

    private static void HandleStringContains(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.AddInstruction(OpCode.PUSH0);
        methodConvert.AddInstruction(OpCode.GE);
    }

    private static void HandleStringIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    private static void HandleStringEndsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(3);
        methodConvert.AddInstruction(OpCode.ROLL);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMPGT, validCountTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.PUSHF);
        methodConvert.Jump(OpCode.JMP, endTarget);
        validCountTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push(3);
        methodConvert.AddInstruction(OpCode.ROLL);
        methodConvert.AddInstruction(OpCode.REVERSE3);
        methodConvert.AddInstruction(OpCode.SUBSTR);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AddInstruction(OpCode.EQUAL);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.SUBSTR);
    }

    private static void HandleStringSubStringToEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.OVER);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.AddInstruction(OpCode.OVER);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SUBSTR);
    }

    private static void HandleStringIsNullOrEmpty(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF, nullOrEmptyTarget);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMP, endTarget);
        nullOrEmptyTarget.Instruction = methodConvert.AddInstruction(OpCode.DROP); // drop the duped item
        methodConvert.AddInstruction(OpCode.PUSHT);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleObjectEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.EQUAL);
    }

    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SIGN);
    }

    private static void HandleBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        JumpTarget trueTarget = new(), endTarget = new();
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);
        methodConvert.Push("False");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleCharToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    // Handler for object.ToString()
    private static void HandleObjectToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    // Handler for numeric types' ToString() methods
    private static void HandleToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
    }

    private static void HandleStringToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
    }

    private static void HandleStringConcat(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.CAT);
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        methodConvert.AddInstruction(OpCode.NEWARRAY0); // Create an empty array
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsUpper = new JumpTarget();

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the array reference
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.Push((ushort)'Z' + 1); // Push 'Z' + 1
        methodConvert.AddInstruction(OpCode.WITHIN); // Check if character is within 'A' to 'Z'
        methodConvert.Jump(OpCode.JMPIF, charIsUpper); // If true, jump to charIsUpper

        methodConvert.AddInstruction(OpCode.APPEND); // Append the original character to the array
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsUpper.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.AddInstruction(OpCode.SUB); // Subtract 'A' from the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.AddInstruction(OpCode.ADD); // Add 'a' to the result
        methodConvert.AddInstruction(OpCode.APPEND); // Append the lower case character to the array
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    // Handler for string.ToUpper()
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.NEWARRAY0); // Create an empty array
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the array reference
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Push((ushort)'z' + 1); // Push 'z' + 1
        methodConvert.AddInstruction(OpCode.WITHIN); // Check if character is within 'a' to 'z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower

        methodConvert.AddInstruction(OpCode.APPEND); // Append the original character to the array
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.AddInstruction(OpCode.SUB); // Subtract 'a' from the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.AddInstruction(OpCode.ADD); // Add 'A' to the result
        methodConvert.AddInstruction(OpCode.APPEND); // Append the upper case character to the array
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    // implement HandleStringTrim
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string

        // Trim leading whitespace
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the string
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.Push((ushort)' '); // Push space character
        methodConvert.AddInstruction(OpCode.EQUAL); // Check if character is a space
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.INC); // Increment the index
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring from the first non-space character

        // Trim trailing whitespace
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.DEC); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.PUSH0); // Push 0
        methodConvert.AddInstruction(OpCode.GT); // Check if index > 0
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.Push((ushort)' '); // Push space character
        methodConvert.AddInstruction(OpCode.EQUAL); // Check if character is a space
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DEC); // Decrement the index
        methodConvert.Jump(OpCode.JMP, loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-space character

        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Trim leading characters
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the string
        methodConvert.Push(0); // Push 0 to start from the beginning
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.LDARG1); // Load the character to trim
        methodConvert.AddInstruction(OpCode.EQUAL); // Check if character is the trim character
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.INC); // Increment the index
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring from the first non-trim character

        // Trim trailing characters
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.DEC); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.PUSH0); // Push 0
        methodConvert.AddInstruction(OpCode.GT); // Check if index > 0
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.LDARG1); // Load the character to trim
        methodConvert.AddInstruction(OpCode.EQUAL); // Check if character is the trim character
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DEC); // Decrement the index
        methodConvert.Jump(OpCode.JMP, loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-trim character

        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleStringReplace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var replaceStart = new JumpTarget();
        var replaceEnd = new JumpTarget();

        // Duplicate the original string
        methodConvert.AddInstruction(OpCode.DUP);

        // Start of the loop to find the substring
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // Check if the string contains the substring
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.PUSHM1);
        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMPIF, loopEnd);

        // Get the index of the substring
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);

        // Replace the substring with the new value
        replaceStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.LDARG2);
        methodConvert.AddInstruction(OpCode.CAT);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.AddInstruction(OpCode.ADD);
        methodConvert.AddInstruction(OpCode.SUBSTR);
        methodConvert.AddInstruction(OpCode.CAT);
        replaceEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // Continue the loop
        methodConvert.Jump(OpCode.JMP, loopStart);

        // End of the loop
        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
    }

    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

}
