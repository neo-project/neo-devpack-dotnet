using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.InputGeneration;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class AdvancedParameterGeneratorTests
    {
        private AdvancedParameterGenerator _generator;

        [TestInitialize]
        public void Setup()
        {
            // Use a fixed seed for reproducibility
            _generator = new AdvancedParameterGenerator(42);
        }

        [TestMethod]
        public void TestGenerateInteger()
        {
            // Generate 100 integers and verify they're all valid
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameter("Integer", 0);
                Assert.IsInstanceOfType(result, typeof(VM.Types.Integer));
            }
        }

        [TestMethod]
        public void TestGenerateByteString()
        {
            // Generate 100 byte strings and verify they're all valid
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameter("ByteArray", 0);
                Assert.IsInstanceOfType(result, typeof(VM.Types.ByteString));
            }
        }

        [TestMethod]
        public void TestGenerateBoolean()
        {
            // Generate 100 booleans and verify they're all valid
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameter("Boolean", 0);
                Assert.IsTrue(result == VM.Types.StackItem.True || result == VM.Types.StackItem.False);
            }
        }

        [TestMethod]
        public void TestGenerateArray()
        {
            // Generate 100 arrays and verify they're all valid
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameter("Array", 0);
                Assert.IsInstanceOfType(result, typeof(VM.Types.Array));
            }
        }

        [TestMethod]
        public void TestGenerateMap()
        {
            // Generate 100 maps and verify they're all valid
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameter("Map", 0);
                Assert.IsInstanceOfType(result, typeof(VM.Types.Map));
            }
        }

        [TestMethod]
        public void TestGenerateEdgeCaseInteger()
        {
            // Generate 100 edge case integers and verify they include boundary values
            var values = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateEdgeCaseValue("Integer");
                Assert.IsInstanceOfType(result, typeof(VM.Types.Integer));
                values.Add(((VM.Types.Integer)result).GetInteger().ToString());
            }

            // Verify that we generated some interesting boundary values
            Assert.IsTrue(values.Contains("0"));
            Assert.IsTrue(values.Contains("1") || values.Contains("-1"));
        }

        [TestMethod]
        public void TestGenerateParameterWithContext()
        {
            // Test parameter generation with context information
            var intParam = _generator.GenerateParameterWithContext("Integer", "transfer", "amount", 0);
            Assert.IsInstanceOfType(intParam, typeof(VM.Types.Integer));

            var hashParam = _generator.GenerateParameterWithContext("Hash160", "transfer", "from", 0);
            Assert.IsInstanceOfType(hashParam, typeof(VM.Types.ByteString));
            Assert.AreEqual(20, ((VM.Types.ByteString)hashParam).GetSpan().Length);

            var boolParam = _generator.GenerateParameterWithContext("Boolean", "setActive", "isActive", 0);
            Assert.IsTrue(boolParam == VM.Types.StackItem.True || boolParam == VM.Types.StackItem.False);
        }

        [TestMethod]
        public void TestKnownGoodValues()
        {
            // Add a known good value
            var knownGood = new VM.Types.Integer(42);
            _generator.AddKnownGoodValue("transfer", "amount", knownGood);

            // Set mutation rate to 0 to ensure we always get the known good value
            _generator.SetMutationRate(0);

            // Generate a parameter and verify it's the known good value
            var result = _generator.GenerateParameterWithContext("Integer", "transfer", "amount", 0);
            Assert.IsInstanceOfType(result, typeof(VM.Types.Integer));
            Assert.AreEqual(42, ((VM.Types.Integer)result).GetInteger());
        }

        [TestMethod]
        public void TestMutationRate()
        {
            // Add a known good value
            var knownGood = new VM.Types.Integer(42);
            _generator.AddKnownGoodValue("transfer", "amount", knownGood);

            // Set mutation rate to 100 to ensure we never get the known good value
            _generator.SetMutationRate(100);

            // Generate 100 parameters and verify none are the known good value
            int matchCount = 0;
            for (int i = 0; i < 100; i++)
            {
                var result = _generator.GenerateParameterWithContext("Integer", "transfer", "amount", 0);
                if (result is VM.Types.Integer integer && integer.GetInteger() == 42)
                {
                    matchCount++;
                }
            }

            // We should have very few or no matches due to high mutation rate
            Assert.IsTrue(matchCount < 10);
        }
    }
}
