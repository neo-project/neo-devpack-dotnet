// Copyright (C) 2015-2026 The Neo Project.
//
// RelationalPattern.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convert a relational pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about convert pattern.</param>
    /// <param name="pattern">The convert pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <exception cref="CompilationException"></exception>
    /// <remarks>
    /// In a relational pattern, you can use any of the relational operators <![CDATA[<, >, <=, or >=]]>.
    /// The right-hand part of a relational pattern must be a constant expression.
    /// The constant expression can be of an integer, char, or enum type.
    /// To check if an expression result is in a certain range, match it against a <see cref="ConvertBinaryPattern(SemanticModel, BinaryPatternSyntax, byte)">conjunctive and pattern</see>.
    /// </remarks>
    /// <example>
    /// You use a relational pattern to compare an expression result with a constant,
    /// as the following example shows:
    /// <code>
    /// int a = 1;
    /// var b = a switch
    /// {
    ///     > 1 => true,
    ///     <= 1 => false
    /// };
    /// </code>
    /// <c>> 1</c> and <c><= 1</c> is RelationalPatternSyntax;
    /// </example>
    private void ConvertRelationalPattern(SemanticModel model, RelationalPatternSyntax pattern, byte localIndex)
    {
        // A relational pattern lowers to a NeoVM numeric comparison (LT/LE/GT/GE) on the
        // scrutinee. That is only well defined when the scrutinee is guaranteed to be a
        // concrete numeric value at runtime. If the scrutinee may be null or a non-numeric
        // reference value (e.g. an `object` or a `Nullable<T>`), C# requires the pattern to
        // evaluate to false, but the comparison would instead fault the VM. Reject that
        // shape with a clear diagnostic rather than emitting code that faults at runtime.
        if (model.GetOperation(pattern) is IRelationalPatternOperation { InputType: { } inputType } &&
            RelationalPatternInputMayFault(inputType))
        {
            throw CompilationException.UnsupportedSyntax(pattern,
                $"Relational patterns ('<', '<=', '>', '>=') are only supported on non-nullable numeric value types. " +
                $"The scrutinee type '{inputType.ToDisplayString()}' may be null or non-numeric at runtime. " +
                $"Match the value as a numeric type first (for example 'value is int and > 5').");
        }

        AccessSlot(OpCode.LDLOC, localIndex);
        ConvertExpression(model, pattern.Expression);
        AddInstruction(pattern.OperatorToken.ValueText switch
        {
            "<" => OpCode.LT,
            "<=" => OpCode.LE,
            ">" => OpCode.GT,
            ">=" => OpCode.GE,
            _ => throw CompilationException.UnsupportedSyntax(pattern, $"Relational operator '{pattern.OperatorToken.ValueText}' is not supported in patterns. Only <, <=, >, and >= are allowed.")
        });
    }

    /// <summary>
    /// Determines whether a relational pattern over the given scrutinee type could fault the
    /// NeoVM comparison at runtime. Non-nullable value types (int, BigInteger, char, enum, ...)
    /// are always a concrete value and are safe; reference types and <see cref="System.Nullable{T}"/>
    /// may be null or a non-numeric value and are not.
    /// </summary>
    private static bool RelationalPatternInputMayFault(ITypeSymbol inputType)
    {
        if (!inputType.IsValueType)
            return true;
        if (inputType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T })
            return true;
        return false;
    }
}
