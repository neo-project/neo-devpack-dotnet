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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
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
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }
}
