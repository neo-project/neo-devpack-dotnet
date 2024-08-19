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
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts a switch expression to OpCodes.
    /// Switch expressions are a new feature introduced in C# 8.0(Released September, 2019).
    /// For a traditional switch statement, see <see cref="ConvertSwitchStatement(SemanticModel, SwitchStatementSyntax)"/>.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about switch expression.</param>
    /// <param name="expression">The syntax representation of the switch expression statement being converted.</param>
    /// <exception cref="CompilationException">Unsupported symbols will result in a compilation exception, such as methods.</exception>
    /// <remarks>
    /// The method processes each switch expression arm and evaluates the pattern matching for each case.
    /// It generates OpCodes based on the matching results and expressions in each arm.
    /// After evaluating each arm, it throws an exception if none of the cases match.
    /// </remarks>
    /// <example>
    /// The switch statement selects the appropriate case branch based on the value of day.
    /// <code>
    /// int day = 4;
    /// string dayName = day switch
    /// {
    ///     1 => "Monday",
    ///     2 => "Tuesday",
    ///     3 => "Wednesday",
    ///     4 => "Thursday",
    ///     5 => "Friday",
    ///     6 => "Saturday",
    ///     7 => "Sunday",
    ///     _ => "Unknown",
    /// };
    /// Runtime.Log($"Today is {dayName}");
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression">switch expression - pattern matching expressions using the switch keyword</seealso>
    private void ConvertSwitchExpression(SemanticModel model, SwitchExpressionSyntax expression)
    {
        var arms = expression.Arms.Select(p => (p, new JumpTarget())).ToArray();
        JumpTarget breakTarget = new();
        byte anonymousIndex = AddAnonymousVariable();
        ConvertExpression(model, expression.GoverningExpression);
        AccessSlot(OpCode.STLOC, anonymousIndex);
        foreach (var (arm, nextTarget) in arms)
        {
            ConvertPattern(model, arm.Pattern, anonymousIndex);
            Jump(OpCode.JMPIFNOT_L, nextTarget);
            if (arm.WhenClause is not null)
            {
                ConvertExpression(model, arm.WhenClause.Condition);
                Jump(OpCode.JMPIFNOT_L, nextTarget);
            }
            ConvertExpression(model, arm.Expression);
            Jump(OpCode.JMP_L, breakTarget);
            nextTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        AccessSlot(OpCode.LDLOC, anonymousIndex);
        AddInstruction(OpCode.THROW);
        breakTarget.Instruction = AddInstruction(OpCode.NOP);
        RemoveAnonymousVariable(anonymousIndex);
    }
}
