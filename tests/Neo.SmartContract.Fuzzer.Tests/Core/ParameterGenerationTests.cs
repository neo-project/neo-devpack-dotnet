using System;
using System.IO;
using System.Linq;
using Neo.SmartContract.Fuzzer.ParameterGeneration;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.Core
{
    /// <summary>
    /// Tests for the parameter generation system
    /// </summary>
    public class ParameterGenerationTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private readonly string _manifestPath;

        /// <summary>
        /// Initialize test data
        /// </summary>
        public ParameterGenerationTests()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
        }

        /// <summary>
        /// Test basic parameter generation for different types
        /// </summary>
        [Fact]
        public void BasicTypeGenerationTest()
        {
            // Arrange
            var generator = new EnhancedParameterGenerator(seed: 12345);
            
            // Act & Assert
            
            // Boolean
            var boolParam = generator.GenerateParameter("Boolean");
            Assert.IsType<Boolean>(boolParam);
            
            // Integer
            var intParam = generator.GenerateParameter("Integer");
            Assert.IsType<Integer>(intParam);
            
            // String
            var stringParam = generator.GenerateParameter("String");
            Assert.IsType<ByteString>(stringParam);
            
            // ByteArray
            var byteArrayParam = generator.GenerateParameter("ByteArray");
            Assert.IsType<ByteString>(byteArrayParam);
            
            // Hash160
            var hash160Param = generator.GenerateParameter("Hash160");
            Assert.IsType<ByteString>(hash160Param);
            Assert.Equal(20, hash160Param.GetSpan().Length);
            
            // Hash256
            var hash256Param = generator.GenerateParameter("Hash256");
            Assert.IsType<ByteString>(hash256Param);
            Assert.Equal(32, hash256Param.GetSpan().Length);
        }

        /// <summary>
        /// Test collection type generation
        /// </summary>
        [Fact]
        public void CollectionTypeGenerationTest()
        {
            // Arrange
            var generator = new EnhancedParameterGenerator(seed: 12345);
            
            // Act
            var arrayParam = generator.GenerateParameter("Array");
            var mapParam = generator.GenerateParameter("Map");
            
            // Assert
            Assert.IsType<VM.Types.Array>(arrayParam);
            Assert.IsType<VM.Types.Map>(mapParam);
            
            // Check array elements
            var array = (VM.Types.Array)arrayParam;
            Assert.True(array.Count >= 0); // Could be empty or have elements
            
            // Check map entries
            var map = (VM.Types.Map)mapParam;
            Assert.True(map.Count >= 0); // Could be empty or have entries
        }

        /// <summary>
        /// Test parameter generation with manifest initialization
        /// </summary>
        [Fact]
        public void ManifestBasedGenerationTest()
        {
            // Arrange
            var generator = new EnhancedParameterGenerator(seed: 12345);
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            // Act
            generator.InitializeFromManifest(manifest);
            
            // Find the transfer method
            var transferMethod = manifest.Abi.Methods.FirstOrDefault(m => m.Name == "transfer");
            Assert.NotNull(transferMethod);
            
            // Generate parameters for the transfer method
            var parameters = generator.GenerateParameters(transferMethod.Name, transferMethod.Parameters);
            
            // Assert
            Assert.Equal(4, parameters.Length);
            
            // Check parameter types
            Assert.IsType<ByteString>(parameters[0]); // from: Hash160
            Assert.IsType<ByteString>(parameters[1]); // to: Hash160
            Assert.IsType<Integer>(parameters[2]);    // amount: Integer
            
            // The 4th parameter is 'data' of type 'Any', which could be any type
            Assert.NotNull(parameters[3]);
        }

        /// <summary>
        /// Test context-aware parameter generation
        /// </summary>
        [Fact]
        public void ContextAwareGenerationTest()
        {
            // Arrange
            var generator = new EnhancedParameterGenerator(seed: 12345);
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            generator.InitializeFromManifest(manifest);
            
            // Find the balanceOf method
            var balanceOfMethod = manifest.Abi.Methods.FirstOrDefault(m => m.Name == "balanceOf");
            Assert.NotNull(balanceOfMethod);
            
            // Act
            // Generate parameters for the balanceOf method
            var parameters = generator.GenerateParameters(balanceOfMethod.Name, balanceOfMethod.Parameters);
            
            // Assert
            Assert.Single(parameters);
            
            // Check parameter type
            Assert.IsType<ByteString>(parameters[0]); // account: Hash160
            Assert.Equal(20, parameters[0].GetSpan().Length); // UInt160 is 20 bytes
        }

        /// <summary>
        /// Test parameter generation with a fixed seed for reproducibility
        /// </summary>
        [Fact]
        public void ReproducibleGenerationTest()
        {
            // Arrange
            int seed = 42;
            var generator1 = new EnhancedParameterGenerator(seed);
            var generator2 = new EnhancedParameterGenerator(seed);
            
            // Act
            var boolParam1 = generator1.GenerateParameter("Boolean");
            var intParam1 = generator1.GenerateParameter("Integer");
            var stringParam1 = generator1.GenerateParameter("String");
            
            var boolParam2 = generator2.GenerateParameter("Boolean");
            var intParam2 = generator2.GenerateParameter("Integer");
            var stringParam2 = generator2.GenerateParameter("String");
            
            // Assert
            Assert.Equal(boolParam1.ToJson().ToString(), boolParam2.ToJson().ToString());
            Assert.Equal(intParam1.ToJson().ToString(), intParam2.ToJson().ToString());
            Assert.Equal(stringParam1.ToJson().ToString(), stringParam2.ToJson().ToString());
        }

        /// <summary>
        /// Test boundary value generation for numeric types
        /// </summary>
        [Fact]
        public void BoundaryValueGenerationTest()
        {
            // Arrange
            var generator = new EnhancedParameterGenerator(seed: 12345);
            var values = new System.Collections.Generic.List<Integer>();
            
            // Act
            // Generate a large number of integers to increase the chance of getting boundary values
            for (int i = 0; i < 1000; i++)
            {
                var intParam = (Integer)generator.GenerateParameter("Integer");
                values.Add(intParam);
            }
            
            // Assert
            // Check if we generated some common boundary values
            var bigIntValues = values.Select(v => v.GetInteger()).ToList();
            
            // Check for zero
            Assert.Contains(BigInteger.Zero, bigIntValues);
            
            // Check for small values
            Assert.Contains(BigInteger.One, bigIntValues);
            Assert.Contains(BigInteger.MinusOne, bigIntValues);
            
            // Check for large values (harder to assert exact values)
            Assert.Contains(bigIntValues, v => v > 1000000);
            Assert.Contains(bigIntValues, v => v < -1000000);
        }
    }
}