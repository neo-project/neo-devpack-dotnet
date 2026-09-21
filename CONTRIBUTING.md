# Contributing to Neo DevPack for .NET

Thank you for improving Neo DevPack for .NET. This guide describes the checks expected for changes targeting `master-n3`.

## Before opening a pull request

1. Create a focused branch from the latest `master-n3`.
2. Keep compiler, framework, analyzer, testing, example, and documentation changes in separately reviewable commits when they address separate concerns.
3. Add a regression test for a bug or compatibility change. Update the relevant example or documentation when the public behavior changes.
4. Run the smallest affected test project first, then run the broader solution checks when the change crosses project boundaries.
5. Check the generated artifacts when a contract source or compiler lowering changes.
6. Review the final diff for generated files, package changes, diagnostics, ABI changes, and accidental credentials.

## Build and test

The repository pins the .NET SDK in `global.json`. Install that SDK before running the normal commands:

```shell
dotnet restore
dotnet build ./neo-devpack-dotnet.sln
dotnet test ./tests/Neo.Compiler.CSharp.UnitTests
dotnet test ./tests/Neo.SmartContract.Analyzer.UnitTests
dotnet format --no-restore --verify-no-changes --verbosity minimal
```

Use the project-specific test command for changes limited to one test or example project. Do not commit local build output, coverage files, or generated artifacts unless the affected project requires them.

## Pull request description

A pull request should state:

- the user-visible problem and the resulting behavior;
- the issue or compatibility checklist item it addresses;
- the tests and other validation that were run;
- any generated artifacts, manifest, ABI, diagnostic, or migration impact;
- follow-up work that is intentionally outside the pull request.

Keep the scope focused and make the acceptance evidence easy to reproduce from a clean checkout.

## Security reports

Please use the private reporting process in [SECURITY.md](SECURITY.md) for vulnerabilities. Do not put exploit details, credentials, or private contract data in a public issue or pull request.
