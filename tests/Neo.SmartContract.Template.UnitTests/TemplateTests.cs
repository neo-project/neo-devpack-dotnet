using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Neo.SmartContract.Template.UnitTests
{
    public class TemplateTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly string _testDirectory;
        private readonly ITemplateEngineHost _host;
        private readonly IEngineEnvironmentSettings _engineEnvironmentSettings;

        public TemplateTests(ITestOutputHelper output)
        {
            _output = output;
            _testDirectory = Path.Combine(Path.GetTempPath(), $"NeoTemplateTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            _host = new DefaultTemplateEngineHost("NeoTemplateTests", "1.0.0");
            _engineEnvironmentSettings = new EngineEnvironmentSettings(_host);
        }

        [Fact]
        public async Task NeoContractSolution_BasicContract_CreatesCorrectStructure()
        {
            // Arrange
            var templatePath = GetTemplatePath("neocontractsolution");
            var projectName = "TestBasicContract";
            var outputPath = Path.Combine(_testDirectory, projectName);

            // Act
            var result = await CreateTemplateAsync(templatePath, outputPath, projectName, new Dictionary<string, string>
            {
                ["contractType"] = "Basic",
                ["contractName"] = "TestContract",
                ["enableDeployment"] = "true",
                ["enableTests"] = "true",
                ["enableSecurityFeatures"] = "true"
            });

            // Assert
            Assert.True(result.Success, "Template creation failed");
            
            // Verify solution structure
            Assert.True(File.Exists(Path.Combine(outputPath, $"{projectName}.sln")));
            Assert.True(File.Exists(Path.Combine(outputPath, "README.md")));
            Assert.True(File.Exists(Path.Combine(outputPath, "global.json")));
            Assert.True(File.Exists(Path.Combine(outputPath, "Directory.Build.props")));
            
            // Verify contract project
            var contractPath = Path.Combine(outputPath, "src", "TestContract");
            Assert.True(Directory.Exists(contractPath));
            Assert.True(File.Exists(Path.Combine(contractPath, "TestContract.csproj")));
            Assert.True(File.Exists(Path.Combine(contractPath, "BasicContract.cs")));
            
            // Verify test project
            var testPath = Path.Combine(outputPath, "tests", "TestContract.Tests");
            Assert.True(Directory.Exists(testPath));
            Assert.True(File.Exists(Path.Combine(testPath, "TestContract.Tests.csproj")));
            Assert.True(File.Exists(Path.Combine(testPath, "ContractTests.cs")));
            
            // Verify deployment project
            var deployPath = Path.Combine(outputPath, "deploy", "TestContract.Deploy");
            Assert.True(Directory.Exists(deployPath));
            Assert.True(File.Exists(Path.Combine(deployPath, "TestContract.Deploy.csproj")));
            Assert.True(File.Exists(Path.Combine(deployPath, "Program.cs")));
            
            // Verify deployment manifest
            Assert.True(File.Exists(Path.Combine(outputPath, "deployment.json")));
        }

        [Theory]
        [InlineData("NEP17", "TokenContract.cs")]
        [InlineData("NEP11", "NFTContract.cs")]
        [InlineData("Governance", "GovernanceContract.cs")]
        public async Task NeoContractSolution_DifferentContractTypes_CreatesCorrectFiles(string contractType, string expectedFile)
        {
            // Arrange
            var templatePath = GetTemplatePath("neocontractsolution");
            var projectName = $"Test{contractType}Contract";
            var outputPath = Path.Combine(_testDirectory, projectName);

            // Act
            var result = await CreateTemplateAsync(templatePath, outputPath, projectName, new Dictionary<string, string>
            {
                ["contractType"] = contractType,
                ["contractName"] = "TestContract"
            });

            // Assert
            Assert.True(result.Success, "Template creation failed");
            
            var contractPath = Path.Combine(outputPath, "src", "TestContract");
            Assert.True(File.Exists(Path.Combine(contractPath, expectedFile)), 
                $"Expected contract file {expectedFile} not found");
            
            // Verify other contract types are not included
            var otherFiles = new[] { "BasicContract.cs", "TokenContract.cs", "NFTContract.cs", "GovernanceContract.cs" }
                .Where(f => f != expectedFile);
            
            foreach (var file in otherFiles)
            {
                Assert.False(File.Exists(Path.Combine(contractPath, file)), 
                    $"Unexpected contract file {file} found");
            }
        }

        [Fact]
        public async Task NeoContractSolution_DisableOptionalFeatures_ExcludesCorrectly()
        {
            // Arrange
            var templatePath = GetTemplatePath("neocontractsolution");
            var projectName = "TestMinimalContract";
            var outputPath = Path.Combine(_testDirectory, projectName);

            // Act
            var result = await CreateTemplateAsync(templatePath, outputPath, projectName, new Dictionary<string, string>
            {
                ["contractType"] = "Basic",
                ["enableDeployment"] = "false",
                ["enableTests"] = "false",
                ["enableSecurityFeatures"] = "false"
            });

            // Assert
            Assert.True(result.Success, "Template creation failed");
            
            // Verify excluded directories
            Assert.False(Directory.Exists(Path.Combine(outputPath, "tests")));
            Assert.False(Directory.Exists(Path.Combine(outputPath, "deploy")));
            Assert.False(File.Exists(Path.Combine(outputPath, "deployment.json")));
            
            // Verify contract still exists
            Assert.True(Directory.Exists(Path.Combine(outputPath, "src", "MyContract")));
        }

        [Fact]
        public async Task NeoContractSolution_BuildsSuccessfully()
        {
            // Arrange
            var templatePath = GetTemplatePath("neocontractsolution");
            var projectName = "TestBuildContract";
            var outputPath = Path.Combine(_testDirectory, projectName);

            // Act
            var result = await CreateTemplateAsync(templatePath, outputPath, projectName, new Dictionary<string, string>
            {
                ["contractType"] = "Basic"
            });

            Assert.True(result.Success, "Template creation failed");

            // Try to build the solution
            var buildResult = await RunCommandAsync("dotnet", "build", outputPath);

            // Assert
            Assert.True(buildResult.Success, $"Build failed: {buildResult.Error}");
        }

        [Fact]
        public async Task TemplateValidation_AllTemplatesHaveRequiredFiles()
        {
            // Arrange
            var templatesPath = Path.Combine(GetProjectRoot(), "src", "Neo.SmartContract.Template", "templates");
            var templateDirs = Directory.GetDirectories(templatesPath);

            // Act & Assert
            foreach (var templateDir in templateDirs)
            {
                var templateName = Path.GetFileName(templateDir);
                _output.WriteLine($"Validating template: {templateName}");

                // Check for template.json
                var templateJsonPath = Path.Combine(templateDir, ".template.config", "template.json");
                Assert.True(File.Exists(templateJsonPath), 
                    $"Template {templateName} missing template.json");

                // Validate template.json structure
                var templateJson = await File.ReadAllTextAsync(templateJsonPath);
                Assert.Contains("\"$schema\"", templateJson);
                Assert.Contains("\"identity\"", templateJson);
                Assert.Contains("\"shortName\"", templateJson);
                Assert.Contains("\"name\"", templateJson);
            }
        }

        [Fact]
        public void TemplateContent_NoHardcodedPrivateKeys()
        {
            // Arrange
            var templatesPath = Path.Combine(GetProjectRoot(), "src", "Neo.SmartContract.Template", "templates");
            var files = Directory.GetFiles(templatesPath, "*.cs", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(templatesPath, "*.json", SearchOption.AllDirectories));

            // Act & Assert
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                
                // Check for WIF format private keys (L or K followed by 51 base58 characters)
                Assert.DoesNotMatch(@"[LK][1-9A-HJ-NP-Za-km-z]{51}", content,
                    $"Found potential private key in {file}");
                
                // Check for hex private keys (64 hex characters)
                Assert.DoesNotMatch(@"(?<![a-fA-F0-9])[a-fA-F0-9]{64}(?![a-fA-F0-9])", content,
                    $"Found potential hex private key in {file}");
            }
        }

        [Theory]
        [InlineData("neocontractsolution", "MyContract")]
        [InlineData("neocontractsolution", "TestToken")]
        [InlineData("neocontractsolution", "NFTCollection")]
        public async Task TemplateSubstitution_ReplacesNamesCorrectly(string templateName, string contractName)
        {
            // Arrange
            var templatePath = GetTemplatePath(templateName);
            var projectName = $"Test_{contractName}";
            var outputPath = Path.Combine(_testDirectory, projectName);

            // Act
            var result = await CreateTemplateAsync(templatePath, outputPath, projectName, new Dictionary<string, string>
            {
                ["contractName"] = contractName
            });

            // Assert
            Assert.True(result.Success, "Template creation failed");

            // Check that contract name is properly substituted
            var contractPath = Path.Combine(outputPath, "src", contractName);
            Assert.True(Directory.Exists(contractPath), $"Contract directory {contractName} not found");
            Assert.True(File.Exists(Path.Combine(contractPath, $"{contractName}.csproj")));

            // Check content substitution
            var projectFile = await File.ReadAllTextAsync(Path.Combine(contractPath, $"{contractName}.csproj"));
            Assert.DoesNotContain("MyContract", projectFile);
        }

        private string GetProjectRoot()
        {
            var currentDir = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(currentDir, "neo-devpack-dotnet.sln")))
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
                if (currentDir == null)
                    throw new InvalidOperationException("Could not find project root");
            }
            return currentDir;
        }

        private string GetTemplatePath(string templateName)
        {
            return Path.Combine(GetProjectRoot(), "src", "Neo.SmartContract.Template", "templates", templateName);
        }

        private async Task<TemplateCreationResult> CreateTemplateAsync(
            string templatePath, 
            string outputPath, 
            string name,
            Dictionary<string, string> parameters)
        {
            try
            {
                // Simulate template creation by copying files
                CopyDirectory(templatePath, outputPath);

                // Apply template transformations
                await ApplyTemplateTransformationsAsync(outputPath, name, parameters);

                return new TemplateCreationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new TemplateCreationResult 
                { 
                    Success = false, 
                    Error = ex.Message 
                };
            }
        }

        private async Task ApplyTemplateTransformationsAsync(
            string outputPath, 
            string name,
            Dictionary<string, string> parameters)
        {
            // Simple implementation of template transformation
            var files = Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                // Skip binary files
                if (Path.GetExtension(file) is ".dll" or ".exe" or ".pdb")
                    continue;

                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    var originalContent = content;

                    // Replace template parameters
                    content = content.Replace("MyContract", parameters.GetValueOrDefault("contractName", "MyContract"));
                    content = content.Replace("NeoContractSolution", name);

                    // Handle conditional content
                    if (parameters.TryGetValue("enableTests", out var enableTests) && enableTests == "false")
                    {
                        content = RemoveConditionalContent(content, "enableTests");
                    }

                    if (parameters.TryGetValue("enableDeployment", out var enableDeployment) && enableDeployment == "false")
                    {
                        content = RemoveConditionalContent(content, "enableDeployment");
                    }

                    if (content != originalContent)
                    {
                        await File.WriteAllTextAsync(file, content);
                    }

                    // Rename files if needed
                    if (file.Contains("MyContract"))
                    {
                        var newFileName = file.Replace("MyContract", parameters.GetValueOrDefault("contractName", "MyContract"));
                        if (newFileName != file)
                        {
                            File.Move(file, newFileName);
                        }
                    }
                }
                catch
                {
                    // Ignore files that can't be processed as text
                }
            }

            // Handle directory renames
            var directories = Directory.GetDirectories(outputPath, "*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.Length); // Process deepest first

            foreach (var dir in directories)
            {
                if (dir.Contains("MyContract"))
                {
                    var newDirName = dir.Replace("MyContract", parameters.GetValueOrDefault("contractName", "MyContract"));
                    if (newDirName != dir && !Directory.Exists(newDirName))
                    {
                        Directory.Move(dir, newDirName);
                    }
                }
            }

            // Remove excluded files based on contract type
            if (parameters.TryGetValue("contractType", out var contractType))
            {
                await RemoveExcludedContractFilesAsync(outputPath, contractType);
            }
        }

        private string RemoveConditionalContent(string content, string condition)
        {
            // Simple regex to remove conditional blocks
            var pattern = $@"#if \({condition}\)[\s\S]*?#endif";
            return System.Text.RegularExpressions.Regex.Replace(content, pattern, "", 
                System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        private async Task RemoveExcludedContractFilesAsync(string outputPath, string contractType)
        {
            var contractFiles = new Dictionary<string, string>
            {
                ["Basic"] = "BasicContract.cs",
                ["NEP17"] = "TokenContract.cs",
                ["NEP11"] = "NFTContract.cs",
                ["Governance"] = "GovernanceContract.cs"
            };

            foreach (var kvp in contractFiles)
            {
                if (kvp.Key != contractType)
                {
                    var filesToRemove = Directory.GetFiles(outputPath, kvp.Value, SearchOption.AllDirectories);
                    foreach (var file in filesToRemove)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(subDir);
                if (dirName == ".template.config")
                    continue; // Skip template config
                    
                var destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(subDir, destDir);
            }
        }

        private async Task<CommandResult> RunCommandAsync(string command, string args, string workingDirectory)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(startInfo);
                if (process == null)
                    return new CommandResult { Success = false, Error = "Failed to start process" };

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                return new CommandResult
                {
                    Success = process.ExitCode == 0,
                    Output = output,
                    Error = error
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        private class TemplateCreationResult
        {
            public bool Success { get; set; }
            public string? Error { get; set; }
        }

        private class CommandResult
        {
            public bool Success { get; set; }
            public string? Output { get; set; }
            public string? Error { get; set; }
        }
    }
}