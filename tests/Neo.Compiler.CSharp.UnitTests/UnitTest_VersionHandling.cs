using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Xml.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_VersionHandling
    {
        [TestMethod]
        public void TestVersionPriority()
        {
            // Test that Version takes priority over VersionPrefix and VersionSuffix
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, version: "1.0.0", versionPrefix: "2.0.0", versionSuffix: "alpha");

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("1.0.0", manifest.Extra["Version"]!.AsString());
            }
            finally
            {
                // Clean up
                string? projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir != null && Directory.Exists(projectDir))
                {
                    Directory.Delete(projectDir, true);
                }
            }
        }

        [TestMethod]
        public void TestVersionPrefixAndSuffix()
        {
            // Test that VersionPrefix and VersionSuffix are combined
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, versionPrefix: "2.0.0", versionSuffix: "beta");

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("2.0.0-beta", manifest.Extra["Version"]!.AsString());
            }
            finally
            {
                // Clean up
                string? projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir != null && Directory.Exists(projectDir))
                {
                    Directory.Delete(projectDir, true);
                }
            }
        }

        [TestMethod]
        public void TestVersionPrefixOnly()
        {
            // Test that VersionPrefix is used when VersionSuffix is not set
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, versionPrefix: "3.0.0");

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("3.0.0", manifest.Extra["Version"]!.AsString());
            }
            finally
            {
                // Clean up
                string? projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir != null && Directory.Exists(projectDir))
                {
                    Directory.Delete(projectDir, true);
                }
            }
        }

        [TestMethod]
        public void TestVersionSuffixOnly()
        {
            // Test that VersionSuffix is used when VersionPrefix is not set
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, versionSuffix: "preview");

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("preview", manifest.Extra["Version"]!.AsString());
            }
            finally
            {
                // Clean up
                string? projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir != null && Directory.Exists(projectDir))
                {
                    Directory.Delete(projectDir, true);
                }
            }
        }

        [TestMethod]
        public void TestDirectoryBuildProps()
        {
            // Test that Directory.Build.props is used when project doesn't have version info
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(projectDir);

            try
            {
                // Create Directory.Build.props
                string buildPropsContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Version>4.0.0</Version>
  </PropertyGroup>
</Project>";
                File.WriteAllText(Path.Combine(projectDir, "Directory.Build.props"), buildPropsContent);

                // Create project file without version info
                string projectFile = CreateTempProject(contractCode, projectDir: projectDir);

                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("4.0.0", manifest.Extra["Version"]!.AsString());
            }
            finally
            {
                // Clean up
                if (Directory.Exists(projectDir))
                {
                    Directory.Delete(projectDir, true);
                }
            }
        }

        private static string CreateTempProject(string contractCode, string? version = null, string? versionPrefix = null, string? versionSuffix = null, string? projectDir = null)
        {
            // Create a temporary directory for the project
            string directory = projectDir ?? Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(directory);

            // Create the contract file
            string contractPath = Path.Combine(directory, "TestContract.cs");
            File.WriteAllText(contractPath, contractCode);

            // Create NuGet.config file
            string nugetConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" />
    <add key=""neo"" value=""https://www.myget.org/F/neo/api/v3/index.json"" />
  </packageSources>
</configuration>";
            File.WriteAllText(Path.Combine(directory, "NuGet.config"), nugetConfigContent);

            // Create the project file with version information
            XElement propertyGroup = new XElement("PropertyGroup",
                new XElement("TargetFramework", "net9.0"),
                new XElement("ImplicitUsings", "enable"),
                new XElement("Nullable", "enable")
            );

            // Add version properties if provided
            if (!string.IsNullOrEmpty(version))
            {
                propertyGroup.Add(new XElement("Version", version));
            }
            if (!string.IsNullOrEmpty(versionPrefix))
            {
                propertyGroup.Add(new XElement("VersionPrefix", versionPrefix));
            }
            if (!string.IsNullOrEmpty(versionSuffix))
            {
                propertyGroup.Add(new XElement("VersionSuffix", versionSuffix));
            }

            XElement project = new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                propertyGroup,
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "net9.0"),
                    new XElement("RestoreSources", "https://api.nuget.org/v3/index.json;https://www.myget.org/F/neo/api/v3/index.json")
                ),
                new XElement("ItemGroup",
                    new XElement("PackageReference",
                        new XAttribute("Include", "Neo.SmartContract.Framework"),
                        new XAttribute("Version", "3.8.1")
                    )
                )
            );

            string projectPath = Path.Combine(directory, "TestProject.csproj");
            project.Save(projectPath);

            return projectPath;
        }
    }
}
