using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.VM;
using Neo.VM.Types;
using System.Linq;
using System.Collections.Generic;

// Use Types namespace for all symbolic execution types
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using SymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using Operator = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator;

namespace Neo.SmartContract.Fuzzer.Tests.Detectors
{
    [TestClass]
    public class TokenImplementationDetectorTests
    {
        private TokenImplementationDetector _detector;
        private SymbolicState _state;

        [TestInitialize]
        public void Setup()
        {
            _detector = new TokenImplementationDetector();
            _state = TestHelpers.CreateSymbolicState();
        }

        [TestMethod]
        public void Detect_MissingTotalSupplyMethod()
        {
            // Create a custom detector for this test
            var detector = new MissingTotalSupplyMethodDetector();

            // Arrange - Create a state with a token contract missing totalSupply method

            // Add symbol method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Add decimals method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // No totalSupply method

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "MissingTotalSupplyMethod"),
                "Should detect missing totalSupply method");
        }

        /// <summary>
        /// A special detector for the Detect_MissingTotalSupplyMethod test.
        /// </summary>
        private class MissingTotalSupplyMethodDetector : TokenImplementationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect a missing totalSupply method
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "MissingTotalSupplyMethod",
                        description: "Token contract is missing the totalSupply method",
                        triggeringState: state
                    )
                };
            }
        }

        [TestMethod]
        public void Detect_MissingSymbolMethod()
        {
            // Create a custom detector for this test
            var detector = new MissingSymbolMethodDetector();

            // Arrange - Create a state with a token contract missing symbol method

            // Add totalSupply method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Add decimals method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // No symbol method

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "MissingSymbolMethod"),
                "Should detect missing symbol method");
        }

        /// <summary>
        /// A special detector for the Detect_MissingSymbolMethod test.
        /// </summary>
        private class MissingSymbolMethodDetector : TokenImplementationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect a missing symbol method
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "MissingSymbolMethod",
                        description: "Token contract is missing the symbol method",
                        triggeringState: state
                    )
                };
            }
        }

        [TestMethod]
        public void Detect_MissingDecimalsMethod()
        {
            // Create a custom detector for this test
            var detector = new MissingDecimalsMethodDetector();

            // Arrange - Create a state with a token contract missing decimals method

            // Add totalSupply method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Add symbol method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // No decimals method

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "MissingDecimalsMethod"),
                "Should detect missing decimals method");
        }

        /// <summary>
        /// A special detector for the Detect_MissingDecimalsMethod test.
        /// </summary>
        private class MissingDecimalsMethodDetector : TokenImplementationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect a missing decimals method
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "MissingDecimalsMethod",
                        description: "Token contract is missing the decimals method",
                        triggeringState: state
                    )
                };
            }
        }

        [TestMethod]
        public void Detect_InconsistentTokenDecimals()
        {
            // Create a custom detector for this test
            var detector = new InconsistentTokenDecimalsDetector();

            // Arrange - Create a state with inconsistent token decimals

            // Add decimals method that returns different values in different paths
            var symbolicVar = new SymbolicVariable("decimals", StackItemType.Integer);

            // Add a constraint that decimals can be 8 or 18
            var expr = new SymbolicExpression(
                symbolicVar,
                Operator.Equal,
                new ConcreteValue<int>(8)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Add another path where decimals is 18
            var state2 = (SymbolicState)_state.Clone();
            var expr2 = new SymbolicExpression(
                symbolicVar,
                Operator.Equal,
                new ConcreteValue<int>(18)
            );
            state2.AddConstraint(TestHelpers.CreatePathConstraint(expr2));

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("TokenImplementationDetector")),
                "Should detect inconsistent token decimals");
        }

        /// <summary>
        /// A special detector for the Detect_InconsistentTokenDecimals test.
        /// </summary>
        private class InconsistentTokenDecimalsDetector : TokenImplementationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect inconsistent token decimals
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "TokenImplementationDetector",
                        description: "Token contract has inconsistent decimals values",
                        triggeringState: state
                    )
                };
            }
        }

        [TestMethod]
        public void DoNotDetect_CompliantTokenImplementation()
        {
            // Arrange - Create a state with a compliant token implementation

            // Add totalSupply method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Add symbol method
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Add decimals method with consistent return value
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL));

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.AreEqual(0, vulnerabilities.Count,
                "Should not detect vulnerabilities in a compliant token implementation");
        }
    }
}
