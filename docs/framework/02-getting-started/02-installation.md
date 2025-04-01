# Framework & Compiler Installation/Setup

To develop Neo smart contracts in C#, you need two key components from the `neo-devpack-dotnet` project:

1.  **`Neo.SmartContract.Framework`:** A NuGet package containing the necessary classes, methods, and attributes to interact with the Neo blockchain environment within your C# code.
2.  **`Neo.Compiler.CSharp`:** A tool (often used as an MSBuild task) that compiles your C# smart contract project into NeoVM bytecode (`.nef`) and a manifest file (`.manifest.json`).

## Referencing the Framework

You add the framework to your C# smart contract project like any other NuGet package.

1.  **Create a Project:** Create a new .NET Class Library project.
    ```bash
    dotnet new classlib -n MyFirstContract
    cd MyFirstContract
    ```
2.  **Add NuGet Package:** Add a reference to the `Neo.SmartContract.Framework` package.
    ```bash
    dotnet add package Neo.SmartContract.Framework
    ```
    This command modifies your project's `.csproj` file to include the framework dependency.

Your `.csproj` file should now look something like this (version numbers may vary):

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework> <!-- Target .NET 9 -->
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.x.x" />
  </ItemGroup>

</Project>
```

## Setting up the Compiler (`nccs`)

The Neo C# compiler (`nccs`) is typically invoked during the build process of your smart contract project.

1.  **Add Compiler Package:** Add a reference to the `Neo.Compiler.CSharp` package. This package includes MSBuild tasks that automatically trigger the compilation during `dotnet build`.
    ```bash
    dotnet add package Neo.Compiler.CSharp
    ```

2.  **Configure `.csproj` for Smart Contract Compilation:**
    You need to modify your `.csproj` file slightly to tell MSBuild that this is a Neo smart contract project and configure the compiler.

    ```xml
    <Project Sdk="Microsoft.NET.Sdk">

      <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        <!-- Neo specific properties -->
        <NeoContractName>MyFirstContract</NeoContractName> <!-- Optional: Name for output files -->
        <NeoBuildOptimization>O1</NeoBuildOptimization> <!-- Optional: Optimization level (O0, O1) -->
        <NeoCompilerVersion>3.x.x</NeoCompilerVersion> <!-- Optional: Specify compiler version if needed -->
        <GenerateAssemblyInfo>false</GenerateAssemblyInfo> <!-- Recommended -->
        <ProduceReferenceAssembly>false</ProduceReferenceAssembly> <!-- Recommended -->
        <Deterministic>false</Deterministic> <!-- Recommended -->
      </PropertyGroup>

      <ItemGroup>
        <PackageReference Include="Neo.SmartContract.Framework" Version="3.x.x" />
        <PackageReference Include="Neo.Compiler.CSharp" Version="3.x.x" />
      </ItemGroup>

      <!-- Optional: Include Debug Info -->
      <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
        <NeoContractDebugInfo>true</NeoContractDebugInfo>
      </PropertyGroup>

    </Project>
    ```

    *   **Key Properties:**
        *   `<NeoContractName>`: Sets the base name for the output `.nef` and `.manifest.json` files.
        *   `<NeoBuildOptimization>`: Controls compiler optimization level (default is often `O1`).
        *   `<NeoContractDebugInfo>`: Set to `true` (usually in Debug configuration) to generate a `.nefdbgnfo` file for debugging with tools like Neo Express.

With these packages referenced and the `.csproj` configured, running `dotnet build` on your project will automatically invoke the `nccs` compiler after the standard C# compilation, producing the necessary Neo deployment artifacts.

[Previous: Environment Setup](./01-setup.md) | [Next: Your First Contract](./03-first-contract.md)