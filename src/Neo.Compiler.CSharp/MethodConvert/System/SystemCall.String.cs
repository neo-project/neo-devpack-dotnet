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
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
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

        var firstNotNull = new JumpTarget();
        var secondNotNull = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Isnull();
        methodConvert.JumpIfNot(firstNotNull);
        methodConvert.Drop();
        methodConvert.Push("");
        firstNotNull.Instruction = methodConvert.Nop();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Isnull();
        methodConvert.JumpIfNot(secondNotNull);
        methodConvert.Drop();
        methodConvert.Push("");
        secondNotNull.Instruction = methodConvert.Nop();
        methodConvert.AddInstruction(OpCode.CAT);
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToLower(methodConvert);
    }

    // Handler for string.ToUpper()
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToUpper(methodConvert);
    }

    // implement HandleStringTrim
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);       // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);   // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);     // endIndex = string.Length - 1

        // loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex); // pick a char to check
        CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // done processing leading whitespace, start processing trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex); // pick a char to check
        CheckWithinWhiteSpace(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // get the substring
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.INC);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-space character
    }

    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);       // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);   // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);     // endIndex = string.Length - 1
        methodConvert.Drop();
        // loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex); // pick a char to check
        CheckTrimChar(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // done processing leading whitespace, start processing trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex); // pick a char to check
        CheckTrimChar(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // get the substring
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.INC);
        methodConvert.AddInstruction(OpCode.SUBSTR); // Get the substring up to the last non-space character
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
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    private static void ConvertToUpper(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push(""); // Create an empty ByteString

        // methodConvert.AddInstruction(OpCode.LDARG0); // Load the string | arr str
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)  arr str 0
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index | arr str 0 0
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the character
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.Push((ushort)'z' + 1); // Push 'z' + 1
        methodConvert.AddInstruction(OpCode.WITHIN); // Check if character is within 'a' to 'z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the original character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'a'); // Push 'a'
        methodConvert.AddInstruction(OpCode.SUB); // Subtract 'a' from the character
        methodConvert.Push((ushort)'A'); // Push 'A'
        methodConvert.AddInstruction(OpCode.ADD); // Add 'A' to the result
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the upper case character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Drop();
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void ConvertToLower(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push(""); // Create an empty ByteString

        // methodConvert.AddInstruction(OpCode.LDARG0); // Load the string | arr str
        methodConvert.AddInstruction(OpCode.PUSH0); // Push the initial index (0)  arr str 0
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.AddInstruction(OpCode.SIZE); // Get the length of the string
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop

        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the index | arr str 0 0
        methodConvert.AddInstruction(OpCode.LDARG0); // Load the string
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
        methodConvert.AddInstruction(OpCode.DUP); // Duplicate the character
        methodConvert.Push((ushort)'A'); // Push 'a'
        methodConvert.Push((ushort)'Z' + 1); // Push 'z' + 1
        methodConvert.AddInstruction(OpCode.WITHIN); // Check if character is within 'a' to 'z'
        methodConvert.Jump(OpCode.JMPIF, charIsLower); // If true, jump to charIsLower
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the original character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        charIsLower.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push((ushort)'A'); // Push 'a'
        methodConvert.AddInstruction(OpCode.SUB); // Subtract 'a' from the character
        methodConvert.Push((ushort)'a'); // Push 'A'
        methodConvert.AddInstruction(OpCode.ADD); // Add 'A' to the result
        methodConvert.Rot();
        methodConvert.Swap();
        methodConvert.AddInstruction(OpCode.CAT); // Append the upper case character to the array
        methodConvert.Swap();
        methodConvert.Inc();
        methodConvert.Jump(OpCode.JMP, loopStart); // Jump to the start of the loop

        loopEnd.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Drop();
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert the array to a byte string
    }

    private static void InitStrLen(MethodConvert methodConvert, byte strLen)
    {
        GetString(methodConvert);
        methodConvert.AddInstruction(OpCode.SIZE);
        methodConvert.AccessSlot(OpCode.STLOC, strLen);
    }

    // Local function to get strLen
    private static void GetStrLen(MethodConvert methodConvert, byte strLen) => methodConvert.AccessSlot(OpCode.LDLOC, strLen);

    private static void InitStartIndex(MethodConvert methodConvert, byte startIndex)
    {
        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);
    }

    private static void InitEndIndex(MethodConvert methodConvert, byte endIndex, byte strLen)
    {
        GetStrLen(methodConvert, strLen);
        methodConvert.AddInstruction(OpCode.DEC); // len-1
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);
    }

    // Local function to get endIndex
    private static void GetEndIndex(MethodConvert methodConvert, byte endIndex) => methodConvert.AccessSlot(OpCode.LDLOC, endIndex);

    private static void GetString(MethodConvert methodConvert) => methodConvert.AddInstruction(OpCode.LDARG0); // Load the string

    // Local function to get startIndex
    private static void GetStartIndex(MethodConvert methodConvert, byte startIndex) => methodConvert.AccessSlot(OpCode.LDLOC, startIndex);

    private static void CheckStartIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte startIndex, byte strLen)
    {
        GetStartIndex(methodConvert, startIndex);
        GetStrLen(methodConvert, strLen);
        methodConvert.AddInstruction(OpCode.LT); // Check if index < length
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    // Local function to set startIndex
    private static void MoveStartIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte startIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, startIndex);
        methodConvert.AddInstruction(OpCode.INC);
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);
        methodConvert.Jump(OpCode.JMP, loopStart);
    }

    private static void PickCharStart(MethodConvert methodConvert, byte startIndex)
    {
        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
    }

    // Local function to set endIndex
    private static void MoveEndIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte endIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, endIndex);
        methodConvert.AddInstruction(OpCode.DEC);
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);
        methodConvert.Jump(OpCode.JMP, loopStart);
    }

    private static void CheckWithinWhiteSpace(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push((ushort)'\t');
        methodConvert.Push((ushort)'\r' + 1);
        methodConvert.AddInstruction(OpCode.WITHIN); // check if '\t' <= c <= '\r'
        methodConvert.AddInstruction(OpCode.SWAP);

        methodConvert.Push((ushort)' '); // Push space character
        methodConvert.AddInstruction(OpCode.EQUAL); // Check if character is a space
        methodConvert.AddInstruction(OpCode.BOOLOR); // check if '\t' <= c <= '\r' or ' ' == c

        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void CheckTrimChar(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.AddInstruction(OpCode.LDARG1);
        methodConvert.NumEqual();
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void CheckEndIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte endIndex, byte startIndex)
    {
        GetEndIndex(methodConvert, endIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.AddInstruction(OpCode.GT); // Check if index > start
        methodConvert.Jump(OpCode.JMPIFNOT, loopEnd); // If not, exit the loop
    }

    private static void PickCharEnd(MethodConvert methodConvert, byte endIndex)
    {
        GetString(methodConvert);
        GetEndIndex(methodConvert, endIndex);
        methodConvert.AddInstruction(OpCode.PICKITEM); // Get the character at the current index
    }
}
