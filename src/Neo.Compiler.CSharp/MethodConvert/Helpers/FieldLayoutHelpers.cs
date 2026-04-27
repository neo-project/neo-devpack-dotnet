// Copyright (C) 2015-2026 The Neo Project.
//
// FieldLayoutHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private static int GetInstanceFieldIndex(IFieldSymbol field)
    {
        int index = System.Array.IndexOf(field.ContainingType.GetFields(), field);
        if (index < 0)
            throw new CompilationException(field, DiagnosticId.SyntaxNotSupported, $"Field '{field.Name}' was not found on containing type '{field.ContainingType.Name}'.");
        return index;
    }
}
