# Neo C# Contract Compatibility Profile

## Purpose

Neo smart contracts use C# source syntax, but they execute on NeoVM rather than the .NET runtime. The Neo C# contract compatibility profile defines the exact language, type, API, ABI, and runtime behavior that a contract developer may rely on.

The profile has two goals:

1. Give developers one authoritative answer about whether a construct is supported.
2. Keep the compiler, analyzers, framework, templates, tests, and documentation from silently drifting apart.

This document defines the normative policy for that profile. The machine-readable schema and enforcement described here are delivered incrementally. Until that migration is complete, the existing syntax probes, analyzer descriptors, compiler diagnostics, lowering registrations, runtime tests, and semantic-difference documentation remain inputs that must be reconciled. None of those inputs is authoritative by itself.

The terms MUST, MUST NOT, SHOULD, SHOULD NOT, and MAY describe requirements for profile entries and tooling.

## Scope

The profile covers every construct that can affect emitted contract behavior:

- C# syntax and language-version features;
- source and recursive type shapes;
- constructors, methods, properties, fields, operators, conversions, and overloads;
- framework and supported .NET APIs;
- ABI and manifest constraints;
- compiler lowering and NeoVM execution semantics;
- security and resource-usage constraints that can be determined with high confidence;
- compiler invariants and classified failure behavior.

The profile applies to user-authored and generated source that contributes to a contract, as well as referenced symbols used by that source.

The profile does not attempt to prove arbitrary business logic correct. It also does not govern off-chain test code, deployment applications, or other host-side .NET code unless that code is compiled into the contract.

## Support states

Every capability MUST have exactly one state. Missing entries are unclassified and MUST NOT be treated as supported.

| State | Meaning | Required behavior |
| --- | --- | --- |
| `supported` | The documented construct is accepted and preserves the stated C# behavior. | Analysis is clean, compilation succeeds, and execution evidence covers the behavior. |
| `unsupported` | The construct cannot be represented or executed safely by the current contract toolchain. | A source-located Neo error is reported before lowering, with compiler-side defensive rejection. |
| `partially-supported` | Only explicitly listed forms or contexts are supported. | Supported forms have positive evidence and every excluded form has a source-located diagnostic. |
| `supported-with-different-semantics` | The construct is usable, but observable behavior intentionally differs from standard C# or .NET. | The difference is documented and execution-tested; likely misuse produces a warning or error. |

`supported` MUST NOT be inferred only because Roslyn accepts the source, the compiler does not throw, or one example compiles. Support requires the evidence defined below.

## Capability classes

Each profile entry belongs to one primary class:

| Class | Examples |
| --- | --- |
| Syntax | statements, expressions, patterns, declarations, modifiers |
| Type | primitives, arrays, tuples, generics, nullable wrappers, delegates |
| API | exact constructors, members, overloads, operators, and conversions |
| ABI | exported parameters, return types, events, names, and manifest rules |
| Semantic difference | evaluation order, string behavior, exception behavior, numeric behavior |
| Security | authorization, storage, external-call, and callback constraints |
| Resource usage | unbounded iteration, recursion, dynamic growth, and repeated storage operations |
| Compiler invariant | internal conditions that user source must never reach without a classified diagnostic |

Security and resource entries describe high-confidence contract constraints. Advisory or expensive whole-program checks MAY remain opt-in, but their profile state and confidence must be explicit.

## Capability records

The future machine-readable profile MUST represent the following information for each capability:

- a stable profile ID independent of a diagnostic ID;
- the capability class and support state;
- the relevant C# and Roslyn language versions;
- exact syntax kinds or symbol signatures used for matching;
- allowed and excluded contexts;
- recursive type constraints where applicable;
- the compiler lowering or framework implementation;
- the diagnostic ID, severity, and replacement guidance for rejected use;
- any intentional semantic difference and its rationale;
- the version in which support was introduced, changed, or removed;
- links to positive, negative, and runtime evidence.

API entries MUST identify symbols by metadata identity and full signature. A method name alone is not sufficient because another overload can have different behavior or no lowering implementation.

Type entries MUST define recursive constraints. Validating only the outer declaration is insufficient for arrays, tuples, generic arguments, nullable wrappers, base types, constraints, and delegate signatures.

Partially supported entries MUST enumerate their supported contexts. For example, support for a range expression on one receiver type does not imply support on every indexable type.

## Diagnostic policy

Diagnostics are part of the compatibility contract. IDs remain stable after release and are not reused for unrelated behavior.

| Severity | Policy |
| --- | --- |
| Error | Unsupported constructs, behavior that cannot preserve the profile, invalid ABI, or user-reachable compiler invariants. |
| Warning | Supported code with a material semantic, security, or resource risk that is not necessarily incorrect in every contract. |
| Info | Optional optimization, migration, or style guidance that does not affect correctness. |

Every compatibility diagnostic MUST provide:

- the smallest useful source location;
- a stable ID and category;
- a message that names the unsupported or risky construct;
- an explanation of why it differs under NeoVM;
- a supported alternative when one exists;
- a help link to version-matched documentation.

An unsupported construct remains unsupported if a developer suppresses its analyzer diagnostic. Compiler-side validation MUST still prevent unsupported code from being emitted. Only diagnostics whose underlying behavior remains valid MAY be safely suppressible.

Unexpected compiler failures use a classified internal-error diagnostic and MUST NOT be presented as ordinary unsupported syntax. User source MUST NOT expose a raw `NotImplementedException`, `NotSupportedException`, or unclassified exception.

## Required evidence

Evidence is cumulative. A capability is not complete when only one layer passes.

### Supported

A `supported` entry requires:

1. a positive analyzer test with no Neo error;
2. successful compilation through the public compiler entry point;
3. a runtime test on the production Neo VM implementation;
4. boundary cases for types, conversions, and numeric behavior;
5. comparison with standard C# behavior when the construct is expected to preserve it;
6. user documentation and at least one maintained example when the feature is not self-explanatory.

### Unsupported

An `unsupported` entry requires:

1. a stable analyzer error with source location and guidance;
2. a negative test for every relevant source context;
3. compiler-side defensive validation;
4. proof that malformed or incomplete source does not crash the analyzer or compiler;
5. an entry in the unsupported capability documentation.

### Partially supported

A `partially-supported` entry requires:

1. the complete list of allowed contexts;
2. positive compilation and runtime evidence for each allowed family;
3. negative diagnostics for excluded contexts;
4. tests showing that an allowed form does not accidentally enable a broader form.

### Supported with different semantics

A `supported-with-different-semantics` entry requires:

1. equivalent source executed under standard .NET and NeoVM;
2. a deterministic test recording the difference;
3. documentation of observable behavior and migration guidance;
4. a warning or error when the difference is likely to cause incorrect contract behavior;
5. an explicit rationale for retaining the difference instead of fixing the compiler.

## Entry-point consistency

The same source and profile version MUST produce equivalent Neo diagnostics in:

- an IDE using `Neo.SmartContract.Analyzer`;
- `dotnet build` for an official contract template;
- `nccs` compilation.

Equivalent means the same diagnostic ID, severity, source range, and remediation. Ordering MUST be deterministic. Duplicate Roslyn, analyzer, and compiler reports for the same root cause SHOULD be merged.

Official templates MUST include the analyzer package as a private development asset. The compiler MUST run or consume the same compatibility rules so command-line compilation cannot bypass the profile.

## Ownership

Compatibility changes cross component boundaries and require explicit ownership:

| Area | Responsibility |
| --- | --- |
| Profile | Defines classification, versioning, evidence, and public compatibility guarantees. |
| Compiler | Implements lowering, defensive validation, and classified compiler diagnostics. |
| Analyzer | Reports unsupported or risky source before lowering and supplies safe guidance. |
| Framework | Exposes only APIs whose contract behavior is implemented and tested. |
| Templates | Deliver the matching analyzer and framework versions to every new project. |
| Tests | Prove positive, negative, differential, boundary, and package-consumer behavior. |
| Documentation | Publishes generated support, diagnostic, and semantic-difference catalogs. |

A pull request that changes compatibility MUST identify affected owners. A change is incomplete if it updates one component while leaving another component inconsistent.

## Versioning

The profile has two versions:

- `schemaVersion` changes when the structure or interpretation of profile data changes;
- `profileVersion` changes when the classified contract surface or semantics change.

Each DevPack release MUST publish one profile version shared by its compiler, analyzer, framework, and templates. Package validation MUST reject incompatible profile versions.

Version changes follow these rules:

- adding a newly supported capability increments the compatible feature revision;
- adding a diagnostic for behavior that was already unsupported is a compatible correction;
- removing support or changing observable supported behavior is a breaking profile change;
- changing intentional semantics is breaking unless it corrects behavior that was already documented as a compiler defect;
- diagnostic IDs remain stable even when wording or guidance improves;
- a Roslyn, C# language-version, target-framework, or Neo protocol upgrade requires a compatibility review.

Release notes MUST identify profile changes separately from implementation details.

## Change process

Every compatibility change follows this sequence:

1. Classify the capability and record the intended state.
2. Identify exact syntax kinds, symbols, signatures, and contexts.
3. Add analyzer and compiler behavior appropriate to the state.
4. Add all evidence required for that state.
5. Update generated catalogs, semantic-difference documentation, examples, and migration guidance.
6. Validate the packed analyzer, compiler tool, and official templates as external consumers.
7. Record the profile-version impact in release notes.

Support MUST NOT be added solely by changing documentation from `unsupported` to `supported`. Removing a diagnostic also requires positive compiler and runtime evidence.

Automatic code fixes are held to a stronger rule: fixed output MUST pass analyzers and `nccs`, and the fix MUST preserve documented behavior. If the replacement requires a precision, storage, ABI, gas, or application-policy decision, the diagnostic provides guidance instead of an automatic rewrite.

## CI and release gates

The completed profile system MUST fail CI when:

- a profile record is missing required fields or evidence;
- a diagnostic ID is duplicated, undocumented, or missing release tracking;
- a compiler lowering has no supported profile entry;
- a supported profile entry has no compiler implementation;
- unsupported probes lack an expected Neo diagnostic;
- supported probes report a Neo error or fail compilation;
- a semantic-difference case lacks differential execution evidence;
- compiler, analyzer, framework, or template profile versions differ;
- generated support and diagnostic documentation is stale;
- a language or Roslyn upgrade introduces an unclassified feature;
- analyzer or compiler input reaches an unclassified exception.

Large fuzz campaigns MAY run outside normal CI, but every minimized failure becomes a deterministic regression test that does run in CI.

## Migration from current sources

The current repository already contains useful compatibility information:

- [C# syntax probes](CSharpSyntaxCheckList.md) encode `supported` and `unsupported` syntax cases;
- [unsupported features](csharp-syntax/UnsupportedFeatures.md) summarize known syntax gaps;
- [semantic differences](csharp-syntax/SemanticDifferences.md) record accepted runtime differences;
- [analyzer documentation](../src/Neo.SmartContract.Analyzer/README.md) and release tracking list source diagnostics;
- [compiler diagnostic IDs](../src/Neo.Compiler.CSharp/Diagnostic/DiagnosticId.cs) classify compiler failures;
- compiler lowering registrations and runtime tests provide implementation evidence.

Migration proceeds by inventorying those sources, assigning stable profile IDs, reconciling conflicts, and then generating documentation and validation from the profile. Existing `supported` and `unsupported` probe values remain valid input states, while context-specific probes represent partial support until the schema can encode it directly.

The migration is complete only when maintainers can determine support from the profile, all user entry points enforce it consistently, and every published claim has the required evidence.
