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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts an implicit array creation expression to OpCodes.
    /// Implicitly typed arrays are those arrays in which the type of the array is deduced from the element specified in the array initializer.
    /// The implicitly typed arrays are similar to implicitly typed variable.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about implicit array creation expression.</param>
    /// <param name="expression">The syntax representation of the implicit array creation expression statement being converted.</param>
    /// <example>
    /// Below program illustrates how to use 1-Dimensional Implicitly typed array.
    /// <code>
    /// var authorNames = new[] {"Shilpa", "Soniya", "Shivi", "Ritika"};
    /// Runtime.Log("List of Authors is: ");
    /// foreach (var name in authorNames)
    /// {
    ///     Runtime.Log(name);
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// Multidimensional implicitly typed arrays are not supported.
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays#implicitly-typed-arrays">Implicitly typed arrays</seealso>
    private void ConvertImplicitArrayCreationExpression(SemanticModel model, ImplicitArrayCreationExpressionSyntax expression)
    {
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression).ConvertedType!;
        ConvertInitializerExpression(model, type, expression.Initializer);
    }
}
