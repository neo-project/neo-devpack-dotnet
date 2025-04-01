# Using the Compiler (`nccs`)

The Neo C# Compiler, often invoked as `nccs` (Neo Compiler C Sharp), is the command-line tool responsible for transforming your C# smart contract project into executable Neo Virtual Machine (NeoVM) bytecode and associated metadata files.

## Installation

The compiler is typically used as part of the `neo-devpack-dotnet` package or SDK installation. Ensure you have the .NET SDK installed. Depending on your setup, you might invoke the compiler via:

1.  **Direct DLL Execution:** `dotnet <path_to_nccs_dll>/nccs.dll <arguments...>`
2.  **.NET Tool (if installed):** `nccs <arguments...>`
3.  **Within Visual Studio/IDE:** Build process might automatically invoke the compiler.

Refer to the [Getting Started](./../02-getting-started/02-installation.md) section for detailed installation instructions.

## Command-Line Interface (CLI)

The basic syntax for the compiler is:

```bash
nccs [paths...] [options...]
```

### Input Paths (`paths...`)

You need to provide the compiler with the C# source code to compile. This can be done in several ways:

*   **Project File (`.csproj`):**
    ```bash
    nccs MyContractProject.csproj [options...]
    ```
    This is the recommended approach as it uses the project file to determine source files, dependencies, and some compilation settings.

*   **Directory:**
    ```bash
    nccs ./path/to/contract/directory/ [options...]
    ```
    The compiler will search for a `.csproj` file in the specified directory. If found, it processes it. If not, it searches for `.cs` files recursively (excluding `obj` folders) and compiles them.

*   **Specific Source Files (`.cs`):**
    ```bash
    nccs Contract1.cs Helper.cs [options...]
    ```
    You can specify one or more C# source files directly. The output base name will typically be derived from the first `.cs` file unless overridden with `--base-name`.

*   **No Path Specified:**
    ```bash
    nccs [options...]
    ```
    If no path is provided, the compiler attempts to process the current working directory, looking for `.csproj` or `.cs` files as described above.

*   **NEF File for Optimization (`.nef`):**
    ```bash
    nccs MyContract.nef --optimize=Experimental [other_options...]
    ```
    The compiler can also take a compiled `.nef` file as input *specifically for optimization* using the `--optimize=Experimental` flag. It requires the corresponding `.manifest.json` and optionally the `.nefdbgnfo` file to be present in the same directory.

### Compiler Outputs

Upon successful compilation, `nccs` generates several files, typically in the output directory (default: `bin/sc/` relative to the project/source location):

1.  **`.nef` (Neo Executable Format):**
    *   **File:** `<BaseName>.nef` (e.g., `MyContract.nef`)
    *   **Content:** The compiled NeoVM bytecode that can be deployed to the Neo blockchain.

2.  **`.manifest.json` (Contract Manifest):**
    *   **File:** `<BaseName>.manifest.json` (e.g., `MyContract.manifest.json`)
    *   **Content:** JSON metadata describing the contract's ABI (methods, parameters, return types, events), permissions, supported standards, and other essential information needed for interaction and deployment.

3.  **`.nefdbgnfo` / `.debug.json` (Debug Information):**
    *   **File:** `<BaseName>.nefdbgnfo` (e.g., `MyContract.nefdbgnfo`)
    *   **Content:** A zip archive containing a `<BaseName>.debug.json` file. This JSON file maps NeoVM instructions back to the original C# source code lines and includes information about variables and method entry/exit points. This is crucial for debugging smart contracts using tools like Neo Debugger. Generated when the `--debug` option is enabled.

### Optional Outputs

Depending on the compiler options used, additional files might be generated:

*   **`.asm` (Assembly Listing):**
    *   **File:** `<BaseName>.asm` (e.g., `MyContract.asm`)
    *   **Content:** Human-readable representation of the generated NeoVM instructions (opcodes). Generated with the `--assembly` option.

*   **`.nef.txt` (DumpNef Output):**
    *   **File:** `<BaseName>.nef.txt` (e.g., `MyContract.nef.txt`)
    *   **Content:** Detailed breakdown of the NEF file structure, including script hash, opcodes with offsets, and potentially integrated debug information. Generated with the `--assembly` option.

*   **Artifacts (`.cs`, `.dll`):**
    *   **Files:** `<BaseName>.artifacts.cs`, `<BaseName>.artifacts.dll`
    *   **Content:** C# source code (`.cs`) or a compiled library (`.dll`) generated from the contract's manifest. These artifacts can simplify contract interaction in off-chain C# applications or testing environments. Generated with the `--generate-artifacts` option.

### Example Usage

*   **Compile a project file with debug info:**
    ```bash
    nccs MyProject.csproj --debug
    ```

*   **Compile specific C# files into a specific output directory:**
    ```bash
    nccs Contract.cs Storage.cs --output ./build --base-name MyAwesomeContract
    ```

*   **Compile a project and generate assembly listing:**
    ```bash
    nccs . --assembly
    ```

## Integration

While `nccs` is a powerful command-line tool, it's often integrated into development workflows:

*   **Visual Studio / Rider:** Building a Neo Smart Contract project within these IDEs typically triggers the `nccs` compiler automatically as part of the build process defined in the `.csproj` file.
*   **Build Scripts:** Custom build scripts can incorporate `nccs` commands for automated compilation and deployment pipelines.

See the [Compiler Options](./02-compiler-options.md) page for a detailed list of available flags and settings.
