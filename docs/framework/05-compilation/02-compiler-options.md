# Compiler Options (`.csproj` Properties)

You can configure the behavior of the `Neo.Compiler.CSharp` (`nccs`) compiler by setting specific properties within your project's `.csproj` file, typically inside a `<PropertyGroup>`.

## Key Configuration Properties

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>disable</ImplicitUsings> 
    <Nullable>disable</Nullable> 

    <!-- Required/Recommended for Contract Compilation -->
    <NeoContractName>MyAwesomeContract</NeoContractName> <!-- (Optional) Base name for output files -->
    <RootNamespace>Neo.Contracts.Examples</RootNamespace> <!-- (Optional) Affects namespace in manifest? -->
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo> <!-- Recommended: Avoids conflicts -->
    <ProduceReferenceAssembly>false</ProduceReferenceAssembly> <!-- Recommended: Not needed -->
    <Deterministic>false</Deterministic> <!-- Recommended: Avoids path issues -->

    <!-- NCCS Specific Options -->
    <NeoCompilerVersion>Latest</NeoCompilerVersion> <!-- (Optional) Specify nccs version, e.g., "3.6.0". Defaults to package version. -->
    <NeoBuildOptimization>O1</NeoBuildOptimization> <!-- (Optional) Set optimization level: O0 (None), O1 (Default). -->
    <NeoContractDebugInfo>false</NeoContractDebugInfo> <!-- (Optional) Generate .nefdbgnfo file. Set to true for Debug config. -->
    <NeoContractVersion>1.0.0.0</NeoContractVersion> <!-- (Optional) Embeds version in NEF. Defaults if omitted. -->
    <NeoWarningLevel>4</NeoWarningLevel> <!-- (Optional) Compiler warning level (0-4). Default is 4 (highest). -->
    <NeoScript>someScriptBytes</NeoScript> <!-- (Advanced/Rare) Inject custom script bytes. -->

  </PropertyGroup>

  <!-- Example: Set Debug Info only for Debug configuration -->
  <PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
    <NeoContractDebugInfo>true</NeoContractDebugInfo>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.*" />
    <PackageReference Include="Neo.Compiler.CSharp" Version="3.*" />
  </ItemGroup>

</Project>
```

**Explanation:**

*   **`<NeoContractName>`**: 
    *   Specifies the base filename for the output `.nef`, `.manifest.json`, and `.nefdbgnfo` files.
    *   If omitted, the project assembly name (usually the `.csproj` filename) is used.

*   **`<NeoCompilerVersion>`**: 
    *   Forces the use of a specific `nccs` version if multiple are available or for compatibility.
    *   Defaults to the version of the `Neo.Compiler.CSharp` package referenced.
    *   Use `Latest` to always try the newest installed version.

*   **`<NeoBuildOptimization>`**: 
    *   Controls the optimization level applied by `nccs`.
    *   `O0`: No optimization.
    *   `O1`: Standard optimizations (method inlining, basic peephole optimizations). This is usually the default and recommended setting.
    *   Higher optimization levels might exist in future versions.

*   **`<NeoContractDebugInfo>`**: 
    *   Set to `true` to generate the `.nefdbgnfo` debug information file.
    *   Crucial for debugging contracts using tools like the Neo Blockchain Toolkit (Neo Express).
    *   Typically enabled only for the `Debug` configuration.

*   **`<NeoContractVersion>`**: 
    *   Embeds a version string into the compiled NEF file header.
    *   Useful for tracking deployed contract versions.
    *   If omitted, a default version might be used by the compiler.

*   **`<NeoWarningLevel>`**: 
    *   Controls the level of warnings reported by `nccs` during compilation (0 = Off, 4 = Report all warnings).
    *   Default is usually 4.

*   **`<NeoScript>`**: (Advanced)
    *   Allows injecting raw script bytes. Use cases are very specific and rare.

*   **`<GenerateAssemblyInfo>`, `<ProduceReferenceAssembly>`, `<Deterministic>`**: 
    *   Standard .NET properties recommended to be set to `false` for Neo contract projects to avoid potential path embedding issues or conflicts with the compiler.

By adjusting these properties, you can fine-tune the compilation process to suit your needs, enabling debugging or applying specific optimizations.

[Previous: Using the Compiler](./01-using-compiler.md) | [Next: Debugging Information](./03-debugging.md)