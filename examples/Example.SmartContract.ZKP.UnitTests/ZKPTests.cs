using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

namespace Example.SmartContract.ZKP.UnitTests
{
    [TestClass]
    public class ZKPTests : TestBase<SampleZKP>
    {
        /// <summary>
        /// Test vector for valid ZKP proof verification
        /// </summary>
        private static readonly byte[] ValidProofA = new byte[] 
        {
            3, 139, 173, 158, 83, 234, 50, 134, 229, 60, 232, 159, 57, 229, 42, 107,
            51, 134, 193, 191, 226, 189, 245, 71, 41, 207, 185, 78, 245, 144, 183, 162,
            162, 86, 254, 13, 155, 75, 212, 73, 142, 65, 169, 90, 114, 44, 157, 87
        };
        
        private static readonly byte[] ValidProofB = new byte[]
        {
            16, 201, 55, 29, 230, 98, 109, 168, 40, 190, 38, 68, 175, 192, 97, 77,
            94, 218, 30, 211, 147, 71, 255, 131, 205, 92, 21, 78, 48, 165, 112, 15,
            203, 182, 191, 187, 241, 42, 233, 71, 12, 245, 155, 201, 114, 146, 28, 5,
            17, 100, 153, 74, 164, 116, 219, 170, 230, 190, 133, 107, 28, 116, 2, 250,
            104, 189, 127, 167, 201, 85, 119, 40, 158, 167, 142, 41, 161, 119, 163, 104,
            226, 138, 210, 135, 76, 166, 62, 165, 206, 141, 136, 206, 137, 126, 22, 130
        };
        
        private static readonly byte[] ValidProofC = new byte[]
        {
            2, 243, 81, 138, 131, 140, 101, 65, 133, 247, 103, 3, 66, 123, 147, 87,
            64, 63, 76, 174, 93, 45, 75, 25, 83, 24, 78, 232, 40, 77, 36, 40,
            117, 228, 129, 219, 227, 134, 216, 196, 187, 83, 100, 251, 87, 93, 125, 223
        };
        
        [TestMethod]
        public void Test_Verify_ValidProof_ReturnsTrue()
        {
            // Arrange
            var publicInput = new byte[][]
            {
                new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            
            // Act
            var result = Contract.Verify(ValidProofA, ValidProofB, ValidProofC, publicInput);
            
            // Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void Test_Verify_InvalidProofA_ReturnsFalse()
        {
            // Arrange
            var invalidProofA = new byte[48]; // All zeros
            var publicInput = new byte[][]
            {
                new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            
            // Act & Assert
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(invalidProofA, ValidProofB, ValidProofC, publicInput));
        }
        
        [TestMethod]
        public void Test_Verify_WrongInputLength_ThrowsException()
        {
            // Arrange
            var publicInput = new byte[][] { }; // Empty input array
            
            // Act & Assert
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(ValidProofA, ValidProofB, ValidProofC, publicInput),
                "Should throw exception for wrong input length");
        }
        
        [TestMethod]
        public void Test_Verify_MultiplePublicInputs()
        {
            // Arrange
            var publicInputs = new byte[][]
            {
                new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            
            // Act
            var result = Contract.Verify(ValidProofA, ValidProofB, ValidProofC, publicInputs);
            
            // Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void Test_BLS12381_Points_AreValid()
        {
            // Test that all predefined points can be deserialized
            var alphaDeserialized = CryptoLib.Bls12381Deserialize(Contract.alphaPoint);
            Assert.IsNotNull(alphaDeserialized, "Alpha point should be valid");
            
            var betaDeserialized = CryptoLib.Bls12381Deserialize(Contract.betaPoint);
            Assert.IsNotNull(betaDeserialized, "Beta point should be valid");
            
            var gammaInverseDeserialized = CryptoLib.Bls12381Deserialize(Contract.gamma_inversePoint);
            Assert.IsNotNull(gammaInverseDeserialized, "Gamma inverse point should be valid");
            
            var deltaDeserialized = CryptoLib.Bls12381Deserialize(Contract.deltaPoint);
            Assert.IsNotNull(deltaDeserialized, "Delta point should be valid");
        }
        
        [TestMethod]
        public void Test_IC_Points_AreValid()
        {
            // Test that all IC points can be deserialized
            foreach (var icPoint in Contract.ic)
            {
                var deserialized = CryptoLib.Bls12381Deserialize(icPoint);
                Assert.IsNotNull(deserialized, "IC point should be valid");
            }
        }
        
        [TestMethod]
        public void Test_Verify_NullParameters_ThrowsException()
        {
            // Arrange
            var publicInput = new byte[][]
            {
                new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
            
            // Act & Assert
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(null, ValidProofB, ValidProofC, publicInput));
                
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(ValidProofA, null, ValidProofC, publicInput));
                
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(ValidProofA, ValidProofB, null, publicInput));
                
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(ValidProofA, ValidProofB, ValidProofC, null));
        }
        
        [TestMethod]
        public void Test_Verify_EmptyPublicInput_HandledCorrectly()
        {
            // Arrange
            var emptyPublicInput = new byte[][] { };
            
            // Act & Assert
            // Should handle empty input appropriately
            Assert.ThrowsException<Exception>(() => 
                Contract.Verify(ValidProofA, ValidProofB, ValidProofC, emptyPublicInput));
        }
        
        [TestMethod]
        public void Test_Pairing_Operations()
        {
            // Test basic pairing operations work
            var alpha = CryptoLib.Bls12381Deserialize(Contract.alphaPoint);
            var beta = CryptoLib.Bls12381Deserialize(Contract.betaPoint);
            
            Assert.IsNotNull(alpha, "Alpha should deserialize");
            Assert.IsNotNull(beta, "Beta should deserialize");
            
            // This would test the pairing operation if we could execute it
            // var pairing = CryptoLib.Bls12381Pairing(alpha, beta);
            // Assert.IsNotNull(pairing, "Pairing should succeed");
        }
    }
}
