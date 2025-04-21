using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class ParameterGeneratorTests
    {
        [TestMethod]
        public void TestIntegerGeneration()
        {
            // Create parameter generator with fixed seed for reproducibility
            var generator = new ParameterGenerator(42);
            
            // Generate integer parameters
            var param1 = generator.GenerateParameter("Integer");
            var param2 = generator.GenerateParameter("Integer");
            
            // Assert that parameters are of the correct type
            Assert.IsInstanceOfType(param1, typeof(Integer));
            Assert.IsInstanceOfType(param2, typeof(Integer));
            
            // Assert that different values are generated
            Assert.AreNotEqual(param1.GetInteger(), param2.GetInteger());
        }
        
        [TestMethod]
        public void TestBooleanGeneration()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate boolean parameters
            var param = generator.GenerateParameter("Boolean");
            
            // Assert that parameter is of the correct type
            Assert.IsInstanceOfType(param, typeof(Boolean));
        }
        
        [TestMethod]
        public void TestStringGeneration()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate string parameters
            var param = generator.GenerateParameter("String");
            
            // Assert that parameter is of the correct type
            Assert.IsInstanceOfType(param, typeof(ByteString));
            
            // Try to convert to string
            string str = System.Text.Encoding.UTF8.GetString(param.GetSpan());
            
            // Assert that the string is not empty
            Assert.IsFalse(string.IsNullOrEmpty(str));
        }
        
        [TestMethod]
        public void TestHash160Generation()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate Hash160 parameters
            var param = generator.GenerateParameter("Hash160");
            
            // Assert that parameter is of the correct type
            Assert.IsInstanceOfType(param, typeof(ByteString));
            
            // Assert that the byte length is correct for Hash160
            Assert.AreEqual(20, param.GetSpan().Length);
        }
        
        [TestMethod]
        public void TestArrayGeneration()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate array parameters
            var param = generator.GenerateParameter("Array");
            
            // Assert that parameter is of the correct type
            Assert.IsInstanceOfType(param, typeof(VM.Types.Array));
            
            // Assert that the array has elements
            Assert.IsTrue(((VM.Types.Array)param).Count > 0);
        }
        
        [TestMethod]
        public void TestMapGeneration()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate map parameters
            var param = generator.GenerateParameter("Map");
            
            // Assert that parameter is of the correct type
            Assert.IsInstanceOfType(param, typeof(VM.Types.Map));
        }
        
        [TestMethod]
        public void TestUnsupportedTypeGeneration()
        {
            // Create parameter generator
            var generator = new ParameterGenerator(42);
            
            // Generate parameter for unsupported type
            var param = generator.GenerateParameter("UnsupportedType");
            
            // Assert that a default value is returned (null)
            Assert.IsTrue(param.IsNull);
        }
    }
}
