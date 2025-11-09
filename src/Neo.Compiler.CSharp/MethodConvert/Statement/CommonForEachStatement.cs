// Copyright (C) 2015-2025 The Neo Project.
//
// CommonForEachStatement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'foreach' statement based on the type of the collection being iterated over.
        /// It delegates to 'ConvertIteratorForEachStatement' for iterators, and
        /// 'ConvertArrayForEachStatement' for arrays.
        /// </summary>
        /// <param name="model">The semantic model providing context for the 'foreach' statement.</param>
        /// <param name="syntax">The syntax of the 'foreach' statement.</param>
        /// <example>
        /// Example of 'foreach' statement syntax:
        /// <code>
        /// foreach (var item in collection)
        /// {
        ///     // Processing each item
        /// }
        /// </code>
        /// </example>
        private void ConvertForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            if (type is IArrayTypeSymbol arrayType && arrayType.Rank > 1)
            {
                ConvertMultiDimensionalArrayForEachStatement(model, syntax, arrayType);
            }
            else if (type.Name == "Iterator")
            {
                ConvertIteratorForEachStatement(model, syntax);
            }
            else
            {
                ConvertArrayForEachStatement(model, syntax);
            }
        }

        /// <summary>
        /// Converts a 'foreach' statement that iterates over an iterator, handling control flow and
        /// iterator management.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="syntax">The syntax of the 'foreach' statement.</param>
        /// <example>
        /// Example of 'foreach' over an iterator:
        /// <code>
        /// foreach (var item in iterator)
        /// {
        ///     // Processing each item
        /// }
        /// </code>
        /// </example>
        private void ConvertIteratorForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                CallInteropMethod(ApplicationEngine.System_Iterator_Value);
                AccessSlot(OpCode.STLOC, elementIndex);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                CallInteropMethod(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(iteratorIndex);
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        /// <summary>
        /// Converts a 'foreach' statement with variable declarations, determining the conversion
        /// method based on the collection type.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="syntax">The syntax of the 'foreach' variable statement.</param>
        /// <example>
        /// Example of 'foreach' with variable declaration:
        /// <code>
        /// foreach (var (key, value) in dictionary)
        /// {
        ///     // Processing each key and value pair
        /// }
        /// </code>
        /// </example>
        private void ConvertForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            if (type is IArrayTypeSymbol arrayType && arrayType.Rank > 1)
            {
                ConvertMultiDimensionalArrayForEachVariableStatement(model, syntax, arrayType);
            }
            else if (type.Name == "Iterator")
            {
                ConvertIteratorForEachVariableStatement(model, syntax);
            }
            else
            {
                ConvertArrayForEachVariableStatement(model, syntax);
            }
        }

        /// <summary>
        /// Converts a 'foreach' statement (with variable declarations) that iterates over an iterator,
        /// handling the unpacking of values into variables and managing loop control flow.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="syntax">The syntax of the 'foreach' variable statement.</param>
        /// <example>
        /// Example of 'foreach' over an iterator with variable declaration:
        /// <code>
        /// foreach (var (key, value) in iterator)
        /// {
        ///     // Processing each key and value
        /// }
        /// </code>
        /// </example>
        private void ConvertIteratorForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            var designation = ((DeclarationExpressionSyntax)syntax.Variable).Designation;
            ILocalSymbol?[] symbols = [.. ((ParenthesizedVariableDesignationSyntax)designation).Variables
                .Select(p => p switch
                {
                    SingleVariableDesignationSyntax single => (ILocalSymbol)model.GetDeclaredSymbol(single)!,
                    DiscardDesignationSyntax => null,
                    _ => throw CompilationException.UnsupportedSyntax(p, $"`foreach` variable designation type '{p.GetType().Name}' is not supported. Only single variable and discard designations are allowed.")
                })];
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Variable))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                CallInteropMethod(ApplicationEngine.System_Iterator_Value);
                AddInstruction(OpCode.UNPACK);
                AddInstruction(OpCode.DROP);
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i] is null)
                    {
                        AddInstruction(OpCode.DROP);
                    }
                    else
                    {
                        byte variableIndex = AddLocalVariable(symbols[i]!);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
                }
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                CallInteropMethod(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(iteratorIndex);
            foreach (var symbol in symbols)
            {
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            }
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        /// <summary>
        /// Converts a 'foreach' statement that iterates over an array, managing array indices and
        /// loop control flow.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="syntax">The syntax of the 'foreach' statement.</param>
        /// <example>
        /// Example of 'foreach' over an array:
        /// <code>
        /// foreach (var item in array)
        /// {
        ///     // Processing each array item
        /// }
        /// </code>
        /// </example>
        private void ConvertArrayForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            byte arrayIndex = AddAnonymousVariable();
            byte lengthIndex = AddAnonymousVariable();
            byte iIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STLOC, arrayIndex);
                AddInstruction(OpCode.SIZE);
                AccessSlot(OpCode.STLOC, lengthIndex);
                Push(0);
                AccessSlot(OpCode.STLOC, iIndex);
                Jump(OpCode.JMP_L, conditionTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, arrayIndex);
                AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.PICKITEM);
                AccessSlot(OpCode.STLOC, elementIndex);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.INC);
                AccessSlot(OpCode.STLOC, iIndex);
                conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AccessSlot(OpCode.LDLOC, lengthIndex);
                Jump(OpCode.JMPLT_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(arrayIndex);
            RemoveAnonymousVariable(lengthIndex);
            RemoveAnonymousVariable(iIndex);
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        /// <summary>
        /// Converts a 'foreach' statement (with variable declarations) that iterates over an array,
        /// including logic for unpacking array elements into variables.
        /// </summary>
        /// <param name="model">The semantic model.</param>
        /// <param name="syntax">The syntax of the 'foreach' variable statement.</param>
        /// <example>
        /// Example of 'foreach' over an array with variable declaration:
        /// <code>
        /// foreach (var (first, second) in arrayOfPairs)
        /// {
        ///     // Processing each pair of elements
        /// }
        /// </code>
        /// </example>
        private void ConvertArrayForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            var designation = ((DeclarationExpressionSyntax)syntax.Variable).Designation;
            ILocalSymbol?[] symbols = [.. ((ParenthesizedVariableDesignationSyntax)designation).Variables
                .Select(p => p switch
                {
                    SingleVariableDesignationSyntax single => (ILocalSymbol)model.GetDeclaredSymbol(single)!,
                    DiscardDesignationSyntax => null,
                    _ => throw CompilationException.UnsupportedSyntax(p, $"`foreach` variable designation type '{p.GetType().Name}' is not supported. Only single variable and discard designations are allowed.")
                })];
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            byte arrayIndex = AddAnonymousVariable();
            byte lengthIndex = AddAnonymousVariable();
            byte iIndex = AddAnonymousVariable();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STLOC, arrayIndex);
                AddInstruction(OpCode.SIZE);
                AccessSlot(OpCode.STLOC, lengthIndex);
                Push(0);
                AccessSlot(OpCode.STLOC, iIndex);
                Jump(OpCode.JMP_L, conditionTarget);
            }
            using (InsertSequencePoint(syntax.Variable))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, arrayIndex);
                AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.UNPACK);
                AddInstruction(OpCode.DROP);
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i] is null)
                    {
                        AddInstruction(OpCode.DROP);
                    }
                    else
                    {
                        byte variableIndex = AddLocalVariable(symbols[i]!);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
                }
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.INC);
                AccessSlot(OpCode.STLOC, iIndex);
                conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AccessSlot(OpCode.LDLOC, lengthIndex);
                Jump(OpCode.JMPLT_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(arrayIndex);
            RemoveAnonymousVariable(lengthIndex);
            RemoveAnonymousVariable(iIndex);
            foreach (var symbol in symbols)
            {
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            }
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        private void ConvertMultiDimensionalArrayForEachStatement(SemanticModel model, ForEachStatementSyntax syntax, IArrayTypeSymbol arrayType)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);

            byte rootArraySlot = AddAnonymousVariable();
            byte elementSlot = AddLocalVariable(elementSymbol);
            byte[] indexSlots = new byte[arrayType.Rank];
            for (int i = 0; i < indexSlots.Length; i++)
                indexSlots[i] = AddAnonymousVariable();

            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, rootArraySlot);
            }

            EmitMultiDimensionalArrayForEachLoop(arrayType, rootArraySlot, indexSlots, 0, continueTarget, breakTarget, () =>
            {
                using (InsertSequencePoint(syntax.Identifier))
                {
                    AccessSlot(OpCode.STLOC, elementSlot);
                }
                ConvertStatement(model, syntax.Statement);
            });

            breakTarget.Instruction = AddInstruction(OpCode.NOP);

            for (int i = indexSlots.Length - 1; i >= 0; i--)
                RemoveAnonymousVariable(indexSlots[i]);
            RemoveAnonymousVariable(rootArraySlot);
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        private void ConvertMultiDimensionalArrayForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax, IArrayTypeSymbol arrayType)
        {
            var designation = ((DeclarationExpressionSyntax)syntax.Variable).Designation;
            ILocalSymbol?[] symbols = [.. ((ParenthesizedVariableDesignationSyntax)designation).Variables
                .Select(p => p switch
                {
                    SingleVariableDesignationSyntax single => (ILocalSymbol)model.GetDeclaredSymbol(single)!,
                    DiscardDesignationSyntax => null,
                    _ => throw CompilationException.UnsupportedSyntax(p, $"`foreach` variable designation type '{p.GetType().Name}' is not supported. Only single variable and discard designations are allowed.")
                })];

            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, continueTarget: continueTarget);
            _generalStatementStack.Push(sc);

            byte rootArraySlot = AddAnonymousVariable();
            byte[] indexSlots = new byte[arrayType.Rank];
            for (int i = 0; i < indexSlots.Length; i++)
                indexSlots[i] = AddAnonymousVariable();

            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, rootArraySlot);
            }

            EmitMultiDimensionalArrayForEachLoop(arrayType, rootArraySlot, indexSlots, 0, continueTarget, breakTarget, () =>
            {
                using (InsertSequencePoint(syntax.Variable))
                {
                    AddInstruction(OpCode.UNPACK);
                    AddInstruction(OpCode.DROP);
                    for (int i = 0; i < symbols.Length; i++)
                    {
                        if (symbols[i] is null)
                        {
                            AddInstruction(OpCode.DROP);
                        }
                        else
                        {
                            byte variableIndex = AddLocalVariable(symbols[i]!);
                            AccessSlot(OpCode.STLOC, variableIndex);
                        }
                    }
                }
                ConvertStatement(model, syntax.Statement);
            });

            breakTarget.Instruction = AddInstruction(OpCode.NOP);

            for (int i = indexSlots.Length - 1; i >= 0; i--)
                RemoveAnonymousVariable(indexSlots[i]);
            RemoveAnonymousVariable(rootArraySlot);
            foreach (var symbol in symbols)
            {
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            }
            PopContinueTarget();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in foreach statement handling. This is a compiler bug that should be reported.");
        }

        private void EmitMultiDimensionalArrayForEachLoop(IArrayTypeSymbol arrayType, byte rootArraySlot, byte[] indexSlots, int dimension, JumpTarget continueTarget, JumpTarget breakTarget, Action emitElementBody)
        {
            byte currentArraySlot = AddAnonymousVariable();
            JumpTarget startTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget loopContinueTarget = dimension == arrayType.Rank - 1 ? continueTarget : new();

            LoadArrayForDimension(rootArraySlot, indexSlots, dimension);
            AccessSlot(OpCode.STLOC, currentArraySlot);

            Push(0);
            AccessSlot(OpCode.STLOC, indexSlots[dimension]);
            Jump(OpCode.JMP_L, conditionTarget);

            startTarget.Instruction = AddInstruction(OpCode.NOP);

            if (dimension == arrayType.Rank - 1)
            {
                AccessSlot(OpCode.LDLOC, currentArraySlot);
                AccessSlot(OpCode.LDLOC, indexSlots[dimension]);
                AddInstruction(OpCode.PICKITEM);
                emitElementBody();
            }
            else
            {
                EmitMultiDimensionalArrayForEachLoop(arrayType, rootArraySlot, indexSlots, dimension + 1, continueTarget, breakTarget, emitElementBody);
            }

            loopContinueTarget.Instruction = AccessSlot(OpCode.LDLOC, indexSlots[dimension]);
            AddInstruction(OpCode.INC);
            AccessSlot(OpCode.STLOC, indexSlots[dimension]);
            conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, indexSlots[dimension]);
            AccessSlot(OpCode.LDLOC, currentArraySlot);
            AddInstruction(OpCode.SIZE);
            Jump(OpCode.JMPLT_L, startTarget);

            RemoveAnonymousVariable(currentArraySlot);
        }

        private void LoadArrayForDimension(byte rootArraySlot, byte[] indexSlots, int dimension)
        {
            AccessSlot(OpCode.LDLOC, rootArraySlot);
            for (int i = 0; i < dimension; i++)
            {
                AccessSlot(OpCode.LDLOC, indexSlots[i]);
                AddInstruction(OpCode.PICKITEM);
            }
        }
    }
}
