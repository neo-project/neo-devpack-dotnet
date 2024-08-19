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
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// When the array is initialized to null, this code converts it to "array length" + OpCode.NEWBUFFER (only for byte[]) or  OpCode.NEWARRAY_T.
    /// When the array is not initialized to null, this code converts the initialized constants one by one in reverse order, then adds the "array length" and OpCode.PACK
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
        if (specifier.Rank != 1)
            throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {specifier}");
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression.Type).Type!;
        if (expression.Initializer is null)
        {
            ConvertExpression(model, specifier.Sizes[0]);
            if (type.ElementType.SpecialType == SpecialType.System_Byte)
                AddInstruction(OpCode.NEWBUFFER);
            else
                AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.ElementType.GetStackItemType() } });
        }
        else
        {
            ConvertInitializerExpression(model, type, expression.Initializer);
        }
    }
}
