# Contract Version Fallback Feature

## Overview

This feature enhances the Neo smart contract compilation process by allowing the compiler to automatically extract and use the project's `VersionPrefix` when a contract does not explicitly set the `ContractVersionAttribute`.

## Implementation Details

### Version Extraction Logic

1. When compiling a project, the compiler extracts the `VersionPrefix` value from:
   - The project file (.csproj) being compiled
   - If not found in the project file, it looks in any referenced `Directory.Build.props` files, starting from the project directory and moving up the directory tree

2. The extracted version is stored in the `CompilationEngine` and made available to the compilation context.

### Version Application Logic

1. During manifest creation:
   - The compiler first checks if the contract class has a `ContractVersionAttribute` specified
   - If the attribute is not present and a project version was found, that version is used in the contract manifest

## Usage

### Explicit Version Specification (No Change)

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractVersion("1.0.0")]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

### Implicit Version from Project

```csharp
// No ContractVersionAttribute specified
public class MyContract : SmartContract
{
    // Contract implementation
}
```

In the project file:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <VersionPrefix>1.2.3</VersionPrefix>
    <!-- Other properties -->
  </PropertyGroup>
</Project>
```

The resulting contract will have version "1.2.3" in its manifest.

## Technical Notes

- The version extraction is done once per project compilation
- Directory.Build.props files are checked recursively up the directory tree
- If no version is found in either location, no version will be added to the contract manifest
