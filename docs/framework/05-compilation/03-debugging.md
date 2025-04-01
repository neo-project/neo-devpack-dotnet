# Debugging Information (`.nefdbgnfo`)

Debugging smart contracts directly on a public blockchain is impractical. However, the Neo C# compiler and associated tooling provide mechanisms for debugging contracts locally.

A key component of this is the debug information file.

## Generating Debug Info

To enable debugging, you need the compiler (`nccs`) to generate a file containing debug symbols that map the compiled NeoVM bytecode back to your original C# source code.

This is controlled by the `<NeoContractDebugInfo>` property in your `.csproj` file.

1.  **Edit `.csproj`**: Ensure the property is set to `true`, typically within a conditional block for the `Debug` configuration:
    ```xml
    <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
      <NeoContractDebugInfo>true</NeoContractDebugInfo>
    </PropertyGroup>
    ```

2.  **Build in Debug Configuration**: Compile your project specifically using the `Debug` configuration:
    ```bash
    dotnet build -c Debug
    ```

3.  **Output File**: If successful, alongside the `.nef` and `.manifest.json` files in your output directory (e.g., `bin/Debug/net6.0/`), you will find a file named:
    `YourContractName.nefdbgnfo`

This `.nefdbgnfo` file contains the mapping between NeoVM instruction offsets and your C# source code lines, variable names, and method boundaries.

## Using Debug Info

The `.nefdbgnfo` file is essential for tools that provide step-through debugging capabilities for Neo smart contracts.

*   **Neo Blockchain Toolkit (NBT):** This is the primary toolset for debugging Neo C# contracts.
    *   **Neo Express:** Allows you to run a private Neo blockchain instance locally.
    *   **Visual Studio Code Extension:** The NBT extension for VS Code integrates with Neo Express and uses the `.nefdbgnfo` file to allow you to:
        *   Set breakpoints in your C# code.
        *   Step through execution line by line.
        *   Inspect variable values.
        *   Examine the NeoVM stack and storage.

**Typical Debugging Workflow with NBT:**

1.  Install Neo Express (`dotnet tool install Neo.Express -g`).
2.  Create a Neo Express instance (`neox create ...`).
3.  Compile your contract in `Debug` configuration (`dotnet build -c Debug`) to generate `.nef`, `.manifest.json`, and `.nefdbgnfo`.
4.  Deploy your compiled contract to your local Neo Express instance (`neox contract deploy ...`).
5.  Configure VS Code launch settings (`launch.json`) to attach the debugger to Neo Express.
6.  Set breakpoints in your C# `.cs` files.
7.  Invoke your contract method on Neo Express (e.g., using `neox contract invoke ...` or via a custom client application connected to Neo Express).
8.  VS Code should hit the breakpoint, allowing you to step through the C# code as it executes within the simulated NeoVM environment.

Refer to the [Neo Blockchain Toolkit documentation](https://github.com/neo-project/neo-blockchain-toolkit) for detailed instructions on setting up and using the debugger.

**Note:** Never deploy contracts compiled with debug information (`.nefdbgnfo` present) to MainNet or public TestNets. Debug information increases the deployment size and is not needed for production execution.

[Previous: Compiler Options](./02-compiler-options.md) | [Next Section: Advanced Topics](../06-advanced-topics/README.md)