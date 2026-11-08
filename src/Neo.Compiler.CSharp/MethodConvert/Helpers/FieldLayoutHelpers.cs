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
        int tupleIndex = GetTupleElementIndex(field.ContainingType, field);
        if (tupleIndex >= 0)
            return tupleIndex;
        int index = System.Array.IndexOf(field.ContainingType.GetFields(), field);
        if (index < 0)
            throw new CompilationException(field, DiagnosticId.SyntaxNotSupported, $"Field '{field.Name}' was not found on containing type '{field.ContainingType.Name}'.");
        return index;
    }

    /// <summary>
    /// Returns the storage index of a tuple element, or -1 when the field is not an element of the
    /// given tuple type.
    /// </summary>
    /// <remarks>
    /// Tuples are packed in element order. Their member list can also include named aliases,
    /// so member positions must not be used as storage indices. Both a named element and its
    /// corresponding ItemN field resolve to the same slot.
    /// </remarks>
    private static int GetTupleElementIndex(ITypeSymbol? receiverType, IFieldSymbol field)
    {
        if (receiverType is not INamedTypeSymbol { IsTupleType: true } tuple)
            return -1;

        System.Collections.Immutable.ImmutableArray<IFieldSymbol> elements = tuple.TupleElements;
        for (int i = 0; i < elements.Length; i++)
            if (SymbolEqualityComparer.Default.Equals(elements[i], field) ||
                SymbolEqualityComparer.Default.Equals(
                    elements[i].CorrespondingTupleField ?? elements[i], field.CorrespondingTupleField ?? field))
                return i;
        return -1;
    }

    /// <summary>
    /// Returns the storage index of a field, taking tuple element ordering into account.
    /// </summary>
    /// <param name="receiverType">The type of the expression the field is read from, if it is known.</param>
    /// <param name="field">The field being accessed.</param>
    private static int GetFieldIndex(ITypeSymbol? receiverType, IFieldSymbol field)
    {
        int tupleIndex = GetTupleElementIndex(receiverType, field);
        return tupleIndex >= 0 ? tupleIndex : GetInstanceFieldIndex(field);
    }
}
