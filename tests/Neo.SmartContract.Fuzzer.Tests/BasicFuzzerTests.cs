using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class BasicFuzzerTests
    {
        [TestMethod]
        public void FuzzerConfiguration_DefaultValues()
        {
            // Act
            var config = new FuzzerConfiguration();

            // Assert
            Assert.AreEqual(string.Empty, config.NefPath);
            Assert.AreEqual(string.Empty, config.ManifestPath);
            Assert.AreEqual(string.Empty, config.SourcePath);
            Assert.AreEqual("fuzzer-output", config.OutputDirectory);
            Assert.AreEqual(10, config.IterationsPerMethod);
            Assert.AreEqual(20_000_000, config.GasLimit);
            Assert.IsTrue(config.EnableCoverage);
            Assert.AreEqual("html", config.CoverageFormat);
            Assert.IsFalse(config.PersistStateBetweenCalls);
            Assert.IsFalse(config.SaveFailingInputsOnly);
            Assert.IsNotNull(config.MethodsToFuzz);
            Assert.AreEqual(0, config.MethodsToFuzz.Count);
            Assert.IsTrue(config.EnableCoverageGuidedFuzzing);
            Assert.IsTrue(config.PrioritizeBranchCoverage);
            Assert.IsTrue(config.PrioritizePathCoverage);
        }

        [TestMethod]
        public void FuzzerConfiguration_CustomValues()
        {
            // Act
            var config = new FuzzerConfiguration
            {
                NefPath = "test.nef",
                ManifestPath = "test.manifest.json",
                SourcePath = "test.cs",
                OutputDirectory = "custom-output",
                IterationsPerMethod = 100,
                GasLimit = 10_000_000,
                EnableCoverage = false,
                CoverageFormat = "json",
                PersistStateBetweenCalls = true,
                SaveFailingInputsOnly = true,
                EnableCoverageGuidedFuzzing = false,
                PrioritizeBranchCoverage = false,
                PrioritizePathCoverage = false
            };

            // Assert
            Assert.AreEqual("test.nef", config.NefPath);
            Assert.AreEqual("test.manifest.json", config.ManifestPath);
            Assert.AreEqual("test.cs", config.SourcePath);
            Assert.AreEqual("custom-output", config.OutputDirectory);
            Assert.AreEqual(100, config.IterationsPerMethod);
            Assert.AreEqual(10_000_000, config.GasLimit);
            Assert.IsFalse(config.EnableCoverage);
            Assert.AreEqual("json", config.CoverageFormat);
            Assert.IsTrue(config.PersistStateBetweenCalls);
            Assert.IsTrue(config.SaveFailingInputsOnly);
            Assert.IsFalse(config.EnableCoverageGuidedFuzzing);
            Assert.IsFalse(config.PrioritizeBranchCoverage);
            Assert.IsFalse(config.PrioritizePathCoverage);
        }

        [TestMethod]
        public void FuzzerConfiguration_AddMethodToFuzz()
        {
            // Arrange
            var config = new FuzzerConfiguration();

            // Act
            config.MethodsToFuzz.Add("TestMethod");

            // Assert
            Assert.AreEqual(1, config.MethodsToFuzz.Count);
            Assert.AreEqual("TestMethod", config.MethodsToFuzz[0]);
        }
    }
}
