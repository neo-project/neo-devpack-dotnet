// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OutputNameSecurity.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_OutputNameSecurity
    {
        [TestMethod]
        public void TestContractDisplayNameCannotEscapeOutputFolder()
        {
            using var workspace = TempWorkspace.Create();
            string projectPath = workspace.CreateProject("""
using Neo.SmartContract.Framework;
using System.ComponentModel;

[DisplayName("../outside-name")]
public class Contract : SmartContract
{
    public static int Main() => 1;
}
""");

            string outputPath = Path.Combine(workspace.ProjectDirectory, "out");

            int exitCode = Program.Main([projectPath, "--debug", "None", "-o", outputPath]);

            Assert.AreEqual(1, exitCode);
            Assert.IsFalse(File.Exists(Path.Combine(workspace.ProjectDirectory, "outside-name.nef")));
            Assert.IsFalse(Directory.EnumerateFiles(workspace.ProjectDirectory, "outside-name.*").Any());
        }

        [TestMethod]
        public void TestBaseNameCannotEscapeOutputFolder()
        {
            using var workspace = TempWorkspace.Create();
            string projectPath = workspace.CreateProject("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main() => 1;
}
""");

            string outputPath = Path.Combine(workspace.ProjectDirectory, "out");

            int exitCode = Program.Main([projectPath, "--debug", "None", "--base-name", "../outside-name", "-o", outputPath]);

            Assert.AreEqual(1, exitCode);
            Assert.IsFalse(File.Exists(Path.Combine(workspace.ProjectDirectory, "outside-name.nef")));
            Assert.IsFalse(Directory.EnumerateFiles(workspace.ProjectDirectory, "outside-name.*").Any());
        }

        [TestMethod]
        public void TestBaseNameRejectsEmptyAndInvalidFileNameCharacters()
        {
            using var workspace = TempWorkspace.Create();
            string projectPath = workspace.CreateProject("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Main() => 1;
}
""");

            string outputPath = Path.Combine(workspace.ProjectDirectory, "out");

            int whitespaceExitCode = Program.Main([projectPath, "--debug", "None", "--base-name", " ", "-o", outputPath]);
            int invalidCharExitCode = Program.Main([projectPath, "--debug", "None", "--base-name", "bad\0name", "-o", outputPath]);

            Assert.AreEqual(1, whitespaceExitCode);
            Assert.AreEqual(1, invalidCharExitCode);
            Assert.IsFalse(Directory.EnumerateFiles(workspace.ProjectDirectory, "*.nef", SearchOption.AllDirectories).Any());
        }

        [TestMethod]
        public void TestDuplicateContractNamesAreRejectedBeforeWritingOutputs()
        {
            using var workspace = TempWorkspace.Create();
            string projectPath = workspace.CreateProject("""
using Neo.SmartContract.Framework;

namespace First
{
    public class SharedContract : SmartContract
    {
        public static int First() => 1;
    }
}

namespace Second
{
    public class SharedContract : SmartContract
    {
        public static int Second() => 2;
    }
}
""");

            string outputPath = Path.Combine(workspace.ProjectDirectory, "out");

            using var error = new StringWriter();
            var previousError = Console.Error;
            int exitCode;
            try
            {
                Console.SetError(error);
                exitCode = Program.Main([projectPath, "--debug", "None", "-o", outputPath]);
            }
            finally
            {
                Console.SetError(previousError);
            }

            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(error.ToString(), "Output base name 'SharedContract' is shared by contracts: First.SharedContract, Second.SharedContract.");
            Assert.IsFalse(Directory.Exists(outputPath) && Directory.EnumerateFiles(outputPath).Any());
        }

        private sealed class TempWorkspace : IDisposable
        {
            public string Root { get; }
            public string ProjectDirectory { get; }

            private TempWorkspace(string root)
            {
                Root = root;
                ProjectDirectory = Path.Combine(root, "ContractProject");
                Directory.CreateDirectory(ProjectDirectory);
            }

            public static TempWorkspace Create()
            {
                string root = Path.Combine(Path.GetTempPath(), $"NeoOutputNameSecurity_{Guid.NewGuid():N}");
                Directory.CreateDirectory(root);
                return new TempWorkspace(root);
            }

            public string CreateProject(string source)
            {
                string frameworkProject = Path.GetFullPath(Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "..", "..", "..", "..", "..",
                    "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj"));

                string projectPath = Path.Combine(ProjectDirectory, "ContractProject.csproj");
                File.WriteAllText(projectPath, $$"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="{{frameworkProject}}" />
  </ItemGroup>
</Project>
""");
                File.WriteAllText(Path.Combine(ProjectDirectory, "Contract.cs"), source);
                return projectPath;
            }

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(Root))
                    {
                        Directory.Delete(Root, true);
                    }
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }
        }
    }
}
