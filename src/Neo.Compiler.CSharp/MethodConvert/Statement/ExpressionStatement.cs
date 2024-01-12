// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertExpressionStatement(SemanticModel model, ExpressionStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            using (InsertSequencePoint(syntax))
            {
                ConvertExpression(model, syntax.Expression);
                if (type.SpecialType != SpecialType.System_Void)
                    AddInstruction(OpCode.DROP);
            }
        }
    }
}
