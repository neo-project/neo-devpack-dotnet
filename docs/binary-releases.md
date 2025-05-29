# Neo C# Compiler Binary Releases

## Overview
Starting with version 3.7.4, the Neo C# Compiler (nccs) is available as standalone binary applications for multiple platforms. These binaries eliminate the need to install .NET SDK on your system to compile Neo smart contracts.

## Supported Platforms
- **Windows x64** - `nccs-windows-x64.zip`
- **macOS x64** - `nccs-macos-x64.tar.gz`
- **macOS Apple Silicon** - `nccs-macos-arm64.tar.gz`
- **Linux x64** - `nccs-linux-x64.tar.gz`

## Download and Installation

### 1. Download the Binary
Visit the [releases page](https://github.com/neo-project/neo-devpack-dotnet/releases) and download the appropriate binary for your platform.

### 2. Extract the Archive

#### Windows
```cmd
# Extract the zip file to a directory of your choice
tar -xf nccs-windows-x64.zip -C C:\neo-tools\
```

#### macOS/Linux
```bash
# Extract the tar.gz file
tar -xzf nccs-macos-x64.tar.gz
# or
tar -xzf nccs-linux-x64.tar.gz

# Make the binary executable
chmod +x nccs
```

### 3. Add to PATH (Optional)
For easier access, add the directory containing the nccs binary to your system's PATH environment variable.

## Usage Examples

### Basic Compilation
```bash
# Compile a smart contract project
./nccs path/to/your/contract.csproj

# Compile with custom output directory
./nccs path/to/your/contract.csproj -o ./build/
```

### Advanced Options
```bash
# Compile with debugging information and optimization
./nccs path/to/your/contract.csproj --debug --optimize=All

# Generate assembly output and interface files
./nccs path/to/your/contract.csproj --assembly --generate-interface

# Compile multiple source files
./nccs Contract1.cs Contract2.cs Contract3.cs
```

### Available Command-Line Options
- `-o, --output` - Specify output directory
- `--base-name` - Set base name for output files
- `--debug` - Generate debug information
- `--assembly` - Generate assembly (.asm) file
- `--optimize` - Set optimization level (None, Basic, All, Experimental)
- `--generate-interface` - Generate C# interface file
- `--security-analysis` - Perform security analysis
- `--generate-artifacts` - Generate additional artifacts

## Benefits of Binary Releases
1. **No .NET SDK Required** - Run the compiler without installing .NET
2. **Self-Contained** - All dependencies included in the binary
3. **Fast Setup** - Download and run immediately
4. **Consistent Environment** - Same compiler version across all platforms
5. **CI/CD Friendly** - Easy to integrate into build pipelines

## Verification
After installation, verify the compiler works correctly:

```bash
./nccs --help
```

This should display the help information and available command-line options.

## Troubleshooting

### Permission Denied (macOS/Linux)
If you encounter permission errors, ensure the binary is executable:
```bash
chmod +x nccs
```

### Security Warning (macOS)
On macOS, you might see a security warning for unsigned binaries. You can bypass this by:
1. Go to System Preferences > Security & Privacy
2. Click "Allow Anyway" for the blocked application

### Antivirus Issues (Windows)
Some antivirus software may flag the executable. This is a false positive common with packed executables. Add an exception for the nccs.exe file if needed.

## Build Information
These binaries are built with:
- Single-file deployment
- Self-contained runtime
- Native libraries included
- No trimming (full compatibility)

For the latest updates and release notes, visit the [project repository](https://github.com/neo-project/neo-devpack-dotnet). 