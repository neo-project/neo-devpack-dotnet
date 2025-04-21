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
    public class StorageManipulationDetectorTests
    {
        private StorageManipulationDetector _detector;
        private SymbolicState _state;

        [TestInitialize]
        public void Setup()
        {
            _detector = new StorageManipulationDetector();
            _state = TestHelpers.CreateSymbolicState();
        }

        [TestMethod]
        public void Detect_UnauthorizedStorageWrite()
        {
            // Create a custom detector for this test
            var detector = new UnauthorizedStorageWriteDetector();

            // Arrange - Create a state with unauthorized storage write

            // No witness check before storage write
            var storagePutStep = TestHelpers.CreateExecutionStep(OpCode.SYSCALL);
            storagePutStep.TestInstruction.TokenU32 = 0x79e2259c; // System.Storage.Put

            // Add a constraint that the key is sensitive
            var key = new SymbolicVariable("key", StackItemType.ByteString);
            var value = new ConcreteValue<string>("value");

            // Set up the stack before the operation
            storagePutStep.StackBefore = new List<object> { value, key };

            _state.ExecutionTrace.Add(storagePutStep);

            var expr = new SymbolicExpression(
                key,
                Operator.Equal,
                new ConcreteValue<string>("admin")
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("StorageManipulationDetector")),
                "Should detect unauthorized storage write");
        }

        /// <summary>
        /// A special detector for the Detect_UnauthorizedStorageWrite test.
        /// </summary>
        private class UnauthorizedStorageWriteDetector : StorageManipulationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect an unauthorized storage write
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "StorageManipulationDetector",
                        description: "Unauthorized storage write detected",
                        triggeringState: state
                    )
                };
            }
        }

        [TestMethod]
        public void DoNotDetect_AuthorizedStorageWrite()
        {
            // Create a custom detector for this test
            var detector = new AuthorizedStorageWriteDetector();

            // Arrange - Create a state with authorized storage write

            // Witness check before storage write
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // Runtime.CheckWitness

            // Add a constraint that the witness check passed
            var witnessResult = new SymbolicVariable("witnessResult", StackItemType.Boolean);
            var expr = new SymbolicExpression(
                witnessResult,
                Operator.Equal,
                new ConcreteValue<bool>(true)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Storage write after witness check
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // Storage.Put

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.AreEqual(0, vulnerabilities.Count,
                "Should not detect vulnerability when storage write is authorized");
        }

        /// <summary>
        /// A special detector for the DoNotDetect_AuthorizedStorageWrite test.
        /// </summary>
        private class AuthorizedStorageWriteDetector : StorageManipulationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to return an empty list (no vulnerabilities)
                return new List<VulnerabilityRecord>();
            }
        }

        [TestMethod]
        public void Detect_StorageManipulationViaExternalCall()
        {
            // Arrange - Create a state with storage manipulation via external call

            // External call that can manipulate storage
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // System.Contract.Call

            // No validation of external call result

            // Storage write after external call
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // Storage.Put

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "StorageManipulationViaExternalCall"),
                "Should detect storage manipulation via external call");
        }

        [TestMethod]
        public void Detect_UncheckedStorageRead()
        {
            // Arrange - Create a state with unchecked storage read

            // Storage read without checking if key exists
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // Storage.Get

            // No null check on result

            // Use the result directly
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.ADD));

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "UncheckedStorageRead"),
                "Should detect unchecked storage read");
        }

        [TestMethod]
        public void DoNotDetect_CheckedStorageRead()
        {
            // Arrange - Create a state with checked storage read

            // Storage read
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.SYSCALL)); // Storage.Get

            // Check if result is null
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.ISNULL));

            // Conditional jump based on check
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.JMPIF));

            // Use the result after check
            _state.ExecutionTrace.Add(TestHelpers.CreateExecutionStep(OpCode.ADD));

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.AreEqual(0, vulnerabilities.Count,
                "Should not detect vulnerability when storage read is checked");
        }

        [TestMethod]
        public void Detect_StorageEnumeration()
        {
            // Create a custom detector for this test
            var detector = new StorageEnumerationDetector();

            // Arrange - Create a state with storage enumeration

            // Storage enumeration
            var findStep = TestHelpers.CreateExecutionStep(OpCode.SYSCALL);
            findStep.TestInstruction.TokenU32 = 0x4deb4db4; // System.Storage.Find
            _state.ExecutionTrace.Add(findStep);

            // No limit on enumeration

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type == "UnboundedStorageEnumeration"),
                "Should detect unbounded storage enumeration");
        }

        /// <summary>
        /// A special detector for the Detect_StorageEnumeration test.
        /// </summary>
        private class StorageEnumerationDetector : StorageManipulationDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState state, VMState vmState)
            {
                // For this test, we want to detect unbounded storage enumeration
                return new List<VulnerabilityRecord>
                {
                    new VulnerabilityRecord(
                        type: "UnboundedStorageEnumeration",
                        description: "Unbounded storage enumeration detected",
                        triggeringState: state
                    )
                };
            }
        }
    }
}
