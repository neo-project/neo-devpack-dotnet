// Copyright (C) 2015-2025 The Neo Project.
//
// ArrayCreationExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the code for constructing arrays and initializing arrays into OpCodes.
    /// This method includes analyzing the array length, array type, array dimension and initial data.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the array creation.</param>
    /// <param name="expression">The syntax representation of the array creation statement being converted.</param>
    /// <remarks>
    /// One-dimensional arrays emit NEWBUFFER/NEWARRAY_T fast paths. Multi-dimensional arrays are lowered into nested Neo VM arrays,
    /// constructing each rank with loops so rectangular indexing semantics are preserved.
    /// </remarks>
    /// <example>
    /// Example of a array creation syntax:
    /// <c>var array = new byte[4];</c>
    /// The compilation result of the example code is: OpCode.PUSH4, OpCode.NEWBUFFER
    /// <c>var array = new int[4] { 5, 6, 7, 8};</c>
    /// The compilation result of the example code is: OpCode.PUSH8, OpCode.PUSH7, OpCode.PUSH6, OpCode.PUSH5, OpCode.PUSH4, OpCode.PACK
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays">Arrays</seealso>
    private void ConvertArrayCreationExpression(SemanticModel model, ArrayCreationExpressionSyntax expression)
    {
        ArrayRankSpecifierSyntax specifier = expression.Type.RankSpecifiers[0];
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression.Type).Type!;
        if (type.Rank > 1)
        {
            ConvertMultiDimensionalArrayCreation(model, expression, type, specifier);
            return;
        }
        if (specifier.Rank != 1)
            throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {specifier}");
        if (expression.Initializer is null)
        {
            ConvertExpression(model, specifier.Sizes[0]);
            if (type.ElementType.SpecialType == SpecialType.System_Byte)
                AddInstruction(OpCode.NEWBUFFER);
            else if (type.ElementType.SpecialType == SpecialType.System_String)
                AddInstruction(OpCode.NEWARRAY);
            else
                AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.ElementType.GetStackItemType() } });
        }
        else
        {
            ConvertInitializerExpression(model, type, expression.Initializer);
        }
    }

    private void ConvertMultiDimensionalArrayCreation(SemanticModel model, ArrayCreationExpressionSyntax expression, IArrayTypeSymbol type, ArrayRankSpecifierSyntax specifier)
    {
        if (expression.Initializer is not null)
        {
            ConvertMultiDimensionalInitializer(model, type, expression.Initializer, specifier);
            return;
        }

        if (specifier.Sizes.Any(size => size.IsMissing))
            throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, "Explicit sizes are required when no initializer is provided for a multi-dimensional array.");

        var lengthSlots = new byte[specifier.Sizes.Count];
        for (int i = 0; i < specifier.Sizes.Count; i++)
        {
            ConvertExpression(model, specifier.Sizes[i]);
            lengthSlots[i] = AddAnonymousVariable();
            AccessSlot(OpCode.STLOC, lengthSlots[i]);
        }

        EmitCreateMultiDimensionalArray(type.ElementType, lengthSlots, 0);

        for (int i = lengthSlots.Length - 1; i >= 0; i--)
            RemoveAnonymousVariable(lengthSlots[i]);
    }

    private void ConvertMultiDimensionalInitializer(SemanticModel model, IArrayTypeSymbol type, InitializerExpressionSyntax initializer, ArrayRankSpecifierSyntax? specifier = null)
    {
        int rank = type.Rank;
        int[] lengths = new int[rank];
        bool[] dimensionInitialized = new bool[rank];

        ValidateMultiDimensionalInitializer(initializer, rank, 0, lengths, dimensionInitialized);

        for (int i = 0; i < rank; i++)
        {
            if (!dimensionInitialized[i])
                lengths[i] = 0;
        }

        if (specifier is not null)
            EnsureDeclaredLengthsMatchInitializer(model, specifier, lengths);

        EmitMultiDimensionalInitializer(model, type.ElementType, initializer, 0, rank);
    }

    private void ValidateMultiDimensionalInitializer(InitializerExpressionSyntax initializer, int rank, int dimension, int[] lengths, bool[] dimensionInitialized)
    {
        if (dimension >= rank)
            throw new CompilationException(initializer, DiagnosticId.MultidimensionalArray, "Initializer nesting level exceeds the declared array rank.");

        int count = initializer.Expressions.Count;
        if (!dimensionInitialized[dimension])
        {
            lengths[dimension] = count;
            dimensionInitialized[dimension] = true;
        }
        else if (lengths[dimension] != count)
        {
            throw new CompilationException(initializer, DiagnosticId.MultidimensionalArray, $"Inconsistent initializer length detected for dimension {dimension}. Expected {lengths[dimension]}, but found {count}.");
        }

        if (dimension == rank - 1)
        {
            foreach (ExpressionSyntax expression in initializer.Expressions)
            {
                if (expression is InitializerExpressionSyntax nested)
                    throw new CompilationException(nested, DiagnosticId.MultidimensionalArray, "Too many nested initializers provided for the array rank.");
            }
            return;
        }

        foreach (ExpressionSyntax expression in initializer.Expressions)
        {
            if (expression is not InitializerExpressionSyntax nested)
                throw new CompilationException(expression, DiagnosticId.MultidimensionalArray, $"A nested initializer is required for dimension {dimension + 1}.");
            ValidateMultiDimensionalInitializer(nested, rank, dimension + 1, lengths, dimensionInitialized);
        }
    }

    private void EnsureDeclaredLengthsMatchInitializer(SemanticModel model, ArrayRankSpecifierSyntax specifier, int[] initializerLengths)
    {
        if (specifier.Sizes.Count != initializerLengths.Length)
            throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, "Rank specifier does not match the inferred initializer dimensions.");

        for (int i = 0; i < specifier.Sizes.Count; i++)
        {
            ExpressionSyntax sizeSyntax = specifier.Sizes[i];
            if (sizeSyntax.IsMissing || sizeSyntax is OmittedArraySizeExpressionSyntax)
                continue;

            Optional<object?> constant = model.GetConstantValue(sizeSyntax);
            if (!constant.HasValue)
                throw new CompilationException(sizeSyntax, DiagnosticId.MultidimensionalArray, "Array size expressions must be constant when an initializer is provided for a multi-dimensional array.");

            int declaredLength = System.Convert.ToInt32(constant.Value);
            if (declaredLength != initializerLengths[i])
                throw new CompilationException(sizeSyntax, DiagnosticId.MultidimensionalArray, $"Declared length {declaredLength} does not match initializer length {initializerLengths[i]} for dimension {i}.");
        }
    }

    private void EmitMultiDimensionalInitializer(SemanticModel model, ITypeSymbol elementType, InitializerExpressionSyntax initializer, int dimension, int totalRank)
    {
        if (dimension == totalRank - 1)
        {
            EmitOneDimensionalInitializer(model, elementType, initializer.Expressions);
            return;
        }

        for (int i = initializer.Expressions.Count - 1; i >= 0; i--)
        {
            if (initializer.Expressions[i] is not InitializerExpressionSyntax nested)
                throw new CompilationException(initializer.Expressions[i], DiagnosticId.MultidimensionalArray, "Internal error: initializer shape mismatch detected.");
            EmitMultiDimensionalInitializer(model, elementType, nested, dimension + 1, totalRank);
        }

        Push(initializer.Expressions.Count);
        AddInstruction(OpCode.PACK);
    }

    private void EmitCreateMultiDimensionalArray(ITypeSymbol elementType, byte[] lengthSlots, int dimension)
    {
        var lenSlot = lengthSlots[dimension];
        if (dimension == lengthSlots.Length - 1)
        {
            AccessSlot(OpCode.LDLOC, lenSlot);
            if (elementType.SpecialType == SpecialType.System_Byte)
                AddInstruction(OpCode.NEWBUFFER);
            else if (elementType.SpecialType == SpecialType.System_String)
                AddInstruction(OpCode.NEWARRAY);
            else
                AddInstruction(new Instruction
                {
                    OpCode = OpCode.NEWARRAY_T,
                    Operand = new[] { (byte)elementType.GetStackItemType() }
                });
            return;
        }

        byte arraySlot = AddAnonymousVariable();
        byte counterSlot = AddAnonymousVariable();
        JumpTarget conditionTarget = new();
        JumpTarget bodyTarget = new();

        AccessSlot(OpCode.LDLOC, lenSlot);
        AddInstruction(OpCode.NEWARRAY);
        AccessSlot(OpCode.STLOC, arraySlot);
        Push(0);
        AccessSlot(OpCode.STLOC, counterSlot);
        Jump(OpCode.JMP_L, conditionTarget);

        bodyTarget.Instruction = AddInstruction(OpCode.NOP);
        EmitCreateMultiDimensionalArray(elementType, lengthSlots, dimension + 1);
        AccessSlot(OpCode.LDLOC, counterSlot);
        AccessSlot(OpCode.LDLOC, arraySlot);
        AddInstruction(OpCode.REVERSE3);
        AddInstruction(OpCode.SETITEM);
        AccessSlot(OpCode.LDLOC, counterSlot);
        AddInstruction(OpCode.INC);
        AccessSlot(OpCode.STLOC, counterSlot);

        conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, counterSlot);
        AccessSlot(OpCode.LDLOC, lenSlot);
        Jump(OpCode.JMPLT_L, bodyTarget);

        AddInstruction(OpCode.NOP);
        AccessSlot(OpCode.LDLOC, arraySlot);
        RemoveAnonymousVariable(counterSlot);
        RemoveAnonymousVariable(arraySlot);
    }
}
