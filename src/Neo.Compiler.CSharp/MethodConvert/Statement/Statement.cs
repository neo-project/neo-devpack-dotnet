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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertStatement(SemanticModel model, StatementSyntax statement)
        {
            switch (statement)
            {
                // Converts a block statement, which is a series of statements enclosed in braces.
                // Example: { int x = 1; Console.WriteLine(x); }
                case BlockSyntax syntax:
                    ConvertBlockStatement(model, syntax);
                    break;
                // Converts a break statement, typically used within loops or switch cases.
                // Example: break;
                case BreakStatementSyntax syntax:
                    ConvertBreakStatement(syntax);
                    break;
                // Converts a checked statement, used for arithmetic operations with overflow checking.
                // Example: checked { int x = int.MaxValue; }
                case CheckedStatementSyntax syntax:
                    ConvertCheckedStatement(model, syntax);
                    break;
                // Converts a continue statement, used to skip the current iteration of a loop.
                // Example: continue;
                case ContinueStatementSyntax syntax:
                    ConvertContinueStatement(syntax);
                    break;
                // Converts a do-while loop statement.
                // Example: do { /* actions */ } while (condition);
                case DoStatementSyntax syntax:
                    ConvertDoStatement(model, syntax);
                    break;
                // Converts an empty statement, which is typically just a standalone semicolon.
                // Example: ;
                case EmptyStatementSyntax syntax:
                    ConvertEmptyStatement(syntax);
                    break;
                // Converts an expression statement, which is a statement consisting of a single expression.
                // Example: Console.WriteLine("Hello");
                case ExpressionStatementSyntax syntax:
                    ConvertExpressionStatement(model, syntax);
                    break;
                // Converts a foreach loop statement.
                // Example: foreach (var item in collection) { /* actions */ }
                case ForEachStatementSyntax syntax:
                    ConvertForEachStatement(model, syntax);
                    break;
                // Converts a foreach loop statement with variable declarations.
                // Example: foreach (var (key, value) in dictionary) { /* actions */ }
                case ForEachVariableStatementSyntax syntax:
                    ConvertForEachVariableStatement(model, syntax);
                    break;
                // Converts a for loop statement.
                // Example: for (int i = 0; i < 10; i++) { /* actions */ }
                case ForStatementSyntax syntax:
                    ConvertForStatement(model, syntax);
                    break;
                // Converts a goto statement, used for jumping to a labeled statement.
                // Example: goto myLabel;
                case GotoStatementSyntax syntax:
                    ConvertGotoStatement(model, syntax);
                    break;
                // Converts an if statement, including any else or else if branches.
                // Example: if (condition) { /* actions */ } else { /* actions */ }
                case IfStatementSyntax syntax:
                    ConvertIfStatement(model, syntax);
                    break;
                // Converts a labeled statement, used as a target for goto statements.
                // Example: myLabel: /* actions */
                case LabeledStatementSyntax syntax:
                    ConvertLabeledStatement(model, syntax);
                    break;
                // Converts a local variable declaration statement.
                // Example: int x = 5;
                case LocalDeclarationStatementSyntax syntax:
                    ConvertLocalDeclarationStatement(model, syntax);
                    break;
                // Currently, local function statements are not supported in this context.
                case LocalFunctionStatementSyntax:
                    break;
                // Converts a return statement, used to exit a method and optionally return a value.
                // Example: return x + y;
                case ReturnStatementSyntax syntax:
                    ConvertReturnStatement(model, syntax);
                    break;
                // Converts a switch statement, including its cases and default case.
                // Example: switch (variable) { case 1: /* actions */ break; default: /* actions */
                case SwitchStatementSyntax syntax:
                    ConvertSwitchStatement(model, syntax);
                    break;
                // Converts a throw statement, used for exception handling.
                // Example: throw new Exception("Error");
                case ThrowStatementSyntax syntax:
                    ConvertThrowStatement(model, syntax);
                    break;
                // Converts a try-catch-finally statement, used for exception handling.
                // Example: try { /* actions */ } catch (Exception e) { /* actions */ } finally { /* actions */ }
                case TryStatementSyntax syntax:
                    ConvertTryStatement(model, syntax);
                    break;
                // Converts a while loop statement.
                // Example: while (condition) { /* actions */ }
                case WhileStatementSyntax syntax:
                    ConvertWhileStatement(model, syntax);
                    break;
                default:
                    throw new CompilationException(statement, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {statement}");
            }
        }
    }
}
