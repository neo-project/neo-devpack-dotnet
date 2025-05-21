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
    public class NeoN3SpecificFeaturesTests
    {
        private FragmentGenerator _generator;
        private string _testOutputDir;

        [TestInitialize]
        public void TestSetup()
        {
            // Create a fragment generator with a fixed seed for reproducibility
            _generator = new FragmentGenerator(seed: 42);

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
        public void TestGenerateNFTOperations()
        {
            // Generate NFT operations
            string code = _generator.GenerateNFTOperations();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("NFT"), "NFT operations should contain 'NFT'");
        }

        [TestMethod]
        public void TestGenerateNameServiceOperations()
        {
            // Generate NameService operations
            string code = _generator.GenerateNameServiceOperations();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("NameService"), "NameService operations should contain 'NameService'");
        }

        [TestMethod]
        public void TestGenerateEnhancedCryptographyOperations()
        {
            // Generate enhanced cryptography operations
            string code = _generator.GenerateEnhancedCryptographyOperations();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("CryptoLib") || code.Contains("Hash") || code.Contains("Verify"),
                "Enhanced cryptography operations should contain cryptographic terms");
        }

        [TestMethod]
        public void TestGenerateIOSOperations()
        {
            // Generate IOS operations
            string code = _generator.GenerateIOSOperations();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("ExecutionEngine") || code.Contains("Iterator") ||
                          code.Contains("bytes") || code.Contains("str"),
                "IOS operations should contain interoperability service terms");
        }

        [TestMethod]
        public void TestGenerateAttributeUsage()
        {
            // Generate attribute usage
            string code = _generator.GenerateAttributeUsage();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("["), "Attribute usage should contain '['");
            Assert.IsTrue(code.Contains("]"), "Attribute usage should contain ']'");
        }

        [TestMethod]
        public void TestGenerateOracleCallback()
        {
            // Generate Oracle callback
            string code = _generator.GenerateOracleCallback();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("Oracle"), "Oracle callback should contain 'Oracle'");
            Assert.IsTrue(code.Contains("OracleResponseCode"), "Oracle callback should contain 'OracleResponseCode'");
        }

        [TestMethod]
        public void TestGenerateStructUsage()
        {
            // Generate struct usage
            string code = _generator.GenerateStructUsage();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("struct") || code.Contains("structExample"), "Struct usage should contain 'struct' or 'structExample'");
        }

        [TestMethod]
        public void TestGenerateStructDeclaration()
        {
            // Generate struct declaration
            string code = _generator.GenerateStructDeclaration();

            // Verify the code was generated
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("struct"), "Struct declaration should contain 'struct'");
        }

        [TestMethod]
        public void TestCompileContractWithNeoN3Features()
        {
            // Create a contract with Neo N3 features
            var contractBuilder = new StringBuilder();
            contractBuilder.AppendLine("using Neo.SmartContract.Framework;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Attributes;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Services;");
            contractBuilder.AppendLine("using Neo.SmartContract.Framework.Native;");
            contractBuilder.AppendLine("using System;");
            contractBuilder.AppendLine("using System.Numerics;");
            contractBuilder.AppendLine("using System.ComponentModel;");
            contractBuilder.AppendLine();
            contractBuilder.AppendLine("namespace NeoN3FeaturesContract");
            contractBuilder.AppendLine("{");
            contractBuilder.AppendLine("    [DisplayName(\"NeoN3Features\")]");
            contractBuilder.AppendLine("    [ManifestExtra(\"Author\", \"Neo\")]");
            contractBuilder.AppendLine("    [SupportedStandards(\"NEP-17\")]");
            contractBuilder.AppendLine("    public class NeoN3FeaturesContract : SmartContract");
            contractBuilder.AppendLine("    {");

            // Add Neo N3 features
            contractBuilder.AppendLine("        // Storage feature");
            contractBuilder.AppendLine("        public static void TestStorage()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            Storage.Put(Storage.CurrentContext, \"test_key\", \"test_value\");");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Runtime feature");
            contractBuilder.AppendLine("        public static void TestRuntime()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            Runtime.Log(\"Test log\");");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Native contract feature");
            contractBuilder.AppendLine("        public static string TestNativeContract()");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            return NEO.Symbol;");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("        // Oracle callback");
            contractBuilder.AppendLine("        public static void OracleCallback(string url, byte[] userData, OracleResponseCode code, string result)");
            contractBuilder.AppendLine("        {");
            contractBuilder.AppendLine("            if (code == OracleResponseCode.Success)");
            contractBuilder.AppendLine("            {");
            contractBuilder.AppendLine("                Storage.Put(Storage.CurrentContext, \"oracle_result\", result);");
            contractBuilder.AppendLine("            }");
            contractBuilder.AppendLine("        }");
            contractBuilder.AppendLine();

            contractBuilder.AppendLine("    }");
            contractBuilder.AppendLine("}");

            // Save the contract to a file
            string contractPath = Path.Combine(_testOutputDir, "NeoN3FeaturesContract.cs");
            File.WriteAllText(contractPath, contractBuilder.ToString());

            // Create a contract compiler
            var compiler = new ContractCompiler(_testOutputDir);

            // Compile the contract
            var result = compiler.Compile(contractPath);

            // Verify compilation succeeded
            Assert.IsTrue(result.Success, "Neo N3 features contract compilation should succeed");

            // Verify output files were created
            Assert.IsTrue(File.Exists(result.NefPath), "NEF file should be created");
            Assert.IsTrue(File.Exists(result.ManifestPath), "Manifest file should be created");
        }
    }
}
