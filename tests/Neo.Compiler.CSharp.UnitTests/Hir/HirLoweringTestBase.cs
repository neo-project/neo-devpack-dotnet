extern alias scfx;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Import;
using Neo.Compiler.MIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

internal static class HirLoweringTestBase
{
    public static HirFunction LowerMethod(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create(
            "HIRTests",
            new[] { tree },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Numerics.BigInteger).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(scfx::Neo.SmartContract.Framework.Services.Storage).Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var model = compilation.GetSemanticModel(tree, ignoreAccessibility: true);
        var root = tree.GetCompilationUnitRoot();
        var targetType = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(type => type.Members.OfType<MethodDeclarationSyntax>().Any());
        var method = targetType.Members
            .OfType<MethodDeclarationSyntax>()
            .First();

        var symbol = model.GetDeclaredSymbol(method)!;

        var engine = new CompilationEngine(new CompilationOptions { EnableHir = true });
        engine.Compilation = compilation;
        var context = new CompilationContext(engine, symbol.ContainingType, allowBaseName: true);
        var importer = new HirMethodImporter(context, symbol, model);
        var builder = importer.Import();

        return builder.Function;
    }

    public static HirModule ImportClass(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create(
            "HIRTests",
            new[] { tree },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Numerics.BigInteger).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(scfx::Neo.SmartContract.Framework.Services.Storage).Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var model = compilation.GetSemanticModel(tree, ignoreAccessibility: true);
        var root = tree.GetCompilationUnitRoot();
        var targetType = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(type => type.Members.OfType<MethodDeclarationSyntax>().Any());
        var targetSymbol = model.GetDeclaredSymbol(targetType)!;

        var engine = new CompilationEngine(new CompilationOptions { EnableHir = true });
        engine.Compilation = compilation;
        var context = new CompilationContext(engine, targetSymbol, allowBaseName: true);

        foreach (var method in targetType.Members.OfType<MethodDeclarationSyntax>())
        {
            var methodSymbol = model.GetDeclaredSymbol(method)!;
            var importer = new HirMethodImporter(context, methodSymbol, model);
            importer.Import();
        }

        return context.HirModule!;
    }
}
