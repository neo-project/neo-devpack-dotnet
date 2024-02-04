// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
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
            if (type.Name == "Iterator")
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
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
                AccessSlot(OpCode.STLOC, elementIndex);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(iteratorIndex);
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
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
            if (type.Name == "Iterator")
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
            ILocalSymbol[] symbols = ((ParenthesizedVariableDesignationSyntax)((DeclarationExpressionSyntax)syntax.Variable).Designation).Variables.Select(p => (ILocalSymbol)model.GetDeclaredSymbol(p)!).ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Variable))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
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
                        byte variableIndex = AddLocalVariable(symbols[i]);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
                }
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(iteratorIndex);
            foreach (ILocalSymbol symbol in symbols)
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            PopContinueTarget();
            PopBreakTarget();
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
            ILocalSymbol[] symbols = ((ParenthesizedVariableDesignationSyntax)((DeclarationExpressionSyntax)syntax.Variable).Designation).Variables.Select(p => (ILocalSymbol)model.GetDeclaredSymbol(p)!).ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            byte arrayIndex = AddAnonymousVariable();
            byte lengthIndex = AddAnonymousVariable();
            byte iIndex = AddAnonymousVariable();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
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
                        byte variableIndex = AddLocalVariable(symbols[i]);
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
            foreach (ILocalSymbol symbol in symbols)
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
