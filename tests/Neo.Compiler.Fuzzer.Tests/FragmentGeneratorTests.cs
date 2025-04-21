using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.Fuzzer;

namespace Neo.Compiler.Fuzzer.Tests
{
    [TestClass]
    public class FragmentGeneratorTests
    {
        private FragmentGenerator _generator;

        [TestInitialize]
        public void Setup()
        {
            // Use a fixed seed for deterministic tests
            _generator = new FragmentGenerator(seed: 42);
        }

        [TestMethod]
        public void TestGenerateIdentifier()
        {
            // Test that identifiers are unique
            var id1 = _generator.GenerateIdentifier("test");
            var id2 = _generator.GenerateIdentifier("test");
            var id3 = _generator.GenerateIdentifier("test");

            Assert.AreNotEqual(id1, id2);
            Assert.AreNotEqual(id1, id3);
            Assert.AreNotEqual(id2, id3);

            // Test that identifiers start with the prefix
            Assert.IsTrue(id1.StartsWith("test"));
            Assert.IsTrue(id2.StartsWith("test"));
            Assert.IsTrue(id3.StartsWith("test"));
        }

        [TestMethod]
        public void TestGeneratePrimitiveTypeDeclaration()
        {
            var code = _generator.GeneratePrimitiveTypeDeclaration();
            Assert.IsNotNull(code);
            Assert.IsTrue(code.Contains("="));

            // Should contain a type and a variable name
            var match = Regex.Match(code, @"(\w+)\s+(\w+)\s*=");
            Assert.IsTrue(match.Success);
            Assert.IsTrue(match.Groups.Count > 2);
        }

        [TestMethod]
        public void TestGenerateComplexTypeDeclaration()
        {
            var code = _generator.GenerateComplexTypeDeclaration();
            Assert.IsNotNull(code);

            // Should contain a complex type
            var complexTypes = new[] {
                "UInt160", "UInt256", "ECPoint", "ByteString",
                "Map<", "List<", "BigInteger", "StorageMap",
                "StorageContext", "Iterator<", "Notification",
                "Block", "Transaction", "Header", "Contract"
            };

            bool containsComplexType = false;
            foreach (var type in complexTypes)
            {
                if (code.Contains(type))
                {
                    containsComplexType = true;
                    break;
                }
            }

            Assert.IsTrue(containsComplexType, $"Code should contain a complex type: {code}");
        }

        [TestMethod]
        public void TestGenerateArrayDeclaration()
        {
            var code = _generator.GenerateArrayDeclaration();
            Assert.IsNotNull(code);

            // Should contain an array declaration
            Assert.IsTrue(code.Contains("[]"));
            Assert.IsTrue(code.Contains("new"));
            Assert.IsTrue(code.Contains("{"));
            Assert.IsTrue(code.Contains("}"));
        }

        [TestMethod]
        public void TestGenerateIfStatement()
        {
            var code = _generator.GenerateIfStatement();
            Assert.IsNotNull(code);

            // Should contain if and else
            Assert.IsTrue(code.Contains("if"));
            Assert.IsTrue(code.Contains("else"));
            Assert.IsTrue(code.Contains("{"));
            Assert.IsTrue(code.Contains("}"));
        }

        [TestMethod]
        public void TestGenerateForLoop()
        {
            var code = _generator.GenerateForLoop();
            Assert.IsNotNull(code);

            // Should contain for loop syntax
            Assert.IsTrue(code.Contains("for"));
            Assert.IsTrue(code.Contains("int"));
            Assert.IsTrue(code.Contains("++"));
            Assert.IsTrue(code.Contains("{"));
            Assert.IsTrue(code.Contains("}"));
        }

        [TestMethod]
        public void TestGenerateStorageOperation()
        {
            var code = _generator.GenerateStorageOperation();
            Assert.IsNotNull(code);

            // Should contain Storage operations
            Assert.IsTrue(code.Contains("Storage"));
        }

        [TestMethod]
        public void TestGenerateRuntimeOperation()
        {
            var code = _generator.GenerateRuntimeOperation();
            Assert.IsNotNull(code);

            // Should contain Runtime operations
            Assert.IsTrue(code.Contains("Runtime"));
        }

        [TestMethod]
        public void TestGenerateNativeContractCall()
        {
            var code = _generator.GenerateNativeContractCall();
            Assert.IsNotNull(code);

            // Should contain a native contract call
            var nativeContracts = new[] {
                "NEO", "GAS", "CryptoLib", "Ledger",
                "ContractManagement", "StdLib"
            };

            bool containsNativeContract = false;
            foreach (var contract in nativeContracts)
            {
                if (code.Contains(contract))
                {
                    containsNativeContract = true;
                    break;
                }
            }

            Assert.IsTrue(containsNativeContract, $"Code should contain a native contract call: {code}");
        }

        [TestMethod]
        public void TestGenerateEventDeclaration()
        {
            var code = _generator.GenerateEventDeclaration();
            Assert.IsNotNull(code);

            // Should contain event-related code
            Assert.IsTrue(code.Contains("Event"));
        }

        [TestMethod]
        public void TestGenerateEventEmission()
        {
            var code = _generator.GenerateEventEmission();
            Assert.IsNotNull(code);

            // Should contain event emission code
            Assert.IsTrue(code.Contains("Event"));
            Assert.IsTrue(code.Contains("OnMainCompleted"));
        }

        [TestMethod]
        public void TestGeneratePatternMatching()
        {
            var code = _generator.GeneratePatternMatching();
            Assert.IsNotNull(code);

            // Should contain pattern matching code
            Assert.IsTrue(code.Contains("is string"));
        }

        [TestMethod]
        public void TestGeneratePropertyPatternMatching()
        {
            var code = _generator.GeneratePropertyPatternMatching();
            Assert.IsNotNull(code);

            // Should contain property pattern matching code
            Assert.IsTrue(code.Contains("=="));
            Assert.IsTrue(code.Contains("&&"));
        }

        [TestMethod]
        public void TestGenerateSwitchExpression()
        {
            var code = _generator.GenerateSwitchExpression();
            Assert.IsNotNull(code);

            // Should contain switch expression code
            Assert.IsTrue(code.Contains("switch"));
            Assert.IsTrue(code.Contains("=>"));
        }

        [TestMethod]
        public void TestGenerateContractCall()
        {
            var code = _generator.GenerateContractCall();
            Assert.IsNotNull(code);

            // Should contain contract call code
            Assert.IsTrue(code.Contains("Contract.Call"));
            Assert.IsTrue(code.Contains("CallFlags"));
        }

        [TestMethod]
        public void TestGenerateEnhancedNativeContractOperation()
        {
            var code = _generator.GenerateEnhancedNativeContractOperation();
            Assert.IsNotNull(code);

            // Should contain enhanced native contract operations
            var operations = new[] {
                "NEO", "GAS", "Policy", "Ledger", "Oracle",
                "RoleManagement", "ContractManagement", "StdLib"
            };

            bool containsOperation = false;
            foreach (var op in operations)
            {
                if (code.Contains(op))
                {
                    containsOperation = true;
                    break;
                }
            }

            Assert.IsTrue(containsOperation, $"Code should contain an enhanced native contract operation: {code}");
        }

        [TestMethod]
        public void TestGenerateOracleCallback()
        {
            var code = _generator.GenerateOracleCallback();
            Assert.IsNotNull(code);

            // Should contain Oracle callback code
            Assert.IsTrue(code.Contains("Oracle"));
            Assert.IsTrue(code.Contains("OracleResponseCode"));
            Assert.IsTrue(code.Contains("public static void"));
        }
    }
}
