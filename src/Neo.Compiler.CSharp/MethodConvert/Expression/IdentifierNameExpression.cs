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

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts identifier name expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The identifier expression syntax node</param>
    /// <remarks>
    /// This handles identifiers referring to fields, locals, parameters,
    /// properties etc.
    ///
    /// Examples:
    ///
    /// myLocal
    /// MyField
    /// obj.Prop
    ///
    /// The method determines the target symbol and emits instructions
    /// to load/access the value based on its type.
    ///
    /// This includes handling constant values, static vs instance
    /// members, properties and more.
    /// </remarks>
    private void ConvertIdentifierNameExpression(SemanticModel model, IdentifierNameSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    byte index = context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    AddInstruction(OpCode.LDARG0);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case ILocalSymbol local:
                if (local.IsConst)
                    Push(local.ConstantValue);
                else
                    AccessSlot(OpCode.LDLOC, _localVariables[local]);
                break;
            case IMethodSymbol method:
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                MethodConvert convert = context.ConvertMethod(model, method);
                Jump(OpCode.PUSHA, convert._startTarget);
                break;
            case IParameterSymbol parameter:
                if (!_internalInline)
                    AccessSlot(OpCode.LDARG, _parameters[parameter]);
                break;
            case IPropertySymbol property:
                if (property.IsStatic)
                {
                    Call(model, property.GetMethod!);
                }
                else
                {
                    AddInstruction(OpCode.LDARG0);
                    Call(model, property.GetMethod!);
                }
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
