// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ProjectBoundaries.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class ProjectBoundaryTests
    {
        [TestMethod]
        public void CompilerProject_DoesNotReference_TestingProject()
        {
            string csprojPath = GetCompilerProjectPath();

            var project = XDocument.Load(csprojPath);
            var references = project.Descendants("ProjectReference");

            Assert.IsFalse(
                references.Any(reference => (string?)reference.Attribute("Include") is string include
                    && include.Contains("Neo.SmartContract.Testing.csproj")),
                "Neo.Compiler.CSharp.csproj should not reference Neo.SmartContract.Testing.csproj");
        }

        [TestMethod]
        public void CompilerToolPackage_Includes_ArtifactLibraryDependency()
        {
            string outputPath = Path.Combine(Path.GetTempPath(), $"NeoCompilerPackageTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(outputPath);

            try
            {
                string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
                var startInfo = new ProcessStartInfo("dotnet")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                startInfo.Environment["MSBUILDDISABLENODEREUSE"] = "1";
                startInfo.ArgumentList.Add("pack");
                startInfo.ArgumentList.Add(GetCompilerProjectPath());
                startInfo.ArgumentList.Add("--configuration");
                startInfo.ArgumentList.Add(configuration);
                startInfo.ArgumentList.Add("--no-build");
                startInfo.ArgumentList.Add("--no-restore");
                startInfo.ArgumentList.Add("--output");
                startInfo.ArgumentList.Add(outputPath);
                startInfo.ArgumentList.Add("/p:PackageVersion=0.0.0-package-test");

                using var process = Process.Start(startInfo)!;
                var stdoutTask = process.StandardOutput.ReadToEndAsync();
                var stderrTask = process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                string stdout = stdoutTask.GetAwaiter().GetResult();
                string stderr = stderrTask.GetAwaiter().GetResult();

                Assert.AreEqual(0, process.ExitCode, $"Package creation failed. Output: {stdout}{stderr}");

                string packagePath = Directory.GetFiles(outputPath, "*.nupkg")
                    .Single(path => !path.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase));
                using var package = ZipFile.OpenRead(packagePath);
                string[] runtimeAssemblies =
                [
                    "Neo.dll",
                    "Neo.IO.dll",
                    "Neo.SmartContract.Analyzer.dll",
                    "Neo.SmartContract.Testing.dll"
                ];

                foreach (string assembly in runtimeAssemblies)
                {
                    Assert.IsTrue(
                        package.Entries.Any(entry => entry.FullName == $"tools/net10.0/any/{assembly}"),
                        $"The compiler tool package should contain {assembly}.");
                }
            }
            finally
            {
                Directory.Delete(outputPath, true);
            }
        }

        private static string GetCompilerProjectPath()
        {
            return Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "src", "Neo.Compiler.CSharp", "Neo.Compiler.CSharp.csproj"));
        }
    }
}
