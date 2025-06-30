using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Neo.SmartContract.Init.Models;

namespace Neo.SmartContract.Init.Services;

public class ProjectService
{
    private readonly TemplateService _templateService;

    public ProjectService()
    {
        _templateService = new TemplateService();
    }

    public async Task CreateProject(ProjectConfig config, bool force, Action<double, string> progressCallback)
    {
        progressCallback(0, "Validating configuration...");
        ValidateConfig(config);

        var projectPath = Path.Combine(config.OutputPath, config.Name);
        
        progressCallback(10, "Creating project directory...");
        if (Directory.Exists(projectPath) && !force)
        {
            throw new InvalidOperationException($"Directory '{projectPath}' already exists. Use --force to overwrite.");
        }

        Directory.CreateDirectory(projectPath);

        progressCallback(30, "Generating contract files...");
        await CreateContractFiles(projectPath, config);

        progressCallback(50, "Creating project file...");
        await CreateProjectFile(projectPath, config);

        if (config.Features.Contains("Unit Tests"))
        {
            progressCallback(60, "Adding test project...");
            await AddTestProject(projectPath);
        }

        if (config.Features.Contains("GitHub Actions CI/CD"))
        {
            progressCallback(70, "Adding GitHub Actions...");
            await AddGitHubActions(projectPath);
        }

        if (config.Features.Contains("Docker Support"))
        {
            progressCallback(80, "Adding Docker support...");
            await AddDockerSupport(projectPath);
        }

        if (config.Features.Contains("VS Code Configuration"))
        {
            progressCallback(85, "Adding VS Code configuration...");
            await AddVSCodeConfiguration(projectPath);
        }

        progressCallback(90, "Creating README...");
        await CreateReadme(projectPath, config);

        progressCallback(95, "Creating .gitignore...");
        await CreateGitignore(projectPath);

        progressCallback(100, "Project created successfully!");
    }

    private void ValidateConfig(ProjectConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.Name))
            throw new ArgumentException("Project name cannot be empty");

        if (!Regex.IsMatch(config.Name, @"^[a-zA-Z][a-zA-Z0-9._]*$"))
            throw new ArgumentException("Project name must start with a letter and contain only letters, numbers, dots, and underscores");

        // Check for reserved keywords and system names
        var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        if (reservedNames.Contains(config.Name.ToUpperInvariant()))
            throw new ArgumentException($"'{config.Name}' is a reserved system name and cannot be used as a project name");

        // Validate output path
        if (!string.IsNullOrWhiteSpace(config.OutputPath))
        {
            try
            {
                var fullPath = Path.GetFullPath(config.OutputPath);
                if (fullPath.Contains(".."))
                    throw new ArgumentException("Output path cannot contain relative path traversal elements (..)");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid output path: {ex.Message}");
            }
        }
    }

    private async Task CreateContractFiles(string projectPath, ProjectConfig config)
    {
        var contractContent = _templateService.GetTemplateContent(config.Template, "Contract.cs", config);
        var contractFile = Path.Combine(projectPath, $"{config.Name}.cs");
        await File.WriteAllTextAsync(contractFile, contractContent);
    }

    private async Task CreateProjectFile(string projectPath, ProjectConfig config)
    {
        var projectContent = _templateService.GetTemplateContent(config.Template, ".csproj", config);
        var projectFile = Path.Combine(projectPath, $"{config.Name}.csproj");
        await File.WriteAllTextAsync(projectFile, projectContent);
    }

    public async Task AddTestProject(string projectPath)
    {
        var projectName = Path.GetFileName(projectPath);
        var testProjectPath = Path.Combine(Path.GetDirectoryName(projectPath)!, $"{projectName}.Tests");
        Directory.CreateDirectory(testProjectPath);

        // Create test project file
        var testProjectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""17.8.0"" />
    <PackageReference Include=""MSTest.TestAdapter"" Version=""3.1.1"" />
    <PackageReference Include=""MSTest.TestFramework"" Version=""3.1.1"" />
    <PackageReference Include=""Neo.SmartContract.Testing"" Version=""3.6.2"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""../{projectName}/{projectName}.csproj"" />
  </ItemGroup>

</Project>";

        await File.WriteAllTextAsync(Path.Combine(testProjectPath, $"{projectName}.Tests.csproj"), testProjectContent);

        // Create test class
        var testClassContent = $@"using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System;

namespace {projectName}.Tests
{{
    [TestClass]
    public class {projectName}Tests : TestBase<{projectName}>
    {{
        [TestInitialize]
        public void TestSetup()
        {{
            var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
            TestBaseSetup(nef, manifest);
        }}

        [TestMethod]
        public void Test_ContractDeployment()
        {{
            // Arrange & Act
            var owner = Contract.GetOwner();
            
            // Assert
            Assert.IsNotNull(owner);
            Assert.IsFalse(owner.IsZero);
        }}

        [TestMethod]
        public void Test_OwnerOnly()
        {{
            // Arrange
            var nonOwner = Neo.UInt160.Zero;
            
            // Act & Assert
            Engine.SetTransactionSigners(nonOwner);
            Assert.ThrowsException<Exception>(() => 
                Contract.SetData(""test"", ""value""));
        }}
    }}
}}";

        await File.WriteAllTextAsync(Path.Combine(testProjectPath, $"{projectName}Tests.cs"), testClassContent);
    }

    public async Task AddGitHubActions(string projectPath)
    {
        var workflowsPath = Path.Combine(projectPath, ".github", "workflows");
        Directory.CreateDirectory(workflowsPath);

        var workflowContent = @"name: Build and Test

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
      with:
        submodules: recursive

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 9.0.x

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Test
      run: dotnet test --no-build --verbosity normal

    - name: Compile Contract
      run: |
        if [ -f ""../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj"" ]; then
          dotnet run --project ../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- *.csproj
        else
          echo ""Neo compiler not found in expected location""
        fi

    - name: Upload artifacts
      uses: actions/upload-artifact@v3
      with:
        name: contract-artifacts
        path: |
          bin/sc/*.nef
          bin/sc/*.manifest.json
";

        await File.WriteAllTextAsync(Path.Combine(workflowsPath, "build.yml"), workflowContent);
    }

    public async Task AddDockerSupport(string projectPath)
    {
        var dockerContent = @"FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY *.csproj .
RUN dotnet restore

# Copy source code
COPY . .
RUN dotnet build -c Release

# Compile contract
FROM build AS compile
RUN dotnet tool install -g Neo.Compiler.CSharp
ENV PATH=""${PATH}:/root/.dotnet/tools""
RUN nccs *.csproj

# Final stage
FROM scratch AS export
COPY --from=compile /src/bin/sc/*.nef /
COPY --from=compile /src/bin/sc/*.manifest.json /
";

        await File.WriteAllTextAsync(Path.Combine(projectPath, "Dockerfile"), dockerContent);

        var dockerComposeContent = @"version: '3.8'

services:
  build:
    build:
      context: .
      target: export
    volumes:
      - ./output:/output
";

        await File.WriteAllTextAsync(Path.Combine(projectPath, "docker-compose.yml"), dockerComposeContent);
    }

    public async Task AddVSCodeConfiguration(string projectPath)
    {
        var vscodePath = Path.Combine(projectPath, ".vscode");
        Directory.CreateDirectory(vscodePath);

        var launchContent = @"{
    ""version"": ""0.2.0"",
    ""configurations"": [
        {
            ""name"": ""Run Tests"",
            ""type"": ""coreclr"",
            ""request"": ""launch"",
            ""preLaunchTask"": ""build"",
            ""program"": ""dotnet"",
            ""args"": [""test""],
            ""cwd"": ""${workspaceFolder}"",
            ""console"": ""internalConsole"",
            ""stopAtEntry"": false
        }
    ]
}";

        var tasksContent = @"{
    ""version"": ""2.0.0"",
    ""tasks"": [
        {
            ""label"": ""build"",
            ""command"": ""dotnet"",
            ""type"": ""process"",
            ""args"": [
                ""build"",
                ""${workspaceFolder}""
            ],
            ""problemMatcher"": ""$msCompile""
        },
        {
            ""label"": ""compile"",
            ""command"": ""dotnet"",
            ""type"": ""process"",
            ""args"": [
                ""run"",
                ""--project"",
                ""../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj"",
                ""--"",
                ""${workspaceFolder}/*.csproj""
            ],
            ""problemMatcher"": ""$msCompile""
        }
    ]
}";

        await File.WriteAllTextAsync(Path.Combine(vscodePath, "launch.json"), launchContent);
        await File.WriteAllTextAsync(Path.Combine(vscodePath, "tasks.json"), tasksContent);
    }

    public async Task AddSecurityAnalyzer(string projectPath)
    {
        // Add analyzer to project file
        var projectFiles = Directory.GetFiles(projectPath, "*.csproj");
        if (projectFiles.Length == 0) return;

        var projectFile = projectFiles[0];
        var content = await File.ReadAllTextAsync(projectFile);
        
        if (!content.Contains("Neo.SmartContract.Analyzer"))
        {
            var analyzerReference = @"
    <PackageReference Include=""Neo.SmartContract.Analyzer"" Version=""3.6.2"">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>";

            content = content.Replace("</ItemGroup>", analyzerReference + "\n  </ItemGroup>");
            await File.WriteAllTextAsync(projectFile, content);
        }
    }

    private async Task CreateReadme(string projectPath, ProjectConfig config)
    {
        var readmeContent = $@"# {config.Name}

{config.Description}

## Overview

This is a NEO smart contract project built with the NEO DevPack for .NET.

- **Template**: {config.Template}
- **Author**: {config.Author}
- **Contact**: {config.Email}

## Prerequisites

- .NET SDK 9.0 or later
- NEO DevPack for .NET

## Build Instructions

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run tests:
   ```bash
   dotnet test
   ```

3. Compile to NEO bytecode:
   ```bash
   dotnet run --project ../../src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- {config.Name}.csproj
   ```

## Contract Methods

See the contract source code for available methods and their documentation.

## Deployment

1. Install NEO CLI
2. Create or open a wallet
3. Deploy the compiled .nef and manifest.json files

## Security

This contract follows NEO security best practices. Always audit your contract before mainnet deployment.

## License

[Specify your license here]
";

        await File.WriteAllTextAsync(Path.Combine(projectPath, "README.md"), readmeContent);
    }

    private async Task CreateGitignore(string projectPath)
    {
        var gitignoreContent = @"# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio
.vs/
*.user
*.userosscache
*.sln.docstates

# VS Code
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json

# .NET
project.lock.json
project.fragment.lock.json
artifacts/

# NEO Contract artifacts
*.nef
*.manifest.json
*.nefdbgnfo
*.abi.json
*.artifacts.cs

# Testing
TestResults/
coverage/
*.coverage
*.coveragexml

# JetBrains Rider
.idea/
*.sln.iml

# macOS
.DS_Store
";

        await File.WriteAllTextAsync(Path.Combine(projectPath, ".gitignore"), gitignoreContent);
    }
}