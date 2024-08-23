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
    /// This method converts an interpolated string expression to OpCodes.
    /// The $ character identifies a string literal as an interpolated string.
    /// An interpolated string is a string literal that might contain interpolation expressions.
    /// When an interpolated string is resolved to a result string,
    /// items with interpolation expressions are replaced by the string representations of the expression results.
    /// Interpolated string expression are a new feature introduced in C# 8.0(Released September, 2019).
    /// </summary>
    /// <param name="model">The semantic model providing context and information about interpolated string expression.</param>
    /// <param name="expression">The syntax representation of the interpolated string expression statement being converted.</param>
    /// <remarks>
    /// The method processes each interpolated string content segment and concatenates them using the CAT opcode.
    /// If the interpolated string contains no segments, it pushes an empty string onto the evaluation stack.
    /// If the interpolated string contains two or more segments, it changes the type of the resulting string to ByteString.
    /// </remarks>
    /// <example>
    /// The following interpolated string will be divided into 5 parts and concatenated via OpCode.CAT
    /// <code>
    /// var name = "Mark";
    /// var timestamp = Ledger.GetBlock(Ledger.CurrentHash).Timestamp;
    /// Runtime.Log($"Hello, {name}! Current timestamp is {timestamp}.");
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated">String interpolation using $</seealso>
    private void ConvertInterpolatedStringExpression(SemanticModel model, InterpolatedStringExpressionSyntax expression)
    {
        if (expression.Contents.Count == 0)
        {
            Push(string.Empty);
            return;
        }
        ConvertInterpolatedStringContent(model, expression.Contents[0]);
        for (int i = 1; i < expression.Contents.Count; i++)
        {
            ConvertInterpolatedStringContent(model, expression.Contents[i]);
            AddInstruction(OpCode.CAT);
        }
        if (expression.Contents.Count >= 2)
            ChangeType(VM.Types.StackItemType.ByteString);
    }

    private void ConvertInterpolatedStringContent(SemanticModel model, InterpolatedStringContentSyntax content)
    {
        switch (content)
        {
            case InterpolatedStringTextSyntax syntax:
                Push(syntax.TextToken.ValueText);
                break;
            case InterpolationSyntax syntax:
                if (syntax.AlignmentClause is not null)
                    throw new CompilationException(syntax.AlignmentClause, DiagnosticId.AlignmentClause, $"Alignment clause is not supported: {syntax.AlignmentClause}");
                if (syntax.FormatClause is not null)
                    throw new CompilationException(syntax.FormatClause, DiagnosticId.FormatClause, $"Format clause is not supported: {syntax.FormatClause}");
                ConvertObjectToString(model, syntax.Expression);
                break;
        }
    }
}
