// Copyright (C) 2015-2026 The Neo Project.
//
// TupleShapeProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Prints how Roslyn represents tuple types, which drives the struct copy field order.
/// </summary>
public static class TupleShapeProbe
{
    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>Zero.</returns>
    public static int Run()
    {
        const string source = """
            public static class Shapes
            {
                public static void Pair() { (int, int) t = default; }
                public static void Seven() { (int, int, int, int, int, int, int) t = default; }
                public static void Eight() { (int, int, int, int, int, int, int, int) t = default; }
            }
            """;

        ImmutableArray<MetadataReference> references = CreateReferences();
        SyntaxTree tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        CSharpCompilation compilation = CSharpCompilation.Create(
            "TupleShapes",
            [tree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        SemanticModel model = compilation.GetSemanticModel(tree);
        foreach (Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method in tree.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax>())
        {
            var local = method.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax>().FirstOrDefault();
            if (local is null) continue;

            ITypeSymbol? type = model.GetTypeInfo(local.Type).Type;
            if (type is not INamedTypeSymbol named) continue;

            Console.WriteLine($"{method.Identifier.ValueText}: {named} isTuple={named.IsTupleType} " +
                              $"tupleElements={named.TupleElements.Length} " +
                              $"fields=[{string.Join(", ", named.GetMembers().OfType<IFieldSymbol>().Select(f => f.Name))}]");

            if (named.IsTupleType)
            {
                Console.WriteLine($"    elements=[{string.Join(", ", named.TupleElements.Select(e => $"{e.Name}:{e.Type}"))}]");
                Console.WriteLine($"    underlying={named.TupleUnderlyingType}");
            }
        }

        return 0;
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa && !string.IsNullOrWhiteSpace(tpa))
        {
            foreach (string path in tpa.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                if (File.Exists(path))
                    paths.Add(path);
        }

        return paths
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .Select(static p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToImmutableArray();
    }
}
