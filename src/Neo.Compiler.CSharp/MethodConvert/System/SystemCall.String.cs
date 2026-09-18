// Copyright (C) 2015-2026 The Neo Project.
//
// SystemCall.String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private enum SplitSeparatorKind
    {
        Char,
        String,
        CharArray,
        StringArray,
    }

    private bool TryProcessStringConstructor(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        if (!IsStringCharCountConstructor(symbol))
            return false;

        if (arguments.Count != 2)
            throw new CompilationException(symbol, DiagnosticId.InvalidArgument, "string(char, int) requires exactly two arguments.");

        var charExpression = GetConstructorArgument(arguments, symbol.Parameters[0], symbol);
        var countExpression = GetConstructorArgument(arguments, symbol.Parameters[1], symbol);
        var charConstant = model.GetConstantValue(charExpression);
        var countConstant = model.GetConstantValue(countExpression);

        ValidateConstantStringCharCountSize(countExpression, charConstant, countConstant);

        if (charConstant.HasValue && charConstant.Value is char character &&
            countConstant.HasValue && countConstant.Value is int repeatCount &&
            repeatCount >= 0)
        {
            Push(new string(NormalizeCharForUtf8(character), repeatCount));
            return true;
        }

        EmitStringCharCountConstructor(model, charExpression, countExpression, charConstant);
        return true;
    }

    private static bool IsStringCharCountConstructor(IMethodSymbol symbol)
    {
        return symbol.MethodKind == MethodKind.Constructor &&
               symbol.ContainingType.SpecialType == SpecialType.System_String &&
               symbol.Parameters.Length == 2 &&
               symbol.Parameters[0].Type.SpecialType == SpecialType.System_Char &&
               symbol.Parameters[1].Type.SpecialType == SpecialType.System_Int32;
    }

    private static ExpressionSyntax GetConstructorArgument(IReadOnlyList<ArgumentSyntax> arguments, IParameterSymbol parameter, IMethodSymbol symbol)
    {
        foreach (var argument in arguments)
        {
            if (argument.NameColon?.Name.Identifier.ValueText == parameter.Name)
                return argument.Expression;
        }

        if (parameter.Ordinal < arguments.Count && arguments[parameter.Ordinal].NameColon is null)
            return arguments[parameter.Ordinal].Expression;

        throw new CompilationException(symbol, DiagnosticId.InvalidArgument, $"string(char, int) requires an argument for '{parameter.Name}'.");
    }

    private static void ValidateConstantStringCharCountSize(ExpressionSyntax countExpression, Optional<object?> charConstant, Optional<object?> countConstant)
    {
        if (!countConstant.HasValue || countConstant.Value is not int repeatCount || repeatCount < 0)
            return;

        long maxItemSize = ExecutionEngineLimits.Default.MaxItemSize;
        long byteCount = repeatCount;
        if (charConstant.HasValue && charConstant.Value is char character)
            byteCount *= Encoding.UTF8.GetByteCount(NormalizeCharForUtf8(character).ToString());

        if (byteCount > maxItemSize)
            throw new CompilationException(countExpression, DiagnosticId.InvalidArgument, $"String byte length {byteCount} exceeds VM max item size {maxItemSize}.");
    }

    private void EmitStringCharCountConstructor(SemanticModel model, ExpressionSyntax charExpression, ExpressionSyntax countExpression, Optional<object?> charConstant)
    {
        byte charSlot = AddAnonymousVariable();
        byte countSlot = AddAnonymousVariable();
        byte resultSlot = AddAnonymousVariable();

        if (charConstant.HasValue && charConstant.Value is char character)
        {
            Push(NormalizeCharForUtf8(character).ToString());
        }
        else
        {
            ConvertExpression(model, charExpression);
            ConvertCharToUtf8();
        }
        AccessSlot(OpCode.STLOC, charSlot);

        ConvertExpression(model, countExpression);
        AccessSlot(OpCode.STLOC, countSlot);

        JumpTarget validCountTarget = new();
        AccessSlot(OpCode.LDLOC, countSlot);
        Push(0);
        Jump(OpCode.JMPGE_L, validCountTarget);
        Push("Negative count");
        AddInstruction(OpCode.THROW);

        validCountTarget.Instruction = Push("");
        AccessSlot(OpCode.STLOC, resultSlot);

        JumpTarget conditionTarget = new();
        JumpTarget endTarget = new();

        conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, countSlot);
        Push(0);
        Jump(OpCode.JMPLE_L, endTarget);

        AccessSlot(OpCode.LDLOC, resultSlot);
        AccessSlot(OpCode.LDLOC, charSlot);
        Cat();
        AccessSlot(OpCode.STLOC, resultSlot);

        AccessSlot(OpCode.LDLOC, countSlot);
        Dec();
        AccessSlot(OpCode.STLOC, countSlot);
        Jump(OpCode.JMP_L, conditionTarget);

        endTarget.Instruction = AccessSlot(OpCode.LDLOC, resultSlot);
        ChangeType(StackItemType.ByteString);

        RemoveAnonymousVariable(resultSlot);
        RemoveAnonymousVariable(countSlot);
        RemoveAnonymousVariable(charSlot);
    }

    private static void HandleStringPickItem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.PickItem();
    }

    private static void HandleStringLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Size();
    }

    private static void HandleStringContains(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (symbol.Parameters.Length == 1 && symbol.Parameters[0].Type.SpecialType == SpecialType.System_Char)
            NormalizeNulChar(methodConvert);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Push0();
        methodConvert.Ge();
    }

    /// <summary>
    /// Replaces a NUL character on top of the stack with a single 0x00 byte, so that the value
    /// keeps one byte of length instead of collapsing into an empty byte string.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    private static void NormalizeNulChar(MethodConvert methodConvert)
    {
        JumpTarget endTarget = new();

        methodConvert.Dup();
        methodConvert.JumpIfTrue(endTarget);
        methodConvert.Drop();
        methodConvert.Push(new byte[] { 0 });
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringStartsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Not();
    }

    private static void HandleStringIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    private static void HandleStringLastIndexOf(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte valueSlot = methodConvert.AddAnonymousVariable();
        byte strLenSlot = methodConvert.AddAnonymousVariable();
        byte valueLenSlot = methodConvert.AddAnonymousVariable();
        byte startSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, strLenSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.Size();
        methodConvert.Dup();
        methodConvert.AccessSlot(OpCode.STLOC, valueLenSlot);

        JumpTarget valueNotEmptyTarget = new();
        JumpTarget canSearchTarget = new();
        JumpTarget endTarget = new();

        methodConvert.JumpIfTrue(valueNotEmptyTarget); // NonZero int means true.
        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.JumpAlways(endTarget);
        valueNotEmptyTarget.Instruction = methodConvert.Nop();

        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, valueLenSlot);
        methodConvert.JumpIfGreaterOrEqual(canSearchTarget);
        methodConvert.Push(-1);
        methodConvert.JumpAlways(endTarget);
        canSearchTarget.Instruction = methodConvert.Nop();

        byte currentSlot = methodConvert.AddAnonymousVariable();
        byte nextSlot = methodConvert.AddAnonymousVariable();
        byte lastSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.AccessSlot(OpCode.STLOC, currentSlot);

        JumpTarget notFoundTarget = new();
        methodConvert.AccessSlot(OpCode.LDLOC, currentSlot);
        methodConvert.Push(-1);
        methodConvert.JumpIfEqual(notFoundTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, currentSlot);
        methodConvert.AccessSlot(OpCode.STLOC, lastSlot);

        JumpTarget loopStart = new();
        JumpTarget loopEnd = new();

        loopStart.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, currentSlot);
        methodConvert.Push(1);
        methodConvert.Add();
        methodConvert.AccessSlot(OpCode.STLOC, startSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 3, true);
        methodConvert.AccessSlot(OpCode.STLOC, nextSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, nextSlot);
        methodConvert.Push(-1);
        methodConvert.JumpIfEqual(loopEnd);

        methodConvert.AccessSlot(OpCode.LDLOC, nextSlot);
        methodConvert.AccessSlot(OpCode.STLOC, currentSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, currentSlot);
        methodConvert.AccessSlot(OpCode.STLOC, lastSlot);
        methodConvert.JumpAlways(loopStart);

        loopEnd.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, lastSlot);
        methodConvert.JumpAlways(endTarget);

        notFoundTarget.Instruction = methodConvert.Nop();
        methodConvert.Push(-1);
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringSplit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (symbol.Parameters.Length == 0)
            throw new CompilationException(DiagnosticId.SyntaxNotSupported, "string.Split() without separators is not supported.");

        var separatorParameterType = symbol.Parameters[0].Type;
        SplitSeparatorKind separatorKind = separatorParameterType switch
        {
            { SpecialType: SpecialType.System_Char } => SplitSeparatorKind.Char,
            { SpecialType: SpecialType.System_String } => SplitSeparatorKind.String,
            IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Char } => SplitSeparatorKind.CharArray,
            IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_String } => SplitSeparatorKind.StringArray,
            _ => throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported string.Split separator type '{separatorParameterType}'.")
        };

        var parameters = symbol.Parameters;
        int optionsParameterIndex = -1;
        int countParameterIndex = -1;
        for (int i = 1; i < parameters.Length; i++)
        {
            ITypeSymbol paramType = parameters[i].Type;
            if (paramType.SpecialType == SpecialType.System_Int32)
            {
                countParameterIndex = i;
                continue;
            }
            if (paramType.ToDisplayString() == "System.StringSplitOptions")
            {
                optionsParameterIndex = i;
                continue;
            }
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported parameter type '{paramType}' in string.Split overload.");
        }

        bool hasOptions = optionsParameterIndex >= 0;
        bool hasCount = countParameterIndex >= 0;

        bool substituteArraySeparator = separatorKind is SplitSeparatorKind.CharArray or SplitSeparatorKind.StringArray;
        char? arrayCharSeparator = null;
        string? arrayStringSeparator = null;

        if (substituteArraySeparator)
        {
            if (arguments is null)
                throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Unable to analyze separator array for string.Split.");

            var separatorArgument = arguments[0] as ArgumentSyntax
                ?? throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Unsupported argument syntax for string.Split separator.");

            if (!TryExtractSingleSeparator(model, separatorArgument.Expression, separatorKind, out arrayCharSeparator, out arrayStringSeparator, out var errorNode))
                throw new CompilationException(errorNode, DiagnosticId.SyntaxNotSupported, "Only single-element constant separator arrays are supported for string.Split.");

            separatorKind = separatorKind == SplitSeparatorKind.CharArray ? SplitSeparatorKind.Char : SplitSeparatorKind.String;
        }

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte separatorSlot = methodConvert.AddAnonymousVariable();
        byte optionsSlot = hasOptions ? methodConvert.AddAnonymousVariable() : (byte)0;
        byte countSlot = hasCount ? methodConvert.AddAnonymousVariable() : (byte)0;

        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        if (hasOptions)
        {
            methodConvert.AccessSlot(OpCode.STLOC, optionsSlot);
            if (hasCount)
                methodConvert.AccessSlot(OpCode.STLOC, countSlot);
        }
        methodConvert.AccessSlot(OpCode.STLOC, separatorSlot);

        if (substituteArraySeparator)
        {
            methodConvert.AccessSlot(OpCode.LDLOC, separatorSlot);
            methodConvert.Drop();

            if (arrayCharSeparator.HasValue)
                methodConvert.Push((ushort)arrayCharSeparator.Value);
            else
                methodConvert.Push(arrayStringSeparator!);

            methodConvert.AccessSlot(OpCode.STLOC, separatorSlot);
        }

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, separatorSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, separatorSlot);

        if (!hasOptions)
        {
            methodConvert.AccessSlot(OpCode.LDLOC, separatorSlot);
            methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
            methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "stringSplit", 2, true);
            return;
        }

        bool removeEmptyEntries = false;
        if (hasOptions && TryGetArgument(arguments, optionsParameterIndex, out var optionSyntax))
        {
            var constantValue = model.GetConstantValue(optionSyntax.Expression);
            if (!constantValue.HasValue)
                throw new CompilationException(optionSyntax, DiagnosticId.SyntaxNotSupported, "StringSplitOptions must be a compile-time constant.");

            int optionInt = constantValue.Value switch
            {
                int i => i,
                StringSplitOptions opt => (int)opt,
                _ => throw new CompilationException(optionSyntax, DiagnosticId.SyntaxNotSupported, "Unsupported StringSplitOptions value."),
            };

            var options = (StringSplitOptions)optionInt;
            if ((options & ~StringSplitOptions.RemoveEmptyEntries) != 0)
                throw new CompilationException(optionSyntax, DiagnosticId.SyntaxNotSupported, "Only StringSplitOptions.None and RemoveEmptyEntries are supported.");

            removeEmptyEntries = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
        }

        if (hasCount && TryGetArgument(arguments, countParameterIndex, out var countSyntax))
        {
            var countValue = model.GetConstantValue(countSyntax.Expression);
            if (!countValue.HasValue || countValue.Value is not int countInt || countInt != int.MaxValue)
                throw new CompilationException(countSyntax, DiagnosticId.SyntaxNotSupported, "Only the default count value is supported for string.Split.");
        }

        if (removeEmptyEntries)
            methodConvert.PushT();
        else
            methodConvert.PushF();

        methodConvert.AccessSlot(OpCode.LDLOC, separatorSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "stringSplit", 3, true);

        static bool TryGetArgument(IReadOnlyList<SyntaxNode>? args, int index, out ArgumentSyntax argument)
        {
            if (args is not null && index >= 0 && index < args.Count && args[index] is ArgumentSyntax argSyntax)
            {
                argument = argSyntax;
                return true;
            }
            argument = null!;
            return false;
        }

        static bool TryExtractSingleSeparator(SemanticModel semanticModel, ExpressionSyntax expression, SplitSeparatorKind kind, out char? charSeparator, out string? stringSeparator, out SyntaxNode errorNode)
        {
            charSeparator = null;
            stringSeparator = null;
            errorNode = expression;

            static SeparatedSyntaxList<ExpressionSyntax>? GetInitializerExpressions(ExpressionSyntax expr) =>
                expr switch
                {
                    ArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
                    ImplicitArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
                    InitializerExpressionSyntax initializer => initializer.Expressions,
                    _ => null
                };

            var initializerExpressions = GetInitializerExpressions(expression);
            if (initializerExpressions is null || initializerExpressions.Value.Count != 1)
                return false;

            var elementExpression = initializerExpressions.Value[0];
            var elementValue = semanticModel.GetConstantValue(elementExpression);
            if (!elementValue.HasValue)
            {
                errorNode = elementExpression;
                return false;
            }

            switch (kind)
            {
                case SplitSeparatorKind.CharArray:
                    if (elementValue.Value is char c)
                    {
                        charSeparator = c;
                        return true;
                    }
                    if (elementValue.Value is string s && s.Length == 1)
                    {
                        charSeparator = s[0];
                        return true;
                    }
                    errorNode = elementExpression;
                    return false;

                case SplitSeparatorKind.StringArray:
                    if (elementValue.Value is string strValue)
                    {
                        stringSeparator = strValue;
                        return true;
                    }
                    if (elementValue.Value is char charValue)
                    {
                        stringSeparator = charValue.ToString();
                        return true;
                    }
                    errorNode = elementExpression;
                    return false;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Handles the string.EndsWith method by checking if a string ends with a specified substring.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the end portion of the string with the target substring
    /// </remarks>
    private static void HandleStringEndsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        JumpTarget valueNonEmptyTarget = new();
        JumpTarget validCountTarget = new();
        JumpTarget endTarget = new();

        // After preparing the single argument and evaluating the instance,the stack is [value, source].
        methodConvert.Over();                                 // [value, source, value]
        methodConvert.Size();                                 // [value, source, valueSize]
        methodConvert.Dup();                                  // [value, source, valueSize, valueSize]
        methodConvert.JumpIfTrue(valueNonEmptyTarget);        // [value, source, valueSize]
        methodConvert.Drop(3);                                // []
        methodConvert.PushT();                                // Any string ends with an empty value.
        methodConvert.JumpAlways(endTarget);

        valueNonEmptyTarget.Instruction = methodConvert.Nop();
        methodConvert.Over();                                 // [value, source, valueSize, source]
        methodConvert.Size();                                 // [value, source, valueSize, sourceSize]
        methodConvert.Over();                                 // [value, source, valueSize, sourceSize, valueSize]
        methodConvert.JumpIfGreaterOrEqual(validCountTarget); // [value, source, valueSize]
        methodConvert.Drop(3);                                // []
        methodConvert.PushF();                                // source shorter than value.
        methodConvert.JumpAlways(endTarget);

        validCountTarget.Instruction = methodConvert.Right(null); // [value, suffix]
        methodConvert.ChangeType(StackItemType.ByteString);       // [value, suffix]
        methodConvert.Equal();                                    // [bool]

        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);        // Convert to ByteString
    }

    private static void HandleStringSubStringToEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Over();
        methodConvert.Size();
        methodConvert.Over();
        methodConvert.Sub();
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);        // Convert to ByteString
    }

    private static void HandleStringIsNullOrEmpty(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget endTarget = new();
        JumpTarget nullOrEmptyTarget = new();
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.JumpIfTrue(nullOrEmptyTarget);
        methodConvert.Size();
        methodConvert.Not();
        methodConvert.JumpAlways(endTarget);
        nullOrEmptyTarget.Instruction = methodConvert.Drop();
        methodConvert.PushT();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringIsNullOrWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        JumpTarget falseTarget = new();
        JumpTarget trueTarget = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte lengthSlot = methodConvert.AddAnonymousVariable();
        byte indexSlot = methodConvert.AddAnonymousVariable();

        methodConvert.Dup();
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.IsNull();
        methodConvert.JumpIfTrue(trueTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, lengthSlot);

        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);

        loopStart.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, lengthSlot);
        methodConvert.JumpIfGreaterOrEqual(trueTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.PickItem();
        CheckIsWhiteSpaceByte(methodConvert, falseTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.Inc();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);

        methodConvert.JumpAlways(loopStart);

        falseTarget.Instruction = methodConvert.PushF();
        methodConvert.JumpAlways(endTarget);

        trueTarget.Instruction = methodConvert.PushT();

        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleObjectEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Equal();
    }

    /// <summary>
    /// Handles string.Compare(strA, strB) using StdLib.MemoryCompare with null handling.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// - null == null =&gt; 0
    /// - null &lt; non-null =&gt; -1
    /// - non-null &gt; null =&gt; 1
    /// - otherwise uses StdLib.MemoryCompare for ordinal byte comparison
    /// </remarks>
    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        byte leftSlot = methodConvert.AddAnonymousVariable();
        byte rightSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, leftSlot);
        methodConvert.AccessSlot(OpCode.STLOC, rightSlot);

        JumpTarget leftNotNullTarget = new();
        JumpTarget rightNotNullWhenLeftNullTarget = new();
        JumpTarget bothNotNullTarget = new();
        JumpTarget endTarget = new();

        methodConvert.AccessSlot(OpCode.LDLOC, leftSlot);
        methodConvert.IsNull();
        methodConvert.JumpIfFalse(leftNotNullTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, rightSlot);
        methodConvert.IsNull();
        methodConvert.JumpIfFalse(rightNotNullWhenLeftNullTarget);
        methodConvert.Push(0);
        methodConvert.JumpAlways(endTarget);

        rightNotNullWhenLeftNullTarget.Instruction = methodConvert.Nop();
        methodConvert.Push(-1);
        methodConvert.JumpAlways(endTarget);

        leftNotNullTarget.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, rightSlot);
        methodConvert.IsNull();
        methodConvert.JumpIfFalse(bothNotNullTarget);
        methodConvert.Push(1);
        methodConvert.JumpAlways(endTarget);

        bothNotNullTarget.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, rightSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, leftSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memoryCompare", 2, true);

        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        JumpTarget trueTarget = new(), endTarget = new();
        methodConvert.JumpIfTrueLong(trueTarget);
        methodConvert.Push("False");
        methodConvert.JumpAlwaysLong(endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleCharToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is null)
            return;

        var constant = model.GetConstantValue(instanceExpression);
        if (constant.HasValue && constant.Value is char character)
        {
            methodConvert.Push(NormalizeCharForUtf8(character).ToString());
            return;
        }

        methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ConvertCharToUtf8();
    }

    private static char NormalizeCharForUtf8(char character)
    {
        return char.IsSurrogate(character) ? '\ufffd' : character;
    }

    /// <summary>
    /// Encodes one runtime UTF-16 code unit for Neo's UTF-8 string representation.
    /// Isolated surrogate code units cannot be represented and are replaced with U+FFFD.
    /// </summary>
    private void ConvertCharToUtf8()
    {
        byte charSlot = AddAnonymousVariable();
        AccessSlot(OpCode.STLOC, charSlot);

        JumpTarget nullTarget = new();
        JumpTarget asciiTarget = new();
        JumpTarget twoByteTarget = new();
        JumpTarget replacementTarget = new();
        JumpTarget endTarget = new();

        AccessSlot(OpCode.LDLOC, charSlot);
        Push0();
        JumpIfEqual(nullTarget);

        AccessSlot(OpCode.LDLOC, charSlot);
        Push(0x7f);
        JumpIfLessOrEqual(asciiTarget);

        AccessSlot(OpCode.LDLOC, charSlot);
        Push(0x7ff);
        JumpIfLessOrEqual(twoByteTarget);

        AccessSlot(OpCode.LDLOC, charSlot);
        Within(0xd800, 0xdfff);
        JumpIfTrue(replacementTarget);

        NewBuffer(3);
        SetUtf8BufferByte(charSlot, 0, shift: 12, mask: 0x0f, prefix: 0xe0);
        SetUtf8BufferByte(charSlot, 1, shift: 6, mask: 0x3f, prefix: 0x80);
        SetUtf8BufferByte(charSlot, 2, shift: 0, mask: 0x3f, prefix: 0x80);
        JumpAlways(endTarget);

        replacementTarget.Instruction = Push("\ufffd");
        JumpAlways(endTarget);

        twoByteTarget.Instruction = Nop();
        NewBuffer(2);
        SetUtf8BufferByte(charSlot, 0, shift: 6, mask: 0x1f, prefix: 0xc0);
        SetUtf8BufferByte(charSlot, 1, shift: 0, mask: 0x3f, prefix: 0x80);
        JumpAlways(endTarget);

        asciiTarget.Instruction = Nop();
        AccessSlot(OpCode.LDLOC, charSlot);
        JumpAlways(endTarget);

        nullTarget.Instruction = Nop();
        NewBuffer(1);
        SetUtf8BufferByte(charSlot, 0, shift: 0, mask: 0x7f, prefix: 0);

        endTarget.Instruction = ChangeType(StackItemType.ByteString);
        RemoveAnonymousVariable(charSlot);
    }

    private void SetUtf8BufferByte(byte charSlot, int index, int shift, int mask, int prefix)
    {
        Dup();
        Push(index);
        AccessSlot(OpCode.LDLOC, charSlot);
        if (shift != 0)
        {
            Push(shift);
            ShR();
        }
        Push(mask);
        And();
        if (prefix != 0)
        {
            Push(prefix);
            Or();
        }
        SetItem();
    }

    // Handler for object.ToString()
    private static void HandleObjectToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
        {
            ITypeSymbol? instanceType = model.GetTypeInfo(instanceExpression).Type;
            if (!IsSupportedObjectToStringType(instanceType))
            {
                string typeName = instanceType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "unknown";
                throw new CompilationException(instanceExpression, DiagnosticId.InvalidToStringType, $"ToString() is not supported for type '{typeName}'. Use StdLib.Serialize(...) for maps and arrays, or convert a supported primitive type.");
            }

            methodConvert.ConvertExpression(model, instanceExpression);
        }

        methodConvert.ChangeType(StackItemType.ByteString);
    }

    private static bool IsSupportedObjectToStringType(ITypeSymbol? type)
    {
        if (type?.SpecialType is SpecialType.System_String or SpecialType.System_Object)
            return true;

        return type?.ToString() switch
        {
            "char" or
            "Neo.SmartContract.Framework.ECPoint" or
            "Neo.SmartContract.Framework.ByteString" or
            "Neo.SmartContract.Framework.UInt160" or
            "Neo.SmartContract.Framework.UInt256" => true,
            _ => false
        };
    }

    // Handler for numeric types' ToString() methods
    private static void HandleToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
    }

    private static void HandleStringToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
    }

    /// <summary>
    /// Handles the string.Concat method by concatenating two strings with null handling.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Concatenates strings after converting null values to empty strings
    /// </remarks>
    private static void HandleStringConcat(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var firstNotNull = new JumpTarget();
        var secondNotNull = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate first string
        methodConvert.IsNull();                                    // Check if null
        methodConvert.JumpIfNot(firstNotNull);                     // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        firstNotNull.Instruction = methodConvert.Nop();            // First not null target
        methodConvert.Swap();                                      // Swap strings
        methodConvert.Dup();                                       // Duplicate second string
        methodConvert.IsNull();                                    // Check if null
        methodConvert.JumpIfNot(secondNotNull);                    // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        secondNotNull.Instruction = methodConvert.Nop();           // Second not null target
        methodConvert.Cat();                                       // Concatenate strings
        methodConvert.ChangeType(StackItemType.ByteString);        // The CAT result is a buffer, so add CONVERT opcode.
    }

    private static void HandleStringPadLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => EmitStringPad(methodConvert, model, symbol, instanceExpression, arguments, padLeft: true);

    private static void HandleStringPadRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => EmitStringPad(methodConvert, model, symbol, instanceExpression, arguments, padLeft: false);

    /// <summary>
    /// Handles string.PadLeft / string.PadRight by prepending or appending the padding
    /// character until the length reaches totalWidth. The string is returned unchanged when it
    /// is already at least totalWidth long (no truncation), matching .NET. The default padding
    /// character is a space (' ').
    /// </summary>
    /// <remarks>
    /// Width is measured in bytes, consistent with how the compiler treats string length
    /// (<see cref="OpCode.SIZE"/>); for ASCII text this matches .NET's character semantics.
    /// The padding character is an integer, so concatenating it emits its minimal little-endian
    /// bytes and its size is exactly that byte count. That allows the padding to be doubled and
    /// trimmed once instead of concatenating the character one byte group at a time.
    /// A NUL character is normalized to a single 0x00 byte so that it still pads one byte per repetition.
    /// </remarks>
    private static void EmitStringPad(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, bool padLeft)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, callingConvention: CallingConvention.StdCall);

        using var tempScope = methodConvert.PreserveAnonymousVariables();
        byte fillSlot = methodConvert.AddAnonymousVariable();
        byte padLengthSlot = methodConvert.AddAnonymousVariable();

        JumpTarget loopStart = new();
        JumpTarget loopEnd = new();
        JumpTarget noPaddingTarget = new();
        JumpTarget endTarget = new();

        // stack: [string, count, char]
        // Store the padding character (default is a space).
        if (arguments?.Count < 2) methodConvert.Push(' ');
        NormalizeNulChar(methodConvert);                           // '\0' -> a single 0x00 byte
        methodConvert.AccessSlot(OpCode.STLOC, fillSlot);          // stack: [string, count]

        // padLength = (totalWidth - size(str)) * size(fill), keeping the string on the stack.
        methodConvert.Over();                                      // stack: [string, count, string]
        methodConvert.Size();
        methodConvert.Sub();
        methodConvert.AccessSlot(OpCode.LDLOC, fillSlot);
        methodConvert.Size();
        methodConvert.Mul();                                       // stack: [string, padLength]
        methodConvert.Dup();
        methodConvert.Push0();
        methodConvert.JumpIfLessOrEqual(noPaddingTarget);          // Nothing to pad, return the string unchanged
        methodConvert.AccessSlot(OpCode.STLOC, padLengthSlot);     // stack: [string]

        // Double the fill until it covers padLength bytes, keeping the padding on the stack.
        methodConvert.AccessSlot(OpCode.LDLOC, fillSlot);          // stack: [string, pad]
        loopStart.Instruction = methodConvert.Nop();
        methodConvert.Dup();                                       // stack: [string, pad, pad]
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.LDLOC, padLengthSlot);
        methodConvert.JumpIfGreaterOrEqual(loopEnd);               // stack: [string, pad]
        methodConvert.Dup();
        methodConvert.Cat();
        methodConvert.JumpAlways(loopStart);                       // stack: [string, pad * 2]
        loopEnd.Instruction = methodConvert.Nop();

        // Trim the padding to the exact number of bytes: [string, padding].
        methodConvert.AccessSlot(OpCode.LDLOC, padLengthSlot);
        methodConvert.Left(null);

        // Combine: PadLeft -> padding + str ; PadRight -> str + padding.
        if (padLeft)
            methodConvert.Swap();                                  // stack: [padding, string]
        methodConvert.Cat();
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.JumpAlways(endTarget);

        // The string is already at least as wide as requested.
        noPaddingTarget.Instruction = methodConvert.Drop();        // stack: [string]
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToLower(methodConvert);
    }

    /// <summary>
    /// Converts ASCII uppercase bytes to lowercase while preserving all other UTF-8 bytes.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Copies the UTF-8 bytes into a mutable buffer, then rewrites only the ASCII uppercase bytes.
    /// </remarks>
    private static void ConvertToLower(MethodConvert methodConvert, bool preserveInput = false)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        byte sizeSlot = methodConvert.AddAnonymousVariable();
        byte indexSlot = methodConvert.AddAnonymousVariable();
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var skipTarget = new JumpTarget();

        if (preserveInput)
            methodConvert.Dup();                                   // stack: [string]; duplicate the input when it must be preserved
        methodConvert.Dup();                                       // stack: [string]; keep one copy to measure and convert
        methodConvert.Size();                                      // stack: [string, string]; get string length
        methodConvert.Dup();                                       // stack: [string, size]; store the size
        methodConvert.StLoc(sizeSlot);                             // stack: [string, size, size]; store the size
        methodConvert.Left(null);                                  // stack: [string, size]; copy the string into a mutable buffer
        methodConvert.Push0();                                     // stack: [buffer]; initialize index to 0
        methodConvert.StLoc(indexSlot);                            // stack: [buffer, 0]

        loopStart.Instruction = methodConvert.LdLoc(indexSlot);    // stack: [buffer]; loop start, load index
        methodConvert.LdLoc(sizeSlot);                             // stack: [buffer, index]; load size
        methodConvert.JumpIfGreaterOrEqual(loopEnd);               // stack: [buffer, index, size]; exit if done
        methodConvert.Dup();                                       // stack: [buffer]; duplicate the buffer to read one byte
        methodConvert.LdLoc(indexSlot);                            // stack: [buffer, buffer]; load index
        methodConvert.PickItem();                                  // stack: [buffer, buffer, index]; read the byte at index
        methodConvert.Within('A', 'Z');                            // stack: [buffer, byte]; is it ASCII uppercase?
        methodConvert.JumpIfNot(skipTarget);                       // stack: [buffer, isUpper]; skip when it is not uppercase
        methodConvert.Dup();                                       // stack: [buffer]; duplicate the buffer to keep it
        methodConvert.Dup();                                       // stack: [buffer, buffer]; duplicate again for SETITEM
        methodConvert.LdLoc(indexSlot);                            // stack: [buffer, buffer, buffer]; load index
        methodConvert.PickItem();                                  // stack: [buffer, buffer, buffer, index]; read the byte again
        methodConvert.Push(32);                                    // stack: [buffer, buffer, byte]; push 32 (uppercase to lowercase delta)
        methodConvert.Add();                                       // stack: [buffer, buffer, byte, 32]; byte + 32
        methodConvert.LdLoc(indexSlot);                            // stack: [buffer, buffer, lower]; load index
        methodConvert.Swap();                                      // stack: [buffer, buffer, lower, index]; order for SETITEM
        methodConvert.SetItem();                                   // stack: [buffer, buffer, index, lower]; buffer[index] = lower
        skipTarget.Instruction = methodConvert.LdLoc(indexSlot);   // stack: [buffer]; load index
        methodConvert.Inc();                                       // stack: [buffer, index]; next byte
        methodConvert.StLoc(indexSlot);                            // stack: [buffer, index + 1]
        methodConvert.JumpAlways(loopStart);                       // stack: [buffer]; continue loop

        loopEnd.Instruction = methodConvert.ChangeType(StackItemType.ByteString); // stack: [buffer]; convert the buffer to a ByteString
    }

    /// <summary>
    /// Handles string.ToUpper by converting ASCII lowercase bytes to uppercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through UTF-8 bytes and preserves bytes outside the ASCII lowercase range.
    /// </remarks>
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToUpper(methodConvert);
    }

    /// <summary>
    /// Converts ASCII lowercase bytes to uppercase while preserving all other UTF-8 bytes.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Iterates through UTF-8 bytes, converting only the ASCII lowercase range.
    /// </remarks>
    private static void ConvertToUpper(MethodConvert methodConvert, bool preserveInput = false)
    {
        byte strSlot = methodConvert.AddAnonymousVariable();
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        var appendAsciiByte = new JumpTarget();
        var copyOriginalByte = new JumpTarget();

        if (preserveInput)
            methodConvert.Dup();
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.Push("");                                    // Create empty result string

        methodConvert.Push0();                                     // Initialize index to 0
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);           // Load string
        methodConvert.Size();                                      // Get string length
        methodConvert.JumpIfGreaterOrEqual(loopEnd);               // Exit if done

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);           // Load string
        methodConvert.Swap();                                      // Swap for PickItem
        methodConvert.PickItem();                                  // Get character at index
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('a', 'z');                            // Check if lowercase
        methodConvert.JumpIfTrue(charIsLower);                     // Jump if lowercase
        methodConvert.Dup();                                       // Integer zero converts to an empty byte string
        methodConvert.Push0();
        methodConvert.JumpIfLessOrEqual(copyOriginalByte);
        methodConvert.Dup();                                       // Preserve the byte while checking whether it is ASCII
        methodConvert.Push(0x80);
        methodConvert.JumpIfLess(appendAsciiByte);
        copyOriginalByte.Instruction = methodConvert.Nop();        // Copy NUL and non-ASCII bytes from the original string
        AppendCurrentStringByte(methodConvert, strSlot);
        methodConvert.JumpAlways(loopStart);

        appendAsciiByte.Instruction = methodConvert.Nop();
        AppendStringByte(methodConvert);
        methodConvert.JumpAlways(loopStart);                       // Continue loop

        charIsLower.Instruction = methodConvert.Nop();             // Lowercase processing
        methodConvert.Push(32);                                    // Push 32 (difference between uppercase and lowercase for ASCII)
        methodConvert.Sub();                                       // Subtract 32 to get uppercase for ASCII
        AppendStringByte(methodConvert);
        methodConvert.JumpAlways(loopStart);                       // Continue loop

        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Drop index
        methodConvert.ChangeType(StackItemType.ByteString);        // Convert to ByteString
        methodConvert.RemoveAnonymousVariable(strSlot);
    }

    private static void AppendCurrentStringByte(MethodConvert methodConvert, byte strSlot)
    {
        methodConvert.Drop();
        methodConvert.Dup();
        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Swap();
        methodConvert.Push1();
        methodConvert.SubStr();
        AppendStringByte(methodConvert);
    }

    private static void AppendStringByte(MethodConvert methodConvert)
    {
        // stack: [result, index, char]
        methodConvert.Rot();   // [index, char, result]
        methodConvert.Swap();  // [index, result, char]
        methodConvert.Cat();   // [index, result']
        methodConvert.Swap();  // [result', index]
        methodConvert.Inc();   // [result', index+1]
    }

    /// <summary>
    /// Initializes the string length variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="stringIndex">Variable to store string</param>
    /// <param name="lengthIndex">Variable to store string length</param>
    /// <remarks>
    /// Algorithm: Gets string size and stores it in local variable
    /// </remarks>
    private static void InitStringLength(MethodConvert methodConvert, byte stringIndex, byte lengthIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);       // Get string
        methodConvert.Size();                                      // Get string size
        methodConvert.AccessSlot(OpCode.STLOC, lengthIndex);       // Store in local variable
    }

    /// <summary>
    /// Gets the string length from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="lengthIndex">Variable containing string length</param>
    /// <returns>String length on stack</returns>
    private static void GetStringLength(MethodConvert methodConvert, byte lengthIndex)
        => methodConvert.AccessSlot(OpCode.LDLOC, lengthIndex);

    /// <summary>
    /// Initializes the start index variable to 0.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable to store start index</param>
    /// <remarks>
    /// Algorithm: Sets start index to 0 for trimming operations
    /// </remarks>
    private static void InitStartIndex(MethodConvert methodConvert, byte startIndex)
    {
        methodConvert.Push(0);                                     // Push initial value 0
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);        // Store in local variable
    }

    /// <summary>
    /// Initializes the end index variable to string length - 1.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable to store end index</param>
    /// <param name="lengthIndex">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Sets end index to last character position
    /// </remarks>
    private static void InitEndIndex(MethodConvert methodConvert, byte endIndex, byte lengthIndex)
    {
        GetStringLength(methodConvert, lengthIndex);               // Get string length
        methodConvert.Dec();                                       // Subtract 1 for last index
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);          // Store in local variable
    }

    /// <summary>
    /// Gets the end index from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <returns>End index on stack</returns>
    private static void GetEndIndex(MethodConvert methodConvert, byte endIndex) => methodConvert.AccessSlot(OpCode.LDLOC, endIndex);


    /// <summary>
    /// Gets the start index from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <returns>Start index on stack</returns>
    private static void GetStartIndex(MethodConvert methodConvert, byte startIndex) => methodConvert.AccessSlot(OpCode.LDLOC, startIndex);

    /// <summary>
    /// Checks if start index is less than string length.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <param name="lengthIndex">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Exits loop if start index >= string length
    /// </remarks>
    private static void CheckStartIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte startIndex, byte lengthIndex)
    {
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        GetStringLength(methodConvert, lengthIndex);                    // Get string length
        methodConvert.JumpIfGreaterOrEqual(loopEnd);               //  Check if index < length, Exit if not less than
    }

    /// <summary>
    /// Increments start index and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Increments start index and continues loop
    /// </remarks>
    private static void MoveStartIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte startIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, startIndex);        // Load start index
        methodConvert.Inc();                                       // Increment by 1
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);        // Store back
        methodConvert.JumpAlways(loopStart);                       // Continue loop
    }

    /// <summary>
    /// Advances the start index by the trim character byte length and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="startIndex">Variable containing the start index</param>
    /// <param name="trimLenIndex">Variable containing the trim character UTF-8 byte length</param>
    /// <remarks>
    /// Used by the UTF-8 aware char trim loop so a multi-byte character is skipped as one unit.
    /// </remarks>
    private static void MoveStartIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte startIndex, byte trimLenIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, startIndex);        // Load start index
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);      // Load trim length
        methodConvert.Add();                                       // startIndex += trimLen
        methodConvert.AccessSlot(OpCode.STLOC, startIndex);        // Store back
        methodConvert.JumpAlways(loopStart);                       // Continue loop
    }

    /// <summary>
    /// Picks character at the start index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="stringIndex">Variable to store string</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current start position
    /// </remarks>
    private static void PickCharStart(MethodConvert methodConvert, byte stringIndex, byte startIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);       // Get string
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        methodConvert.PickItem();                                  // Get character at index
    }

    /// <summary>
    /// Decrements end index and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <remarks>
    /// Algorithm: Decrements end index and continues loop
    /// </remarks>
    private static void MoveEndIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte endIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, endIndex);          // Load end index
        methodConvert.Dec();                                       // Decrement by 1
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);          // Store back
        methodConvert.JumpAlways(loopStart);                 // Continue loop
    }

    /// <summary>
    /// Moves the end index back by the trim character byte length and loops back.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopStart">Jump target for loop start</param>
    /// <param name="endIndex">Variable containing the end index</param>
    /// <param name="trimLenIndex">Variable containing the trim character UTF-8 byte length</param>
    /// <remarks>
    /// Used by the UTF-8 aware char trim loop so a multi-byte character is skipped as one unit.
    /// </remarks>
    private static void MoveEndIndexAndLoop(MethodConvert methodConvert, JumpTarget loopStart, byte endIndex, byte trimLenIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, endIndex);          // Load end index
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);      // Load trim length
        methodConvert.Sub();                                       // endIndex -= trimLen
        methodConvert.AccessSlot(OpCode.STLOC, endIndex);          // Store back
        methodConvert.JumpAlways(loopStart);                       // Continue loop
    }

    /// <summary>
    /// Mask of the whitespace bytes: '\t' (0x09) to '\r' (0x0D) and ' ' (0x20).
    /// </summary>
    private const long WhiteSpaceByteMask = (1L << 9) | (1L << 10) | (1L << 11) | (1L << 12) | (1L << 13) | (1L << 32);

    /// <summary>
    /// Checks if the byte on the top of the stack is an ASCII whitespace byte and exits loop if not.
    /// The top stack item should be a byte picked from a string, so it is always in the range [0, 255].
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Shifts <see cref="WhiteSpaceByteMask"/> right by the byte value, the lowest bit of the
    /// result is set only when the byte is a whitespace byte. The shift amount is at most 255, which is
    /// within <see cref="ExecutionEngineLimits.MaxShift"/>.
    /// </remarks>
    private static void CheckIsWhiteSpaceByte(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.Push(WhiteSpaceByteMask);                    // Mask of the whitespace bytes
        methodConvert.Swap();                                      // Put the byte on the top as the shift amount
        methodConvert.ShR();                                       // Move the bit of the byte value to the lowest bit
        methodConvert.Push(1);                                     // Test the lowest bit
        methodConvert.And();                                       // 1 if the byte is whitespace
        methodConvert.JumpIfFalse(loopEnd);                        // Exit if not whitespace
    }

    /// <summary>
    /// Checks if character equals the trim character and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Compares character with the specified trim character
    /// </remarks>
    private static void CheckTrimChar(MethodConvert methodConvert, JumpTarget loopEnd, byte trimCharIndex, char? constantTrimChar)
    {
        if (constantTrimChar.HasValue)
            methodConvert.Push((ushort)constantTrimChar.Value);
        else
            methodConvert.LdLoc(trimCharIndex);             // Load trim character
        methodConvert.JumpIfNotEqual(loopEnd);              // Exit if not equal
    }

    /// <summary>
    /// Stores the UTF-8 byte sequence for the trim character into a slot.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="constantTrimChar">The constant trim character, if any</param>
    /// <param name="trimBytesIndex">Output slot that holds the UTF-8 byte sequence</param>
    /// <param name="trimLenIndex">Output slot that holds the UTF-8 byte length</param>
    /// <remarks>
    /// For a constant character the sequence is emitted directly at compile time. For a runtime
    /// character it is produced with <see cref="ConvertCharToUtf8"/> and measured with <c>SIZE</c>.
    /// </remarks>
    private static void EmitTrimCharUtf8(MethodConvert methodConvert, char? constantTrimChar, out byte trimBytesIndex, out byte trimLenIndex)
    {
        trimBytesIndex = methodConvert.AddAnonymousVariable();
        trimLenIndex = methodConvert.AddAnonymousVariable();

        if (constantTrimChar is { } character)
        {
            methodConvert.Drop();                       // Remove the argument from the evaluation stack
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(NormalizeCharForUtf8(character).ToString());
            methodConvert.Push(bytes);
            methodConvert.AccessSlot(OpCode.STLOC, trimBytesIndex);
            methodConvert.Push(bytes.Length);
            methodConvert.AccessSlot(OpCode.STLOC, trimLenIndex);
        }
        else
        {
            methodConvert.ConvertCharToUtf8();
            methodConvert.AccessSlot(OpCode.STLOC, trimBytesIndex);
            methodConvert.AccessSlot(OpCode.LDLOC, trimBytesIndex);
            methodConvert.Size();
            methodConvert.AccessSlot(OpCode.STLOC, trimLenIndex);
        }
    }

    /// <summary>
    /// Checks whether the character at the leading boundary equals the trim character encoded as UTF-8.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="stringIndex">Slot holding the string</param>
    /// <param name="indexIndex">Slot holding the byte index of the leading boundary</param>
    /// <param name="trimBytesIndex">Slot holding the trim character UTF-8 byte sequence</param>
    /// <param name="trimLenIndex">Slot holding the trim character UTF-8 byte length</param>
    /// <param name="lengthIndex">Slot holding the string byte length</param>
    /// <remarks>
    /// Loads the string, the index and the UTF-8 length, extracts the boundary slice with
    /// <c>SUBSTR</c>, and compares it with the trim character bytes. Exits the loop when there is
    /// not enough room or the slice does not equal the trim character.
    /// </remarks>
    private static void CheckTrimCharUtf8Start(MethodConvert methodConvert, JumpTarget loopEnd, byte stringIndex, byte indexIndex, byte trimBytesIndex, byte trimLenIndex, byte lengthIndex)
    {
        // If index + trimLen > length there is not enough room for the character, stop trimming.
        GetStartIndex(methodConvert, indexIndex);
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);
        methodConvert.Add();
        GetStringLength(methodConvert, lengthIndex);
        methodConvert.JumpIfGreater(loopEnd);

        // Extract string[index .. index + trimLen) and compare with the trim bytes.
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);
        GetStartIndex(methodConvert, indexIndex);
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);   // SUBSTR returns a Buffer; convert before Equal
        methodConvert.AccessSlot(OpCode.LDLOC, trimBytesIndex);
        methodConvert.Equal();
        methodConvert.JumpIfFalse(loopEnd);
    }

    /// <summary>
    /// Checks whether the character at the trailing boundary equals the trim character encoded as UTF-8.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="stringIndex">Slot holding the string</param>
    /// <param name="endIndex">Slot holding the byte index of the trailing boundary</param>
    /// <param name="trimBytesIndex">Slot holding the trim character UTF-8 byte sequence</param>
    /// <param name="trimLenIndex">Slot holding the trim character UTF-8 byte length</param>
    /// <param name="startIndex">Slot holding the leading boundary (0 for a one-sided trailing trim)</param>
    /// <remarks>
    /// Loads the string and the trailing boundary, extracts the last <c>trimLen</c> bytes with
    /// <c>SUBSTR</c>, and compares them with the trim character bytes. Exits the loop when there is
    /// not enough room (the boundary slice would start below the leading boundary) or the slice does
    /// not equal the trim character.
    /// </remarks>
    private static void CheckTrimCharUtf8End(MethodConvert methodConvert, JumpTarget loopEnd, byte stringIndex, byte endIndex, byte trimBytesIndex, byte trimLenIndex, byte startIndex)
    {
        // If end - trimLen + 1 < startIndex there is not enough room for the character, stop trimming.
        GetEndIndex(methodConvert, endIndex);
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);
        methodConvert.Sub();
        methodConvert.Push1();
        methodConvert.Add();
        GetStartIndex(methodConvert, startIndex);
        methodConvert.JumpIfLess(loopEnd);

        // Extract string[end - trimLen + 1 .. end + 1) and compare with the trim bytes.
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);
        GetEndIndex(methodConvert, endIndex);
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);
        methodConvert.Sub();
        methodConvert.Push1();
        methodConvert.Add();
        methodConvert.AccessSlot(OpCode.LDLOC, trimLenIndex);
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);   // SUBSTR returns a Buffer; convert before Equal
        methodConvert.AccessSlot(OpCode.LDLOC, trimBytesIndex);
        methodConvert.Equal();
        methodConvert.JumpIfFalse(loopEnd);
    }

    /// <summary>
    /// Checks if end index is greater than start index.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Exits loop if end index <= start index
    /// </remarks>
    private static void CheckEndIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte endIndex, byte startIndex)
    {
        GetEndIndex(methodConvert, endIndex);                      // Get end index
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        methodConvert.JumpIfLessOrEqual(loopEnd);                  // Check if end > start, Exit if not greater
    }

    /// <summary>
    /// Checks if end index is non-negative and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <param name="endIndex">Variable containing end index</param>
    private static void CheckEndIndexNonNegative(MethodConvert methodConvert, JumpTarget loopEnd, byte endIndex)
    {
        GetEndIndex(methodConvert, endIndex);
        methodConvert.Push(-1);
        methodConvert.JumpIfLessOrEqual(loopEnd);                  // Check if end > start, Exit if not greater
    }

    /// <summary>
    /// Picks character at the end index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="stringIndex">Variable to store string</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current end position
    /// </remarks>
    private static void PickCharEnd(MethodConvert methodConvert, byte stringIndex, byte endIndex)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);       // Get string
        GetEndIndex(methodConvert, endIndex);                      // Get end index
        methodConvert.PickItem();                                  // Get character at index
    }

    /// <summary>
    /// Handles the string.Trim method by removing leading and trailing whitespace.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Finds first and last non-whitespace characters, then extracts substring
    /// </remarks>
    private static void HandleStringTrim(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        var stringIndex = methodConvert.AddAnonymousVariable();
        var lengthIndex = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, stringIndex);       // Store string
        InitStringLength(methodConvert, stringIndex, lengthIndex); // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, lengthIndex);        // endIndex = string.Length - 1

        // Loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();                       // Loop start marker
        CheckStartIndex(methodConvert, loopEnd, startIndex, lengthIndex);
        PickCharStart(methodConvert, stringIndex, startIndex);             // Pick character to check
        CheckIsWhiteSpaceByte(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker

        // Process trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();                 // Second loop start
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, stringIndex, endIndex);            // Pick character to check
        CheckIsWhiteSpaceByte(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();                // Second loop end

        // Extract the trimmed substring
        methodConvert.AccessSlot(OpCode.LDLOC, stringIndex);       // Load string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
        methodConvert.ChangeType(StackItemType.ByteString);        // The SUBSTR result is a buffer, so add CONVERT opcode.
    }

    /// <summary>
    /// Handles the string.Trim(char) method by removing leading and trailing specified characters.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Finds first and last characters that don't match the trim character
    /// </remarks>
    private static void HandleStringTrimChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => HandleStringTrimCharInternal(methodConvert, model, symbol, instanceExpression, arguments, null);

    private static void HandleStringTrimCharArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null || arguments.Count == 0 || arguments[0] is not ArgumentSyntax separatorArgument)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Unsupported trim array usage.");

        if (!TryGetSingleCharFromArray(model, separatorArgument.Expression, out var trimChar, out var errorNode))
            throw new CompilationException(errorNode, DiagnosticId.SyntaxNotSupported, "Only single-element constant trim arrays are supported for string trim operations.");

        HandleStringTrimCharInternal(methodConvert, model, symbol, instanceExpression, arguments, trimChar);
    }

    private static void HandleStringTrimCharInternal(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, char? constantTrimChar)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        var stringIndex = methodConvert.AddAnonymousVariable();
        var lengthIndex = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();
        methodConvert.StLoc(stringIndex);      // Store string

        EmitTrimCharUtf8(methodConvert, constantTrimChar, out byte trimBytesIndex, out byte trimLenIndex);

        InitStringLength(methodConvert, stringIndex, lengthIndex); // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, lengthIndex);        // endIndex = string.Length - 1


        // Loop to trim leading characters
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();                        // Loop start marker
        CheckTrimCharUtf8Start(methodConvert, loopEnd, stringIndex, startIndex, trimBytesIndex, trimLenIndex, lengthIndex);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex, trimLenIndex);
        loopEnd.Instruction = methodConvert.Nop();                           // Loop end marker

        // Process trailing characters
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();                  // Second loop start
        CheckTrimCharUtf8End(methodConvert, loopEnd2, stringIndex, endIndex, trimBytesIndex, trimLenIndex, startIndex);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex, trimLenIndex);
        loopEnd2.Instruction = methodConvert.Nop();                    // Second loop end

        // Extract the trimmed substring
        methodConvert.LdLoc(stringIndex);                          // Load string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
        methodConvert.ChangeType(StackItemType.ByteString);        // The SUBSTR result is a buffer, so add CONVERT opcode.
    }

    private static void HandleStringTrimStart(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => HandleStringTrimStartInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: false, constantTrimChar: null);

    private static void HandleStringTrimStartChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => HandleStringTrimStartInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: true, constantTrimChar: null);

    private static void HandleStringTrimStartCharArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null || arguments.Count == 0 || arguments[0] is not ArgumentSyntax trimArgument)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Unsupported trim array usage.");

        if (!TryGetSingleCharFromArray(model, trimArgument.Expression, out var trimChar, out var errorNode))
            throw new CompilationException(errorNode, DiagnosticId.SyntaxNotSupported, "Only single-element constant trim arrays are supported for string trim operations.");

        HandleStringTrimStartInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: true, constantTrimChar: trimChar);
    }

    private static void HandleStringTrimStartInternal(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, bool useTrimChar, char? constantTrimChar)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        var stringIndex = methodConvert.AddAnonymousVariable();
        var lengthIndex = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        methodConvert.StLoc(stringIndex);       // Store string

        byte trimBytesIndex = 0;
        byte trimLenIndex = 0;
        if (useTrimChar)
            EmitTrimCharUtf8(methodConvert, constantTrimChar, out trimBytesIndex, out trimLenIndex);

        InitStringLength(methodConvert, stringIndex, lengthIndex);
        InitStartIndex(methodConvert, startIndex);

        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        if (useTrimChar)
            CheckTrimCharUtf8Start(methodConvert, loopEnd, stringIndex, startIndex, trimBytesIndex, trimLenIndex, lengthIndex);
        else
            CheckStartIndex(methodConvert, loopEnd, startIndex, lengthIndex);
        if (!useTrimChar)
            PickCharStart(methodConvert, stringIndex, startIndex);
        if (!useTrimChar)
            CheckIsWhiteSpaceByte(methodConvert, loopEnd);
        if (useTrimChar)
            MoveStartIndexAndLoop(methodConvert, loopStart, startIndex, trimLenIndex);
        else
            MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();

        methodConvert.LdLoc(stringIndex);    // Load string
        GetStartIndex(methodConvert, startIndex);
        GetStringLength(methodConvert, lengthIndex);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.Sub();
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString); // The SUBSTR result is a buffer, so add CONVERT opcode.
    }

    private static void HandleStringTrimEnd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => HandleStringTrimEndInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: false, constantTrimChar: null);

    private static void HandleStringTrimEndChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        => HandleStringTrimEndInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: true, constantTrimChar: null);

    private static void HandleStringTrimEndCharArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null || arguments.Count == 0 || arguments[0] is not ArgumentSyntax trimArgument)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Unsupported trim array usage.");

        if (!TryGetSingleCharFromArray(model, trimArgument.Expression, out var trimChar, out var errorNode))
            throw new CompilationException(errorNode, DiagnosticId.SyntaxNotSupported, "Only single-element constant trim arrays are supported for string trim operations.");

        HandleStringTrimEndInternal(methodConvert, model, symbol, instanceExpression, arguments, useTrimChar: true, constantTrimChar: trimChar);
    }

    private static void HandleStringTrimEndInternal(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, bool useTrimChar, char? constantTrimChar)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        var stringIndex = methodConvert.AddAnonymousVariable();
        var lengthIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        methodConvert.StLoc(stringIndex);       // Store string

        byte trimBytesIndex = 0;
        byte trimLenIndex = 0;
        if (useTrimChar)
        {
            EmitTrimCharUtf8(methodConvert, constantTrimChar, out trimBytesIndex, out trimLenIndex);
            methodConvert.Push0();                               // Leading boundary is fixed at 0
            methodConvert.AccessSlot(OpCode.STLOC, startIndex);
        }
        InitStringLength(methodConvert, stringIndex, lengthIndex);
        InitEndIndex(methodConvert, endIndex, lengthIndex);

        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        if (useTrimChar)
            CheckTrimCharUtf8End(methodConvert, loopEnd, stringIndex, endIndex, trimBytesIndex, trimLenIndex, startIndex);
        else
            CheckEndIndexNonNegative(methodConvert, loopEnd, endIndex);
        if (!useTrimChar)
            PickCharEnd(methodConvert, stringIndex, endIndex);
        if (!useTrimChar)
            CheckIsWhiteSpaceByte(methodConvert, loopEnd);
        if (useTrimChar)
            MoveEndIndexAndLoop(methodConvert, loopStart, endIndex, trimLenIndex);
        else
            MoveEndIndexAndLoop(methodConvert, loopStart, endIndex);
        loopEnd.Instruction = methodConvert.Nop();

        JumpTarget allTrimmed = new();
        JumpTarget endTarget = new();

        GetEndIndex(methodConvert, endIndex);
        methodConvert.Push(-1);
        methodConvert.JumpIfEqual(allTrimmed);

        methodConvert.LdLoc(stringIndex);    // Load string
        methodConvert.Push0();
        GetEndIndex(methodConvert, endIndex);
        methodConvert.Inc();
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString); // The SUBSTR result is a buffer, so add CONVERT opcode.
        methodConvert.JumpAlways(endTarget);

        allTrimmed.Instruction = methodConvert.Nop();
        methodConvert.Push("");
        endTarget.Instruction = methodConvert.Nop();
    }

    private static bool TryGetSingleCharFromArray(SemanticModel model, ExpressionSyntax expression, out char value, out SyntaxNode errorNode)
    {
        errorNode = expression;
        value = default;

        static SeparatedSyntaxList<ExpressionSyntax>? GetInitializerExpressions(ExpressionSyntax expr) =>
            expr switch
            {
                ArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
                ImplicitArrayCreationExpressionSyntax { Initializer: { } initializer } => initializer.Expressions,
                InitializerExpressionSyntax initializer => initializer.Expressions,
                _ => null
            };

        var initializerExpressions = GetInitializerExpressions(expression);
        if (initializerExpressions is null || initializerExpressions.Value.Count != 1)
            return false;

        var elementExpression = initializerExpressions.Value[0];
        errorNode = elementExpression;

        var constantValue = model.GetConstantValue(elementExpression);
        if (!constantValue.HasValue)
            return false;

        switch (constantValue.Value)
        {
            case char c:
                value = c;
                return true;
            case string s when s.Length == 1:
                value = s[0];
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Handles the string.Replace method by replacing all occurrences of a substring.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iteratively finds and replaces all occurrences of the search string
    /// </remarks>
    private static void HandleStringReplace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        // Build the result by scanning the remaining tail of the string for the search
        // value, appending the gap before each match plus the replacement, then continuing
        // from just past the match. Scanning the tail (rather than the freshly built result)
        // both terminates and avoids re-matching inside newValue when it contains oldValue.
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Stack (top-down) after the calls above: instance, oldValue, newValue.
        byte restSlot = methodConvert.AddAnonymousVariable();
        byte oldSlot = methodConvert.AddAnonymousVariable();
        byte newSlot = methodConvert.AddAnonymousVariable();
        byte resultSlot = methodConvert.AddAnonymousVariable();
        byte oldLenSlot = methodConvert.AddAnonymousVariable();
        byte idxSlot = methodConvert.AddAnonymousVariable();
        byte consumedSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, restSlot);          // instance
        methodConvert.AccessSlot(OpCode.STLOC, oldSlot);           // oldValue
        methodConvert.AccessSlot(OpCode.STLOC, newSlot);           // newValue

        // Normalize all three operands to ByteString.
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, restSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, oldSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, oldSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, newSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, newSlot);

        // result = ""
        methodConvert.Push("");
        methodConvert.AccessSlot(OpCode.STLOC, resultSlot);

        // oldLen = oldValue.Length; C# throws on an empty oldValue, so fault here too
        // (ASSERT faults on a zero length) instead of looping forever.
        methodConvert.AccessSlot(OpCode.LDLOC, oldSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, oldLenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, oldLenSlot);
        methodConvert.Assert();

        var loopStart = new JumpTarget();
        var notFound = new JumpTarget();

        loopStart.Instruction = methodConvert.Nop();

        // idx = StdLib.memorySearch(rest, oldValue)
        // memorySearch takes the haystack (mem) on top of the needle (value).
        methodConvert.AccessSlot(OpCode.LDLOC, oldSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.AccessSlot(OpCode.STLOC, idxSlot);

        // if (idx == -1) goto notFound
        methodConvert.AccessSlot(OpCode.LDLOC, idxSlot);
        methodConvert.PushM1();
        methodConvert.JumpIfEqual(notFound);

        // result = result + rest[0..idx] + newValue
        methodConvert.AccessSlot(OpCode.LDLOC, resultSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.LDLOC, idxSlot);
        methodConvert.SubStr();                                    // rest[0..idx]
        methodConvert.Cat();
        methodConvert.AccessSlot(OpCode.LDLOC, newSlot);
        methodConvert.Cat();
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, resultSlot);

        // consumed = idx + oldLen
        methodConvert.AccessSlot(OpCode.LDLOC, idxSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, oldLenSlot);
        methodConvert.Add();
        methodConvert.AccessSlot(OpCode.STLOC, consumedSlot);

        // rest = rest[consumed .. end]
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, consumedSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.LDLOC, consumedSlot);
        methodConvert.Sub();                                       // length = Size(rest) - consumed
        methodConvert.SubStr();
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, restSlot);

        methodConvert.JumpAlways(loopStart);

        // No further match: append the remaining tail and return.
        notFound.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, resultSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, restSlot);
        methodConvert.Cat();
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    private static void HandleStringRemove(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        bool hasCount = symbol.Parameters.Length == 2;

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, callingConvention: CallingConvention.StdCall);

        if (!hasCount) // [string, start]
        {
            methodConvert.Left(null);
            methodConvert.ChangeType(StackItemType.ByteString);
            return;
        }

        // stack: [string, start, count]
        byte startSlot = methodConvert.AddAnonymousVariable();
        byte countSlot = methodConvert.AddAnonymousVariable();
        byte strSlot = methodConvert.AddAnonymousVariable();
        methodConvert.AccessSlot(OpCode.STLOC, countSlot);
        methodConvert.AccessSlot(OpCode.STLOC, startSlot);

        methodConvert.Dup();                               // [string, string]
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot); // [string, string, start]
        methodConvert.Left(null);                          // [string, prefix]

        methodConvert.Swap();                              // [prefix, string]
        methodConvert.Dup();                               // [prefix, string, string]
        methodConvert.Size();                              // [prefix, string, size]
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot); // [prefix, string, size, start]
        methodConvert.Sub();                               // [prefix, string, remain]
        methodConvert.AccessSlot(OpCode.LDLOC, countSlot); // [prefix, string, remain, count]
        methodConvert.Sub();                               // [prefix, string, remain']

        methodConvert.Right(null);                         // [prefix, suffix]
        methodConvert.Cat();                               // [result]
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    private static void HandleStringInsert(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte startSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.AccessSlot(OpCode.STLOC, startSlot);

        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.Not();
        methodConvert.Assert();

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.Left(null);
        methodConvert.Swap();
        methodConvert.Cat();

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Dup();
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.Sub();
        methodConvert.Right(null);
        methodConvert.Cat();
        methodConvert.ChangeType(StackItemType.ByteString); // The CAT result is a buffer, so add CONVERT opcode.
    }

    /// <summary>
    /// Handles the string.IndexOf(char) method by finding the index of a character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses StdLib memorySearch to find the character position
    /// </remarks>
    private static void HandleStringIndexOfChar(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        NormalizeNulChar(methodConvert);

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }

    /// <summary>
    /// Handles the string.LastIndexOf(char) method by finding the last index of a character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses StdLib memorySearch with backward = true, starting from the end of the
    /// string, to find the last occurrence of the character.
    /// </remarks>
    private static void HandleStringLastIndexOfChar(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        // The native memorySearch(mem, value, start, backward) takes its parameters back to
        // front, so the stack is built as [backward, start, value, mem].
        methodConvert.Push(true);                                    // [true]
        methodConvert.ConvertExpression(model, instanceExpression!); // [true, string]
        methodConvert.Dup();                                         // [true, string, string]
        methodConvert.Size();                                        // [true, string, size]

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments); // [true, string, size, char]
        methodConvert.ConvertCharToUtf8();

        methodConvert.Rot();                                                   // [true, size, char, string]
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 4, true);
    }
}
