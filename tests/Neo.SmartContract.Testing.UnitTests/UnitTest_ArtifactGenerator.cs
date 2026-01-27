// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ArtifactGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests;

[TestClass]
public class UnitTest_ArtifactGenerator
{
    private static string? FindTestDataPath()
    {
        // Try to find test artifacts in various locations
        var baseDir = Path.GetDirectoryName(typeof(UnitTest_ArtifactGenerator).Assembly.Location)!;

        // Look for Compiler.CSharp.UnitTests TestingArtifacts
        var searchPaths = new[]
        {
            Path.Combine(baseDir, "..", "..", "..", "..", "Neo.Compiler.CSharp.UnitTests", "TestingArtifacts"),
            Path.Combine(baseDir, "TestingArtifacts"),
        };

        foreach (var path in searchPaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (Directory.Exists(fullPath) && Directory.GetFiles(fullPath, "*.nef").Length > 0)
                return fullPath;
        }
        return null;
    }

    [TestMethod]
    public void Test_GenerateArtifactSource_FromFiles()
    {
        var testDataPath = FindTestDataPath();
        if (testDataPath == null)
        {
            Assert.Inconclusive("No test data found");
            return;
        }

        var nefFiles = Directory.GetFiles(testDataPath, "*.nef");
        Assert.IsTrue(nefFiles.Length > 0, "No .nef files found");

        var nefPath = nefFiles[0];
        var baseName = Path.GetFileNameWithoutExtension(nefPath);
        var manifestPath = Path.Combine(testDataPath, $"{baseName}.manifest.json");

        if (!File.Exists(manifestPath))
        {
            Assert.Inconclusive($"Manifest not found for {baseName}");
            return;
        }

        var source = ArtifactGenerator.GenerateArtifactSource(nefPath, manifestPath);

        Assert.IsNotNull(source);
        Assert.IsTrue(source.Contains("namespace Neo.SmartContract.Testing"));
        Assert.IsTrue(source.Contains("public abstract class"));
    }

    [TestMethod]
    public void Test_GenerateArtifactFile_CreatesFile()
    {
        var testDataPath = FindTestDataPath();
        if (testDataPath == null)
        {
            Assert.Inconclusive("No test data found");
            return;
        }

        var nefFiles = Directory.GetFiles(testDataPath, "*.nef");
        Assert.IsTrue(nefFiles.Length > 0, "No .nef files found");

        var nefPath = nefFiles[0];
        var baseName = Path.GetFileNameWithoutExtension(nefPath);
        var manifestPath = Path.Combine(testDataPath, $"{baseName}.manifest.json");

        if (!File.Exists(manifestPath))
        {
            Assert.Inconclusive($"Manifest not found for {baseName}");
            return;
        }

        var outputPath = Path.Combine(Path.GetTempPath(), $"{baseName}_test.cs");

        try
        {
            ArtifactGenerator.GenerateArtifactFile(nefPath, manifestPath, outputPath);
            Assert.IsTrue(File.Exists(outputPath));
            var content = File.ReadAllText(outputPath);
            Assert.IsTrue(content.Contains("namespace Neo.SmartContract.Testing"));
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Test_GenerateArtifactSource_ThrowsOnMissingNef()
    {
        ArtifactGenerator.GenerateArtifactSource(
            "/nonexistent/path.nef",
            "/nonexistent/path.manifest.json");
    }
}
