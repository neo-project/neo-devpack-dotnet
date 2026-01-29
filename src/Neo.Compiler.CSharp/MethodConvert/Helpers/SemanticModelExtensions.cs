// Copyright (C) 2015-2026 The Neo Project.
//
// SemanticModelExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.Compiler;

/// <summary>
/// Extension methods for SemanticModel to handle partial classes correctly.
/// </summary>
internal static class SemanticModelExtensions
{
    /// <summary>
    /// Gets the correct semantic model for a given syntax node.
    /// This is necessary when dealing with partial classes where syntax nodes
    /// may come from different syntax trees than the current semantic model.
    /// </summary>
    public static SemanticModel GetModelForNode(this SemanticModel model, SyntaxNode node)
    {
        if (model.SyntaxTree.Equals(node.SyntaxTree))
            return model;
        return model.Compilation.GetSemanticModel(node.SyntaxTree);
    }
}
