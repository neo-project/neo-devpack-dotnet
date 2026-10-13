# API documentation inputs

The public DevPack projects generate XML documentation files during a normal
build. The files are emitted beside each assembly and are included in package
outputs by the .NET SDK, so a documentation site can consume the same API
comments as the published packages.

The generated files are build artifacts and are intentionally not checked in.
When adding a public type or member, add its XML summary, parameter, return,
and exception documentation in the source project so package consumers and
future site generation receive the same contract.

The source projects covered by the shared setting are:

- `Neo.SmartContract.Framework`
- `Neo.SmartContract.Analyzer`
- `Neo.Compiler.CSharp`
- `Neo.SmartContract.Testing`
- `Neo.SmartContract.Template`
- `Neo.Disassembler.CSharp`
