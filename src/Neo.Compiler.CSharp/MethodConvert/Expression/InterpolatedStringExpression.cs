// Copyright (C) 2015-2023 The Neo Project.
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

partial class MethodConvert
{
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
