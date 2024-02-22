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
    private void ConvertPattern(SemanticModel model, PatternSyntax pattern, byte localIndex)
    {
        switch (pattern)
        {
            case BinaryPatternSyntax binaryPattern:
                ConvertBinaryPattern(model, binaryPattern, localIndex);
                break;
            case ConstantPatternSyntax constantPattern:
                ConvertConstantPattern(model, constantPattern, localIndex);
                break;
            case DeclarationPatternSyntax declarationPattern:
                ConvertDeclarationPattern(model, declarationPattern, localIndex);
                break;
            case DiscardPatternSyntax:
                Push(true);
                break;
            case RelationalPatternSyntax relationalPattern:
                ConvertRelationalPattern(model, relationalPattern, localIndex);
                break;
            case TypePatternSyntax typePattern:
                ConvertTypePattern(model, typePattern, localIndex);
                break;
            case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                ConvertNotPattern(model, unaryPattern, localIndex);
                break;
            default:
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }
}
