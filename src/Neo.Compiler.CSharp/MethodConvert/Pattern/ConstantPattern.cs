// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convet a constant pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about constant pattern.</param>
    /// <param name="pattern">The constant pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <remarks>
    /// You use a constant pattern to test if an expression result equals a specified constant.
    /// In a constant pattern, you can use any constant expression, such as:
    /// integer, char, string, Boolean value true or false, enum value, the name of a declared const field or local, null
    /// </remarks>
    /// <example>
    /// <code>
    /// public static int GetGroupTicketPrice(int visitorCount) => visitorCount switch
    /// {
    ///     1 => 12,
    ///     2 => 20,
    ///     3 => 27,
    ///     4 => 32,
    ///     0 => 0,
    ///     _ => throw new Exception($"Not supported number of visitors: {visitorCount}"),
    /// };
    /// </code>
    /// </example>
    private void ConvertConstantPattern(SemanticModel model, ConstantPatternSyntax pattern, byte localIndex)
    {
        AccessSlot(OpCode.LDLOC, localIndex);
        ConvertExpression(model, pattern.Expression);
        AddInstruction(OpCode.EQUAL);
    }
}
