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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertObjectCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            if (type.TypeKind == TypeKind.Delegate)
            {
                ConvertDelegateCreationExpression(model, expression);
                return;
            }
            IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
            IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
            if (TryProcessSystemConstructors(model, constructor, arguments))
                return;
            bool needCreateObject = !type.DeclaringSyntaxReferences.IsEmpty && !constructor.IsExtern;
            if (needCreateObject)
            {
                CreateObject(model, type, null);
            }
            Call(model, constructor, needCreateObject, arguments);
            if (expression.Initializer is not null)
            {
                ConvertObjectCreationExpressionInitializer(model, expression.Initializer);
            }
        }

        private void ConvertObjectCreationExpressionInitializer(SemanticModel model, InitializerExpressionSyntax initializer)
        {
            foreach (ExpressionSyntax e in initializer.Expressions)
            {
                if (e is not AssignmentExpressionSyntax ae)
                    throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
                ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
                switch (symbol)
                {
                    case IFieldSymbol field:
                        AddInstruction(OpCode.DUP);
                        int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                        Push(index);
                        ConvertExpression(model, ae.Right);
                        AddInstruction(OpCode.SETITEM);
                        break;
                    case IPropertySymbol property:
                        ConvertExpression(model, ae.Right);
                        AddInstruction(OpCode.OVER);
                        Call(model, property.SetMethod!, CallingConvention.Cdecl);
                        break;
                    default:
                        throw new CompilationException(ae.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
                }
            }
        }

        private void ConvertDelegateCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
        {
            if (expression.ArgumentList!.Arguments.Count != 1)
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported delegate: {expression}");
            IMethodSymbol symbol = (IMethodSymbol)model.GetSymbolInfo(expression.ArgumentList.Arguments[0].Expression).Symbol!;
            if (!symbol.IsStatic)
                throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {symbol}");
            MethodConvert convert = context.ConvertMethod(model, symbol);
            Jump(OpCode.PUSHA, convert._startTarget);
        }


    }
}
