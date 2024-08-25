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
        sb.Push(0);
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
        endTarget.Instruction = sb.Nop();
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
        endTarget.Instruction = sb.Nop();
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

        sb.Dup(); // Duplicate the array reference
        sb.LdArg0(); // Load the string
        sb.Push0(); // Push the initial index (0)
        loopStart.Instruction = sb.Nop();

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.Size(); // Get the length of the string
        sb.Lt(); // Check if index < length
        sb.JmpIfNot(loopEnd); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.Dup(); // Duplicate the character
        sb.Within((ushort)'A', (ushort)'Z'); // Check if character is within 'A' to 'Z'
        sb.JmpIf(charIsUpper); // If true, jump to charIsUpper

        sb.Append(); // Append the original character to the array
        sb.Jmp(loopStart); // Jump to the start of the loop

        charIsUpper.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.Push((ushort)'A'); // Push 'A'
        sb.Sub(); // Subtract 'A' from the character
        sb.Push((ushort)'a'); // Push 'a'
        sb.Add(); // Add 'a' to the result
        sb.Append(); // Append the lower case character to the array
        sb.Jmp(loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.Nop();
        sb.ChangeType(StackItemType.ByteString); // Convert the array to a byte string
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

        sb.Dup(); // Duplicate the array reference
        sb.LdArg0(); // Load the string
        sb.Push0(); // Push the initial index (0)
        loopStart.Instruction = sb.Nop();

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.Size(); // Get the length of the string
        sb.Lt(); // Check if index < length
        sb.JmpIfNot(loopEnd); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.Dup(); // Duplicate the character
        sb.Within((ushort)'a', (ushort)'z'); // Check if character is within 'a' to 'z'
        sb.JmpIf(charIsLower); // If true, jump to charIsLower

        sb.Append(); // Append the original character to the array
        sb.Jmp(loopStart); // Jump to the start of the loop

        charIsLower.Instruction = sb.Nop();
        sb.Push((ushort)'a'); // Push 'a'
        sb.Sub(); // Subtract 'a' from the character
        sb.Push((ushort)'A'); // Push 'A'
        sb.Add(); // Add 'A' to the result
        sb.Append(); // Append the upper case character to the array
        sb.Jmp(loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.Nop();
        sb.ChangeType(StackItemType.ByteString); // Convert the array to a byte string
    }

    // implement HandleStringTrim
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        sb.LdArg0(); // Load the string

        // Trim leading whitespace
        sb.Dup(); // Duplicate the string
        sb.Push0(); // Push the initial index (0)
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = sb.Nop();

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.Size(); // Get the length of the string
        sb.Lt(); // Check if index < length
        sb.JmpIfNot(loopEnd); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.Push((ushort)' '); // Push space character
        sb.Equal(); // Check if character is a space
        sb.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        sb.Inc(); // Increment the index
        sb.Jmp(loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.Nop();
        sb.SubStr(); // Get the substring from the first non-space character

        // Trim trailing whitespace
        sb.Dup(); // Duplicate the string
        sb.Size(); // Get the length of the string
        sb.Dec(); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = sb.AddInstruction(OpCode.NOP);

        sb.Dup(); // Duplicate the index
        sb.Push0(); // Push 0
        sb.Gt(); // Check if index > 0
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.Push((ushort)' '); // Push space character
        sb.Equal(); // Check if character is a space
        sb.Jump(OpCode.JMPIFNOT, loopEnd2); // If not, exit the loop

        sb.Dec(); // Decrement the index
        sb.Jmp(loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = sb.Nop();
        sb.SubStr(); // Get the substring up to the last non-space character

        sb.ChangeType(StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Trim leading characters
        sb.Dup(); // Duplicate the string
        sb.Push0(); // Push 0 to start from the beginning
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = sb.Nop();

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.Size(); // Get the length of the string
        sb.Lt(); // Check if index < length
        sb.JmpIfNot(loopEnd); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.LdArg1(); // Load the character to trim
        sb.Equal(); // Check if character is the trim character
        sb.JmpIfNot(loopEnd); // If not, exit the loop

        sb.Inc(); // Increment the index
        sb.Jmp(loopStart); // Jump to the start of the loop

        loopEnd.Instruction = sb.Nop();
        sb.SubStr(); // Get the substring from the first non-trim character

        // Trim trailing characters
        sb.Dup(); // Duplicate the string
        sb.Size(); // Get the length of the string
        sb.Dec(); // Decrement the length to get the last index
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = sb.Nop();

        sb.Dup(); // Duplicate the index
        sb.Push0(); // Push 0
        sb.Gt(); // Check if index > 0
        sb.JmpIfNot(loopEnd2); // If not, exit the loop

        sb.Dup(); // Duplicate the index
        sb.LdArg0(); // Load the string
        sb.PickItem(); // Get the character at the current index
        sb.LdArg1(); // Load the character to trim
        sb.Equal(); // Check if character is the trim character
        sb.JmpIfNot(loopEnd2); // If not, exit the loop

        sb.Dec(); // Decrement the index
        sb.Jmp(loopStart2); // Jump to the start of the loop

        loopEnd2.Instruction = sb.Nop();
        sb.SubStr(); // Get the substring up to the last non-trim character

        sb.ChangeType(StackItemType.ByteString); // Convert the array to a byte string
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
        sb.Dup();

        // Start of the loop to find the substring
        loopStart.Instruction = sb.Nop();

        // Check if the string contains the substring
        sb.Dup();
        sb.Dup();
        sb.LdArg1();
        sb.MemorySearch(methodConvert);
        sb.Dup();
        sb.PushM1();
        sb.Equal();
        sb.Jump(OpCode.JMPIF, loopEnd);

        // Get the index of the substring
        sb.Dup();
        sb.LdArg1();
        sb.MemorySearch(methodConvert);

        // Replace the substring with the new value
        replaceStart.Instruction = sb.Nop();
        sb.Dup();
        sb.LdArg2();
        sb.Cat();
        sb.Dup();
        sb.LdArg1();
        sb.Size();
        sb.Add();
        sb.SubStr();
        sb.Cat();
        replaceEnd.Instruction = sb.Nop();

        // Continue the loop
        sb.Jmp(loopStart);

        // End of the loop
        loopEnd.Instruction = sb.Nop();
        sb.Drop();
    }

    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Call the StdLib memorySearch method to find the index of the character
        sb.MemorySearch(methodConvert);
    }
}
