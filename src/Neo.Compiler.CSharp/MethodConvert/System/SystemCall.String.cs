// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
        methodConvert.PickItem();
    }

    private static void HandleStringLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Size();
    }

    private static void HandleStringContains(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Push0();
        methodConvert.Ge();
    }

    private static void HandleStringIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    /// <summary>
    /// Handles the string.EndsWith method by checking if a string ends with a specified substring.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the end portion of the string with the target substring
    /// </remarks>
    private static void HandleStringEndsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate string for length check
        methodConvert.Size();                                      // Get string length
        methodConvert.Rot();                                       // Rotate stack for comparison
        methodConvert.Dup();                                       // Duplicate comparison string
        methodConvert.Size();                                      // Get comparison string length
        methodConvert.Dup();                                       // Duplicate length for calculation
        methodConvert.Push(3);                                     // Push 3 for ROLL operation
        methodConvert.Roll();                                      // Roll stack elements
        methodConvert.Swap();                                      // Swap top elements
        methodConvert.Sub();                                       // Calculate start position
        methodConvert.Dup();                                       // Duplicate for bounds check
        methodConvert.Push(0);                                     // Push 0 for comparison
        methodConvert.Jump(OpCode.JMPGT, validCountTarget);        // Jump if position > 0
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.PushF();                                     // Push false result
        methodConvert.JumpAlways( endTarget);                 // Jump to end
        validCountTarget.Instruction = methodConvert.Nop();        // Valid position target
        methodConvert.Push(3);                                     // Push 3 for ROLL operation
        methodConvert.Roll();                                      // Roll stack elements
        methodConvert.Reverse3();                                  // Reverse top 3 elements
        methodConvert.SubStr();                                    // Extract substring
        methodConvert.ChangeType(StackItemType.ByteString);        // Convert to ByteString
        methodConvert.Equal();                                     // Compare for equality
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.SubStr();
    }

    private static void HandleStringSubStringToEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Over();
        methodConvert.Size();
        methodConvert.Over();
        methodConvert.Sub();
        methodConvert.SubStr();
    }

    private static void HandleStringIsNullOrEmpty(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.JumpIfTrue( nullOrEmptyTarget);
        methodConvert.Size();
        methodConvert.Push(0);
        methodConvert.NumEqual();
        methodConvert.JumpAlways( endTarget);
        nullOrEmptyTarget.Instruction = methodConvert.Drop();
        methodConvert.PushT();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleObjectEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Equal();
    }

    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Sub();
        methodConvert.Sign();
    }

    private static void HandleBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        JumpTarget trueTarget = new(), endTarget = new();
        methodConvert.JumpIfTrueLong( trueTarget);
        methodConvert.Push("False");
        methodConvert.JumpAlwaysLong( endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        endTarget.Instruction = methodConvert.Nop();
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

    /// <summary>
    /// Handles the string.Concat method by concatenating two strings with null handling.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Concatenates strings after converting null values to empty strings
    /// </remarks>
    private static void HandleStringConcat(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var firstNotNull = new JumpTarget();
        var secondNotNull = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate first string
        methodConvert.Isnull();                                    // Check if null
        methodConvert.JumpIfNot(firstNotNull);                     // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        firstNotNull.Instruction = methodConvert.Nop();            // First not null target
        methodConvert.Swap();                                      // Swap strings
        methodConvert.Dup();                                       // Duplicate second string
        methodConvert.Isnull();                                    // Check if null
        methodConvert.JumpIfNot(secondNotNull);                    // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        secondNotNull.Instruction = methodConvert.Nop();           // Second not null target
        methodConvert.Cat();                                       // Concatenate strings
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToLower(methodConvert);
    }

    /// <summary>
    /// Converts a string to lowercase by processing each character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Iterates through string characters, converting uppercase to lowercase
    /// </remarks>
    private static void ConvertToLower(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push("");                                    // Create empty result string

        methodConvert.Push0();                                     // Initialize index to 0
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Size();                                      // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse( loopEnd);              // Exit if done

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Swap();                                      // Swap for PickItem
        methodConvert.PickItem();                                  // Get character at index
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('A', 'Z');                            // Check if uppercase
        methodConvert.JumpIfTrue( charIsLower);             // Jump if uppercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append original character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        charIsLower.Instruction = methodConvert.Nop();             // Uppercase processing
        methodConvert.Push((ushort)'A');                           // Push 'A'
        methodConvert.Sub();                                       // Subtract 'A'
        methodConvert.Push((ushort)'a');                           // Push 'a'
        methodConvert.Add();                                       // Add 'a' for lowercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append lowercase character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Drop index
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert to ByteString
    }

    /// <summary>
    /// Handles the string.ToUpper method by converting characters to uppercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through each character and converts lowercase letters to uppercase
    /// </remarks>
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToUpper(methodConvert);
    }

    /// <summary>
    /// Converts a string to uppercase by processing each character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Iterates through string characters, converting lowercase to uppercase
    /// </remarks>
    private static void ConvertToUpper(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push("");                                    // Create empty result string

        methodConvert.Push0();                                     // Initialize index to 0
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Size();                                      // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse( loopEnd);              // Exit if done

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Swap();                                      // Swap for PickItem
        methodConvert.PickItem();                                  // Get character at index
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('a', 'z');                     // Check if lowercase
        methodConvert.JumpIfTrue( charIsLower);             // Jump if lowercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append original character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        charIsLower.Instruction = methodConvert.Nop();             // Lowercase processing
        methodConvert.Push((ushort)'a');                           // Push 'a'
        methodConvert.Sub();                                       // Subtract 'a'
        methodConvert.Push((ushort)'A');                           // Push 'A'
        methodConvert.Add();                                       // Add 'A' for uppercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append uppercase character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Drop index
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert to ByteString
    }

    /// <summary>
    /// Initializes the string length variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="strLen">Variable to store string length</param>
    /// <remarks>
    /// Algorithm: Gets string size and stores it in local variable
    /// </remarks>
    private static void InitStrLen(MethodConvert methodConvert, byte strLen)
    {
        GetString(methodConvert);                                  // Get string
        methodConvert.Size();                                      // Get string size
        methodConvert.AccessSlot(OpCode.STLOC, strLen);            // Store in local variable
    }

    /// <summary>
    /// Gets the string length from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="strLen">Variable containing string length</param>
    /// <returns>String length on stack</returns>
    private static void GetStrLen(MethodConvert methodConvert, byte strLen) => methodConvert.AccessSlot(OpCode.LDLOC, strLen);

    /// <summary>
    /// Initializes the start index variable to 0.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable to store start index</param>
    /// <remarks>
    /// Algorithm: Sets start index to 0 for trimming operations
    /// </remarks>
    private static void InitStartIndex(MethodConvert methodConvert, byte startIndex)
    {
        methodConvert.Push(0);                                     // Push initial value 0
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);        // Store in local variable
    }

    /// <summary>
    /// Initializes the end index variable to string length - 1.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable to store end index</param>
    /// <param name="strLen">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Sets end index to last character position
    /// </remarks>
    private static void InitEndIndex(MethodConvert methodConvert, byte endIndex, byte strLen)
    {
        GetStrLen(methodConvert, strLen);                          // Get string length
        methodConvert.Dec();                                       // Subtract 1 for last index
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);          // Store in local variable
    }

    /// <summary>
    /// Gets the end index from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <returns>End index on stack</returns>
    private static void GetEndIndex(MethodConvert methodConvert, byte endIndex) => methodConvert.AccessSlot(OpCode.LDLOC, endIndex);

    /// <summary>
    /// Gets the string argument (LDARG0).
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <returns>String argument on stack</returns>
    private static void GetString(MethodConvert methodConvert) => methodConvert.LdArg0();

    /// <summary>
    /// Gets the start index from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <returns>Start index on stack</returns>
    private static void GetStartIndex(MethodConvert methodConvert, byte startIndex) => methodConvert.AccessSlot(OpCode.LDLOC, startIndex);

    /// <summary>
    /// Checks if start index is less than string length.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <param name="strLen">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Exits loop if start index >= string length
    /// </remarks>
    private static void CheckStartIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte startIndex, byte strLen)
    {
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        GetStrLen(methodConvert, strLen);                          // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse( loopEnd);              // Exit if not less than
    }

    /// <summary>
    /// Increments start index and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Increments start index and continues loop
    /// </remarks>
    private static void MoveStartIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte startIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, startIndex);        // Load start index
        methodConvert.Inc();                                       // Increment by 1
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);        // Store back
        methodConvert.JumpAlways( loopStart);                 // Continue loop
    }

    /// <summary>
    /// Picks character at the start index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current start position
    /// </remarks>
    private static void PickCharStart(MethodConvert methodConvert, byte startIndex)
    {
        GetString(methodConvert);                                  // Get string
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        methodConvert.PickItem();                                  // Get character at index
    }

    /// <summary>
    /// Decrements end index and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <remarks>
    /// Algorithm: Decrements end index and continues loop
    /// </remarks>
    private static void MoveEndIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte endIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, endIndex);          // Load end index
        methodConvert.Dec();                                       // Decrement by 1
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);          // Store back
        methodConvert.JumpAlways( loopStart);                 // Continue loop
    }

    /// <summary>
    /// Checks if character is within whitespace range and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Checks if character is tab-carriage return range or space character
    /// </remarks>
    private static void CheckWithinWhiteSpace(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('\t', '\r');                          // Check if within tab-CR range
        methodConvert.Swap();                                      // Swap for space check

        methodConvert.Push((ushort)' ');                           // Push space character
        methodConvert.Equal();                                     // Check if equals space
        methodConvert.BoolOr();                                    // Combine checks with OR

        methodConvert.JumpIfFalse( loopEnd);              // Exit if not whitespace
    }

    /// <summary>
    /// Checks if character equals the trim character and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Compares character with the specified trim character
    /// </remarks>
    private static void CheckTrimChar(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.LdArg1();                                    // Load trim character
        methodConvert.NumEqual();                                  // Check equality
        methodConvert.JumpIfFalse( loopEnd);              // Exit if not equal
    }

    /// <summary>
    /// Checks if end index is greater than start index.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Exits loop if end index <= start index
    /// </remarks>
    private static void CheckEndIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte endIndex, byte startIndex)
    {
        GetEndIndex(methodConvert, endIndex);                      // Get end index
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        methodConvert.Gt();                                        // Check if end > start
        methodConvert.JumpIfFalse( loopEnd);              // Exit if not greater
    }

    /// <summary>
    /// Picks character at the end index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current end position
    /// </remarks>
    private static void PickCharEnd(MethodConvert methodConvert, byte endIndex)
    {
        GetString(methodConvert);                                  // Get string
        GetEndIndex(methodConvert, endIndex);                      // Get end index
        methodConvert.PickItem();                                  // Get character at index
    }

    /// <summary>
    /// Handles the string.Trim method by removing leading and trailing whitespace.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Finds first and last non-whitespace characters, then extracts substring
    /// </remarks>
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);                         // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);             // endIndex = string.Length - 1

        // Loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex);                  // Pick character to check
        CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker

        // Process trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();              // Second loop start
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex);                      // Pick character to check
        CheckWithinWhiteSpace(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();                // Second loop end

        // Extract the trimmed substring
        GetString(methodConvert);                                  // Get original string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
    }

    /// <summary>
    /// Handles the string.Trim(char) method by removing leading and trailing specified characters.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Finds first and last characters that don't match the trim character
    /// </remarks>
    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);                         // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);             // endIndex = string.Length - 1
        methodConvert.Drop();                                      // Clean up stack

        // Loop to trim leading characters
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex);                  // Pick character to check
        CheckTrimChar(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker

        // Process trailing characters
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();              // Second loop start
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex);                      // Pick character to check
        CheckTrimChar(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();                // Second loop end

        // Extract the trimmed substring
        GetString(methodConvert);                                  // Get original string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
    }

    /// <summary>
    /// Handles the string.Replace method by replacing all occurrences of a substring.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iteratively finds and replaces all occurrences of the search string
    /// </remarks>
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
        methodConvert.Dup();                                       // Duplicate string for processing

        // Start of the loop to find the substring
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        // Check if the string contains the substring
        methodConvert.Dup();                                       // Duplicate string
        methodConvert.Dup();                                       // Duplicate again for search
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Dup();                                       // Duplicate result
        methodConvert.PushM1();                                    // Push -1 for comparison
        methodConvert.Equal();                                     // Check if not found
        methodConvert.JumpIfTrue( loopEnd);                 // Exit if not found

        // Get the index of the substring
        methodConvert.Dup();                                       // Duplicate string
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);

        // Replace the substring with the new value
        replaceStart.Instruction = methodConvert.Nop();            // Replace start marker
        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg2();                                    // Load replacement string
        methodConvert.Cat();                                       // Concatenate replacement
        methodConvert.Dup();                                       // Duplicate result
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.Size();                                      // Get search string length
        methodConvert.Add();                                       // Calculate position after replacement
        methodConvert.SubStr();                                    // Get remaining substring
        methodConvert.Cat();                                       // Concatenate remaining part
        replaceEnd.Instruction = methodConvert.Nop();              // Replace end marker

        // Continue the loop
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        // End of the loop
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Clean up stack
    }

    /// <summary>
    /// Handles the string.IndexOf(char) method by finding the index of a character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses StdLib memorySearch to find the character position
    /// </remarks>
    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }
}
