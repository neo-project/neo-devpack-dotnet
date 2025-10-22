// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Nullable.cs file belongs to the neo project and is free
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

internal partial class MethodConvert
{
    /// <summary>
    /// Handles the Nullable&lt;byte&gt;.HasValue property by checking if the nullable byte has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;byte&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;byte&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for byte)
    /// </remarks>
    private static void HandleNullableByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;sbyte&gt;.HasValue property by checking if the nullable sbyte has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableSByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;sbyte&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableSByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;sbyte&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for sbyte)
    /// </remarks>
    private static void HandleNullableSByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;short&gt;.HasValue property by checking if the nullable short has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;short&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;short&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for short)
    /// </remarks>
    private static void HandleNullableShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;ushort&gt;.HasValue property by checking if the nullable ushort has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableUShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;ushort&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableUShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;ushort&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for ushort)
    /// </remarks>
    private static void HandleNullableUShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;uint&gt;.HasValue property by checking if the nullable uint has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableUIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;uint&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableUIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;uint&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for uint)
    /// </remarks>
    private static void HandleNullableUIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;ulong&gt;.HasValue property by checking if the nullable ulong has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableULongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;ulong&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableULongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;ulong&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for ulong)
    /// </remarks>
    private static void HandleNullableULongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable ToString method by converting the nullable value to string representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns string representation of value if not null, otherwise returns empty string
    /// </remarks>
    private static void HandleNullableToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget endTarget2 = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.JumpIfTrue( endTarget);               // Jump if null
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        methodConvert.JumpAlwaysLong( endTarget2);              // Jump to end
        endTarget.Instruction = methodConvert.Nop();               // Null case target
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        endTarget2.Instruction = methodConvert.Nop();              // End target
    }

    /// <summary>
    /// Handles the nullable bool ToString method by converting the nullable bool to string representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns "True", "False", or empty string for null values
    /// </remarks>
    private static void HandleNullableBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget trueTarget = new(), nullTarget = new(), endTarget = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.JumpIfTrueLong( nullTarget);            // Jump if null
        methodConvert.JumpIfTrueLong( trueTarget);            // Jump if true
        methodConvert.Push("False");                               // Push "False" for false values
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end
        trueTarget.Instruction = methodConvert.Push("True");       // Push "True" for true values
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end
        nullTarget.Instruction = methodConvert.Nop();              // Null case target
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable Equals method by comparing two nullable values for equality.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns true if both are null or both have equal values, false otherwise
    /// </remarks>
    private static void HandleNullableEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.Dup();                                       // x y
        methodConvert.IsNull();                                    // Check if y is null
        methodConvert.JumpIfTrueLong( nullTarget1);           // Jump if y is null

        // y is not null
        methodConvert.Swap();                                      // y x
        methodConvert.Dup();                                       // y x x
        methodConvert.IsNull();                                    // y x (x == null)
        methodConvert.JumpIfTrueLong( nullTarget2);           // Jump if x is null

        // y and x both are not null
        methodConvert.Equal();                                     // Check equality
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.Nop();             // Null target 1
        methodConvert.Drop();                                      // Drop y, keep x
        methodConvert.IsNull();                                    // Check if x is null
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        nullTarget2.Instruction = methodConvert.Nop();             // Null target 2
        methodConvert.Drop();                                      // Drop x
        methodConvert.Drop();                                      // Drop y
        methodConvert.Push(false);                                 // y not null but x is null

        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable BigInteger Equals method by comparing two nullable BigInteger values for equality.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns true if both are null or both have numerically equal values, false otherwise
    /// </remarks>
    private static void HandleNullableBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.Dup();                                       // x y
        methodConvert.IsNull();                                    // Check if y is null
        methodConvert.JumpIfTrueLong( nullTarget1);           // Jump if y is null

        // y is not null
        methodConvert.Swap();                                      // y x
        methodConvert.Dup();                                       // y x x
        methodConvert.IsNull();                                    // y x (x == null)
        methodConvert.JumpIfTrueLong( nullTarget2);           // Jump if x is null

        // y and x both are not null
        methodConvert.NumEqual();                                  // Check numeric equality
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.Nop();             // Null target 1
        methodConvert.Drop();                                      // Drop y, keep x
        methodConvert.IsNull();                                    // Check if x is null
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        nullTarget2.Instruction = methodConvert.Nop();             // Null target 2
        methodConvert.Drop();                                      // Drop x
        methodConvert.Drop();                                      // Drop y
        methodConvert.Push(false);                                 // y not null but x is null

        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable bool Equals method by comparing two nullable bool values for equality.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns true if both are null or both have equal bool values, false otherwise
    /// </remarks>
    private static void HandleNullableBoolEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.Dup();                                       // x y
        methodConvert.IsNull();                                    // Check if y is null
        methodConvert.JumpIfTrueLong( nullTarget1);           // Jump if y is null

        methodConvert.Dup();                                       // x y y
        methodConvert.IsNull();                                    // x y (y == null)
        methodConvert.JumpIfTrueLong( nullTarget2);           // Jump if y is null

        methodConvert.Equal();                                     // Check equality
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        nullTarget1.Instruction = methodConvert.Nop();             // Null target 1
        methodConvert.Drop();                                      // Drop y
        methodConvert.IsNull();                                    // Check if x is null
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        nullTarget2.Instruction = methodConvert.Nop();             // Null target 2
        methodConvert.Drop();                                      // Drop y
        methodConvert.Push(false);                                 // Return false

        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable BigInteger Equals method comparing with non-nullable BigInteger.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns false if nullable is null, otherwise compares values numerically
    /// </remarks>
    private static void HandleNullableBigIntegerEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        methodConvert.Dup();                                       // x y
        methodConvert.IsNull();                                    // Check if y is null
        methodConvert.JumpIfTrueLong( nullTarget);            // Jump if y is null

        // y is not null
        methodConvert.NumEqual();                                  // Check numeric equality
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        // y is null, then return false
        nullTarget.Instruction = methodConvert.Nop();              // Null target
        methodConvert.Drop();                                      // Drop x
        methodConvert.Push(false);                                 // Return false for null

        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the nullable bool Equals method comparing with non-nullable bool.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns false if nullable is null, otherwise compares values for equality
    /// </remarks>
    private static void HandleNullableBoolEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        methodConvert.Dup();                                       // x y
        methodConvert.IsNull();                                    // Check if y is null
        methodConvert.JumpIfTrueLong( nullTarget);            // Jump if y is null

        methodConvert.Equal();                                     // Check equality
        methodConvert.JumpAlwaysLong( endTarget);               // Jump to end

        nullTarget.Instruction = methodConvert.Nop();              // Null target
        methodConvert.Drop();                                      // Drop y
        methodConvert.Push(false);                                 // Return false for null

        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;bool&gt;.HasValue property by checking if the nullable bool has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableBoolHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;bool&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableBoolValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;bool&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (false for bool)
    /// </remarks>
    private static void HandleNullableBoolGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (false)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;char&gt;.HasValue property by checking if the nullable char has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableCharHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;char&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableCharValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;char&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for char)
    /// </remarks>
    private static void HandleNullableCharGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (null character)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;BigInteger&gt;.HasValue property by checking if the nullable BigInteger has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableBigIntegerHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;BigInteger&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableBigIntegerValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;BigInteger&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for BigInteger)
    /// </remarks>
    private static void HandleNullableBigIntegerGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;int&gt;.HasValue property by checking if the nullable int has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;int&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;int&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for int)
    /// </remarks>
    private static void HandleNullableIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;long&gt;.HasValue property by checking if the nullable long has a value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Checks if the nullable value is not null using ISNULL followed by NOT
    /// </remarks>
    private static void HandleNullableLongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.IsNull();                                    // Check if value is null
        methodConvert.Not();                                       // Return NOT null (true if has value)
    }

    /// <summary>
    /// Handles the Nullable&lt;long&gt;.Value property by returning the value or throwing if null.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise throws InvalidOperationException
    /// </remarks>
    private static void HandleNullableLongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Throw();                                     // Throw if null
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Nullable&lt;long&gt;.GetValueOrDefault method by returning the value or default.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns the underlying value if not null, otherwise returns default value (0 for long)
    /// </remarks>
    private static void HandleNullableLongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Dup();                                       // Duplicate value for null check
        methodConvert.IsNull();                                    // Check if value is null
        var endTarget = new JumpTarget();
        methodConvert.JumpIfFalse( endTarget);            // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push(0);                                     // Push default value (0)
        endTarget.Instruction = methodConvert.Nop();               // End target
    }
}
