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
        /// <summary>
        /// Converts a 'try-catch-finally' statement into a set of instructions for exception handling.
        /// This method handles the translation of try blocks, catch clauses, and finally blocks
        /// into an intermediate language.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the try statement.</param>
        /// <param name="syntax">The syntax representation of the try statement being converted.</param>
        /// <remarks>
        /// The method sets up the necessary structure for a try block, including jump targets for catch
        /// and finally blocks. It handles the conversion of each part of the try statement, including
        /// any exception handling logic. The method currently supports only one catch block and
        /// throws a compilation exception if multiple catches or catch filters are present.
        /// </remarks>
        /// <example>
        /// Example of a 'try-catch-finally' statement syntax:
        /// <code>
        /// try
        /// {
        ///     // Try block code
        /// }
        /// catch (Exception e)
        /// {
        ///     // Catch block code
        /// }
        /// finally
        /// {
        ///     // Finally block code
        /// }
        /// </code>
        /// This example demonstrates a typical try statement with one catch block and a finally block.
        /// </example>
        private void ConvertTryStatement(SemanticModel model, TryStatementSyntax syntax)
        {
            JumpTarget catchTarget = new();
            JumpTarget finallyTarget = new();
            JumpTarget endTarget = new();
            AddInstruction(new Instruction { OpCode = OpCode.TRY_L, Target = catchTarget, Target2 = finallyTarget });
            _tryStack.Push(new ExceptionHandling { State = ExceptionHandlingState.Try });
            ConvertStatement(model, syntax.Block);
            Jump(OpCode.ENDTRY_L, endTarget);
            if (syntax.Catches.Count > 1)
                throw new CompilationException(syntax.Catches[1], DiagnosticId.MultiplyCatches, "Only support one single catch.");
            if (syntax.Catches.Count > 0)
            {
                CatchClauseSyntax catchClause = syntax.Catches[0];
                if (catchClause.Filter is not null)
                    throw new CompilationException(catchClause.Filter, DiagnosticId.CatchFilter, $"Unsupported syntax: {catchClause.Filter}");
                _tryStack.Peek().State = ExceptionHandlingState.Catch;
                ILocalSymbol? exceptionSymbol = null;
                byte exceptionIndex;
                if (catchClause.Declaration is null)
                {
                    exceptionIndex = AddAnonymousVariable();
                }
                else
                {
                    exceptionSymbol = model.GetDeclaredSymbol(catchClause.Declaration);
                    exceptionIndex = exceptionSymbol is null
                        ? AddAnonymousVariable()
                        : AddLocalVariable(exceptionSymbol);
                }
                using (InsertSequencePoint(catchClause.CatchKeyword))
                    catchTarget.Instruction = AccessSlot(OpCode.STLOC, exceptionIndex);
                _exceptionStack.Push(exceptionIndex);
                ConvertStatement(model, catchClause.Block);
                Jump(OpCode.ENDTRY_L, endTarget);
                if (exceptionSymbol is null)
                    RemoveAnonymousVariable(exceptionIndex);
                else
                    RemoveLocalVariable(exceptionSymbol);
                _exceptionStack.Pop();
            }
            if (syntax.Finally is not null)
            {
                _tryStack.Peek().State = ExceptionHandlingState.Finally;
                finallyTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertStatement(model, syntax.Finally.Block);
                AddInstruction(OpCode.ENDFINALLY);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
            _tryStack.Pop();
        }
    }
}
