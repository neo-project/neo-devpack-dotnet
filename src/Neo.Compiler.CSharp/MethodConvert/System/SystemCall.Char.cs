// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Char.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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

internal partial class MethodConvert
{
    /// <summary>
    /// Handles the char.Parse method by converting a string to a char value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within char range [0, 65535]
    /// </remarks>
    private static void HandleCharParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.Dup();                                       // Duplicate result for range check
        methodConvert.Within(char.MinValue, char.MaxValue);        // Check if value is within char range
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if within range
        methodConvert.Throw();                                     // Throw if out of range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles equality methods for character comparison.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs numeric equality comparison between character values
    /// </remarks>
    private static void HandleEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.NumEqual();                                  // Numeric equality comparison
    }

    /// <summary>
    /// Handles length properties for arrays and strings.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the size of the collection or string
    /// </remarks>
    private static void HandleLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Size();                                      // Get size using helper method
    }

    /// <summary>
    /// Handles the char.IsDigit method to check if a character is a digit.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within the range '0' to '9'
    /// </remarks>
    private static void HandleCharIsDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)'0', (ushort)'9');            // Check if within digit range
    }

    /// <summary>
    /// Handles the char.IsLetter method to check if a character is a letter.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within 'A'-'Z' or 'a'-'z' ranges
    /// </remarks>
    private static void HandleCharIsLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check for uppercase letters (A-Z)
        methodConvert.Dup();                                       // Duplicate character for second check
        methodConvert.Within((ushort)'A', (ushort)'Z');            // Check if within uppercase range
        methodConvert.Swap();                                      // Bring original character back to top

        // Check for lowercase letters (a-z)
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
        methodConvert.BoolOr();                                    // Combine both checks with OR
    }

    /// <summary>
    /// Handles the char.IsWhiteSpace method to check if a character is whitespace.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within tab-carriage return range or equals space
    /// </remarks>
    private static void HandleCharIsWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check for control whitespace characters (\t to \r range)
        methodConvert.Dup();                                       // Duplicate character for second check
        methodConvert.Within((ushort)'\t', (ushort)'\r');          // Check if within control whitespace range
        methodConvert.Swap();                                      // Bring original character back to top

        // Check for space character
        methodConvert.Push((ushort)' ');                           // Push space character
        methodConvert.NumEqual();                                  // Check if equals space
        methodConvert.BoolOr();                                    // Combine both checks with OR
    }

    /// <summary>
    /// Handles the char.IsLower method to check if a character is lowercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within the range 'a' to 'z'
    /// </remarks>
    private static void HandleCharIsLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
    }

    /// <summary>
    /// Handles the char.IsUpper method to check if a character is uppercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within the range 'A' to 'Z'
    /// </remarks>
    private static void HandleCharIsUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)'A', (ushort)'Z');            // Check if within uppercase range
    }

    /// <summary>
    /// Handles the char.IsPunctuation method to check if a character is punctuation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within ASCII punctuation ranges: !"#$%&'()*+,-./ :;<=>?@ [\]^_` {|}~
    /// </remarks>
    private static void HandleCharIsPunctuation(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate character for multiple checks
        methodConvert.Within((ushort)'!', (ushort)'/');            // Check if within range !"#$%&'()*+,-./
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found punctuation
        methodConvert.Dup();                                       // Duplicate character for next check
        methodConvert.Within((ushort)':', (ushort)'@');            // Check if within range :;<=>?@
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found punctuation
        methodConvert.Dup();                                       // Duplicate character for next check
        methodConvert.Within((ushort)'[', (ushort)'`');            // Check if within range [\]^_`
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found punctuation
        methodConvert.Within((ushort)'{', (ushort)'~');            // Check if within range {|}~
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.IsSymbol method to check if a character is a symbol.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within ASCII symbol ranges: $%&'()*+ <=> >?@ [\]^_` {|}~
    /// </remarks>
    private static void HandleCharIsSymbol(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate character for multiple checks
        methodConvert.Within((ushort)'$', (ushort)'+');            // Check if within range $%&'()*+
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found symbol
        methodConvert.Dup();                                       // Duplicate character for next check
        methodConvert.Within((ushort)'<', (ushort)'=');            // Check if within range <=>
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found symbol
        methodConvert.Dup();                                       // Duplicate character for next check
        methodConvert.Within((ushort)'>', (ushort)'@');            // Check if within range >?@
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found symbol
        methodConvert.Dup();                                       // Duplicate character for next check
        methodConvert.Within((ushort)'[', (ushort)'`');            // Check if within range [\]^_`
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if found symbol
        methodConvert.Within((ushort)'{', (ushort)'~');            // Check if within range {|}~
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.IsControl method to check if a character is a control character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within control character ranges: \0-\x1F or \x7F-\x9F
    /// </remarks>
    private static void HandleCharIsControl(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();                                       // Duplicate character for second check
        methodConvert.Within((ushort)'\0', (ushort)'\x1F');        // Check if within range \0-\x1F
        methodConvert.Swap();                                      // Bring original character back to top
        methodConvert.Within((ushort)'\x7F', (ushort)'\x9F');      // Check if within range \x7F-\x9F
        methodConvert.BoolOr();                                    // Combine both checks with OR
    }

    /// <summary>
    /// Handles the char.IsSurrogate method to check if a character is a surrogate.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within high surrogate range (0xD800-0xDBFF) or low surrogate range (0xDC00-0xDFFF)
    /// </remarks>
    private static void HandleCharIsSurrogate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();                                       // Duplicate character for second check
        methodConvert.Within((ushort)0xD800, (ushort)0xDBFF);      // Check if within high surrogate range
        methodConvert.Swap();                                      // Bring original character back to top
        methodConvert.Within((ushort)0xDC00, (ushort)0xDFFF);      // Check if within low surrogate range
        methodConvert.BoolOr();                                    // Combine both checks with OR
    }

    /// <summary>
    /// Handles the char.IsHighSurrogate method to check if a character is a high surrogate.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within high surrogate range (0xD800-0xDBFF)
    /// </remarks>
    private static void HandleCharIsHighSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)0xD800, (ushort)0xDBFF);      // Check if within high surrogate range
    }

    /// <summary>
    /// Handles the char.IsLowSurrogate method to check if a character is a low surrogate.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within low surrogate range (0xDC00-0xDFFF)
    /// </remarks>
    private static void HandleCharIsLowSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)0xDC00, (ushort)0xDFFF);      // Check if within low surrogate range
    }

    /// <summary>
    /// Handles the char.IsLetterOrDigit method to check if a character is a letter or digit.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within digit range (0-9), uppercase range (A-Z), or lowercase range (a-z)
    /// </remarks>
    private static void HandleCharIsLetterOrDigit(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Dup();                                       // Duplicate character for digit check
        methodConvert.Within((ushort)'0', (ushort)'9');            // Check if within digit range
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if is digit
        methodConvert.Dup();                                       // Duplicate character for uppercase check
        methodConvert.Within((ushort)'A', (ushort)'Z');            // Check if within uppercase range
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if is uppercase
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.IsBetween method to check if a character is between two values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character >= minValue and character <= maxValue
    /// </remarks>
    private static void HandleCharIsBetween(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget validTarget = new();
        JumpTarget endTarget = new();
        methodConvert.Dup();                                       // Duplicate character for first comparison
        methodConvert.Rot();                                       // Rotate stack to get minValue on top
        methodConvert.Ge();                                        // Check if character >= minValue
        methodConvert.Dup();                                       // Duplicate result for validation
        methodConvert.Jump(OpCode.JMPIFNOT, validTarget);          // Jump if character < minValue
        methodConvert.Reverse3();                                  // Reverse stack order
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Jump(OpCode.JMP, endTarget);                 // Jump to end with false result
        validTarget.Instruction = methodConvert.Nop();             // Valid target marker
        methodConvert.Drop();                                      // Drop the duplicate result
        methodConvert.Lt();                                        // Check if character < maxValue
        endTarget.Instruction = methodConvert.Nop();               // End target marker
    }

    /// <summary>
    /// Handles the char.GetNumericValue method to get the numeric value of a digit character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the numeric value for digit characters (0-9), otherwise returns -1
    /// </remarks>
    private static void HandleCharGetNumericValue(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget validTarget = new();
        methodConvert.Dup();                                       // Duplicate character for range check
        methodConvert.Within((ushort)'0', (ushort)'9');            // Check if within digit range
        methodConvert.Jump(OpCode.JMPIF, validTarget);             // Jump if is digit
        methodConvert.Drop();                                      // Drop character if not digit
        methodConvert.PushM1();                                    // Push -1 for non-digit
        methodConvert.Jump(OpCode.JMP, endTarget);                 // Jump to end
        validTarget.Instruction = methodConvert.Nop();             // Valid digit target
        methodConvert.Push((ushort)'0');                           // Push '0' character
        methodConvert.Sub();                                       // Subtract '0' to get numeric value
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.ToLower method to convert an uppercase character to lowercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts uppercase letters (A-Z) to lowercase (a-z), leaves other characters unchanged
    /// </remarks>
    private static void HandleCharToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();                                       // Duplicate character for range check
        methodConvert.Within((ushort)'A', (ushort)'Z');            // Check if within uppercase range
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);            // Jump if not uppercase
        methodConvert.Push((ushort)'A');                           // Push 'A' character
        methodConvert.Sub();                                       // Subtract 'A' to get offset
        methodConvert.Push((ushort)'a');                           // Push 'a' character
        methodConvert.Add();                                       // Add 'a' to get lowercase
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.ToUpper method to convert a lowercase character to uppercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts lowercase letters (a-z) to uppercase (A-Z), leaves other characters unchanged
    /// </remarks>
    private static void HandleCharToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();                                       // Duplicate character for range check
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);            // Jump if not lowercase
        methodConvert.Push((ushort)'a');                           // Push 'a' character
        methodConvert.Sub();                                       // Subtract 'a' to get offset
        methodConvert.Push((ushort)'A');                           // Push 'A' character
        methodConvert.Add();                                       // Add 'A' to get uppercase
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the char.ToLowerInvariant method to check if a character is lowercase (invariant culture).
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within lowercase range (a-z) using invariant culture
    /// </remarks>
    private static void HandleCharToLowerInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Dup();                                       // Duplicate character for range check
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
    }

    /// <summary>
    /// Handles the char.ToUpperInvariant method to check if a character is uppercase (invariant culture).
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within lowercase range (a-z) using invariant culture
    /// </remarks>
    private static void HandleCharToUpperInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Dup();                                       // Duplicate character for range check
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
    }

    /// <summary>
    /// Handles the char.IsAscii method to check if a character is ASCII.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character value is less than 128 (ASCII range is 0-127)
    /// </remarks>
    private static void HandleCharIsAscii(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(128);                                   // Push 128 (first non-ASCII value)
        methodConvert.Lt();                                        // Check if character < 128
    }

    /// <summary>
    /// Handles the char.IsAsciiDigit method to check if a character is an ASCII digit.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within ASCII digit range ('0' to '9')
    /// </remarks>
    private static void HandleCharIsAsciiDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)'0', (ushort)'9');            // Check if within digit range
    }

    /// <summary>
    /// Handles the char.IsAsciiLetter method to check if a character is an ASCII letter.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if character is within ASCII letter ranges ('A' to 'Z' or 'a' to 'z')
    /// </remarks>
    private static void HandleCharIsAsciiLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Within((ushort)'A', (ushort)'Z');            // Check if within uppercase range
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if uppercase letter found
        methodConvert.Within((ushort)'a', (ushort)'z');            // Check if within lowercase range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }
}
