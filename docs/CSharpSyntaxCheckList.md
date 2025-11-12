# C# Syntax Checklists

The syntax checklist has been reorganized into C# version specific documents. Each entry provides a short example along with the compiler support status. The automated syntax probe tests load these files directly, ensuring the documentation and test coverage remain consistent.

## Version Index

- [C# 1](csharp-syntax/csharp-1.md)
- [C# 2](csharp-syntax/csharp-2.md)
- [C# 3](csharp-syntax/csharp-3.md)
- [C# 4](csharp-syntax/csharp-4.md)
- [C# 5](csharp-syntax/csharp-5.md)
- [C# 6](csharp-syntax/csharp-6.md)
- [C# 7](csharp-syntax/csharp-7.md)
- [C# 8](csharp-syntax/csharp-8.md)
- [C# 9](csharp-syntax/csharp-9.md)
- [C# 10](csharp-syntax/csharp-10.md)
- [C# 11](csharp-syntax/csharp-11.md)
- [C# 12](csharp-syntax/csharp-12.md)
- [C# 13](csharp-syntax/csharp-13.md)

## How the probes work

- Each checklist entry includes a unique identifier, support status, scope (method, class, or file), and an example snippet.
- The syntax probe unit tests parse these files and compile the snippets with `Neo.Compiler.CSharp`.
- Supported entries must compile successfully. Unsupported entries are expected to fail, catching regressions as the compiler evolves.

See [Unsupported C# Features in Neo Compiler](csharp-syntax/UnsupportedFeatures.md) for a consolidated list of gaps.

To add new syntax coverage:

1. Append a new section to the relevant version file following the established format.
2. Update `docs/csharp-syntax/UnsupportedFeatures.md` so every unsupported entry remains listed.
3. Run the probe suite via\
   `dotnet test tests/Neo.Compiler.CSharp.UnitTests/Neo.Compiler.CSharp.UnitTests.csproj --filter "ClassName=Neo.Compiler.CSharp.UnitTests.Syntax.SyntaxTests"`.
4. Update the release notes or documentation as needed.
