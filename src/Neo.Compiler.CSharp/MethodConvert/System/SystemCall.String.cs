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
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Push0();
        methodConvert.Ge();
    }

    private static void HandleStringStartsWith(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Push(0);
        methodConvert.NumEqual();
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
        methodConvert.AccessSlot(OpCode.STLOC, valueLenSlot);

        JumpTarget valueNotEmptyTarget = new();
        JumpTarget canSearchTarget = new();
        JumpTarget endTarget = new();

        methodConvert.AccessSlot(OpCode.LDLOC, valueLenSlot);
        methodConvert.Push(0);
        methodConvert.NumEqual();
        methodConvert.JumpIfFalse(valueNotEmptyTarget);
        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.JumpAlways(endTarget);
        valueNotEmptyTarget.Instruction = methodConvert.Nop();

        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, valueLenSlot);
        methodConvert.Lt();
        methodConvert.JumpIfFalse(canSearchTarget);
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
        methodConvert.NumEqual();
        methodConvert.JumpIfTrue(notFoundTarget);

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
        methodConvert.NumEqual();
        methodConvert.JumpIfTrue(loopEnd);

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
        var endTarget = new JumpTarget();
        var validCountTarget = new JumpTarget();
        methodConvert.Dup();                                       // Duplicate string for length check
        methodConvert.Size();                                      // Get string length
        methodConvert.Rot();                                       // Rotate stack for comparison
        methodConvert.Dup();                                       // Duplicate comparison string
        methodConvert.Size();                                      // Get comparison string length
        methodConvert.Dup();                                       // Duplicate length for calculation
        methodConvert.Push(3);                                     // Push 3 for ROLL operation
        methodConvert.Roll();                                      // Roll stack elements
        methodConvert.Swap();                                      // Swap top elements
        methodConvert.Sub();                                       // Calculate start position
        methodConvert.Dup();                                       // Duplicate for bounds check
        methodConvert.Push(0);                                     // Push 0 for comparison
        methodConvert.JumpIfGreater(validCountTarget);        // Jump if position > 0
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.Drop();                                      // Clean stack
        methodConvert.PushF();                                     // Push false result
        methodConvert.JumpAlways(endTarget);                 // Jump to end
        validCountTarget.Instruction = methodConvert.Nop();        // Valid position target
        methodConvert.Push(3);                                     // Push 3 for ROLL operation
        methodConvert.Roll();                                      // Roll stack elements
        methodConvert.Reverse3();                                  // Reverse top 3 elements
        methodConvert.SubStr();                                    // Extract substring
        methodConvert.ChangeType(StackItemType.ByteString);        // Convert to ByteString
        methodConvert.Equal();                                     // Compare for equality
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    private static void HandleStringSubstring(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.SubStr();
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
        methodConvert.Push(0);
        methodConvert.NumEqual();
        methodConvert.JumpAlways(endTarget);
        nullOrEmptyTarget.Instruction = methodConvert.Drop();
        methodConvert.PushT();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleStringIsNullOrWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        JumpTarget nullTarget = new();
        JumpTarget emptyTarget = new();
        JumpTarget loopStart = new();
        JumpTarget allWhitespaceTarget = new();
        JumpTarget nonWhitespaceTarget = new();
        JumpTarget endTarget = new();

        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.JumpIfTrue(nullTarget);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte lengthSlot = methodConvert.AddAnonymousVariable();
        byte indexSlot = methodConvert.AddAnonymousVariable();

        methodConvert.Dup();
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.Drop();

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, lengthSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, lengthSlot);
        methodConvert.Push(0);
        methodConvert.NumEqual();
        methodConvert.JumpIfTrue(emptyTarget);

        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);

        loopStart.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, lengthSlot);
        methodConvert.Lt();
        methodConvert.JumpIfFalse(allWhitespaceTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.PickItem();

        CheckWithinWhiteSpace(methodConvert, nonWhitespaceTarget);

        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.Inc();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);

        methodConvert.JumpAlways(loopStart);

        nonWhitespaceTarget.Instruction = methodConvert.PushF();
        methodConvert.JumpAlways(endTarget);

        allWhitespaceTarget.Instruction = methodConvert.PushT();
        methodConvert.JumpAlways(endTarget);

        emptyTarget.Instruction = methodConvert.PushT();
        methodConvert.JumpAlways(endTarget);

        nullTarget.Instruction = methodConvert.Drop();
        methodConvert.PushT();
        methodConvert.JumpAlways(endTarget);

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

    private static void HandleStringCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Sub();
        methodConvert.Sign();
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
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
    }

    // Handler for object.ToString()
    private static void HandleObjectToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(StackItemType.ByteString);
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
        methodConvert.Isnull();                                    // Check if null
        methodConvert.JumpIfNot(firstNotNull);                     // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        firstNotNull.Instruction = methodConvert.Nop();            // First not null target
        methodConvert.Swap();                                      // Swap strings
        methodConvert.Dup();                                       // Duplicate second string
        methodConvert.Isnull();                                    // Check if null
        methodConvert.JumpIfNot(secondNotNull);                    // Jump if not null
        methodConvert.Drop();                                      // Drop null value
        methodConvert.Push("");                                    // Push empty string
        secondNotNull.Instruction = methodConvert.Nop();           // Second not null target
        methodConvert.Cat();                                       // Concatenate strings
    }

    private static void HandleStringToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
    ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToLower(methodConvert);
    }

    /// <summary>
    /// Converts a string to lowercase by processing each character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Iterates through string characters, converting uppercase to lowercase
    /// </remarks>
    private static void ConvertToLower(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push("");                                    // Create empty result string

        methodConvert.Push0();                                     // Initialize index to 0
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Size();                                      // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse(loopEnd);              // Exit if done

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Swap();                                      // Swap for PickItem
        methodConvert.PickItem();                                  // Get character at index
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('A', 'Z');                            // Check if uppercase
        methodConvert.JumpIfTrue(charIsLower);             // Jump if uppercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append original character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways(loopStart);                 // Continue loop

        charIsLower.Instruction = methodConvert.Nop();             // Uppercase processing
        methodConvert.Push((ushort)'A');                           // Push 'A'
        methodConvert.Sub();                                       // Subtract 'A'
        methodConvert.Push((ushort)'a');                           // Push 'a'
        methodConvert.Add();                                       // Add 'a' for lowercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append lowercase character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways(loopStart);                 // Continue loop

        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Drop index
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert to ByteString
    }

    /// <summary>
    /// Handles the string.ToUpper method by converting characters to uppercase.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through each character and converts lowercase letters to uppercase
    /// </remarks>
    private static void HandleStringToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        ConvertToUpper(methodConvert);
    }

    /// <summary>
    /// Converts a string to uppercase by processing each character.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <remarks>
    /// Algorithm: Iterates through string characters, converting lowercase to uppercase
    /// </remarks>
    private static void ConvertToUpper(MethodConvert methodConvert)
    {
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var charIsLower = new JumpTarget();
        methodConvert.Push("");                                    // Create empty result string

        methodConvert.Push0();                                     // Initialize index to 0
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Size();                                      // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse(loopEnd);              // Exit if done

        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg0();                                    // Load string
        methodConvert.Swap();                                      // Swap for PickItem
        methodConvert.PickItem();                                  // Get character at index
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('a', 'z');                     // Check if lowercase
        methodConvert.JumpIfTrue(charIsLower);             // Jump if lowercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append original character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways(loopStart);                 // Continue loop

        charIsLower.Instruction = methodConvert.Nop();             // Lowercase processing
        methodConvert.Push((ushort)'a');                           // Push 'a'
        methodConvert.Sub();                                       // Subtract 'a'
        methodConvert.Push((ushort)'A');                           // Push 'A'
        methodConvert.Add();                                       // Add 'A' for uppercase
        methodConvert.Rot();                                       // Rotate stack
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Cat();                                       // Append uppercase character
        methodConvert.Swap();                                      // Swap back
        methodConvert.Inc();                                       // Increment index
        methodConvert.JumpAlways(loopStart);                 // Continue loop

        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Drop index
        methodConvert.ChangeType(VM.Types.StackItemType.ByteString); // Convert to ByteString
    }

    /// <summary>
    /// Initializes the string length variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="strLen">Variable to store string length</param>
    /// <remarks>
    /// Algorithm: Gets string size and stores it in local variable
    /// </remarks>
    private static void InitStrLen(MethodConvert methodConvert, byte strLen)
    {
        GetString(methodConvert);                                  // Get string
        methodConvert.Size();                                      // Get string size
        methodConvert.AccessSlot(OpCode.STLOC, strLen);            // Store in local variable
    }

    /// <summary>
    /// Gets the string length from a local variable.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="strLen">Variable containing string length</param>
    /// <returns>String length on stack</returns>
    private static void GetStrLen(MethodConvert methodConvert, byte strLen) => methodConvert.AccessSlot(OpCode.LDLOC, strLen);

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
    /// <param name="strLen">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Sets end index to last character position
    /// </remarks>
    private static void InitEndIndex(MethodConvert methodConvert, byte endIndex, byte strLen)
    {
        GetStrLen(methodConvert, strLen);                          // Get string length
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
    /// Gets the string argument (LDARG0).
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <returns>String argument on stack</returns>
    private static void GetString(MethodConvert methodConvert) => methodConvert.LdArg0();

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
    /// <param name="strLen">Variable containing string length</param>
    /// <remarks>
    /// Algorithm: Exits loop if start index >= string length
    /// </remarks>
    private static void CheckStartIndex(MethodConvert methodConvert, JumpTarget loopEnd, byte startIndex, byte strLen)
    {
        GetStartIndex(methodConvert, startIndex);                  // Get start index
        GetStrLen(methodConvert, strLen);                          // Get string length
        methodConvert.Lt();                                        // Check if index < length
        methodConvert.JumpIfFalse(loopEnd);              // Exit if not less than
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
        methodConvert.JumpAlways(loopStart);                 // Continue loop
    }

    /// <summary>
    /// Picks character at the start index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="startIndex">Variable containing start index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current start position
    /// </remarks>
    private static void PickCharStart(MethodConvert methodConvert, byte startIndex)
    {
        GetString(methodConvert);                                  // Get string
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
    /// Checks if character is within whitespace range and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Checks if character is tab-carriage return range or space character
    /// </remarks>
    private static void CheckWithinWhiteSpace(MethodConvert methodConvert, JumpTarget loopEnd)
    {
        methodConvert.Dup();                                       // Duplicate character
        methodConvert.Within('\t', '\r');                          // Check if within tab-CR range
        methodConvert.Swap();                                      // Swap for space check

        methodConvert.Push((ushort)' ');                           // Push space character
        methodConvert.Equal();                                     // Check if equals space
        methodConvert.BoolOr();                                    // Combine checks with OR

        methodConvert.JumpIfFalse(loopEnd);              // Exit if not whitespace
    }

    /// <summary>
    /// Checks if character equals the trim character and exits loop if not.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="loopEnd">Jump target for loop end</param>
    /// <remarks>
    /// Algorithm: Compares character with the specified trim character
    /// </remarks>
    private static void CheckTrimChar(MethodConvert methodConvert, JumpTarget loopEnd, char? constantTrimChar)
    {
        if (constantTrimChar.HasValue)
            methodConvert.Push((ushort)constantTrimChar.Value);
        else
            methodConvert.LdArg1();                                // Load trim character
        methodConvert.NumEqual();                                  // Check equality
        methodConvert.JumpIfFalse(loopEnd);              // Exit if not equal
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
        methodConvert.Gt();                                        // Check if end > start
        methodConvert.JumpIfFalse(loopEnd);              // Exit if not greater
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
        methodConvert.Gt();
        methodConvert.JumpIfFalse(loopEnd);
    }

    /// <summary>
    /// Picks character at the end index position for processing.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="endIndex">Variable containing end index</param>
    /// <remarks>
    /// Algorithm: Gets character at the current end position
    /// </remarks>
    private static void PickCharEnd(MethodConvert methodConvert, byte endIndex)
    {
        GetString(methodConvert);                                  // Get string
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
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);                         // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);             // endIndex = string.Length - 1

        // Loop to trim leading whitespace
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex);                  // Pick character to check
        CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker

        // Process trailing whitespace
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();              // Second loop start
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex);                      // Pick character to check
        CheckWithinWhiteSpace(methodConvert, loopEnd2);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();                // Second loop end

        // Extract the trimmed substring
        GetString(methodConvert);                                  // Get original string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
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

    private static void HandleStringTrimCharInternal(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, char? constantTrimChar)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);                         // strLen = string.Length
        InitStartIndex(methodConvert, startIndex);                 // startIndex = 0
        InitEndIndex(methodConvert, endIndex, strLen);             // endIndex = string.Length - 1
        methodConvert.Drop();                                      // Clean up stack (remove argument from evaluation stack)

        // Loop to trim leading characters
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex);                  // Pick character to check
        CheckTrimChar(methodConvert, loopEnd, constantTrimChar);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker

        // Process trailing characters
        var loopStart2 = new JumpTarget();
        var loopEnd2 = new JumpTarget();
        loopStart2.Instruction = methodConvert.Nop();              // Second loop start
        CheckEndIndex(methodConvert, loopEnd2, endIndex, startIndex);
        PickCharEnd(methodConvert, endIndex);                      // Pick character to check
        CheckTrimChar(methodConvert, loopEnd2, constantTrimChar);
        MoveEndIndexAndLoop(methodConvert, loopStart2, endIndex);
        loopEnd2.Instruction = methodConvert.Nop();                // Second loop end

        // Extract the trimmed substring
        GetString(methodConvert);                                  // Get original string
        GetStartIndex(methodConvert, startIndex);                  // Get start position
        GetEndIndex(methodConvert, endIndex);                      // Get end position
        GetStartIndex(methodConvert, startIndex);                  // Get start for calculation
        methodConvert.Sub();                                       // Calculate length
        methodConvert.Inc();                                       // Increment for inclusive end
        methodConvert.SubStr();                                    // Extract substring
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
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var startIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);
        InitStartIndex(methodConvert, startIndex);

        if (useTrimChar)
        {
            methodConvert.Drop();
        }

        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        CheckStartIndex(methodConvert, loopEnd, startIndex, strLen);
        PickCharStart(methodConvert, startIndex);
        if (useTrimChar)
            CheckTrimChar(methodConvert, loopEnd, constantTrimChar);
        else
            CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveStartIndexAndLoop(methodConvert, loopStart, startIndex);
        loopEnd.Instruction = methodConvert.Nop();

        GetString(methodConvert);
        GetStartIndex(methodConvert, startIndex);
        GetStrLen(methodConvert, strLen);
        GetStartIndex(methodConvert, startIndex);
        methodConvert.Sub();
        methodConvert.SubStr();
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
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var strLen = methodConvert.AddAnonymousVariable();
        var endIndex = methodConvert.AddAnonymousVariable();

        InitStrLen(methodConvert, strLen);
        InitEndIndex(methodConvert, endIndex, strLen);

        if (useTrimChar)
        {
            methodConvert.Drop();
        }

        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        loopStart.Instruction = methodConvert.Nop();
        CheckEndIndexNonNegative(methodConvert, loopEnd, endIndex);
        PickCharEnd(methodConvert, endIndex);
        if (useTrimChar)
            CheckTrimChar(methodConvert, loopEnd, constantTrimChar);
        else
            CheckWithinWhiteSpace(methodConvert, loopEnd);
        MoveEndIndexAndLoop(methodConvert, loopStart, endIndex);
        loopEnd.Instruction = methodConvert.Nop();

        JumpTarget allTrimmed = new();
        JumpTarget endTarget = new();

        GetEndIndex(methodConvert, endIndex);
        methodConvert.Push(-1);
        methodConvert.Equal();
        methodConvert.JumpIfTrue(allTrimmed);

        GetString(methodConvert);
        methodConvert.Push0();
        GetEndIndex(methodConvert, endIndex);
        methodConvert.Inc();
        methodConvert.SubStr();
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
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var loopStart = new JumpTarget();
        var loopEnd = new JumpTarget();
        var replaceStart = new JumpTarget();
        var replaceEnd = new JumpTarget();

        // Duplicate the original string
        methodConvert.Dup();                                       // Duplicate string for processing

        // Start of the loop to find the substring
        loopStart.Instruction = methodConvert.Nop();               // Loop start marker

        // Check if the string contains the substring
        methodConvert.Dup();                                       // Duplicate string
        methodConvert.Dup();                                       // Duplicate again for search
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        methodConvert.Dup();                                       // Duplicate result
        methodConvert.PushM1();                                    // Push -1 for comparison
        methodConvert.Equal();                                     // Check if not found
        methodConvert.JumpIfTrue(loopEnd);                 // Exit if not found

        // Get the index of the substring
        methodConvert.Dup();                                       // Duplicate string
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);

        // Replace the substring with the new value
        replaceStart.Instruction = methodConvert.Nop();            // Replace start marker
        methodConvert.Dup();                                       // Duplicate index
        methodConvert.LdArg2();                                    // Load replacement string
        methodConvert.Cat();                                       // Concatenate replacement
        methodConvert.Dup();                                       // Duplicate result
        methodConvert.LdArg1();                                    // Load search string
        methodConvert.Size();                                      // Get search string length
        methodConvert.Add();                                       // Calculate position after replacement
        methodConvert.SubStr();                                    // Get remaining substring
        methodConvert.Cat();                                       // Concatenate remaining part
        replaceEnd.Instruction = methodConvert.Nop();              // Replace end marker

        // Continue the loop
        methodConvert.JumpAlways(loopStart);                 // Continue loop

        // End of the loop
        loopEnd.Instruction = methodConvert.Nop();                 // Loop end marker
        methodConvert.Drop();                                      // Clean up stack
    }

    private static void HandleStringRemove(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        bool hasCount = symbol.Parameters.Length == 2;

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte startSlot = methodConvert.AddAnonymousVariable();
        byte countSlot = hasCount ? methodConvert.AddAnonymousVariable() : (byte)0;

        if (hasCount)
        {
            methodConvert.AccessSlot(OpCode.STLOC, strSlot);
            methodConvert.AccessSlot(OpCode.STLOC, startSlot);
            methodConvert.AccessSlot(OpCode.STLOC, countSlot);
        }
        else
        {
            methodConvert.AccessSlot(OpCode.STLOC, strSlot);
            methodConvert.AccessSlot(OpCode.STLOC, startSlot);
        }

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);

        if (!hasCount)
        {
            methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
            methodConvert.Push0();
            methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
            methodConvert.SubStr();
            return;
        }

        byte prefixSlot = methodConvert.AddAnonymousVariable();
        byte suffixSlot = methodConvert.AddAnonymousVariable();
        byte strLenSlot = methodConvert.AddAnonymousVariable();
        byte suffixStartSlot = methodConvert.AddAnonymousVariable();
        byte suffixLengthSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, strLenSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.SubStr();
        methodConvert.AccessSlot(OpCode.STLOC, prefixSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, countSlot);
        methodConvert.Add();
        methodConvert.AccessSlot(OpCode.STLOC, suffixStartSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, suffixStartSlot);
        methodConvert.Sub();
        methodConvert.AccessSlot(OpCode.STLOC, suffixLengthSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, suffixStartSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, suffixLengthSlot);
        methodConvert.SubStr();
        methodConvert.AccessSlot(OpCode.STLOC, suffixSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, prefixSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, suffixSlot);
        methodConvert.Cat();
    }

    private static void HandleStringInsert(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        byte strSlot = methodConvert.AddAnonymousVariable();
        byte startSlot = methodConvert.AddAnonymousVariable();
        byte valueSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, strSlot);
        methodConvert.AccessSlot(OpCode.STLOC, startSlot);
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, strSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.Dup();
        methodConvert.IsNull();
        methodConvert.Not();
        methodConvert.Assert();
        methodConvert.ChangeType(StackItemType.ByteString);
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);

        byte prefixSlot = methodConvert.AddAnonymousVariable();
        byte suffixSlot = methodConvert.AddAnonymousVariable();
        byte strLenSlot = methodConvert.AddAnonymousVariable();
        byte suffixLengthSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.STLOC, strLenSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.SubStr();
        methodConvert.AccessSlot(OpCode.STLOC, prefixSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strLenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.Sub();
        methodConvert.AccessSlot(OpCode.STLOC, suffixLengthSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, strSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, startSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, suffixLengthSlot);
        methodConvert.SubStr();
        methodConvert.AccessSlot(OpCode.STLOC, suffixSlot);

        methodConvert.AccessSlot(OpCode.LDLOC, prefixSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.Cat();
        methodConvert.AccessSlot(OpCode.LDLOC, suffixSlot);
        methodConvert.Cat();
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

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);

        // Call the StdLib memorySearch method to find the index of the character
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
    }
}
