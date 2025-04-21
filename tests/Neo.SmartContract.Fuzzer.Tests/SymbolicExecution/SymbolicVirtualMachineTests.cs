using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    [TestClass]
    public class SymbolicVirtualMachineTests
    {
        private Mock<IConstraintSolver> _mockSolver;
        private SymbolicVirtualMachine _vm;

        [TestInitialize]
        public void Setup()
        {
            _mockSolver = new Mock<IConstraintSolver>();
            _mockSolver.Setup(s => s.IsSatisfiable(It.IsAny<IEnumerable<PathConstraint>>())).Returns(true);
            _mockSolver.Setup(s => s.Simplify(It.IsAny<IEnumerable<PathConstraint>>())).Returns<IEnumerable<PathConstraint>>(c => c);
            
            _vm = new SymbolicVirtualMachine(_mockSolver.Object);
        }

        [TestMethod]
        public void TestExecuteStep_CapturesAddedConstraints()
        {
            // Create a simple script with a branch
            // PUSH1
            // JMPIF 2
            // PUSH0
            // RET
            // PUSH1
            // RET
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH1,
                (byte)OpCode.JMPIF,
                2,
                (byte)OpCode.PUSH0,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH1,
                (byte)OpCode.RET
            };
            
            // Create a symbolic state
            var state = new SymbolicState(script);
            
            // Execute the first step (PUSH1)
            var step1 = _vm.ExecuteStep(state);
            
            // Verify that the step was executed correctly
            Assert.AreEqual(OpCode.PUSH1, step1.Instruction.OpCode);
            Assert.AreEqual(0, step1.AddedConstraints.Count);
            Assert.AreEqual(0, step1.CreatedBranches.Count);
            
            // Execute the second step (JMPIF)
            var step2 = _vm.ExecuteStep(state);
            
            // Verify that the step was executed correctly
            Assert.AreEqual(OpCode.JMPIF, step2.Instruction.OpCode);
            
            // The JMPIF instruction should create a branch
            Assert.IsTrue(step2.CreatedBranches.Count > 0, "JMPIF should create at least one branch");
            
            // Execute the third step (PUSH1 after the jump)
            var step3 = _vm.ExecuteStep(state);
            
            // Verify that the step was executed correctly
            Assert.AreEqual(OpCode.PUSH1, step3.Instruction.OpCode);
            Assert.AreEqual(0, step3.AddedConstraints.Count);
            Assert.AreEqual(0, step3.CreatedBranches.Count);
        }

        [TestMethod]
        public void TestExecute_WithBranching()
        {
            // Create a simple script with a branch
            // PUSH1
            // JMPIF 2
            // PUSH0
            // RET
            // PUSH1
            // RET
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH1,
                (byte)OpCode.JMPIF,
                2,
                (byte)OpCode.PUSH0,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH1,
                (byte)OpCode.RET
            };
            
            // Execute the script
            var result = _vm.Execute(script);
            
            // Verify that both paths were explored
            Assert.AreEqual(2, result.ExecutionPaths.Count, "Should explore both paths");
            
            // Verify that one path returns 0 and the other returns 1
            var returnValues = result.ExecutionPaths.Select(p => p.FinalState.EvaluationStack.Peek()).ToList();
            Assert.IsTrue(returnValues.Any(v => v is ConcreteValue<int> cv && cv.Value == 0), "One path should return 0");
            Assert.IsTrue(returnValues.Any(v => v is ConcreteValue<int> cv && cv.Value == 1), "One path should return 1");
        }

        [TestMethod]
        public void TestExecute_WithSymbolicCondition()
        {
            // Create a symbolic variable
            var symbolicVar = new SymbolicVariable("x", VM.Types.StackItemType.Boolean);
            
            // Create a simple script that uses the symbolic variable for branching
            // PUSH arg0 (symbolic variable)
            // JMPIF 2
            // PUSH0
            // RET
            // PUSH1
            // RET
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH0, // Placeholder for symbolic variable
                (byte)OpCode.JMPIF,
                2,
                (byte)OpCode.PUSH0,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH1,
                (byte)OpCode.RET
            };
            
            // Execute the script with the symbolic variable
            var result = _vm.Execute(script, new List<SymbolicValue> { symbolicVar });
            
            // Verify that both paths were explored
            Assert.AreEqual(2, result.ExecutionPaths.Count, "Should explore both paths");
            
            // Verify that one path returns 0 and the other returns 1
            var returnValues = result.ExecutionPaths.Select(p => p.FinalState.EvaluationStack.Peek()).ToList();
            Assert.IsTrue(returnValues.Any(v => v is ConcreteValue<int> cv && cv.Value == 0), "One path should return 0");
            Assert.IsTrue(returnValues.Any(v => v is ConcreteValue<int> cv && cv.Value == 1), "One path should return 1");
            
            // Verify that the solver was called to check path feasibility
            _mockSolver.Verify(s => s.IsSatisfiable(It.IsAny<IEnumerable<PathConstraint>>()), Times.AtLeast(1));
        }

        [TestMethod]
        public void TestExecute_WithMultipleBranches()
        {
            // Create a script with multiple branches
            // PUSH1
            // JMPIF 4
            // PUSH0
            // JMPIF 2
            // PUSH2
            // RET
            // PUSH3
            // RET
            // PUSH4
            // RET
            byte[] script = new byte[]
            {
                (byte)OpCode.PUSH1,
                (byte)OpCode.JMPIF,
                4,
                (byte)OpCode.PUSH0,
                (byte)OpCode.JMPIF,
                2,
                (byte)OpCode.PUSH2,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH3,
                (byte)OpCode.RET,
                (byte)OpCode.PUSH4,
                (byte)OpCode.RET
            };
            
            // Execute the script
            var result = _vm.Execute(script);
            
            // Verify that all paths were explored
            Assert.AreEqual(3, result.ExecutionPaths.Count, "Should explore all three paths");
            
            // Verify that the paths return the expected values
            var returnValues = result.ExecutionPaths
                .Select(p => p.FinalState.EvaluationStack.Peek())
                .OfType<ConcreteValue<int>>()
                .Select(cv => cv.Value)
                .ToList();
            
            CollectionAssert.Contains(returnValues, 2, "One path should return 2");
            CollectionAssert.Contains(returnValues, 3, "One path should return 3");
            CollectionAssert.Contains(returnValues, 4, "One path should return 4");
        }
    }
}
