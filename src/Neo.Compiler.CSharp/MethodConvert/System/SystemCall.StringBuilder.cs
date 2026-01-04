// Copyright (C) 2015-2026 The Neo Project.
//
// SystemCall.StringBuilder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private const string StringBuilderNewLine = "\n";

    private static void RegisterStringBuilderHandlers()
    {
        const string BuilderType = "System.Text.StringBuilder";

        // Core methods
        RegisterHandler((StringBuilder builder, string value) => builder.Append(value), HandleStringBuilderAppendString, $"{BuilderType}.Append(string)");
        RegisterHandler((StringBuilder builder, char value) => builder.Append(value), HandleStringBuilderAppendChar, $"{BuilderType}.Append(char)");
        RegisterHandler((StringBuilder builder, StringBuilder other) => builder.Append(other), HandleStringBuilderAppendBuilder, $"{BuilderType}.Append({BuilderType})");
        RegisterHandler((StringBuilder builder) => builder.AppendLine(), HandleStringBuilderAppendLine, $"{BuilderType}.AppendLine()");
        RegisterHandler((StringBuilder builder, string value) => builder.AppendLine(value), HandleStringBuilderAppendLineWithValue, $"{BuilderType}.AppendLine(string)");
        RegisterHandler((StringBuilder builder) => builder.ToString(), HandleStringBuilderToString, $"{BuilderType}.ToString()");
        RegisterHandler((StringBuilder builder) => builder.Length, HandleStringBuilderLength, $"{BuilderType}.Length.get");
        RegisterHandler((StringBuilder builder) => builder.Clear(), HandleStringBuilderClear, $"{BuilderType}.Clear()");
    }

    private static void HandleStringBuilderConstructor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<ArgumentSyntax>? arguments)
    {
        ArgumentSyntax? valueExpression = null;
        if (symbol.Parameters.Length > 0 && arguments is not null)
        {
            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.Type.SpecialType == SpecialType.System_String && parameter.Ordinal < arguments.Count)
                {
                    valueExpression = arguments[parameter.Ordinal];
                    break;
                }
            }
        }

        if (valueExpression is not null)
        {
            methodConvert.ConvertExpression(model, ExtractExpression(valueExpression));
            EnsureStringValue(methodConvert);
        }
        else
        {
            methodConvert.Push("");
        }

        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleStringBuilderAppendString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        AppendStringBuilderValue(methodConvert, model, instanceExpression, arguments!, symbol.Parameters[0].Type, appendNewLine: false, includeArgument: true);
    }

    private static void HandleStringBuilderAppendChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        AppendStringBuilderValue(methodConvert, model, instanceExpression, arguments!, symbol.Parameters[0].Type, appendNewLine: false, includeArgument: true);
    }

    private static void HandleStringBuilderAppendBuilder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        AppendStringBuilderValue(methodConvert, model, instanceExpression, arguments!, symbol.Parameters[0].Type, appendNewLine: false, includeArgument: true);
    }

    private static void HandleStringBuilderAppendLine(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        AppendStringBuilderValue(methodConvert, model, instanceExpression, arguments ?? System.Array.Empty<SyntaxNode>(), symbol.Parameters.Length > 0 ? symbol.Parameters[0].Type : null, appendNewLine: true, includeArgument: symbol.Parameters.Length > 0);
    }

    private static void HandleStringBuilderAppendLineWithValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        AppendStringBuilderValue(methodConvert, model, instanceExpression, arguments!, symbol.Parameters[0].Type, appendNewLine: true, includeArgument: true);
    }

    private static void HandleStringBuilderToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        byte builderSlot = CaptureStringBuilderInstance(methodConvert, model, instanceExpression);
        LoadStringBuilderContent(methodConvert, builderSlot);
        methodConvert.RemoveAnonymousVariable(builderSlot);
    }

    private static void HandleStringBuilderLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        byte builderSlot = CaptureStringBuilderInstance(methodConvert, model, instanceExpression);
        LoadStringBuilderContent(methodConvert, builderSlot);
        methodConvert.Size();
        methodConvert.RemoveAnonymousVariable(builderSlot);
    }

    private static void HandleStringBuilderClear(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        byte builderSlot = CaptureStringBuilderInstance(methodConvert, model, instanceExpression);
        methodConvert.AccessSlot(OpCode.LDLOC, builderSlot);
        methodConvert.Push(0);
        methodConvert.Push("");
        methodConvert.SetItem();
        methodConvert.AccessSlot(OpCode.LDLOC, builderSlot);
        methodConvert.RemoveAnonymousVariable(builderSlot);
    }

    private static void AppendStringBuilderValue(MethodConvert methodConvert, SemanticModel model, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode> arguments, ITypeSymbol? parameterType, bool appendNewLine, bool includeArgument)
    {
        byte builderSlot = CaptureStringBuilderInstance(methodConvert, model, instanceExpression);
        byte currentSlot = methodConvert.AddAnonymousVariable();
        LoadStringBuilderContent(methodConvert, builderSlot);
        methodConvert.AccessSlot(OpCode.STLOC, currentSlot);

        byte? valueSlot = null;
        if (includeArgument && parameterType is not null)
            valueSlot = ConvertStringBuilderArgument(methodConvert, model, parameterType, arguments[0]);

        byte resultSlot = methodConvert.AddAnonymousVariable();
        methodConvert.AccessSlot(OpCode.LDLOC, currentSlot);

        if (valueSlot.HasValue)
        {
            methodConvert.AccessSlot(OpCode.LDLOC, valueSlot.Value);
            methodConvert.Cat();
        }

        if (appendNewLine)
        {
            methodConvert.Push(StringBuilderNewLine);
            methodConvert.Cat();
        }

        methodConvert.AccessSlot(OpCode.STLOC, resultSlot);
        StoreStringBuilderContent(methodConvert, builderSlot, resultSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, builderSlot);

        methodConvert.RemoveAnonymousVariable(resultSlot);
        methodConvert.RemoveAnonymousVariable(currentSlot);
        if (valueSlot.HasValue)
            methodConvert.RemoveAnonymousVariable(valueSlot.Value);
        methodConvert.RemoveAnonymousVariable(builderSlot);
    }

    private static byte CaptureStringBuilderInstance(MethodConvert methodConvert, SemanticModel model, ExpressionSyntax? instanceExpression)
    {
        if (instanceExpression is null)
            throw new CompilationException(DiagnosticId.SyntaxNotSupported, "A StringBuilder instance is required for this operation.");
        methodConvert.ConvertExpression(model, instanceExpression);
        byte slot = methodConvert.AddAnonymousVariable();
        methodConvert.AccessSlot(OpCode.STLOC, slot);
        return slot;
    }

    private static void LoadStringBuilderContent(MethodConvert methodConvert, byte builderSlot)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, builderSlot);
        methodConvert.Push(0);
        methodConvert.PickItem();
    }

    private static void StoreStringBuilderContent(MethodConvert methodConvert, byte builderSlot, byte valueSlot)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, builderSlot);
        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.SetItem();
    }

    private static void EnsureStringValue(MethodConvert methodConvert)
    {
        JumpTarget notNull = new();
        methodConvert.Dup();
        methodConvert.Isnull();
        methodConvert.JumpIfFalse(notNull);
        methodConvert.Drop();
        methodConvert.Push("");
        notNull.Instruction = methodConvert.Nop();
    }

    private static byte ConvertStringBuilderArgument(MethodConvert methodConvert, SemanticModel model, ITypeSymbol parameterType, SyntaxNode argument)
    {
        byte slot;
        switch (parameterType.SpecialType)
        {
            case SpecialType.System_String:
                methodConvert.ConvertExpression(model, ExtractExpression(argument));
                EnsureStringValue(methodConvert);
                slot = methodConvert.AddAnonymousVariable();
                methodConvert.AccessSlot(OpCode.STLOC, slot);
                return slot;
            case SpecialType.System_Char:
                methodConvert.ConvertExpression(model, ExtractExpression(argument));
                methodConvert.ChangeType(StackItemType.ByteString);
                slot = methodConvert.AddAnonymousVariable();
                methodConvert.AccessSlot(OpCode.STLOC, slot);
                return slot;
            case SpecialType.System_Object:
                methodConvert.ConvertObjectToString(model, ExtractExpression(argument));
                EnsureStringValue(methodConvert);
                slot = methodConvert.AddAnonymousVariable();
                methodConvert.AccessSlot(OpCode.STLOC, slot);
                return slot;
            default:
                var typeName = parameterType.ToString();
                if (typeName == "System.Text.StringBuilder" || typeName == "System.Text.StringBuilder?")
                {
                    methodConvert.ConvertExpression(model, ExtractExpression(argument));
                    byte nestedSlot = methodConvert.AddAnonymousVariable();
                    methodConvert.AccessSlot(OpCode.STLOC, nestedSlot);
                    LoadStringBuilderContent(methodConvert, nestedSlot);
                    EnsureStringValue(methodConvert);
                    slot = methodConvert.AddAnonymousVariable();
                    methodConvert.AccessSlot(OpCode.STLOC, slot);
                    methodConvert.RemoveAnonymousVariable(nestedSlot);
                    return slot;
                }
                break;
        }

        throw new CompilationException(parameterType, DiagnosticId.SyntaxNotSupported, $"StringBuilder overload '{parameterType}' is not supported in Neo contracts.");
    }
}
