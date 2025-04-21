// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_ContractVersionFallback.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Manifest;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractVersionFallback
    {
        private string CreateTempProject(string contractCode, string? versionPrefix = null)
        {
            string tempPath = Path.GetTempPath();
            string projectDir = Path.Combine(tempPath, Path.GetRandomFileName());
            Directory.CreateDirectory(projectDir);

            // Create project file
            string projectFile = Path.Combine(projectDir, "TestContract.csproj");
            string projectContent = $@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    {(versionPrefix != null ? $"<VersionPrefix>{versionPrefix}</VersionPrefix>" : "")}
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.7.4.1-CI00592"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(projectFile, projectContent);

            // Create contract file
            string contractFile = Path.Combine(projectDir, "TestContract.cs");
            File.WriteAllText(contractFile, contractCode);

            return projectFile;
        }

        private string CreateTempProjectWithDirectoryBuildProps(string contractCode, string versionPrefix)
        {
            string tempPath = Path.GetTempPath();
            string projectDir = Path.Combine(tempPath, Path.GetRandomFileName());
            Directory.CreateDirectory(projectDir);

            // Create Directory.Build.props file
            string dirBuildPropsFile = Path.Combine(projectDir, "Directory.Build.props");
            string dirBuildPropsContent = $@"
<Project>
  <PropertyGroup>
    <VersionPrefix>{versionPrefix}</VersionPrefix>
  </PropertyGroup>
</Project>";
            File.WriteAllText(dirBuildPropsFile, dirBuildPropsContent);

            // Create project file (without version)
            string projectFile = Path.Combine(projectDir, "TestContract.csproj");
            string projectContent = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.7.4.1-CI00592"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(projectFile, projectContent);

            // Create contract file
            string contractFile = Path.Combine(projectDir, "TestContract.cs");
            File.WriteAllText(contractFile, contractCode);

            return projectFile;
        }

        [TestMethod]
        public void TestExplicitContractVersion()
        {
            string contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractVersion(""1.0.0"")]
public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, "2.0.0");

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
        public void TestVersionFallbackFromProjectFile()
        {
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode, "2.0.0");

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();
                Assert.IsNotNull(manifest.Extra);
                Assert.AreEqual("2.0.0", manifest.Extra["Version"]!.AsString());
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
        public void TestVersionFallbackFromDirectoryBuildProps()
        {
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProjectWithDirectoryBuildProps(contractCode, "3.0.0");

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
        public void TestNoVersionAvailable()
        {
            string contractCode = @"
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static string Test() => ""Hello"";
}";

            string projectFile = CreateTempProject(contractCode);

            try
            {
                var compilation = new CompilationEngine(new CompilationOptions()).CompileProject(projectFile);
                Assert.AreEqual(1, compilation.Count);

                ContractManifest manifest = compilation[0].CreateManifest();

                // Version should not be present in the manifest
                if (manifest.Extra != null)
                {
                    Assert.IsFalse(manifest.Extra.ContainsProperty("Version"));
                }
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
    }
}
