// Copyright (C) 2015-2026 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications are permitted.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Neo.SmartContract.Analyzer;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
BenchmarkRunner.Run<AnalyzerBenchmarks>(config);

public class AnalyzerBenchmarks
{
    private CSharpCompilation _compilation = null!;
    private ImmutableArray<Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer> _analyzers;

    [GlobalSetup]
    public void Setup()
    {
        const string source = """
            using System;
            using System.Numerics;
            using System.Text;

            public class BenchmarkContract
            {
                public static int Analyze(int value)
                {
                    var builder = new StringBuilder();
                    builder.Append(value.ToString());
                    return builder.Length;
                }

                public static DateTime ReadClock() => DateTime.UtcNow;
                public static float Unsupported(float value) => value;
                public static BigInteger Add(BigInteger left, BigInteger right) => left + right;
            }
            """;

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToImmutableArray();
        _compilation = CSharpCompilation.Create(
            "NeoAnalyzerBenchmark",
            [CSharpSyntaxTree.ParseText(source, path: "BenchmarkContract.cs")],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
        _analyzers = NeoAnalyzerSuite.Create();
    }

    [Benchmark]
    public int AnalyzeContract()
    {
        return _compilation
            .WithAnalyzers(_analyzers)
            .GetAnalyzerDiagnosticsAsync()
            .GetAwaiter()
            .GetResult()
            .Length;
    }
}
