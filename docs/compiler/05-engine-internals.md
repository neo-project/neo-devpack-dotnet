# Compilation Engine Internals

The `CompilationEngine` is the central component within the Neo C# Compiler (`nccs`) that orchestrates the entire compilation process, from parsing C# source code to generating the final `.nef` file and associated artifacts.

## Core Responsibilities

The `CompilationEngine` is responsible for:

1.  **Loading Source Code:** Reading C# source files (`.cs`) or entire C# projects (`.csproj`).
2.  **Setting up Compilation Environment:** Integrating with the .NET Compiler Platform (Roslyn) to create a `Compilation` object. This involves:
    *   Parsing source code into Abstract Syntax Trees (ASTs).
    *   Resolving dependencies (NuGet packages, project references) using `project.assets.json` and MSBuild information.
    *   Creating `SemanticModel` instances for each syntax tree, enabling deep code analysis.
3.  **Identifying Smart Contracts:** Locating public, non-abstract classes within the compilation that inherit from `Neo.SmartContract.Framework.SmartContract`.
4.  **Managing Compilation Contexts:** Creating a `CompilationContext` for each identified smart contract class. Each context manages the state and artifacts for compiling a single contract.
5.  **Handling Contract Dependencies:** Detecting dependencies between smart contracts within the same compilation (e.g., one contract referencing another via a static field or property). It performs a topological sort to ensure contracts are compiled in the correct order, preventing issues with cyclic dependencies.
6.  **Orchestrating Conversion:** Triggering the compilation process within each `CompilationContext`, which in turn uses components like `MethodConvert`, `TypeConvert`, etc., to translate C# code into NeoVM instructions.
7.  **Collecting Results:** Gathering the generated NEF bytecode, manifest, debug information, and diagnostics (errors/warnings) from each `CompilationContext`.
8.  **Output Generation:** Coordinating the writing of the final output files (`.nef`, `.manifest.json`, `.nefdbgnfo`, `.asm`, etc.) based on the specified compiler options.

## Key Components & Workflow

1.  **Initialization:** A `CompilationEngine` instance is created with `CompilationOptions`.
2.  **Input Processing:** The engine receives input paths (source files, project files, or directories).
3.  **Roslyn Integration:**
    *   It parses source files into `SyntaxTree` objects.
    *   For projects, it parses the `.csproj` file, restores dependencies (`dotnet restore`), reads the `project.assets.json` file, and resolves necessary `MetadataReference` objects for referenced assemblies (like `Neo.SmartContract.Framework`).
    *   A Roslyn `Compilation` object is created, representing the entire set of code and references.
4.  **Contract Discovery & Ordering:**
    *   The engine iterates through the syntax trees and semantic models to find classes derived from `SmartContract`.
    *   It builds a dependency graph if contracts reference each other.
    *   A topological sort ensures compilation order respects these dependencies.
5.  **Per-Contract Compilation:**
    *   For each contract (in the sorted order), a `CompilationContext` is created.
    *   The `CompilationContext.Compile()` method is invoked.
    *   Inside the context, `MethodConvert` is used for each method, `FieldConvert` for fields, etc. These converters interact with the semantic model and emit NeoVM instructions.
6.  **Result Aggregation:** The `CompilationEngine` collects the `CompilationContext` results.
7.  **Output Writing (handled by `Program.cs`):** The main program logic takes the results from the engine and writes the `.nef`, `.manifest.json`, and other files to the designated output directory.

## Role of `CompilationContext`

While the `CompilationEngine` manages the overall process and multiple contracts, the `CompilationContext` focuses on a *single* smart contract. It holds the state specific to that contract's compilation, including:

*   The target contract's `INamedTypeSymbol`.
*   Lists of methods, events, and fields to be included.
*   The generated NeoVM script (`byte[]`).
*   The contract manifest details.
*   Debug information mappings.
*   Any diagnostics (errors/warnings) encountered during its compilation.

Understanding the `CompilationEngine` provides insight into how `nccs` handles different input types, manages dependencies, and coordinates the complex task of translating a .NET project into executable Neo smart contracts.
