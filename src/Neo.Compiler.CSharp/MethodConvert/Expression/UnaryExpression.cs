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
using System.Runtime.InteropServices;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertPostfixUnaryExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "++":
                case "--":
                    ConvertPostIncrementOrDecrementExpression(model, expression);
                    break;
                case "!":
                    ConvertExpression(model, expression.Operand);
                    break;
                default:
                    throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPostIncrementOrDecrementExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new CompilationException(operand.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {operand.ArgumentList.Arguments}");
            if (model.GetSymbolInfo(operand).Symbol is IPropertySymbol property)
            {
                ConvertExpression(model, operand.Expression);
                ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                Call(model, property.GetMethod!, CallingConvention.StdCall);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                AddInstruction(OpCode.REVERSE3);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, property.SetMethod!, CallingConvention.StdCall);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                AddInstruction(OpCode.REVERSE3);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(model, operatorToken, property);
                    break;
                default:
                    throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
        {
            byte index = _localVariables[symbol];
            AccessSlot(OpCode.LDLOC, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken);
            AccessSlot(OpCode.STLOC, index);
        }

        private void ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
        {
            byte index = _parameters[symbol];
            AccessSlot(OpCode.LDARG, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken);
            AccessSlot(OpCode.STARG, index);
        }

        private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, property);
                    break;
                default:
                    throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }



        private void ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertPrefixUnaryExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "+":
                    ConvertExpression(model, expression.Operand);
                    break;
                case "-":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.NEGATE);
                    break;
                case "~":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.INVERT);
                    break;
                case "!":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.NOT);
                    break;
                case "++":
                case "--":
                    ConvertPreIncrementOrDecrementExpression(model, expression);
                    break;
                case "^":
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.SUB);
                    break;
                default:
                    throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPreIncrementOrDecrementExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new CompilationException(operand.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {operand.ArgumentList.Arguments}");
            if (model.GetSymbolInfo(operand).Symbol is IPropertySymbol property)
            {
                ConvertExpression(model, operand.Expression);
                ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                Call(model, property.GetMethod!, CallingConvention.StdCall);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                Call(model, property.SetMethod!, CallingConvention.Cdecl);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.PICKITEM);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                AddInstruction(OpCode.REVERSE3);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(model, operatorToken, property);
                    break;
                default:
                    throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
        {
            byte index = _localVariables[symbol];
            AccessSlot(OpCode.LDLOC, index);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STLOC, index);
        }

        private void ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
        {
            byte index = _parameters[symbol];
            AccessSlot(OpCode.LDARG, index);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STARG, index);
        }

        private void ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, property);
                    break;
                default:
                    throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }
        private void EmitIncrementOrDecrement(SyntaxToken operatorToken)
        {
            AddInstruction(operatorToken.ValueText switch
            {
                "++" => OpCode.INC,
                "--" => OpCode.DEC,
                _ => throw new CompilationException(operatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {operatorToken}")
            });
        }
    }
}
