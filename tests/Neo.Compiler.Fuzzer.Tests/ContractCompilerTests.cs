using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Neo.Compiler.Fuzzer;

namespace Neo.Compiler.Fuzzer.Tests
{
    [TestClass]
    public class ContractCompilerTests
    {
        private string _testOutputDir;

        [TestInitialize]
        public void TestSetup()
        {
            // Create a temporary directory for test output
            _testOutputDir = Path.Combine(Path.GetTempPath(), "Neo.Compiler.Fuzzer.Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testOutputDir);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up the test output directory
            if (Directory.Exists(_testOutputDir))
            {
                Directory.Delete(_testOutputDir, true);
            }
        }

        [TestMethod]
        public void TestCompilerInitialization()
        {
            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Verify the compiler was created successfully
            Assert.IsNotNull(compiler);
        }

        [TestMethod]
        public void TestCompileMinimalContract()
        {
            // Create a minimal valid contract
            string contractCode = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System;

namespace MinimalContract
{
    public class MinimalToken : SmartContract
    {
        [Safe]
        public static string Name() => ""Minimal"";
    }
}";

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "MinimalContract.cs");
            File.WriteAllText(contractPath, contractCode);

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var result = compiler.Compile(contractPath);

            // Verify compilation succeeded
            Assert.IsTrue(result.Success, "Minimal contract compilation should succeed");

            // Verify output files were created
            Assert.IsTrue(File.Exists(result.NefPath), "NEF file should be created");
            Assert.IsTrue(File.Exists(result.ManifestPath), "Manifest file should be created");
            Assert.IsTrue(File.Exists(result.DebugInfoPath), "Debug info file should be created");
        }

        [TestMethod]
        public void TestCompileContractWithFeatures()
        {
            // Create a fragment generator
            var fragmentGenerator = new FragmentGenerator(seed: 42);

            // Generate a contract with multiple features
            var contractBuilder = new StringBuilder();
            contractBuilder.AppendLine("using Neo.SmartContract.Framework;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Attributes;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Services;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Native;");
            contractBuilder.AppendLine("using System;");
            contractBuilder.AppendLine("using System.Numerics;");
            contractBuilder.AppendLine("using System.ComponentModel;");
            contractBuilder.AppendLine();
            contractBuilder.AppendLine("namespace FeatureRichContract");
            contractBuilder.AppendLine("{");
            contractBuilder.AppendLine("    [DisplayName(\"FeatureRichToken\")]");
            contractBuilder.AppendLine("    [ManifestExtra(\"Author\", \"Neo\")]");
            contractBuilder.AppendLine("    public class FeatureRichToken : SmartContract");
            contractBuilder.AppendLine("    {");

            // Add various features as methods
            contractBuilder.AppendLine("        // Feature: Storage");
            contractBuilder.AppendLine("        public static void TestStorage()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            string key = \"test_key\";");
            contractBuilder.AppendLine("            string value = \"test_value\";");
            contractBuilder.AppendLine("            Storage.Put(Storage.CurrentContext, key, value);");
            contractBuilder.AppendLine("            string retrieved = Storage.Get(Storage.CurrentContext, key);");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Feature: Runtime");
            contractBuilder.AppendLine("        public static void TestRuntime()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            string runtimePlatform = Runtime.Platform;");
            contractBuilder.AppendLine("            UInt160 runtimeExecScriptHash = Runtime.ExecutingScriptHash;");
            contractBuilder.AppendLine("            Runtime.Log(\"Test log message\");");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Feature: Native Contract");
            contractBuilder.AppendLine("        public static void TestNativeContract()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            BigInteger neoBalance = NEO.BalanceOf(Runtime.ExecutingScriptHash);");
            contractBuilder.AppendLine("            BigInteger gasBalance = GAS.BalanceOf(Runtime.ExecutingScriptHash);");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Main method");
            contractBuilder.AppendLine("        public static bool Main(string operation, object[] args)");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            if (operation == \"test\")");
            contractBuilder.AppendLine("            {");
            contractBuilder.AppendLine("                return true;");
            contractBuilder.AppendLine("            }");
            contractBuilder.AppendLine("            return false;");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("    }");
            contractBuilder.AppendLine("}");

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "FeatureRichContract.cs");
            File.WriteAllText(contractPath, contractBuilder.ToString());

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var result = compiler.Compile(contractPath);

            // Verify compilation succeeded
            Assert.IsTrue(result.Success, "Feature-rich contract compilation should succeed");

            // Verify output files were created
            Assert.IsTrue(File.Exists(result.NefPath), "NEF file should be created");
            Assert.IsTrue(File.Exists(result.ManifestPath), "Manifest file should be created");
            Assert.IsTrue(File.Exists(result.DebugInfoPath), "Debug info file should be created");
        }

        [TestMethod]
        public void TestCompileInvalidContract()
        {
            // Create an invalid contract with syntax errors
            string contractCode = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System;

namespace InvalidContract
{
    public class InvalidToken : SmartContract
    {
        [Safe]
        public static string Name() => ""Invalid"";

        // Syntax error: missing semicolon
        public static void BrokenMethod() {
            int x = 1
            // This will cause an error
            x++;
        }

        // Note: Neo N3 contracts don't need a Main entry point
    }
}";

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "InvalidContract.cs");
            File.WriteAllText(contractPath, contractCode);

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var result = compiler.Compile(contractPath);

            // Verify compilation result
            // Note: The contract might still compile but will have warnings
            if (result.Success)
            {
                // If it compiles, check for errors in the diagnostics
                Assert.IsTrue(result.Errors.Length > 0, "Invalid contract should have errors or warnings");
            }
            else
            {
                // If it fails, verify output files were not created
                Assert.IsFalse(File.Exists(Path.Combine(_testOutputDir, "InvalidContract.nef")), "NEF file should not be created");
                Assert.IsFalse(File.Exists(Path.Combine(_testOutputDir, "InvalidContract.manifest.json")), "Manifest file should not be created");
            }
        }

        [TestMethod]
        public void TestExecutionTesting()
        {
            // Create a valid contract with a Main method
            string contractCode = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

namespace TestContract
{
    public class TestToken : SmartContract
    {
        public static bool Main(string operation, object[] args)
        {
            if (operation == ""test"")
            {
                Runtime.Log(""Test executed"");
                return true;
            }
            return false;
        }
    }
}";

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "TestContract.cs");
            File.WriteAllText(contractPath, contractCode);

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var compilationResult = compiler.Compile(contractPath);

            // Verify compilation succeeded
            Assert.IsTrue(compilationResult.Success, "Test contract compilation should succeed");

            // Test execution
            var executionResult = compiler.TestExecution(compilationResult);

            // Verify execution testing
            Assert.IsTrue(executionResult.Success, "Execution testing should succeed");
        }
    }
}
