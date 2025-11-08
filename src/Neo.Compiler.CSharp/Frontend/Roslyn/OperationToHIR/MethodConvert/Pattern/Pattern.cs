// Copyright (C) 2015-2025 The Neo Project.
//
// Pattern.cs file belongs to the neo project and is free
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

namespace Neo.Compiler;

internal partial class MethodConvert
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
            //Convet discard pattern (_) to OpCodes.
            //Example: if (greeting2 is string _)
            case DiscardPatternSyntax:
                Push(true);
                break;
            //Convet relational pattern to OpCodes.
            //Example: return value is > 1;
            case RelationalPatternSyntax relationalPattern:
                ConvertRelationalPattern(model, relationalPattern, localIndex);
                break;
            //Convert type pattern to OpCodes.
            //Example:
            //switch (o1)
            //{
            //    case byte[]: break;
            //    case string: break;
            //}
            case TypePatternSyntax typePattern:
                ConvertTypePattern(model, typePattern, localIndex);
                break;
            //Convet "not" pattern  to OpCodes.
            //Example: return value is not null;
            case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                ConvertNotPattern(model, unaryPattern, localIndex);
                break;
            //Convet parenthesized to OpCodes.
            //Example: return value is (> 1 and < 100);
            case ParenthesizedPatternSyntax parenthesizedPattern:
                ConvertParenthesizedPatternSyntax(model, parenthesizedPattern, localIndex);
                break;
            case RecursivePatternSyntax recursivePattern:
                ConvertRecursivePattern(model, recursivePattern, localIndex);
                break;
            default:
                //Example:
                //object greeting = "Hello, World!";
                //if (greeting3 is var message) { }
                //Example:
                //public static void M(object o1, object o2)
                //{
                //  var t = (o1, o2);
                //  if (t is (int, string)) { }
                //}
                throw CompilationException.UnsupportedSyntax(pattern, $"Pattern type '{pattern.GetType().Name}' is not supported. Supported patterns are: binary (and/or), constant, declaration, discard (_), relational, type, unary (not), parenthesized, and recursive patterns.");
        }
    }
}
