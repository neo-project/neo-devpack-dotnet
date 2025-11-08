extern alias scfx;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler;
using scfx::Neo.SmartContract.Framework;
using scfx::Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.UnitTests.TestInfrastructure;

internal static class CompilationTestHelper
{
    private static IReadOnlyList<string>? s_referenceCache;

    internal static IReadOnlyList<string> GetReferenceSet()
    {
        if (s_referenceCache is not null)
            return s_referenceCache;

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var referencePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(runtimeDir, "System.Runtime.dll"),
            Path.Combine(runtimeDir, "System.Runtime.InteropServices.dll"),
            typeof(object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(System.ComponentModel.DisplayNameAttribute).Assembly.Location,
            typeof(System.Numerics.BigInteger).Assembly.Location,
            typeof(scfx::Neo.SmartContract.Framework.SmartContract).Assembly.Location,
            typeof(scfx::Neo.SmartContract.Framework.Services.Storage).Assembly.Location,
        };

        s_referenceCache = referencePaths.ToArray();
        return s_referenceCache;
    }

    internal static CSharpCompilation CreateAdHocCompilation(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        var references = GetReferenceSet().Select(path => MetadataReference.CreateFromFile(path));
        return CSharpCompilation.Create(
            "AdHoc",
            new[] { tree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    internal static IMethodSymbol GetMethodSymbol(CSharpCompilation compilation, string methodName)
    {
        var tree = compilation.SyntaxTrees.First();
        var model = compilation.GetSemanticModel(tree);
        var method = tree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(m => m.Identifier.ValueText == methodName);
        return (IMethodSymbol)model.GetDeclaredSymbol(method)!;
    }

    internal static IReadOnlyList<CompilationContext> CompileSource(string sourceFile, Action<CompilationOptions>? configure = null)
        => CompileSources(new[] { sourceFile }, configure);

    internal static IReadOnlyList<CompilationContext> CompileSources(IEnumerable<string> sourceFiles, Action<CompilationOptions>? configure = null)
    {
        var files = sourceFiles.ToArray();
        if (files.Length == 0)
            throw new ArgumentException("At least one source file must be provided.", nameof(sourceFiles));

        var engine = CreateEngine(configure);
        var references = GetReferenceSet().Select(path => MetadataReference.CreateFromFile(path));
        try
        {
            return engine.Compile(files, references);
        }
        catch (FormatException) when (engine.Options.SkipContractValidation)
        {
            System.Console.Error.WriteLine("[CompilationTestHelper] FormatException triggered, dumping diagnostics");
            foreach (var file in files)
            {
                var source = File.ReadAllText(file);
                var adhoc = CreateAdHocCompilation(source);
                var diagnostics = adhoc.GetDiagnostics();
                foreach (var diagnostic in diagnostics)
                    System.Console.WriteLine(diagnostic.ToString());
            }

            throw;
        }
    }

    private static CompilationEngine CreateEngine(Action<CompilationOptions>? configure)
    {
        var options = new CompilationOptions
        {
            EnableHir = true,
            Nullable = NullableContextOptions.Enable,
            Optimize = CompilationOptions.OptimizationType.Basic,
            SkipContractValidation = true
        };

        configure?.Invoke(options);
        System.Console.WriteLine($"[CompilationTestHelper] SkipContractValidation={options.SkipContractValidation}");
        return new CompilationEngine(options);
    }
}
