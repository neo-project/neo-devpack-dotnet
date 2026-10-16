# Compiler benchmarks

This project provides a repeatable BenchmarkDotNet entry point for measuring
the end-to-end C# contract compilation path. It is intentionally outside the
production solution and is run explicitly so normal library and test builds do
not pay the benchmark dependency or execution cost.

Run it from the repository root with:

```shell
dotnet run --project benchmarks/Neo.Compiler.CSharp.Benchmarks -c Release
```

The benchmark reports the generated script length as its return value and
measures the full `CompilationEngine` path with `All` optimizations enabled.
