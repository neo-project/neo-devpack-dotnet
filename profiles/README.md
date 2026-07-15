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

## Updating the schema

Treat the schema, its complete fixture, and its tests as one versioned
contract. A schema change should follow this process:

1. Update `neo-csharp-profile.schema.json` from the latest `master-n3`.
2. Keep `schemaVersion` unchanged for descriptions, examples, and test-only
   changes that do not change which profile documents are accepted. Increment
   it when the accepted document shape or the meaning of a field changes,
   including new or removed properties, enum values, evidence kinds, or
   state-specific requirements.
3. Update `ContractProfile.valid.json` so the complete fixture exercises every
   new field, state, or evidence rule.
4. Add both a valid case and a focused invalid mutation to
   `ContractProfileSchemaTests` for every new rule. A rule that can only be
   demonstrated by an invalid document still needs a valid fixture path that
   reaches it.
5. Preserve strict validation unless the format intentionally changes. In
   particular, do not weaken `additionalProperties`, stable capability ID
   syntax, semantic-version validation, or the evidence required by each
   support state.
6. Run the focused schema tests above and describe the compatibility impact in
   the pull request. If `schemaVersion` changes, include migration guidance for
   existing profile documents and consumers.

`profileVersion` and `devpackVersion` belong to profile documents, not to the
schema definition. A schema-only clarification does not change them. A future
capability inventory change should keep existing capability IDs stable, update
the relevant evidence references, and advance `profileVersion` when the
classified contract surface changes.

The complete capability inventory will be added separately after existing
compiler, analyzer, syntax-probe, and semantic-difference sources are
reconciled.
