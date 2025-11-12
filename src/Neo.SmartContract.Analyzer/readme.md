# Neo Smart Contract Roslyn Analyzers

`Neo.SmartContract.Analyzer` bundles Roslyn analyzers and code fixes that enforce NeoVM constraints, NEP standards, and high-signal best practices before you deploy a contract.

## Highlights

- **Type Safety** – blocks unsupported primitives such as `float`, `double`, `decimal`, risky `System.Math`/`string`/`char` helpers, and unsafe `BigInteger` construction.
- **BCL Guardrails** – warns when contracts use LINQ, generic collections, `System.Diagnostics`, or other runtime-only APIs.
- **Contract Shape** – validates `SupportedStandards`, method naming, event names, static-field initialization, and NEP boilerplate.
- **Error Handling** – restricts multiple `catch` blocks, `ref`/`volatile`, and suspect cast patterns so on-chain code stays deterministic.

## Analyzer catalog

- [BanCastMethodAnalyzer.cs](BanCastMethodAnalyzer.cs) – disallows unsupported cast helper methods that NeoVM cannot execute.
- [BigIntegerCreationAnalyzer.cs](BigIntegerCreationAnalyzer.cs) – enforces safe `BigInteger` construction patterns.
- [BigIntegerUsageAnalyzer.cs](BigIntegerUsageAnalyzer.cs) – blocks unsupported `BigInteger` APIs.
- [BigIntegerUsingUsageAnalyzer.cs](BigIntegerUsingUsageAnalyzer.cs) – prevents using statements that dispose `BigInteger` in unsupported ways.
- [CatchOnlySystemExceptionAnalyzer.cs](CatchOnlySystemExceptionAnalyzer.cs) – constrains which exception types contracts can catch.
- [CharMethodsUsageAnalyzer.cs](CharMethodsUsageAnalyzer.cs) – forbids `char` helpers that are not available on NeoVM.
- [CollectionTypesUsageAnalyzer.cs](CollectionTypesUsageAnalyzer.cs) – blocks use of `List<T>`, `Dictionary<TKey,TValue>`, and similar containers.
- [DecimalUsageAnalyzer.cs](DecimalUsageAnalyzer.cs) – rejects `decimal`.
- [DoubleUsageAnalyzer.cs](DoubleUsageAnalyzer.cs) – rejects `double`.
- [EnumMethodsUsageAnalyzer.cs](EnumMethodsUsageAnalyzer.cs) – flags unsupported `Enum` helper calls.
- [FloatUsageAnalyzer.cs](FloatUsageAnalyzer.cs) – rejects `float`.
- [InitialValueAnalyzer.cs](InitialValueAnalyzer.cs) – suggests literal initialization where attributes are unsupported.
- [KeywordUsageAnalyzer.cs](KeywordUsageAnalyzer.cs) – guards against restricted C# keywords.
- [LinqUsageAnalyzer.cs](LinqUsageAnalyzer.cs) – disallows LINQ extension usage.
- [MultipleCatchBlockAnalyzer.cs](MultipleCatchBlockAnalyzer.cs) – restricts multiple `catch` clauses in a single `try`.
- [NepStandardAnalyzer.cs](NepStandardAnalyzer.cs) – validates `SupportedStandards` attribute content.
- [NepStandardImplementationAnalyzer.cs](NepStandardImplementationAnalyzer.cs) – verifies NEP interface implementations line up with declared standards.
- [NotifyEventNameAnalyzer.cs](NotifyEventNameAnalyzer.cs) – enforces consistent event names passed to `Notify`.
- [RefKeywordUsageAnalyzer.cs](RefKeywordUsageAnalyzer.cs) – blocks `ref` usage in contracts.
- [SmartContractMethodNamingAnalyzer.cs](SmartContractMethodNamingAnalyzer.cs) – enforces allowed ABI method names.
- [SmartContractMethodNamingAnalyzer.Underline.cs](SmartContractMethodNamingAnalyzer.Underline.cs) – forbids underscores in public contract methods.
- [StaticFieldInitializationAnalyzer.cs](StaticFieldInitializationAnalyzer.cs) – ensures static fields are initialized deterministically.
- [StringBuilderUsageAnalyzer.cs](StringBuilderUsageAnalyzer.cs) – restricts `StringBuilder` APIs to supported members.
- [StringMethodUsageAnalyzer.cs](StringMethodUsageAnalyzer.cs) – flags unsupported `string` helpers.
- [SystemDiagnosticsUsageAnalyzer.cs](SystemDiagnosticsUsageAnalyzer.cs) – blocks `System.Diagnostics` usages.
- [SystemMathUsageAnalyzer.cs](SystemMathUsageAnalyzer.cs) – blocks `System.Math` members that are not NeoVM friendly.
- [VolatileKeywordUsageAnalyzer.cs](VolatileKeywordUsageAnalyzer.cs) – rejects the `volatile` keyword.

## Install

### NuGet (recommended)

```bash
dotnet add package Neo.SmartContract.Analyzer --prerelease
```

or add to the project (or `Directory.Build.props`) and keep it private to the contract:

```xml
<ItemGroup>
  <PackageReference Include="Neo.SmartContract.Analyzer" Version="*"
                    PrivateAssets="all"
                    IncludeAssets="runtime; build; native; contentfiles; analyzers; buildtransitive" />
</ItemGroup>
```

### Source reference

When the analyzer lives in the same repo, reference it directly so you can debug rules:

```xml
<ItemGroup>
  <ProjectReference Include="..\path\to\Neo.SmartContract.Analyzer.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## Using the diagnostics

1. Build or save the contract project—diagnostics show up in VS/VS Code, `dotnet build`, and CI.
2. Treat analyzer output as build failures:

```bash
dotnet build /warnaserror
```

```
dotnet_analyzer_diagnostic.category-Nsc = error
dotnet_diagnostic.NSC001.severity = warning
```

3. Only suppress warnings with `#pragma warning disable` when you fully understand the trade-off.

## CI tips

- Restore/install the analyzer before `dotnet build /warnaserror` so unsupported APIs never land on `dev`.
- Upload build logs or SARIF reports so the whole team can track rule regressions.

## Contributing

Add new rules, refine diagnostics, or update docs—just run `dotnet test Neo.SmartContract.Analyzer.UnitTests` before opening a PR.

## License

MIT – see [`LICENSE`](LICENSE).
