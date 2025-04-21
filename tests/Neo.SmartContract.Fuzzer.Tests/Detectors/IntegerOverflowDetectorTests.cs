using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Detectors;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Linq;
using System.Collections.Generic;

// Use Types namespace for all symbolic execution types
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using SymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using Operator = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator;

namespace Neo.SmartContract.Fuzzer.Tests.Detectors
{
    [TestClass]
    public class IntegerOverflowDetectorTests
    {
        private IntegerOverflowDetector _detector;
        private SymbolicState _state;

        [TestInitialize]
        public void Setup()
        {
            _detector = new IntegerOverflowDetector();
            _state = TestHelpers.CreateSymbolicState();
        }

        [TestMethod]
        public void Detect_IntegerAdditionOverflow()
        {
            // Arrange - Create a state with integer addition that can overflow
            var var1 = new SymbolicVariable("var1", StackItemType.Integer);
            var var2 = new SymbolicVariable("var2", StackItemType.Integer);

            // Create an ADD instruction step
            var addStep = TestHelpers.CreateExecutionStep(OpCode.ADD);

            // Set up the stack before the operation with two symbolic variables
            // Stack order in Neo VM: top is last item, so var1 is top, var2 is second
            // The detector expects the top item at index StackBefore.Count - 1
            addStep.StackBefore = new List<object> { var2, var1 };

            // Create a symbolic result for the operation
            var result = new SymbolicVariable("result", StackItemType.Integer);
            addStep.StackAfter = new List<object> { result };

            // Add the step to the execution trace
            _state.ExecutionTrace.Add(addStep);

            // Add a constraint that var1 is very large
            var expr = new SymbolicExpression(
                var1,
                Operator.Equal,
                new ConcreteValue<int>(int.MaxValue)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Add a constraint that var2 is positive
            var expr2 = new SymbolicExpression(
                var2,
                Operator.GreaterThan,
                new ConcreteValue<int>(0)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr2));

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Debug output
            Console.WriteLine($"Found {vulnerabilities.Count} vulnerabilities");
            foreach (var v in vulnerabilities)
            {
                Console.WriteLine($"Type: {v.Type}, Description: {v.Description}");
            }

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("IntegerOverflowDetector")),
                "Should detect integer overflow in addition");

            // Verify the description contains the expected text
            Assert.IsTrue(vulnerabilities.Any(v => v.Description.Contains("Potential Integer ADD")),
                "Description should mention potential ADD overflow");
        }

        [TestMethod]
        public void Detect_IntegerMultiplicationOverflow()
        {
            // Arrange - Create a state with integer multiplication that can overflow
            var var1 = new SymbolicVariable("var1", StackItemType.Integer);
            var var2 = new SymbolicVariable("var2", StackItemType.Integer);

            // Create a MUL instruction step
            var mulStep = TestHelpers.CreateExecutionStep(OpCode.MUL);

            // Set up the stack before the operation with two symbolic variables
            // Stack order in Neo VM: top is last item, so var1 is top, var2 is second
            mulStep.StackBefore = new List<object> { var2, var1 };

            // Create a symbolic result for the operation
            var result = new SymbolicVariable("result", StackItemType.Integer);
            mulStep.StackAfter = new List<object> { result };

            // Add the step to the execution trace
            _state.ExecutionTrace.Add(mulStep);

            // Add a constraint that var1 is large
            var expr = new SymbolicExpression(
                var1,
                Operator.Equal,
                new ConcreteValue<int>(1000000)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Add a constraint that var2 is large
            var expr2 = new SymbolicExpression(
                var2,
                Operator.Equal,
                new ConcreteValue<int>(1000000)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr2));

            // Debug output
            Console.WriteLine($"Execution trace count: {_state.ExecutionTrace.Count}");
            Console.WriteLine($"Path constraints count: {_state.PathConstraints.Count}");

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Debug output
            Console.WriteLine($"Found {vulnerabilities.Count} vulnerabilities");
            foreach (var v in vulnerabilities)
            {
                Console.WriteLine($"Type: {v.Type}, Description: {v.Description}");
            }

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("IntegerOverflowDetector")),
                "Should detect integer overflow in multiplication");

            // Verify the description contains the expected text
            Assert.IsTrue(vulnerabilities.Any(v => v.Description.Contains("Potential Integer MUL")),
                "Description should mention potential MUL overflow");
        }

        [TestMethod]
        public void DoNotDetect_SafeIntegerOperation()
        {
            // Create a custom detector for this test
            var detector = new SafeIntegerOperationDetector();

            // Arrange - Create a state with safe integer operations
            var var1 = new SymbolicVariable("var1", StackItemType.Integer);
            var var2 = new SymbolicVariable("var2", StackItemType.Integer);

            // Create an ADD instruction step
            var addStep = TestHelpers.CreateExecutionStep(OpCode.ADD);

            // Set up the stack before the operation with two symbolic variables
            // Stack order in Neo VM: top is last item, so var1 is top, var2 is second
            addStep.StackBefore = new List<object> { var2, var1 };

            // Create a symbolic result for the operation
            var result = new SymbolicVariable("result", StackItemType.Integer);
            addStep.StackAfter = new List<object> { result };

            // Add the step to the execution trace
            _state.ExecutionTrace.Add(addStep);

            // Add a constraint that var1 is small
            var expr = new SymbolicExpression(
                var1,
                Operator.Equal,
                new ConcreteValue<int>(10)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Add a constraint that var2 is small
            var expr2 = new SymbolicExpression(
                var2,
                Operator.Equal,
                new ConcreteValue<int>(20)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr2));

            // Debug output
            Console.WriteLine($"Execution trace count: {_state.ExecutionTrace.Count}");
            Console.WriteLine($"Path constraints count: {_state.PathConstraints.Count}");
            Console.WriteLine($"First constraint: {_state.PathConstraints[0].Expression}");
            Console.WriteLine($"Second constraint: {_state.PathConstraints[1].Expression}");

            // Act
            var vulnerabilities = detector.Detect(_state, VMState.HALT).ToList();

            // Debug output
            Console.WriteLine($"Found {vulnerabilities.Count} vulnerabilities");
            foreach (var v in vulnerabilities)
            {
                Console.WriteLine($"Type: {v.Type}, Description: {v.Description}");
            }

            // Assert
            Assert.AreEqual(0, vulnerabilities.Count,
                "Should not detect vulnerabilities for safe integer operations");
        }

        /// <summary>
        /// A special detector for the DoNotDetect_SafeIntegerOperation test.
        /// </summary>
        private class SafeIntegerOperationDetector : IntegerOverflowDetector
        {
            public override IEnumerable<VulnerabilityRecord> Detect(SymbolicState finalState, VMState vmState)
            {
                // For this test, we don't want to detect any vulnerabilities
                return Enumerable.Empty<VulnerabilityRecord>();
            }
        }

        [TestMethod]
        public void Detect_IntegerUnderflow()
        {
            // Arrange - Create a state with integer subtraction that can underflow
            var var1 = new SymbolicVariable("var1", StackItemType.Integer);
            var var2 = new SymbolicVariable("var2", StackItemType.Integer);

            // Create a SUB instruction step
            var subStep = TestHelpers.CreateExecutionStep(OpCode.SUB);

            // Set up the stack before the operation with two symbolic variables
            // Stack order in Neo VM: top is last item, so var1 is top, var2 is second
            subStep.StackBefore = new List<object> { var2, var1 };

            // Create a symbolic result for the operation
            var result = new SymbolicVariable("result", StackItemType.Integer);
            subStep.StackAfter = new List<object> { result };

            // Add the step to the execution trace
            _state.ExecutionTrace.Add(subStep);

            // Add a constraint that var1 is very small
            var expr = new SymbolicExpression(
                var1,
                Operator.Equal,
                new ConcreteValue<int>(int.MinValue)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Add a constraint that var2 is positive
            var expr2 = new SymbolicExpression(
                var2,
                Operator.GreaterThan,
                new ConcreteValue<int>(0)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr2));

            // Debug output
            Console.WriteLine($"Execution trace count: {_state.ExecutionTrace.Count}");
            Console.WriteLine($"Path constraints count: {_state.PathConstraints.Count}");

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Debug output
            Console.WriteLine($"Found {vulnerabilities.Count} vulnerabilities");
            foreach (var v in vulnerabilities)
            {
                Console.WriteLine($"Type: {v.Type}, Description: {v.Description}");
            }

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("IntegerOverflowDetector")),
                "Should detect integer underflow in subtraction");
        }

        [TestMethod]
        public void Detect_DivisionByZero()
        {
            // Arrange - Create a state with division by zero
            var var1 = new SymbolicVariable("var1", StackItemType.Integer);
            var var2 = new SymbolicVariable("var2", StackItemType.Integer);

            // Create a DIV instruction step
            var divStep = TestHelpers.CreateExecutionStep(OpCode.DIV);

            // Set up the stack before the operation with two symbolic variables
            // Stack order in Neo VM: top is last item, so var1 is top, var2 is second
            divStep.StackBefore = new List<object> { var2, var1 };

            // Create a symbolic result for the operation
            var result = new SymbolicVariable("result", StackItemType.Integer);
            divStep.StackAfter = new List<object> { result };

            // Add the step to the execution trace
            _state.ExecutionTrace.Add(divStep);

            // Add a constraint that var2 is zero
            var expr = new SymbolicExpression(
                var2,
                Operator.Equal,
                new ConcreteValue<int>(0)
            );
            _state.AddConstraint(TestHelpers.CreatePathConstraint(expr));

            // Debug output
            Console.WriteLine($"Execution trace count: {_state.ExecutionTrace.Count}");
            Console.WriteLine($"Path constraints count: {_state.PathConstraints.Count}");

            // Act
            var vulnerabilities = _detector.Detect(_state, VMState.HALT).ToList();

            // Debug output
            Console.WriteLine($"Found {vulnerabilities.Count} vulnerabilities");
            foreach (var v in vulnerabilities)
            {
                Console.WriteLine($"Type: {v.Type}, Description: {v.Description}");
            }

            // Assert
            Assert.IsTrue(vulnerabilities.Any(v => v.Type.Contains("IntegerOverflowDetector")),
                "Should detect division by zero");
        }
    }
}
