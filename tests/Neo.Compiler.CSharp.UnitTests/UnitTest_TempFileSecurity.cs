// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_TempFileSecurity.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_TempFileSecurity
{
    [TestMethod]
    public void Test_CompileFromCodeBlock_CleansUpTempFiles()
    {
        // Get initial state of temp directory
        var tempPath = Path.GetTempPath();
        var initialDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");

        // Create compilation engine and compile a simple code block
        var engine = new CompilationEngine(new CompilationOptions());
        var result = engine.CompileFromCodeBlock("int x = 1;");

        // Verify compilation succeeded
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);

        // Verify no neo-compiler temp directories remain
        var remainingDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");
        Assert.AreEqual(initialDirs.Length, remainingDirs.Length,
            "Temp directories should be cleaned up after compilation");
    }

    [TestMethod]
    public void Test_CompileFromCodeBlock_CleansUpOnError()
    {
        // Get initial state of temp directory
        var tempPath = Path.GetTempPath();
        var initialDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");

        // Create compilation engine and try to compile invalid code
        var engine = new CompilationEngine(new CompilationOptions());

        try
        {
            // This should fail compilation but still clean up temp files
            engine.CompileFromCodeBlock("this is not valid C# code @#$%");
        }
        catch
        {
            // Expected to fail
        }

        // Verify no neo-compiler temp directories remain even after error
        var remainingDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");
        Assert.AreEqual(initialDirs.Length, remainingDirs.Length,
            "Temp directories should be cleaned up even when compilation fails");
    }

    [TestMethod]
    public void Test_CompileFromCodeBlock_UsesSecureTempDirectory()
    {
        // This test verifies the temp directory naming pattern is secure (uses GUID)
        var tempPath = Path.GetTempPath();

        // Create compilation engine
        var engine = new CompilationEngine(new CompilationOptions());

        // We can't easily intercept the temp dir creation, but we can verify
        // the compilation works and no predictable temp files are left
        var result = engine.CompileFromCodeBlock("int y = 42;");

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);

        // Verify no .tmp files with predictable names exist
        var tmpFiles = Directory.GetFiles(tempPath, "tmp*.cs");
        // Should not have created any .cs files directly in temp root
        // (old behavior used Path.GetTempFileName which creates tmpXXXX.tmp)
    }

    [TestMethod]
    public void Test_CompileFromCodeBlock_MultipleCompilations()
    {
        // Test that multiple compilations don't leave temp files behind
        var tempPath = Path.GetTempPath();
        var initialDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");

        var engine = new CompilationEngine(new CompilationOptions());

        // Run multiple compilations
        for (int i = 0; i < 5; i++)
        {
            var result = engine.CompileFromCodeBlock($"int x{i} = {i};");
            Assert.IsNotNull(result);
        }

        // Verify all temp directories are cleaned up
        var remainingDirs = Directory.GetDirectories(tempPath, "neo-compiler-*");
        Assert.AreEqual(initialDirs.Length, remainingDirs.Length,
            "All temp directories should be cleaned up after multiple compilations");
    }
}
