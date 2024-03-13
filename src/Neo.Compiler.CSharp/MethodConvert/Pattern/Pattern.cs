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

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Convert pattern to OpCodes.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="pattern"></param>
    /// <param name="localIndex"></param>
    /// <exception cref="CompilationException"></exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#logical-patterns">
    /// Pattern matching - the is and switch expressions, and operators and, or and not in patterns
    /// </seealso>
    private void ConvertPattern(SemanticModel model, PatternSyntax pattern, byte localIndex)
    {
        switch (pattern)
        {
            //Convet "and" / "or" pattern  to OpCodes.
            //Example: return value is > 1 and < 100;
            //Example: return value is >= 80 or <= 20;
            case BinaryPatternSyntax binaryPattern:
                ConvertBinaryPattern(model, binaryPattern, localIndex);
                break;
            //Convet constant pattern to OpCodes.
            //Example: return value is > 1;
            //Example: return value is null;
            case ConstantPatternSyntax constantPattern:
                ConvertConstantPattern(model, constantPattern, localIndex);
                break;
            //Convet declaration pattern to OpCodes.
            //Example: if (greeting is string message)
            case DeclarationPatternSyntax declarationPattern:
                ConvertDeclarationPattern(model, declarationPattern, localIndex);
                break;
            //TODO
            case DiscardPatternSyntax:
                Push(true);
                break;
            //TODO
            case RelationalPatternSyntax relationalPattern:
                ConvertRelationalPattern(model, relationalPattern, localIndex);
                break;
            //TODO
            case TypePatternSyntax typePattern:
                ConvertTypePattern(model, typePattern, localIndex);
                break;
            //Convet "not" pattern  to OpCodes.
            //Example: return value is not null;
            case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                ConvertNotPattern(model, unaryPattern, localIndex);
                break;
            //TODO
            case ParenthesizedPatternSyntax parenthesizedPattern:
                ConvertParenthesizedPatternSyntax(model, parenthesizedPattern, localIndex);
                break;
            default:
                //Example:
                //object greeting = "Hello, World!";
                //if (greeting3 is var message) { }
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }
}
