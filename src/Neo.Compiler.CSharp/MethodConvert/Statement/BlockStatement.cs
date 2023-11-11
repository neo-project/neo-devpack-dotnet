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
using System.Collections.Generic;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertBlockStatement(SemanticModel model, BlockSyntax syntax)
    {
        _blockSymbols.Push(new List<ILocalSymbol>());
        using (InsertSequencePoint(syntax.OpenBraceToken))
            AddInstruction(OpCode.NOP);
        foreach (StatementSyntax child in syntax.Statements)
            ConvertStatement(model, child);
        using (InsertSequencePoint(syntax.CloseBraceToken))
            AddInstruction(OpCode.NOP);
        foreach (ILocalSymbol symbol in _blockSymbols.Pop())
            RemoveLocalVariable(symbol);
    }
}
