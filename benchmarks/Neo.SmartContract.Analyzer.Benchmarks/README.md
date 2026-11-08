# Neo analyzer benchmarks

This project measures the complete `NeoAnalyzerSuite` against a representative
contract source. It is intentionally isolated from the normal test solution so
performance runs do not affect package or contract test timings.

Run a short smoke build with:

```shell
dotnet build benchmarks/Neo.SmartContract.Analyzer.Benchmarks/Neo.SmartContract.Analyzer.Benchmarks.csproj
```

Run the benchmark with:

```shell
dotnet run --project benchmarks/Neo.SmartContract.Analyzer.Benchmarks -c Release
```
