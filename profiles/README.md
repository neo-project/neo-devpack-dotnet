# Neo C# Contract Profiles

This directory contains the machine-readable compatibility contract shared by
the Neo C# compiler, analyzers, documentation, and tests.

`neo-csharp-profile.schema.json` is a JSON Schema Draft 2020-12 document. It
defines the shape and minimum evidence for a profile inventory; it does not
itself claim that any C# capability is currently supported.

## Document shape

A profile document contains:

- `schemaVersion`: the structural version of the profile format;
- `profileVersion`: the semantic version of the classified contract surface;
- `devpackVersion`: the matching DevPack release;
- `capabilities`: an object keyed by stable profile IDs.

Using capability IDs as object keys prevents duplicate IDs in a valid JSON
object. IDs are independent of diagnostic IDs so that supported capabilities
and capabilities without diagnostics still have stable identities.

## Support states

The schema recognizes four states:

- `supported` requires analyzer-positive, compiler, and runtime evidence;
- `unsupported` requires an error diagnostic plus analyzer-negative and
  compiler evidence;
- `partially-supported` requires explicit allowed and excluded contexts,
  positive and negative analysis, compiler evidence, and runtime evidence;
- `supported-with-different-semantics` requires implementation, a documented
  semantic difference, and analyzer-positive, compiler, runtime, and
  differential evidence.

All capabilities also identify an exact matching surface through syntax kinds,
operation kinds, metadata names, member signatures, ABI elements, or compiler
invariants.

## Validation

The schema tests use a fixture that exercises every support state, mutate it to
verify that missing evidence, invalid IDs, unsafe severities, unknown states,
and undeclared properties are rejected, and exercise valid and invalid SemVer
2.0 versions directly.

Run them with:

```shell
cd tests/Neo.SmartContract.Analyzer.UnitTests
dotnet test Neo.SmartContract.Analyzer.UnitTests.csproj \
  --filter "FullyQualifiedName~ContractProfileSchemaTests"
```

The complete capability inventory will be added separately after existing
compiler, analyzer, syntax-probe, and semantic-difference sources are
reconciled.
