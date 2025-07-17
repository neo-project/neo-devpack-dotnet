using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Template.UnitTests
{
    [TestClass]
    public class TemplateCompilationTests
    {
        private const string TemplateProjectBasePath = "../../../../../src/Neo.SmartContract.Template/templates";

        [DataTestMethod]
        [DataRow("neocontractnep17")]
        [DataRow("neocontractowner")]
        [DataRow("neocontractoracle")]
        public void TestTemplateCompilation_DebugConfiguration(string templateName)
        {
            TestTemplateCompilation(templateName, "Debug");
        }

        [DataTestMethod]
        [DataRow("neocontractnep17")]
        [DataRow("neocontractowner")]
        [DataRow("neocontractoracle")]
        public void TestTemplateCompilation_ReleaseConfiguration(string templateName)
        {
            TestTemplateCompilation(templateName, "Release");
        }

        private void TestTemplateCompilation(string templateName, string configuration)
        {
            var projectPath = Path.Combine(TemplateProjectBasePath, templateName);
            var fullProjectPath = Path.GetFullPath(projectPath);
            
            Assert.IsTrue(Directory.Exists(fullProjectPath), $"Template directory not found: {fullProjectPath}");

            var tempDirectory = Path.Combine(Path.GetTempPath(), $"NeoTemplateTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDirectory);

            try
            {
                // Copy template files to temp directory
                CopyDirectory(fullProjectPath, tempDirectory);

                // Find the csproj file
                var csprojFile = Directory.GetFiles(tempDirectory, "*.csproj").FirstOrDefault();
                Assert.IsNotNull(csprojFile, "No csproj file found in template");

                // Replace TemplateNeoVersion with actual version
                var csprojContent = File.ReadAllText(csprojFile);
                csprojContent = csprojContent.Replace("TemplateNeoVersion", "3.8.1");
                File.WriteAllText(csprojFile, csprojContent);

                // Run dotnet build
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{csprojFile}\" -c {configuration}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = tempDirectory
                };

                using var process = Process.Start(startInfo);
                Assert.IsNotNull(process);

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Assert.AreEqual(0, process.ExitCode, 
                    $"Build failed for {templateName} in {configuration} mode.\nOutput: {output}\nError: {error}");

                // Verify that the build artifacts were created
                var binPath = Path.Combine(tempDirectory, "bin", configuration);
                Assert.IsTrue(Directory.Exists(binPath), "Build output directory not found");

                // Check for .nef and .manifest.json files
                var nefFiles = Directory.GetFiles(binPath, "*.nef", SearchOption.AllDirectories);
                var manifestFiles = Directory.GetFiles(binPath, "*.manifest.json", SearchOption.AllDirectories);

                Assert.IsTrue(nefFiles.Length > 0, $"No .nef file generated for {templateName} in {configuration} mode");
                Assert.IsTrue(manifestFiles.Length > 0, $"No .manifest.json file generated for {templateName} in {configuration} mode");
            }
            finally
            {
                // Clean up temp directory
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}