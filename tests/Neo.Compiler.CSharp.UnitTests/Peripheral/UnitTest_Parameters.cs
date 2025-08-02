// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Parameters.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_Parameters
    {
        readonly string csFileDir = Utils.Extensions.TestContractRoot;

        [TestMethod]
        public void TestNoParameter()
        {
            Assert.AreEqual(Program.Main([]), 2);
        }

        [TestMethod]
        public void TestOutput()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "-o", "output"]), 0);
        }

        [TestMethod]
        public void TestBaseName()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--base-name", "MyContract"]), 0);
        }

        [TestMethod]
        public void TestNotCSharpFile()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.txt");
            Assert.AreEqual(Program.Main([path]), 1);
        }

        [TestMethod]
        public void TestGeneratePlugin()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--generate-plugin"]), 0);

            // Verify plugin files were created
            var outputDir = Path.Combine(Path.GetDirectoryName(path), "bin", "sc");
            var pluginDir = Path.Combine(outputDir, "Contract_BigIntegerPlugin");
            Assert.IsTrue(Directory.Exists(pluginDir), "Plugin directory should be created");
            Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "Contract_BigIntegerPlugin.cs")), "Plugin main file should exist");
            Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "Contract_BigIntegerPlugin.csproj")), "Plugin project file should exist");
        }

        [TestMethod]
        public void TestGeneratePluginWithOutput()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            var outputPath = Path.Combine(Path.GetTempPath(), "plugin_test_output");
            Assert.AreEqual(Program.Main([path, "--generate-plugin", "-o", outputPath]), 0);

            // Verify plugin files were created in custom output directory
            var pluginDir = Path.Combine(outputPath, "Contract_BigIntegerPlugin");
            Assert.IsTrue(Directory.Exists(pluginDir), "Plugin directory should be created in custom output");

            // Cleanup
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
        }

        [TestMethod]
        public void TestNotExist()
        {
            var path = Path.Combine(csFileDir, "Contract_NotExist.cs");
            Assert.AreEqual(Program.Main([path]), 1);
        }

        [TestMethod]
        public void TestMultiFile()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            var path2 = Path.Combine(csFileDir, "Contract_Math.cs");
            Assert.AreEqual(Program.Main([path, path2]), 0);
        }

        [TestMethod]
        public void TestNullAble()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--nullable", "Enable"]), 0);
        }

        [TestMethod]
        public void TestDebug()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--debug"]), 0);
        }

        [TestMethod]
        public void TestAssembly()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--assembly"]), 0);
        }
    }
}
