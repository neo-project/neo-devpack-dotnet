// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OptimizedAssemblyOutput.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral;

[TestClass]
public class UnitTest_OptimizedAssemblyOutput
{
    [TestMethod]
    public void AssemblyReflectsFinalOptimizedScript()
    {
        string workspace = Path.Combine(Path.GetTempPath(), $"NeoOptimizedAssembly_{Guid.NewGuid():N}");
        string output = Path.Combine(workspace, "out");
        string sourcePath = Path.Combine(workspace, "Contract.cs");
        Directory.CreateDirectory(workspace);
        File.WriteAllText(sourcePath, """
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Increment(int value) => value + 1;
            }
            """);

        try
        {
            int exitCode = Program.Main([
                sourcePath,
                "--base-name", "OptimizedAssembly",
                "--assembly",
                "--optimize=Experimental",
                "--debug=Extended",
                "-o", output
            ]);
            Assert.AreEqual(0, exitCode);

            var nef = NefFile.Parse(File.ReadAllBytes(Path.Combine(output, "OptimizedAssembly.nef")));
            var opCodes = ((Script)nef.Script).EnumerateInstructions().Select(p => p.instruction.OpCode).ToArray();
            CollectionAssert.Contains(opCodes, OpCode.INC, "The test contract must exercise the ADD-to-INC optimization.");
            CollectionAssert.DoesNotContain(opCodes, OpCode.ADD);

            string assembly = File.ReadAllText(Path.Combine(output, "OptimizedAssembly.asm"));
            StringAssert.Contains(assembly, nameof(OpCode.INC));
            Assert.IsFalse(Regex.IsMatch(assembly, @"\bADD\b"), "Assembly must not contain instructions removed from the final NEF.");
        }
        finally
        {
            Directory.Delete(workspace, recursive: true);
        }
    }

    [TestMethod]
    public void AssemblyWriteFailureReturnsError()
    {
        string workspace = Path.Combine(Path.GetTempPath(), $"NeoOptimizedAssemblyFailure_{Guid.NewGuid():N}");
        string output = Path.Combine(workspace, "out");
        string sourcePath = Path.Combine(workspace, "Contract.cs");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "OptimizedAssembly.asm"));
        File.WriteAllText(sourcePath, """
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Increment(int value) => value + 1;
            }
            """);

        try
        {
            int exitCode = Program.Main([
                sourcePath,
                "--base-name", "OptimizedAssembly",
                "--assembly",
                "--optimize=Experimental",
                "--debug=Extended",
                "-o", output
            ]);

            Assert.AreEqual(1, exitCode);
        }
        finally
        {
            Directory.Delete(workspace, recursive: true);
        }
    }
}
