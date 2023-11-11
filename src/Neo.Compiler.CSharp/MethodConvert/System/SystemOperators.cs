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
    private bool TryProcessSystemOperators(SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
    {
        switch (symbol.ToString())
        {
            case "object.operator ==(object, object)":
            case "string.operator ==(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.EQUAL);
                return true;
            case "object.operator !=(object, object)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.NOTEQUAL);
                return true;
            case "string.operator +(string, string)":
                ConvertExpression(model, arguments[0]);
                ConvertExpression(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
            case "string.operator +(string, object)":
                ConvertExpression(model, arguments[0]);
                ConvertObjectToString(model, arguments[1]);
                AddInstruction(OpCode.CAT);
                ChangeType(VM.Types.StackItemType.ByteString);
                return true;
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
