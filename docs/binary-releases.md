# Neo C# Compiler Binary Releases

## Overview
Starting with version 3.8.1, the Neo C# Compiler (nccs) is available as standalone binary applications for multiple platforms. These binaries eliminate the need to install .NET SDK on your system to compile Neo smart contracts.

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
# Using PowerShell (recommended)
Expand-Archive -Path nccs-windows-x64.zip -DestinationPath C:\neo-tools\

# Or using Windows Explorer
# Right-click on nccs-windows-x64.zip and select "Extract All..."

# Or using 7-Zip/WinRAR if installed
# Right-click and choose "Extract Here" or "Extract to folder"
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

### 3. Add to PATH (Optional but Recommended)

#### Windows
```cmd
# Temporary (current session only)
set PATH=%PATH%;C:\neo-tools\

# Permanent (system-wide)
# 1. Open System Properties > Advanced > Environment Variables
# 2. Add C:\neo-tools\ to the PATH variable
# 3. Or use PowerShell as Administrator:
setx /M PATH "$env:PATH;C:\neo-tools\"
```

#### macOS/Linux
```bash
# Temporary (current session)
export PATH=$PATH:/path/to/nccs

# Permanent (add to ~/.bashrc, ~/.zshrc, or ~/.profile)
echo 'export PATH=$PATH:/path/to/nccs' >> ~/.bashrc
source ~/.bashrc
```

### 4. Verify Installation
```bash
# Check if nccs is accessible
nccs --help

# Verify version
nccs --version
```

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
./nccs path/to/your/contract.csproj --debug=Extended --optimize=All

# Generate assembly output and interface files
./nccs path/to/your/contract.csproj --assembly --generate-interface

# Generate testing artifacts for unit tests
./nccs path/to/your/contract.csproj --generate-artifacts=All -o ./build/

# Compile with security analysis and overflow checking
./nccs path/to/your/contract.csproj --security-analysis --checked

# Compile solution file
./nccs path/to/your/solution.sln -o ./dist/

# Compile multiple source files with custom output name
./nccs Contract1.cs Contract2.cs Contract3.cs --base-name=MyContract

# Full-featured compilation example
./nccs ./MyContract.csproj \
  --output ./build \
  --debug=Extended \
  --optimize=All \
  --assembly \
  --generate-interface \
  --generate-artifacts=All \
  --security-analysis \
  --checked
```

### Available Command-Line Options

#### Output and Files
- `-o, --output <directory>` - Specify output directory for generated files
- `--base-name <name>` - Set base name for output files (default: project name)
- `--assembly` - Generate assembly (.asm) file for debugging

#### Compilation Settings
- `--optimize <level>` - Set optimization level: `None`, `Basic`, `All`, `Experimental`
- `--checked` - Enable overflow and underflow checking
- `--nullable <option>` - Nullable reference types: `Disable`, `Annotations`, `Warnings`, `Enable`
- `--no-inline` - Disable inline code insertion for better debugging

#### Debug Options
- `-d, --debug <level>` - Generate debug information: `None`, `Strict`, `Extended`
- `--address-version <version>` - Address version for contract deployment (default: 0x35)

#### Code Generation
- `--generate-interface` - Generate C# interface files for contract interaction
- `--generate-artifacts <flags>` - Generate testing artifacts: `None`, `Source`, `Library`, `All`
- `--security-analysis` - Perform comprehensive security analysis

#### Advanced Options
- `--help` - Display help information
- `--version` - Show compiler version information

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
3. Or use the command line to bypass Gatekeeper:
```bash
xattr -d com.apple.quarantine nccs
```

### Antivirus Issues (Windows)
Some antivirus software may flag the executable. This is a false positive common with packed executables. Add an exception for the nccs.exe file if needed.

### PATH Configuration Issues
If `nccs` command is not found:
```bash
# Check if nccs is in your PATH
which nccs  # macOS/Linux
where nccs  # Windows

# Run with full path if not in PATH
/full/path/to/nccs --help
```

### .NET Runtime Dependencies
If you encounter runtime errors about missing .NET components:
- The binaries are self-contained and should not require .NET installation
- Try downloading a fresh copy from the releases page
- Ensure you downloaded the correct platform-specific binary

### Common Compilation Errors
```bash
# Error: "Project file not found"
# Solution: Ensure the .csproj file path is correct
./nccs ./path/to/MyContract.csproj

# Error: "Target framework not supported"
# Solution: Ensure your project targets .NET 9.0
# Check your .csproj file contains: <TargetFramework>net9.0</TargetFramework>

# Error: "Neo.SmartContract.Framework not found"
# Solution: Ensure Neo.SmartContract.Framework package is installed
dotnet add package Neo.SmartContract.Framework
```

### Platform-Specific Issues

#### Windows
- Run PowerShell as Administrator if you encounter permission issues
- Disable real-time protection temporarily if antivirus blocks execution

#### macOS
- For Apple Silicon Macs, use the `nccs-macos-arm64.tar.gz` version
- For Intel Macs, use the `nccs-macos-x64.tar.gz` version

#### Linux
- Ensure your system has glibc 2.31 or later
- On older distributions, you may need to install additional dependencies:
```bash
sudo apt-get update
sudo apt-get install libc6 libgcc1 libssl1.1
```

## Build Information
These binaries are built with:
- Single-file deployment
- Self-contained runtime
- Native libraries included
- No trimming (full compatibility)

For the latest updates and release notes, visit the [project repository](https://github.com/neo-project/neo-devpack-dotnet). 