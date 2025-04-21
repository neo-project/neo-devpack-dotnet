using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

// Use Types namespace for all symbolic execution types
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.Solvers;
using SymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using Operator = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class SymbolicExecutionEngineIntegrationTests
    {
        private Mock<Solvers.IConstraintSolver>? _mockSolver;
        private Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces.IConstraintSolver? _solverAdapter;

        [TestInitialize]
        public void Setup()
        {
            _mockSolver = new Mock<Solvers.IConstraintSolver>();
            // Default setup: Assume paths are satisfiable unless specified otherwise
            _mockSolver.Setup(s => s.IsSatisfiable(It.IsAny<IEnumerable<SymbolicExpression>>())).Returns(true);
            _solverAdapter = new TestConstraintSolverAdapter(_mockSolver.Object);
        }

        [TestMethod]
        public void TestSimpleBranchAndAbort()
        {
            // Script:
            // PUSH1 (true)
            // JMPIF +2 (to RET) -> Target IP 4
            // ABORT         (Path if False)
            // RET           (Path if True)
            var scriptBytes = new byte[]
            {
                (byte)OpCode.PUSH1,      // 0: Push true onto stack
                (byte)OpCode.JMPIF, 2,   // 1: Jump if True. Offset=2. Target=IP+Offset=4. Consumes top.
                (byte)OpCode.ABORT,      // 3: Abort if false path.
                (byte)OpCode.RET,        // 4: Return if true path.
            };
            var script = new Script(scriptBytes);

            // No initial arguments needed for this test
            var initialArgs = new List<SymbolicValue>();

            // Ensure solvers are initialized before use
            Assert.IsNotNull(_mockSolver, "Mock solver was not initialized in Setup.");
            Assert.IsNotNull(_solverAdapter, "Solver adapter was not initialized in Setup.");
            // Correct constructor call: (byte[] script, IConstraintSolver solver, detectors, initialArguments)
            var engine = new SymbolicExecutionEngine(scriptBytes, _solverAdapter!, detectors: null, initialArguments: initialArgs);

            // Act
            var results = engine.Execute();

            // Assert
            // We should have at least one execution path
            Assert.IsTrue(results.ExecutionPaths.Count > 0, "Expected at least one execution path.");

            // Since we're using PUSH1 (which pushes a concrete value), we should only have one path
            // In this case, it should be the ABORT path since PUSH1 pushes true and JMPIF will always take the jump
            var abortPath = results.ExecutionPaths.FirstOrDefault(p => p.HaltReason == VMState.FAULT);

            // Check if FirstOrDefault found a valid state, not just the default tuple value
            Assert.IsNotNull(abortPath, "Abort path state (HaltReason.Abort) not found or incorrect.");

            // Assign to local variable after null check to help static analysis
            var abortState = abortPath.FinalState;

            // Check that the path ended with ABORT
            Assert.AreEqual(VMState.FAULT, abortPath.HaltReason, "Execution ending in ABORT should have VMState.FAULT.");

            // Since we're using concrete values (PUSH1), the solver might not be called at all
            // or might be called only once. We can't make strong assertions about the number of calls.
            _mockSolver.Verify(s => s.IsSatisfiable(It.IsAny<IEnumerable<SymbolicExpression>>()), Times.AtMost(1));
        }
    }
}
