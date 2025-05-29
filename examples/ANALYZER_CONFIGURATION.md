# Neo.SmartContract.Analyzer Configuration Guide

## Overview

The Neo smart contract examples are configured to use the Neo.SmartContract.Analyzer for code quality and best practice enforcement. The configuration supports both **local project reference** (for development) and **NuGet package reference** (for distribution).

## Current Configuration

The analyzer configuration is controlled by properties in `examples/Directory.Build.props`:

```xml
<!-- Analyzer Configuration -->
<UseLocalAnalyzer>true</UseLocalAnalyzer>        <!-- Current: Use local project -->
<NeoAnalyzerVersion>3.8.1</NeoAnalyzerVersion>   <!-- Target NuGet version -->
```

## Switching to NuGet Package

When `Neo.SmartContract.Analyzer` version 3.8.1+ becomes available on NuGet:

### Step 1: Update the Configuration

Edit `examples/Directory.Build.props` and change:

```xml
<UseLocalAnalyzer>false</UseLocalAnalyzer>  <!-- Changed from 'true' to 'false' -->
```

### Step 2: Verify the Version

Ensure the version matches the available NuGet package:

```xml
<NeoAnalyzerVersion>3.8.1</NeoAnalyzerVersion>  <!-- Update if needed -->
```

### Step 3: Test the Configuration

```bash
# Clean previous builds
dotnet clean examples

# Restore and build to verify NuGet package usage
dotnet build examples/Example.SmartContract.Transfer --verbosity normal
```

## Configuration Details

### Local Project Reference (Current)
```xml
<ProjectReference Include="..\..\src\Neo.SmartContract.Analyzer\Neo.SmartContract.Analyzer.csproj" 
                  Condition="'$(UseLocalAnalyzer)' == 'true'">
    <OutputItemType>Analyzer</OutputItemType>
    <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
</ProjectReference>
```

**Advantages:**
- ✅ Use latest development version
- ✅ Local debugging and development
- ✅ No external dependencies

**Disadvantages:**
- ❌ Requires full source code
- ❌ Longer build times
- ❌ Development environment setup required

### NuGet Package Reference (Future)
```xml
<PackageReference Include="Neo.SmartContract.Analyzer" 
                  Version="$(NeoAnalyzerVersion)" 
                  Condition="'$(UseLocalAnalyzer)' == 'false'">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

**Advantages:**
- ✅ Faster builds
- ✅ No source code required
- ✅ Easy distribution
- ✅ Version management

**Disadvantages:**
- ❌ Depends on published package
- ❌ Fixed version (no latest features)

## Analyzer Functionality

The Neo.SmartContract.Analyzer provides:

### Code Quality Rules
- **NC4008**: Use direct assignment instead of `new BigInteger(int)`
- **NC4020**: Method naming conventions (no leading underscores except `_deploy`, `_initial`)
- **NC4019**: Event name validation
- More rules for Neo smart contract best practices

### Usage Examples

**❌ Problematic Code:**
```csharp
// Will trigger NC4008
var amount = new BigInteger(100);

// Will trigger NC4020
public static void _customMethod() { }
```

**✅ Correct Code:**
```csharp
// Analyzer suggestion applied
BigInteger amount = 100;

// Proper method naming
public static void CustomMethod() { }
```

## Troubleshooting

### Build Errors After Switching

If you encounter build errors after switching to NuGet:

1. **Clean and Restore:**
   ```bash
   dotnet clean examples
   dotnet restore examples
   ```

2. **Verify Package Availability:**
   ```bash
   dotnet list package --include-transitive | grep Neo.SmartContract.Analyzer
   ```

3. **Check Version Compatibility:**
   - Ensure `NeoAnalyzerVersion` matches available NuGet package
   - Verify framework compatibility (.NET 9.0)

### Analyzer Not Running

If analyzer warnings/errors don't appear:

1. **Check Configuration:**
   ```xml
   <!-- Ensure IncludeAssets includes 'analyzers' -->
   <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
   ```

2. **Verify Project Style:**
   ```xml
   <!-- Ensure project uses PackageReference style -->
   <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
   ```

## Future Updates

When newer versions of Neo.SmartContract.Analyzer become available:

1. Update `NeoAnalyzerVersion` in `Directory.Build.props`
2. Test with example projects
3. Update this documentation if needed

## Questions?

- Check [Neo Developer Documentation](https://developers.neo.org/)
- Visit [Neo DevPack Repository](https://github.com/neo-project/neo-devpack-dotnet)
- Open issues for bug reports or feature requests 