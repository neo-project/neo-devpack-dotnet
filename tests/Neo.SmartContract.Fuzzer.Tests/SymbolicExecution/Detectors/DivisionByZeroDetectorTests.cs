using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution.Detectors
{
    [TestClass]
    public class DivisionByZeroDetectorTests
    {
        private DivisionByZeroDetector _detector;

        [TestInitialize]
        public void Setup()
        {
            _detector = new DivisionByZeroDetector();
        }

        [TestMethod]
        public void Name_ReturnsExpectedValue()
        {
            // Act
            var name = _detector.Name;

            // Assert
            name.Should().Be("Division by Zero Detector");
        }

        [TestMethod]
        public void Description_ReturnsExpectedValue()
        {
            // Act
            var description = _detector.Description;

            // Assert
            description.Should().Be("Detects division by zero vulnerabilities during symbolic execution.");
        }

        [TestMethod]
        public void DetectVulnerabilities_NullPath_ReturnsEmptyList()
        {
            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(null);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().BeEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_EmptyPath_ReturnsEmptyList()
        {
            // Arrange
            var path = new ExecutionPath();

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().BeEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_DivisionByZeroInConstraint_ReturnsVulnerability()
        {
            // Arrange
            var variable = new SymbolicVariable("x", StackItemType.Integer);
            var zero = new SymbolicConstant(0);
            var divisionByZero = new SymbolicExpression(variable, zero, Operator.Divide);
            
            var constraint = new PathConstraint(divisionByZero, 123);
            var path = new ExecutionPath
            {
                PathConstraints = new List<PathConstraint> { constraint }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().HaveCount(1);
            vulnerabilities[0].Type.Should().Be("Division by Zero");
            vulnerabilities[0].Severity.Should().Be(SymbolicVulnerabilitySeverity.High);
            vulnerabilities[0].Location.Should().Be("123");
            vulnerabilities[0].Description.Should().Contain("division by zero");
            vulnerabilities[0].Remediation.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_DivisionByZeroInStep_ReturnsVulnerability()
        {
            // Arrange
            var mockInstruction = new Mock<Instruction>();
            mockInstruction.Setup(i => i.OpCode).Returns(OpCode.DIV);
            
            var zero = new SymbolicConstant(0);
            var variable = new SymbolicVariable("x", StackItemType.Integer);
            
            var step = new ExecutionStep
            {
                InstructionPointer = 456,
                Instruction = mockInstruction.Object,
                StackBefore = new List<object> { variable, zero } // Divisor is on top of stack
            };
            
            var path = new ExecutionPath
            {
                Steps = new List<ExecutionStep> { step }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().HaveCount(1);
            vulnerabilities[0].Type.Should().Be("Division by Zero");
            vulnerabilities[0].Severity.Should().Be(SymbolicVulnerabilitySeverity.High);
            vulnerabilities[0].Location.Should().Be("456");
            vulnerabilities[0].Description.Should().Contain("division by zero");
            vulnerabilities[0].Remediation.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_ModuloByZeroInStep_ReturnsVulnerability()
        {
            // Arrange
            var mockInstruction = new Mock<Instruction>();
            mockInstruction.Setup(i => i.OpCode).Returns(OpCode.MOD);
            
            var zero = new SymbolicConstant(0);
            var variable = new SymbolicVariable("x", StackItemType.Integer);
            
            var step = new ExecutionStep
            {
                InstructionPointer = 789,
                Instruction = mockInstruction.Object,
                StackBefore = new List<object> { variable, zero } // Divisor is on top of stack
            };
            
            var path = new ExecutionPath
            {
                Steps = new List<ExecutionStep> { step }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().HaveCount(1);
            vulnerabilities[0].Type.Should().Be("Division by Zero");
            vulnerabilities[0].Severity.Should().Be(SymbolicVulnerabilitySeverity.High);
            vulnerabilities[0].Location.Should().Be("789");
            vulnerabilities[0].Description.Should().Contain("division by zero");
            vulnerabilities[0].Remediation.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_NonZeroDivisor_ReturnsNoVulnerabilities()
        {
            // Arrange
            var mockInstruction = new Mock<Instruction>();
            mockInstruction.Setup(i => i.OpCode).Returns(OpCode.DIV);
            
            var nonZero = new SymbolicConstant(5);
            var variable = new SymbolicVariable("x", StackItemType.Integer);
            
            var step = new ExecutionStep
            {
                InstructionPointer = 123,
                Instruction = mockInstruction.Object,
                StackBefore = new List<object> { variable, nonZero } // Divisor is on top of stack
            };
            
            var path = new ExecutionPath
            {
                Steps = new List<ExecutionStep> { step }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().BeEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_NonDivisionInstruction_ReturnsNoVulnerabilities()
        {
            // Arrange
            var mockInstruction = new Mock<Instruction>();
            mockInstruction.Setup(i => i.OpCode).Returns(OpCode.ADD);
            
            var zero = new SymbolicConstant(0);
            var variable = new SymbolicVariable("x", StackItemType.Integer);
            
            var step = new ExecutionStep
            {
                InstructionPointer = 123,
                Instruction = mockInstruction.Object,
                StackBefore = new List<object> { variable, zero }
            };
            
            var path = new ExecutionPath
            {
                Steps = new List<ExecutionStep> { step }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().BeEmpty();
        }

        [TestMethod]
        public void DetectVulnerabilities_EmptyStack_ReturnsNoVulnerabilities()
        {
            // Arrange
            var mockInstruction = new Mock<Instruction>();
            mockInstruction.Setup(i => i.OpCode).Returns(OpCode.DIV);
            
            var step = new ExecutionStep
            {
                InstructionPointer = 123,
                Instruction = mockInstruction.Object,
                StackBefore = new List<object>() // Empty stack
            };
            
            var path = new ExecutionPath
            {
                Steps = new List<ExecutionStep> { step }
            };

            // Act
            var vulnerabilities = _detector.DetectVulnerabilities(path);

            // Assert
            vulnerabilities.Should().NotBeNull();
            vulnerabilities.Should().BeEmpty();
        }
    }
}
