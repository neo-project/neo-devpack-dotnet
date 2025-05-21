# Compilation (`Neo.Compiler.CSharp`)

Once you have written your smart contract code using the `Neo.SmartContract.Framework`, you need to compile it into the NeoVM bytecode (`.nef`) and manifest (`.manifest.json`) files required for deployment.

This compilation process is handled by the `Neo.Compiler.CSharp` tool (often referred to as `nccs`).

## Topics

*   [Using the Compiler](./01-using-compiler.md): How the compiler integrates with the `dotnet build` process.
*   [Compiler Options](./02-compiler-options.md): Configuring the compiler via `.csproj` properties.
*   [Debugging Information](./03-debugging.md): Generating debug symbols for use with tools like Neo Express.

[Next: Using the Compiler](./01-using-compiler.md)