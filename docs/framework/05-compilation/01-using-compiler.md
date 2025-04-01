# Using the Compiler (`nccs`)

The `Neo.Compiler.CSharp` NuGet package provides MSBuild integration, meaning the Neo contract compilation process is automatically triggered when you build your .NET project.

## Standard Build Process

Assuming you have correctly set up your C# project (`.csproj` file) as described in [Framework & Compiler Setup](../02-getting-started/02-installation.md), compiling your smart contract is straightforward:

1.  **Navigate:** Open your terminal or command prompt in the directory containing your smart contract's `.csproj` file.
2.  **Build:** Run the standard .NET build command:
    ```bash
    dotnet build
    ```
    Or, for a release build (which might apply different optimizations if configured):
    ```bash
    dotnet build -c Release
    ```

## What Happens During `dotnet build`?

When `Neo.Compiler.CSharp` is referenced, the build process executes these steps:

1.  **Standard C# Compilation:** Your C# code is compiled into a standard .NET assembly (`.dll`).
2.  **`nccs` Invocation:** The MSBuild tasks included in `Neo.Compiler.CSharp` invoke the Neo compiler (`nccs`).
3.  **Analysis & Conversion:** `nccs` analyzes the compiled DLL, focusing on the classes and methods relevant to smart contracts (those using `Neo.SmartContract.Framework`). It converts the CIL (Common Intermediate Language) into NeoVM bytecode.
4.  **NEF Generation:** The resulting NeoVM bytecode is packaged into the `.nef` file.
5.  **Manifest Generation:** Metadata (methods, events, permissions, standards, extra info derived from attributes and code structure) is collected and written to the `.manifest.json` file.
6.  **Debug Info Generation (Optional):** If `<NeoContractDebugInfo>true</NeoContractDebugInfo>` is set in the `.csproj` (typically for Debug configuration), a `.nefdbgnfo` file containing debug symbols is also generated.

## Output Files

After a successful build, the compiled artifacts will be placed in your project's output directory (e.g., `bin/Debug/net6.0/` or `bin/Release/net6.0/`).

You should find:

*   `YourContractName.nef`: The NeoVM bytecode.
*   `YourContractName.manifest.json`: The contract manifest.
*   `YourContractName.nefdbgnfo` (Optional): Debug information.
*   `YourProject.dll`: The standard .NET assembly (not used for deployment).

These `.nef` and `.manifest.json` files are what you need to deploy your contract to a Neo network.

## Compilation Errors

If `nccs` encounters issues converting your C# code to valid NeoVM instructions, it will report errors during the `dotnet build` process.

Common causes include:

*   Using C# features or .NET libraries not supported by the NeoVM or the compiler.
*   Incorrectly using framework types or methods.
*   Syntax errors that pass C# compilation but violate smart contract constraints.
*   Missing necessary `[ContractPermission]` attributes for `Contract.Call` usage.

The error messages usually indicate the problematic C# code section.

[Previous: Compilation Overview](./README.md) | [Next: Compiler Options](./02-compiler-options.md)