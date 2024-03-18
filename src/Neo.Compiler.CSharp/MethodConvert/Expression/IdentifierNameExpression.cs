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

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// This method converts an identifier name expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about identifier name expression.</param>
    /// <param name="expression">The syntax representation of the identifier name expression statement being converted.</param>
    /// <exception cref="CompilationException">Unsupported symbols will result in a compilation exception.</exception>
    /// <example>
    /// Processing of the identifier "param" goes to the "IParameterSymbol parameter" branch of the code;
    /// processing of the identifier "temp" goes to the "ILocalSymbol local" branch of the code.
    /// Unused identifier "param2" will not be processed.
    /// <code>
    /// public static void MyMethod(int param, int param2)
    /// {
    ///     int temp = 0;
    ///     byte[] temp2 = new byte[1];
    ///     Runtime.Log(temp.ToString());
    ///     Runtime.Log(temp2[0].ToString());
    ///     Runtime.Log(param.ToString());
    /// }
    /// </code>
    ///  The <c>symbol.Name</c> of the above code is as follows: "temp", "temp2", "param";
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names">C# identifier naming rules and conventions</seealso>
    private void ConvertIdentifierNameExpression(SemanticModel model, IdentifierNameSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    AddInstruction(OpCode.LDARG0);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case ILocalSymbol local:
                if (local.IsConst)
                    Push(local.ConstantValue);
                else
                    AccessSlot(OpCode.LDLOC, _localVariables[local]);
                break;
            case IMethodSymbol method:
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                MethodConvert convert = _context.ConvertMethod(model, method);
                Jump(OpCode.PUSHA, convert._startTarget);
                break;
            case IParameterSymbol parameter:
                if (!_internalInline)
                    AccessSlot(OpCode.LDARG, _parameters[parameter]);
                break;
            case IPropertySymbol property:
                if (property.IsStatic)
                {
                    Call(model, property.GetMethod!);
                }
                else
                {
                    AddInstruction(OpCode.LDARG0);
                    Call(model, property.GetMethod!);
                }
                break;
            case ITypeSymbol type:
                IsType(type.GetPatternType());
                Push(true);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
