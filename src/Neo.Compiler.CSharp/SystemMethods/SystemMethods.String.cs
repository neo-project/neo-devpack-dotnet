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

internal static partial class SystemMethods
{
    private static void HandleStringPickItem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
         IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.PickItem();
    }

    private static void HandleStringLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Size();
    }

    private static void HandleStringContains(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.MemorySearch(methodConvert);
        sb.Push0();
        sb.Ge();
    }

    private static void HandleStringIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.MemorySearch(methodConvert);
    }

    private static void HandleStringEndsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.SIZE);
        sb.AddInstruction(OpCode.ROT);
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.SIZE);
        sb.AddInstruction(OpCode.DUP);
        sb.Push(3);
        sb.AddInstruction(OpCode.ROLL);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SUB);
        sb.AddInstruction(OpCode.DUP);
        sb.Push0();
        sb.Jump(OpCode.JMPGT, validCountTarget);
        sb.AddInstruction(OpCode.DROP);
        sb.AddInstruction(OpCode.DROP);
        sb.AddInstruction(OpCode.DROP);
        sb.AddInstruction(OpCode.DROP);
        sb.AddInstruction(OpCode.PUSHF);
        sb.Jump(OpCode.JMP, endTarget);
        validCountTarget.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.Push(3);
        sb.AddInstruction(OpCode.ROLL);
        sb.AddInstruction(OpCode.REVERSE3);
        sb.AddInstruction(OpCode.SUBSTR);
        sb.ChangeType(StackItemType.ByteString);
        sb.AddInstruction(OpCode.EQUAL);
        endTarget.Instruction = sb.AddInstruction(OpCode.NOP);
    }

    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.AddInstruction(OpCode.SUBSTR);
    }

    private static void HandleStringSubStringToEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Over();
        sb.Size();
        sb.Over();
        sb.Sub();
        sb.SubStr();
    }

    private static void HandleStringIsNullOrEmpty(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        sb.Dup();
        sb.IsNull();
        sb.JmpIf(nullOrEmptyTarget);
        sb.Size();
        sb.Push0();
        sb.NumEqual();
        sb.Jmp(endTarget);
        nullOrEmptyTarget.Instruction = sb.Drop(); // drop the duped item
        sb.Push(true);
        sb.AddTarget(endTarget);
    }

    private static void HandleObjectEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.AddInstruction(OpCode.EQUAL);
    }

    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Sub();
        sb.Sign();
    }

    private static void HandleBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        JumpTarget trueTarget = new(), endTarget = new();
        sb.Jump(OpCode.JMPIF_L, trueTarget);
        sb.Push("False");
        sb.Jmp(endTarget);
        trueTarget.Instruction = sb.Push("True");
        sb.AddTarget(endTarget);
    }

    private static void HandleCharToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.ChangeType(StackItemType.ByteString);
    }

    // Handler for object.ToString()
    private static void HandleObjectToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.ChangeType(StackItemType.ByteString);
    }

    // Handler for numeric types' ToString() methods
    private static void HandleToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
    }

    private static void HandleStringToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
    }

    private static void HandleStringConcat(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.AddInstruction(OpCode.CAT);
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        sb.AddInstruction(OpCode.NEWARRAY0); // Create an empty array
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsUpper = new JumpTarget();

        sb.AddInstruction(OpCode.DUP); // Duplicate the array reference
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        loopStart.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.LT); // Check if index < length
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.AddInstruction(OpCode.DUP); // Duplicate the character
        sb.Push((ushort)'A'); // Push 'A'
        sb.Push((ushort)'Z' + 1); // Push 'Z' + 1
        sb.AddInstruction(OpCode.WITHIN); // Check if character is within 'A' to 'Z'
        sb.Jump(OpCode.JMPIF, charIsUpper); // If true, jump to charIsUpper

        sb.AddInstruction(OpCode.APPEND); // Append the original character to the array
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsUpper.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.Push((ushort)'A'); // Push 'A'
        sb.AddInstruction(OpCode.SUB); // Subtract 'A' from the character
        sb.Push((ushort)'a'); // Push 'a'
        sb.AddInstruction(OpCode.ADD); // Add 'a' to the result
        sb.AddInstruction(OpCode.APPEND); // Append the lower case character to the array
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    // Handler for string.ToUpper()
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.AddInstruction(OpCode.NEWARRAY0); // Create an empty array
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();

        sb.AddInstruction(OpCode.DUP); // Duplicate the array reference
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        loopStart.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.LT); // Check if index < length
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.AddInstruction(OpCode.DUP); // Duplicate the character
        sb.Push((ushort)'a'); // Push 'a'
        sb.Push((ushort)'z' + 1); // Push 'z' + 1
        sb.AddInstruction(OpCode.WITHIN); // Check if character is within 'a' to 'z'
        sb.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower

        sb.AddInstruction(OpCode.APPEND); // Append the original character to the array
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.Push((ushort)'a'); // Push 'a'
        sb.AddInstruction(OpCode.SUB); // Subtract 'a' from the character
        sb.Push((ushort)'A'); // Push 'A'
        sb.AddInstruction(OpCode.ADD); // Add 'A' to the result
        sb.AddInstruction(OpCode.APPEND); // Append the upper case character to the array
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    // implement HandleStringTrim
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        sb.AddInstruction(OpCode.LDARG0); // Load the string

        // Trim leading whitespace
        sb.AddInstruction(OpCode.DUP); // Duplicate the string
        sb.AddInstruction(OpCode.PUSH0); // Push the initial index (0)
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.LT); // Check if index < length
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.Push((ushort)' '); // Push space character
        sb.AddInstruction(OpCode.EQUAL); // Check if character is a space
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.INC); // Increment the index
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.SUBSTR); // Get the substring from the first non-space character

        // Trim trailing whitespace
        sb.AddInstruction(OpCode.DUP); // Duplicate the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.DEC); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.PUSH0); // Push 0
        sb.AddInstruction(OpCode.GT); // Check if index > 0
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.Push((ushort)' '); // Push space character
        sb.AddInstruction(OpCode.EQUAL); // Check if character is a space
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.AddInstruction(OpCode.DEC); // Decrement the index
        sb.Jump(OpCode.JMP, loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-space character

        sb.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Trim leading characters
        sb.AddInstruction(OpCode.DUP); // Duplicate the string
        sb.Push0(); // Push 0 to start from the beginning
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.LT); // Check if index < length
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.AddInstruction(OpCode.LDARG1); // Load the character to trim
        sb.AddInstruction(OpCode.EQUAL); // Check if character is the trim character
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.AddInstruction(OpCode.INC); // Increment the index
        sb.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.SUBSTR); // Get the substring from the first non-trim character

        // Trim trailing characters
        sb.AddInstruction(OpCode.DUP); // Duplicate the string
        sb.AddInstruction(OpCode.SIZE); // Get the length of the string
        sb.AddInstruction(OpCode.DEC); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.PUSH0); // Push 0
        sb.AddInstruction(OpCode.GT); // Check if index > 0
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.AddInstruction(OpCode.DUP); // Duplicate the index
        sb.AddInstruction(OpCode.LDARG0); // Load the string
        sb.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        sb.AddInstruction(OpCode.LDARG1); // Load the character to trim
        sb.AddInstruction(OpCode.EQUAL); // Check if character is the trim character
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.AddInstruction(OpCode.DEC); // Decrement the index
        sb.Jump(OpCode.JMP, loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-trim character

        sb.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleStringReplace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var replaceStart = new JumpTarget();
        var replaceEnd = new JumpTarget();

        // Duplicate the original string
        sb.AddInstruction(OpCode.DUP);

        // Start of the loop to find the substring
        loopStart.Instruction = sb.AddInstruction(OpCode.NOP);

        // Check if the string contains the substring
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.PUSHM1);
        sb.AddInstruction(OpCode.EQUAL);
        sb.Jump(OpCode.JMPIF, loopEnd);

        // Get the index of the substring
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.LDARG1);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);

        // Replace the substring with the new value
        replaceStart.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.LDARG2);
        sb.AddInstruction(OpCode.CAT);
        sb.AddInstruction(OpCode.DUP);
        sb.AddInstruction(OpCode.LDARG1);
        sb.AddInstruction(OpCode.SIZE);
        sb.AddInstruction(OpCode.ADD);
        sb.AddInstruction(OpCode.SUBSTR);
        sb.AddInstruction(OpCode.CAT);
        replaceEnd.Instruction = sb.AddInstruction(OpCode.NOP);

        // Continue the loop
        sb.Jump(OpCode.JMP, loopStart);

        // End of the loop
        loopEnd.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.DROP);
    }

    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }
}
