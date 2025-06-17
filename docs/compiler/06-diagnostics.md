# Compiler Diagnostic System

The Neo C# Compiler (`nccs`) includes a diagnostic system to report issues found during compilation. These diagnostics help developers identify and fix problems in their smart contract code, ranging from critical errors that prevent compilation to warnings about potential issues or informational messages.

## How Diagnostics are Generated

Diagnostics are generated at various stages of the compilation process:

1.  **Roslyn Analysis:** The underlying Roslyn compiler performs standard C# syntax and semantic analysis. Errors like syntax mistakes, type mismatches, or unresolved symbols are caught here.
2.  **Neo Specific Analysis:** The `nccs` compiler adds its own layer of analysis specific to Neo smart contract development. This includes:
    *   Checking for the use of unsupported .NET APIs or language features.
    *   Validating the correct usage of `Neo.SmartContract.Framework` attributes and methods.
    *   Ensuring adherence to NeoVM limitations (e.g., call stack depth, instruction limits).
    *   Checking for potential security vulnerabilities (if `--security-analysis` is enabled).
    *   Verifying contract structure and required methods (like `_deploy`).

These diagnostics are collected within the `CompilationContext` for each contract being compiled.

## Diagnostic Information

Each diagnostic typically includes:

*   **Severity Level:** Indicates the importance of the diagnostic.
    *   `Error`: A critical issue that prevents successful compilation (e.g., invalid syntax, use of forbidden API). The compilation will fail.
    *   `Warning`: A potential issue that doesn't stop compilation but should be reviewed (e.g., potential inefficiency, use of deprecated features, possible security risks).
    *   `Info`: An informational message (less common).
    *   `Hidden`: Diagnostics not typically shown to the user.
*   **ID:** A unique identifier code for the specific diagnostic type (e.g., `NC1001`, `NC2005`). These IDs help in searching for documentation or specific information about the issue.
*   **Message:** A human-readable description of the problem.
*   **Location:** The source file path, line number, and column number where the issue was detected. This allows developers to quickly pinpoint the relevant code.

## Reporting Diagnostics

After attempting to compile a contract, `nccs` reports the collected diagnostics:

*   **Console Output:** Diagnostics are printed to the console.
    *   `Error` severity messages are typically written to the standard error stream (`stderr`).
    *   `Warning` and `Info` severity messages are usually written to the standard output stream (`stdout`).
*   **IDE Integration:** When compiling within an IDE like Visual Studio or Rider, diagnostics are usually displayed in the "Error List" or "Problems" panel, allowing for easy navigation to the source code location.

## Example Diagnostic Output

A typical warning message might look like this in the console:

```
MyContract.cs(25,10): warning NC2008: Method 'System.Console.WriteLine(string)' is not supported, please use 'Runtime.Log(string)' instead.
```

An error message would look similar but indicate `error` severity:

```
MyContract.cs(30,5): error CS0103: The name 'undefinedVariable' does not exist in the current context
```

## Handling Diagnostics

*   **Errors:** Must be fixed for the compilation to succeed and produce a `.nef` file.
*   **Warnings:** Should be carefully reviewed. While they don't block compilation, they often indicate potential bugs, security risks, or areas for improvement. It's good practice to address or explicitly suppress warnings.

Understanding and addressing compiler diagnostics is a critical part of the smart contract development cycle, ensuring code correctness, security, and adherence to the Neo platform's constraints.
