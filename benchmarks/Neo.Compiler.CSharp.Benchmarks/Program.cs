// Copyright (C) 2015-2026 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

// The compiler references the Neo toolchain assemblies, which are shipped as
// executable tooling and can trigger BenchmarkDotNet's dependency validator.
var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
BenchmarkRunner.Run<CompilerBenchmarks>(config);

public class CompilerBenchmarks
{
    private string _sourcePath = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        _sourcePath = Path.Combine(Path.GetTempPath(), $"neo-devpack-compiler-benchmark-{Guid.NewGuid():N}.cs");
        File.WriteAllText(_sourcePath, """
            using Neo.SmartContract.Framework;

            public class BenchmarkContract : SmartContract
            {
                public static int Calculate(int value)
                {
                    var total = 0;
                    for (var index = 0; index < 16; index++)
                    {
                        total += value * index;
                    }
                    return total;
                }
            }
            """);
    }

    [Benchmark]
    public int CompileContract()
    {
        var options = new CompilationOptions
        {
            Debug = CompilationOptions.DebugType.Extended,
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };
        var contexts = new CompilationEngine(options).CompileSources(_sourcePath);
        return contexts.Single().CreateExecutable().Script.Length;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (File.Exists(_sourcePath))
            File.Delete(_sourcePath);
    }
}
