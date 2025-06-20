// Copyright (C) 2015-2025 The Neo Project.
//
// DeclarationPattern.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convet declaration pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about declaration pattern.</param>
    /// <param name="pattern">The declaration pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// With a declaration pattern, you can also declare a new local variable.
    /// When a declaration pattern matches an expression, that variable is assigned a converted expression result,
    /// as the following example shows:
    /// <code>
    /// object greeting = "Hello, World!";
    /// if (greeting is string message)
    /// {
    ///     Runtime.Log(message);
    /// }
    /// object greeting2 = "Hello, World!";
    /// if (greeting2 is string _)
    /// {
    ///     Runtime.Log("greeting2 is string");
    /// }
    /// </code>
    /// <c>string message</c> is DiscardDesignationSyntax, <c>string _</c> is SingleVariableDesignationSyntax.
    /// </example>
    private void ConvertDeclarationPattern(SemanticModel model, DeclarationPatternSyntax pattern, byte localIndex)
    {
        ITypeSymbol type = model.GetTypeInfo(pattern.Type).Type!;
        AccessSlot(OpCode.LDLOC, localIndex);
        IsType(type.GetPatternType());
        switch (pattern.Designation)
        {
            case DiscardDesignationSyntax:
                break;
            case SingleVariableDesignationSyntax variable:
                ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(variable)!;
                byte index = AddLocalVariable(local);
                AccessSlot(OpCode.LDLOC, localIndex);
                AccessSlot(OpCode.STLOC, index);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(pattern, $"Declaration pattern designation type '{pattern.Designation.GetType().Name}' is not supported. Only single variable (e.g., 'string message') and discard (e.g., 'string _') designations are allowed.");
        }
    }
}
