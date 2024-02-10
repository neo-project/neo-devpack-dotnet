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
    private void ConvertIsPatternExpression(SemanticModel model, IsPatternExpressionSyntax expression)
    {
        byte anonymousIndex = AddAnonymousVariable();
        ConvertExpression(model, expression.Expression);
        AccessSlot(OpCode.STLOC, anonymousIndex);
        ConvertPattern(model, expression.Pattern, anonymousIndex);
        RemoveAnonymousVariable(anonymousIndex);
    }
}
