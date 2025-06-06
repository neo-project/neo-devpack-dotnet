# Compiler Maintenance Guide (`Neo.Compiler.CSharp`)

This guide provides recommendations and insights for developers maintaining or contributing to the Neo C# Compiler (`nccs`). Understanding the compiler's architecture and following best practices is crucial for ensuring its stability, correctness, and performance.

## Code Structure Overview

Familiarize yourself with the main components:

*   **`CompilationEngine/`**: Orchestrates the overall compilation process, manages contexts for individual contracts, and integrates with Roslyn.
    *   `CompilationEngine.cs`: The main entry point for driving compilation.
    *   `CompilationContext.cs`: Manages the state and results for compiling a single contract (methods, fields, manifest details, diagnostics).
*   **`MethodConvert/`**: Handles the core translation of C# method bodies, statements, and expressions into NeoVM bytecode. Contains subdirectories for specific constructs (e.g., `Expression/`, `Statement/`).
*   **`ABI/`**: Defines structures (`AbiMethod`, `AbiEvent`) used for building the contract manifest's ABI section.
*   **`Optimizer/`**: Contains logic for optimizing the generated NeoVM bytecode. Includes basic optimizations and potentially more advanced strategies.
*   **`SecurityAnalyzer/`**: Implements static analysis checks for common security pitfalls.
*   **`Diagnostics/`**: (If present or relevant) Defines error/warning codes and messages.
*   **`Program.cs`**: Handles command-line argument parsing and orchestrates the overall execution flow, including calling the `CompilationEngine` and writing output files.
*   **`Options.cs`**: Defines the command-line options.

## Key Technologies & Concepts

*   **Roslyn (.NET Compiler Platform):** The compiler heavily relies on Roslyn for:
    *   Parsing C# source code (`SyntaxTree`).
    *   Semantic analysis (`SemanticModel`, `ISymbol` hierarchy - e.g., `IMethodSymbol`, `IFieldSymbol`, `ITypeSymbol`).
    *   Understanding code structure, type information, and method calls.
    *   *Maintenance Tip:* A good understanding of Roslyn APIs is essential for modifying code analysis or adding support for new C# features.
*   **NeoVM:** Deep knowledge of NeoVM is critical:
    *   Opcodes and their behavior.
    *   Stack manipulation rules.
    *   Memory management (`INITSLOT`, etc.).
    *   Syscalls (mapping framework methods to `SYSCALL` hashes).
    *   GAS costs associated with instructions.
    *   *Maintenance Tip:* Changes in code generation must produce valid and efficient NeoVM bytecode. Refer to NeoVM specifications.
*   **Neo Smart Contract Framework (`Neo.SmartContract.Framework`):** The compiler must correctly interpret and map framework elements:
    *   Attributes (`[DisplayName]`, `[SupportedStandards]`, `[Safe]`, `[ContractPermission]`, etc.) drive manifest generation and behavior.
    *   Framework method calls often map directly to NeoVM Syscalls.
    *   *Maintenance Tip:* Keep the compiler synchronized with changes or additions to the Framework. New framework features might require compiler updates.

## Development & Maintenance Practices

1.  **Testing is Crucial:**
    *   **Unit Tests:** Add unit tests for specific conversion logic (e.g., how a specific C# expression translates to opcodes), diagnostic reporting, or optimization patterns.
    *   **Integration Tests:** Maintain comprehensive integration tests that compile whole C# contracts (covering various language features and framework APIs) and verify:
        *   Correctness of generated `.nef` bytecode (often by executing it in a `TestEngine`).
        *   Accuracy of the `.manifest.json` file.
        *   Validity of the `.debug.json` information.
        *   Expected diagnostics (errors/warnings).
    *   *Maintenance Tip:* Aim for high test coverage. Every bug fix or new feature should be accompanied by relevant tests.

2.  **Understand the Conversion Flow:** When modifying code generation (primarily in `MethodConvert/`), trace how C# syntax nodes are visited and how corresponding `Instruction` objects are emitted.

3.  **Manage State Carefully:** Be mindful of the state managed within `MethodConvert` (scopes, variable indices, jump targets, try/catch contexts) and `CompilationContext` (static fields, ABI methods/events).

4.  **Diagnostics:** When adding new error or warning diagnostics:
    *   Define a clear `DiagnosticId`.
    *   Provide a concise and informative message.
    *   Ensure the diagnostic is associated with the correct source code `Location` using Roslyn APIs.

5.  **Optimization:** Modifying optimizers (`Optimizer/`) requires extra care:
    *   Ensure optimizations preserve the original program's semantics.
    *   Add tests specifically for optimization patterns.
    *   Consider the trade-offs between optimization effectiveness and compilation time.
    *   Benchmark the impact of optimizations on script size and potentially GAS cost.

6.  **Framework Synchronization:** Regularly align the compiler with the target version(s) of `Neo.SmartContract.Framework`. Updates to Syscall hashes or framework attribute behavior might necessitate compiler changes.

7.  **Code Style & Conventions:** Follow the existing coding style, naming conventions, and project structure.

8.  **Dependency Management:** Keep Roslyn and other dependencies updated, testing thoroughly after updates.

9.  **Documentation:** Update relevant documentation (like the files in this directory) when making significant changes to compiler behavior, options, or internal logic.

## Contributing

*   **Issues:** Use the project's issue tracker to report bugs or suggest features.
*   **Pull Requests:**
    *   Discuss significant changes via an issue first.
    *   Ensure code builds and all tests pass.
    *   Include new tests for your changes.
    *   Follow coding conventions.
    *   Update documentation if necessary.

By adhering to these guidelines, developers can contribute to a robust, reliable, and maintainable Neo C# Compiler.
