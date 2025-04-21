using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Visualization;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    /// <summary>
    /// Tests for the symbolic execution engine
    /// </summary>
    public class SymbolicExecutionTests
    {
        /// <summary>
        /// Tests the creation of symbolic values
        /// </summary>
        [Fact]
        public void TestSymbolicValueCreation()
        {
            // Create a symbolic boolean
            var boolValue = SymbolicValue.CreateBoolean();
            Assert.Equal(SymbolicValue.SymbolicType.Boolean, boolValue.Type);
            Assert.False(boolValue.HasConcreteValue);

            // Create a symbolic boolean with a concrete value
            var concreteBoolValue = SymbolicValue.CreateBoolean(true);
            Assert.Equal(SymbolicValue.SymbolicType.Boolean, concreteBoolValue.Type);
            Assert.True(concreteBoolValue.HasConcreteValue);
            Assert.True(((VM.Types.Boolean)concreteBoolValue.ConcreteValue).GetBoolean());

            // Create a symbolic integer
            var intValue = SymbolicValue.CreateInteger();
            Assert.Equal(SymbolicValue.SymbolicType.Integer, intValue.Type);
            Assert.False(intValue.HasConcreteValue);

            // Create a symbolic integer with a concrete value
            var concreteIntValue = SymbolicValue.CreateInteger(42);
            Assert.Equal(SymbolicValue.SymbolicType.Integer, concreteIntValue.Type);
            Assert.True(concreteIntValue.HasConcreteValue);
            Assert.Equal(42, ((VM.Types.Integer)concreteIntValue.ConcreteValue).GetInteger());

            // Create a symbolic byte string
            var byteStringValue = SymbolicValue.CreateByteString();
            Assert.Equal(SymbolicValue.SymbolicType.ByteString, byteStringValue.Type);
            Assert.False(byteStringValue.HasConcreteValue);

            // Create a symbolic byte string with a concrete value
            var concreteByteStringValue = SymbolicValue.CreateByteString(new byte[] { 1, 2, 3 });
            Assert.Equal(SymbolicValue.SymbolicType.ByteString, concreteByteStringValue.Type);
            Assert.True(concreteByteStringValue.HasConcreteValue);
            Assert.Equal(new byte[] { 1, 2, 3 }, ((VM.Types.ByteString)concreteByteStringValue.ConcreteValue).GetSpan().ToArray());

            // Create a symbolic array
            var arrayValue = SymbolicValue.CreateArray();
            Assert.Equal(SymbolicValue.SymbolicType.Array, arrayValue.Type);
            Assert.False(arrayValue.HasConcreteValue);

            // Create a symbolic map
            var mapValue = SymbolicValue.CreateMap();
            Assert.Equal(SymbolicValue.SymbolicType.Map, mapValue.Type);
            Assert.False(mapValue.HasConcreteValue);
        }

        /// <summary>
        /// Tests the creation of symbolic constraints
        /// </summary>
        [Fact]
        public void TestSymbolicConstraintCreation()
        {
            // Create symbolic values
            var x = SymbolicValue.CreateInteger(5);
            var y = SymbolicValue.CreateInteger(10);

            // Create constraints
            var equalConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.Equal);
            var notEqualConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.NotEqual);
            var lessThanConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.LessThan);
            var lessThanOrEqualConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.LessThanOrEqual);
            var greaterThanConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.GreaterThan);
            var greaterThanOrEqualConstraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.GreaterThanOrEqual);

            // Verify constraints
            Assert.Equal(x, equalConstraint.Left);
            Assert.Equal(y, equalConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.Equal, equalConstraint.Type);

            Assert.Equal(x, notEqualConstraint.Left);
            Assert.Equal(y, notEqualConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.NotEqual, notEqualConstraint.Type);

            Assert.Equal(x, lessThanConstraint.Left);
            Assert.Equal(y, lessThanConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.LessThan, lessThanConstraint.Type);

            Assert.Equal(x, lessThanOrEqualConstraint.Left);
            Assert.Equal(y, lessThanOrEqualConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.LessThanOrEqual, lessThanOrEqualConstraint.Type);

            Assert.Equal(x, greaterThanConstraint.Left);
            Assert.Equal(y, greaterThanConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.GreaterThan, greaterThanConstraint.Type);

            Assert.Equal(x, greaterThanOrEqualConstraint.Left);
            Assert.Equal(y, greaterThanOrEqualConstraint.Right);
            Assert.Equal(SymbolicConstraint.ConstraintType.GreaterThanOrEqual, greaterThanOrEqualConstraint.Type);
        }

        /// <summary>
        /// Tests the constraint solver
        /// </summary>
        [Fact]
        public void TestConstraintSolver()
        {
            // Create a constraint solver with a fixed seed for reproducibility
            var solver = new ConstraintSolver(12345);

            // Create symbolic values
            var x = new SymbolicValue("x", SymbolicValue.SymbolicType.Integer, null, "x");
            var y = new SymbolicValue("y", SymbolicValue.SymbolicType.Integer, null, "y");
            var ten = new SymbolicValue("ten", SymbolicValue.SymbolicType.Integer, new VM.Types.Integer(10), "ten");

            // Create constraints: x > 10 && y < x
            var constraints = new List<SymbolicConstraint>
            {
                new SymbolicConstraint(x, ten, SymbolicConstraint.ConstraintType.GreaterThan),
                new SymbolicConstraint(y, x, SymbolicConstraint.ConstraintType.LessThan)
            };

            // Define parameter types
            var parameters = new Dictionary<string, string>
            {
                { "x", "Integer" },
                { "y", "Integer" }
            };

            // Solve constraints
            var solution = solver.SolveConstraints(constraints, parameters);

            // Verify solution
            Assert.NotNull(solution);
            Assert.Contains("x", solution.Keys);
            Assert.Contains("y", solution.Keys);
            Assert.IsType<VM.Types.Integer>(solution["x"]);
            Assert.IsType<VM.Types.Integer>(solution["y"]);

            // Verify that the solution satisfies the constraints
            var xValue = ((VM.Types.Integer)solution["x"]).GetInteger();
            var yValue = ((VM.Types.Integer)solution["y"]).GetInteger();
            Assert.True(xValue > 10);
            Assert.True(yValue < xValue);
        }

        /// <summary>
        /// Tests the enhanced constraint solver with incremental solving and caching
        /// </summary>
        [Fact]
        public void TestEnhancedConstraintSolver()
        {
            // Create an enhanced constraint solver with incremental solving and caching
            var solver = new ConstraintSolver(
                seed: 12345,
                timeoutMilliseconds: 5000,
                enableIncrementalSolving: true,
                enableConstraintCaching: true);

            // Create symbolic values
            var x = new SymbolicValue("x", SymbolicValue.SymbolicType.Integer, null, "x");
            var y = new SymbolicValue("y", SymbolicValue.SymbolicType.Integer, null, "y");
            var ten = new SymbolicValue("ten", SymbolicValue.SymbolicType.Integer, new VM.Types.Integer(10), "ten");

            // Create constraints: x > 10
            var constraints1 = new List<SymbolicConstraint>
            {
                new SymbolicConstraint(x, ten, SymbolicConstraint.ConstraintType.GreaterThan)
            };

            // Define parameter types
            var parameters = new Dictionary<string, string>
            {
                { "x", "Integer" },
                { "y", "Integer" }
            };

            // Solve first set of constraints
            var solution1 = solver.SolveConstraints(constraints1, parameters);

            // Verify solution
            Assert.NotNull(solution1);
            Assert.Contains("x", solution1.Keys);
            Assert.Contains("y", solution1.Keys);
            Assert.IsType<VM.Types.Integer>(solution1["x"]);
            Assert.IsType<VM.Types.Integer>(solution1["y"]);

            // Verify that the solution satisfies the constraints
            var xValue1 = ((VM.Types.Integer)solution1["x"]).GetInteger();
            Assert.True(xValue1 > 10);

            // Create additional constraints: y < x
            var constraints2 = new List<SymbolicConstraint>
            {
                new SymbolicConstraint(x, ten, SymbolicConstraint.ConstraintType.GreaterThan),
                new SymbolicConstraint(y, x, SymbolicConstraint.ConstraintType.LessThan)
            };

            // Solve second set of constraints
            var solution2 = solver.SolveConstraints(constraints2, parameters);

            // Verify solution
            Assert.NotNull(solution2);
            Assert.Contains("x", solution2.Keys);
            Assert.Contains("y", solution2.Keys);
            Assert.IsType<VM.Types.Integer>(solution2["x"]);
            Assert.IsType<VM.Types.Integer>(solution2["y"]);

            // Verify that the solution satisfies the constraints
            var xValue2 = ((VM.Types.Integer)solution2["x"]).GetInteger();
            var yValue2 = ((VM.Types.Integer)solution2["y"]).GetInteger();
            Assert.True(xValue2 > 10);
            Assert.True(yValue2 < xValue2);

            // Check statistics
            var stats = solver.GetStatistics();
            Assert.Equal(2, stats["SolveAttempts"]);
            Assert.True(stats["CacheHits"] >= 0);
            Assert.True(stats["CacheMisses"] >= 0);
            Assert.Equal(0, stats["Timeouts"]);
        }

        /// <summary>
        /// Tests the symbolic state
        /// </summary>
        [Fact]
        public void TestSymbolicState()
        {
            // Create a symbolic state
            var state = new SymbolicState();

            // Create symbolic values
            var x = SymbolicValue.CreateInteger(5);
            var y = SymbolicValue.CreateInteger(10);

            // Push values onto the evaluation stack
            state.Push(x);
            state.Push(y);

            // Verify stack
            Assert.Equal(2, state.EvaluationStack.Count);
            Assert.Equal(y, state.EvaluationStack.Peek());

            // Pop a value
            var popped = state.Pop();
            Assert.Equal(y, popped);
            Assert.Single(state.EvaluationStack);

            // Add a constraint
            var constraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.LessThan);
            state.AddConstraint(constraint);

            // Verify constraints
            Assert.Single(state.Constraints);
            Assert.Equal(constraint, state.Constraints[0]);

            // Clone the state
            var clonedState = state.Clone();

            // Verify cloned state
            Assert.Single(clonedState.EvaluationStack);
            Assert.Equal(x, clonedState.EvaluationStack.Peek());
            Assert.Single(clonedState.Constraints);
            Assert.Equal(constraint, clonedState.Constraints[0]);

            // Modify the original state
            state.Push(y);
            state.AddConstraint(new SymbolicConstraint(y, x, SymbolicConstraint.ConstraintType.GreaterThan));

            // Verify that the cloned state is not affected
            Assert.Single(clonedState.EvaluationStack);
            Assert.Single(clonedState.Constraints);
        }

        /// <summary>
        /// Tests the symbolic execution result
        /// </summary>
        [Fact]
        public void TestSymbolicExecutionResult()
        {
            // Create symbolic values
            var x = SymbolicValue.CreateInteger(5);
            var y = SymbolicValue.CreateInteger(10);

            // Create constraints
            var constraint = new SymbolicConstraint(x, y, SymbolicConstraint.ConstraintType.LessThan);
            var constraints = new List<SymbolicConstraint> { constraint };

            // Create a symbolic execution result
            var result = new SymbolicExecutionResult
            {
                IsSuccess = true,
                ReturnValue = x,
                PathConstraints = constraints,
                ExecutionPath = new List<OpCode> { OpCode.PUSH1, OpCode.PUSH2, OpCode.LT },
                GasConsumed = 100,
                ExecutionTime = TimeSpan.FromMilliseconds(50)
            };

            // Verify result
            Assert.True(result.IsSuccess);
            Assert.Equal(x, result.ReturnValue);
            Assert.Single(result.PathConstraints);
            Assert.Equal(constraint, result.PathConstraints[0]);
            Assert.Equal(3, result.ExecutionPath.Count);
            Assert.Equal(OpCode.PUSH1, result.ExecutionPath[0]);
            Assert.Equal(OpCode.PUSH2, result.ExecutionPath[1]);
            Assert.Equal(OpCode.LT, result.ExecutionPath[2]);
            Assert.Equal(100, result.GasConsumed);
            Assert.Equal(TimeSpan.FromMilliseconds(50), result.ExecutionTime);
        }

        /// <summary>
        /// Tests the symbolic virtual machine
        /// </summary>
        [Fact]
        public void TestSymbolicVirtualMachine()
        {
            // Create a script that checks if x > 10
            byte[] script = new ScriptBuilder()
                .EmitPush(10)
                .Emit(OpCode.DUPFROMALTSTACK) // Get parameter x from alt stack
                .Emit(OpCode.SWAP)
                .Emit(OpCode.GT)
                .Emit(OpCode.RET)
                .ToArray();

            // Create a symbolic virtual machine
            var vm = new SymbolicVirtualMachine(script, 100, 10, "DFS", 5000, 12345);

            // Define parameter types
            var parameters = new Dictionary<string, string>
            {
                { "x", "Integer" }
            };

            // Execute the script
            var results = vm.ExecuteMethod("TestMethod", parameters);

            // Verify the results
            Assert.Equal(2, results.Count);

            // One result should have x > 10 and return true
            var trueResult = results.Find(r => r.ReturnValue.HasConcreteValue && ((VM.Types.Boolean)r.ReturnValue.ConcreteValue).GetBoolean());
            Assert.NotNull(trueResult);
            Assert.True(trueResult.IsSuccess);
            Assert.Single(trueResult.PathConstraints);
            Assert.Equal(SymbolicConstraint.ConstraintType.Equal, trueResult.PathConstraints[0].Type);

            // One result should have x <= 10 and return false
            var falseResult = results.Find(r => r.ReturnValue.HasConcreteValue && !((VM.Types.Boolean)r.ReturnValue.ConcreteValue).GetBoolean());
            Assert.NotNull(falseResult);
            Assert.True(falseResult.IsSuccess);
            Assert.Single(falseResult.PathConstraints);
            Assert.Equal(SymbolicConstraint.ConstraintType.Equal, falseResult.PathConstraints[0].Type);

            // Generate concrete inputs
            results = vm.GenerateConcreteInputs(results, parameters).ToArray();

            // Verify concrete inputs
            Assert.NotEmpty(trueResult.ConcreteInputs);
            Assert.NotEmpty(falseResult.ConcreteInputs);
            Assert.Contains("x", trueResult.ConcreteInputs.Keys);
            Assert.Contains("x", falseResult.ConcreteInputs.Keys);

            // Verify that the concrete inputs satisfy the constraints
            var xTrue = ((VM.Types.Integer)trueResult.ConcreteInputs["x"]).GetInteger();
            var xFalse = ((VM.Types.Integer)falseResult.ConcreteInputs["x"]).GetInteger();
            Assert.True(xTrue > 10 || xFalse <= 10);
        }
    }
}