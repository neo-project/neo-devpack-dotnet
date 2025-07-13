# Contract Version Handling

## Overview

This feature enhances the Neo smart contract compilation process by allowing the compiler to automatically extract and use the project's version information when a contract does not explicitly set the `ContractVersionAttribute`.

## Implementation Details

### Version Extraction Logic

When compiling a project, the compiler extracts version information in the following order of precedence:

1. `Version` property (highest priority)
2. Combination of `VersionPrefix` and `VersionSuffix` properties
3. `VersionPrefix` property alone
4. `VersionSuffix` property alone

These values are looked for in:
- The project file (.csproj) being compiled
- If not found in the project file, it looks in any referenced `Directory.Build.props` files, starting from the project directory and moving up the directory tree

The extracted version information is stored in the `CompilationEngine` and made available to the compilation context.

### Version Application Logic

1. During manifest creation:
   - The compiler first checks if the contract class has a `ContractVersionAttribute` specified
   - If the attribute is not present and a project version was found, that version is used in the contract manifest

## Usage

### Explicit Version Specification (No Change)

```csharp
using R3E.SmartContract.Framework;
using R3E.SmartContract.Framework.Attributes;

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

#### Using Version Property

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Version>1.2.3</Version>
    <!-- Other properties -->
  </PropertyGroup>
</Project>
```

The resulting contract will have version "1.2.3" in its manifest.

#### Using VersionPrefix and VersionSuffix

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <VersionPrefix>1.2.3</VersionPrefix>
    <VersionSuffix>beta</VersionSuffix>
    <!-- Other properties -->
  </PropertyGroup>
</Project>
```

The resulting contract will have version "1.2.3-beta" in its manifest.

#### Using VersionPrefix Only

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <VersionPrefix>1.2.3</VersionPrefix>
    <!-- Other properties -->
  </PropertyGroup>
</Project>
```

The resulting contract will have version "1.2.3" in its manifest.

#### Using VersionSuffix Only

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <VersionSuffix>preview</VersionSuffix>
    <!-- Other properties -->
  </PropertyGroup>
</Project>
```

The resulting contract will have version "preview" in its manifest.

## Technical Notes

- The version extraction is done once per project compilation
- Directory.Build.props files are checked recursively up the directory tree
- If no version information is found in either location, no version will be added to the contract manifest
- The order of precedence for version information is:
  1. `Version` property (highest priority)
  2. Combined `VersionPrefix` and `VersionSuffix` properties
  3. `VersionPrefix` property alone
  4. `VersionSuffix` property alone
