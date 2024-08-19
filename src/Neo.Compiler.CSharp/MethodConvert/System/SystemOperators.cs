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
    /// Attempts to process system operators. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">An array of expression parameters.</param>
    /// <returns>True if system operators are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemOperators(SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
    {
        switch (symbol.ToString())
        {
            //Handles cases of equality operator (==), comparing whether two objects or strings are equal.
            case "object.operator ==(object, object)":
            case "string.operator ==(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.EQUAL);
                return true;
            //Handles cases of inequality operator (!=), comparing whether two objects are not equal.
            case "object.operator !=(object, object)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.NOTEQUAL);
                return true;
            //Handles cases of string concatenation operator (+), concatenating two strings into one.
            case "string.operator +(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating a string with an object.
            //Unsupported interpolation: object
            case "string.operator +(string, object)":
                ConvertExpression(model, arguments[0]);
                ConvertObjectToString(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            //Handles cases of string concatenation operator (+), concatenating an object with a string.
            //Unsupported interpolation: object
            case "string.operator +(object, string)":
                ConvertObjectToString(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            default:
                return false;
        }
    }
}
