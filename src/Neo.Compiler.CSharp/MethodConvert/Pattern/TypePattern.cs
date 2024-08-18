// Copyright (C) 2015-2024 The Neo Project.
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

internal partial class MethodConvert
{
    /// <summary>
    /// Convet type pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about type pattern.</param>
    /// <param name="pattern">The type pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// <code>
    /// public void M(object o1)
    /// {
    ///     switch (o1)
    ///     {
    ///         case byte[]: break;
    ///         case string: break;
    ///     }
    /// }
    /// </code>
    /// <c>byte[]</c> and <c>string</c> is TypePatternSyntax.
    /// </example>
    /// <remarks>
    /// Only few type judgments are supported, such as: bool, byte[], string,
    /// Not supported ByteString, BigInteger.
    /// <see cref="Helper.GetPatternType(ITypeSymbol)"/>
    /// </remarks>
    private void ConvertTypePattern(SemanticModel model, TypePatternSyntax pattern, byte localIndex)
    {
        ITypeSymbol type = model.GetTypeInfo(pattern.Type).Type!;
        AccessSlot(OpCode.LDLOC, localIndex);
        IsType(type.GetPatternType());
    }
}
