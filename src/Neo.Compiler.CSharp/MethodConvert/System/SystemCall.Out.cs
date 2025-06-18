// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Handles the byte.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with byte range [0, 255]
    /// </remarks>
    private static void HandleByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, byte.MinValue, byte.MaxValue);
    }

    /// <summary>
    /// Handles the sbyte.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with sbyte range [-128, 127]
    /// </remarks>
    private static void HandleSByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, sbyte.MinValue, sbyte.MaxValue);
    }

    /// <summary>
    /// Handles the short.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with short range [-32768, 32767]
    /// </remarks>
    private static void HandleShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, short.MinValue, short.MaxValue);
    }

    /// <summary>
    /// Handles the ushort.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with ushort range [0, 65535]
    /// </remarks>
    private static void HandleUShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ushort.MinValue, ushort.MaxValue);
    }

    /// <summary>
    /// Handles the int.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with int range [-2147483648, 2147483647]
    /// </remarks>
    private static void HandleIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, int.MinValue, int.MaxValue);
    }

    /// <summary>
    /// Handles the uint.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with uint range [0, 4294967295]
    /// </remarks>
    private static void HandleUIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, uint.MinValue, uint.MaxValue);
    }

    /// <summary>
    /// Handles the long.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with long range [-9223372036854775808, 9223372036854775807]
    /// </remarks>
    private static void HandleLongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, long.MinValue, long.MaxValue);
    }

    /// <summary>
    /// Handles the ulong.TryParse method with out parameter by delegating to numeric parsing logic.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to HandleNumericTryParseWithOut with ulong range [0, 18446744073709551615]
    /// </remarks>
    private static void HandleULongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ulong.MinValue, ulong.MaxValue);
    }

    /// <summary>
    /// Handles numeric TryParse methods with out parameters by parsing string and validating range.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="arguments">The method arguments</param>
    /// <param name="minValue">The minimum valid value for the type</param>
    /// <param name="maxValue">The maximum valid value for the type</param>
    /// <remarks>
    /// Algorithm: Uses StdLib.atoi to parse string, validates range, stores result in static field, returns success flag
    /// </remarks>
    private static void HandleNumericTryParseWithOut(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments, BigInteger minValue, BigInteger maxValue)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw CompilationException.UnsupportedSyntax(symbol, "Out parameters in TryParse methods must be captured in a static field. Ensure the out parameter is assigned to a static field before calling TryParse.");

        // Drop the out parameter since it's not needed
        // We use the static field to store the result
        methodConvert.Swap();                                      // Swap arguments order
        methodConvert.Drop();                                      // Drop out parameter

        // Convert string to integer
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful (not null)
        methodConvert.Dup();                                       // Duplicate result for null check
        methodConvert.IsNull();                                    // Check if null (parse failed)

        JumpTarget failTarget = new();
        methodConvert.Jump(OpCode.JMPIF_L, failTarget);            // Jump to fail if null

        // If successful, check if the parsed value is within the valid range
        methodConvert.Dup();                                        // Duplicate value for range check
        methodConvert.Within(minValue, maxValue);                   // Check if within range
        methodConvert.Jump(OpCode.JMPIFNOT_L, failTarget);         // Jump to fail if out of range

        // If within range, store the value and push true
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store value in static field
        methodConvert.Push(true);                                  // Push success flag
        JumpTarget endTarget = new();
        methodConvert.Jump(OpCode.JMP_L, endTarget);               // Jump to end

        // Fail target: push false
        failTarget.Instruction = methodConvert.Drop();             // Drop the failed value
        methodConvert.Push(false);                                 // Push failure flag

        // End target
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the BigInteger.TryParse method with out parameter by parsing string to BigInteger.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses StdLib.atoi to parse string to BigInteger, stores result in static field, returns success flag
    /// </remarks>
    private static void HandleBigIntegerTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw CompilationException.UnsupportedSyntax(symbol, "Out parameters in TryParse methods must be captured in a static field. Ensure the out parameter is assigned to a static field before calling TryParse.");

        JumpTarget endTarget = new();

        // Convert string to BigInteger
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful
        methodConvert.Dup();                                       // Duplicate result for null check
        methodConvert.IsNull();                                    // Check if null (parse failed)
        methodConvert.Jump(OpCode.JMPIF_L, endTarget);             // Jump to end if null

        // If successful, store the value and push true
        methodConvert.Dup();                                       // Duplicate result for storage
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store value in static field
        methodConvert.Push(true);                                  // Push success flag
        methodConvert.Jump(OpCode.JMP_L, endTarget);               // Jump to end

        // End target: clean up stack and push false if parsing failed
        endTarget.Instruction = methodConvert.Nop();               // End target
        methodConvert.Drop();                                      // Drop the failed value
        methodConvert.Push(false);                                 // Push failure flag
    }

    /// <summary>
    /// Handles the bool.TryParse method with out parameter by parsing various string representations to boolean.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks input string against various true/false representations, stores result in static field
    /// </remarks>
    private static void HandleBoolTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw CompilationException.UnsupportedSyntax(symbol, "Out parameters in TryParse methods must be captured in a static field. Ensure the out parameter is assigned to a static field before calling TryParse.");

        JumpTarget trueTarget = new();
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();

        methodConvert.Swap();                                      // Swap arguments order
        methodConvert.Drop();                                      // Drop out parameter

        // Check for true values
        methodConvert.Dup();                                       // x x
        methodConvert.Push("true");                                // x x "true"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.Dup();                                       // x x
        methodConvert.Push("TRUE");                                // x x "TRUE"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.Dup();                                       // x x
        methodConvert.Push("True");                                // x x "True"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.Dup();                                       // x x
        methodConvert.Push("t");                                   // x x "t"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.Dup();                                       // x x
        methodConvert.Push("T");                                   // x x "T"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.Dup();                                       // x x
        methodConvert.Push("1");                                   // x x "1"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);            // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("yes");                                 // x x "yes"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);            // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("YES");                                 // x x "YES"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);            // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("y");                                   // x x "y"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);            // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("Y");                                   // x x "Y"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);            // x

        // Check for false values
        methodConvert.Dup();
        methodConvert.Push("false");                               // x x "false"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("FALSE");                               // x x "FALSE"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("False");                               // x x "False"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("f");                                   // x x "f"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();
        methodConvert.Push("F");                                   // x x "F"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("0");                                   // x x "0"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();
        methodConvert.Push("no");                                  // x x "no"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("NO");                                  // x x "NO"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("n");                                   // x x "n"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        methodConvert.Dup();                                       // x x
        methodConvert.Push("N");                                   // x x "N"
        methodConvert.Equal();                                     // x (equal result)
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget);           // x

        // If parsing failed, clean up stack and push false
        methodConvert.Drop();                                      // Clean up input
        methodConvert.Push(false);                                 // Default out value
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store false in out parameter
        methodConvert.Push(false);                                 // Return false for parsing failure
        methodConvert.Jump(OpCode.JMP_L, endTarget);               // Jump to end

        // True case
        trueTarget.Instruction = methodConvert.Nop();              // True target
        methodConvert.Drop();                                      // Clean up input
        methodConvert.Push(true);                                  // Set out value to true
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store true in out parameter
        methodConvert.Push(true);                                  // Return true for successful parsing
        methodConvert.Jump(OpCode.JMP_L, endTarget);               // Jump to end

        // False case
        falseTarget.Instruction = methodConvert.Nop();             // False target
        methodConvert.Drop();                                      // Clean up input
        methodConvert.Push(false);                                 // Set out value to false
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store false in out parameter
        methodConvert.Push(true);                                  // Return true for successful parsing

        // End target
        endTarget.Instruction = methodConvert.Nop();               // End target
    }
}
