using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Solvers;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Tests.Solvers
{
    [TestClass]
    public class UnifiedConstraintSolverTests
    {
        [TestMethod]
        public void TestIsSatisfiable_SimpleConstraint()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var constraint = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraints = new List<SymbolicExpression> { constraint };

            // Act
            bool result = solver.IsSatisfiable(constraints);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsSatisfiable_ContradictoryConstraints()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var constraint1 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraint2 = new SymbolicExpression(x, new ConcreteValue<long>(5), Operator.LessThan);
            var constraints = new List<SymbolicExpression> { constraint1, constraint2 };

            // Act
            bool result = solver.IsSatisfiable(constraints);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestSolve_SimpleConstraint()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var constraint = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraints = new List<SymbolicExpression> { constraint };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue((long)solution["x"] > 10);
        }

        [TestMethod]
        public void TestSolve_MultipleConstraints()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var constraint1 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraint2 = new SymbolicExpression(x, new ConcreteValue<long>(20), Operator.LessThan);
            var constraints = new List<SymbolicExpression> { constraint1, constraint2 };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            long value = (long)solution["x"];
            Assert.IsTrue(value > 10 && value < 20);
        }

        [TestMethod]
        public void TestSimplify_RemovesTautologies()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var tautology = new SymbolicExpression(x, x, Operator.Equal);
            var constraint = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraints = new List<SymbolicExpression> { tautology, constraint };

            // Act
            var simplified = solver.Simplify(constraints).ToList();

            // Assert
            Assert.AreEqual(1, simplified.Count);
            Assert.AreEqual(constraint, simplified[0]);
        }

        [TestMethod]
        public void TestSimplify_RemovesDuplicates()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var constraint1 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraint2 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraints = new List<SymbolicExpression> { constraint1, constraint2 };

            // Act
            var simplified = solver.Simplify(constraints).ToList();

            // Assert
            Assert.AreEqual(1, simplified.Count);
        }

        [TestMethod]
        public void TestSolve_ComplexArithmeticExpression()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var y = new SymbolicVariable("y", VM.Types.StackItemType.Integer);
            var z = new SymbolicVariable("z", VM.Types.StackItemType.Integer);

            // Create expression: (x + y) * z > 100
            var xPlusY = new SymbolicExpression(x, y, Operator.Add);
            var xPlusYTimesZ = new SymbolicExpression(xPlusY, z, Operator.Multiply);
            var constraint = new SymbolicExpression(xPlusYTimesZ, new ConcreteValue<long>(100), Operator.GreaterThan);

            // Add additional constraints: x > 0, y > 0, z > 0
            var xGt0 = new SymbolicExpression(x, new ConcreteValue<long>(0), Operator.GreaterThan);
            var yGt0 = new SymbolicExpression(y, new ConcreteValue<long>(0), Operator.GreaterThan);
            var zGt0 = new SymbolicExpression(z, new ConcreteValue<long>(0), Operator.GreaterThan);

            var constraints = new List<SymbolicExpression> { constraint, xGt0, yGt0, zGt0 };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("y"));
            Assert.IsTrue(solution.ContainsKey("z"));

            long xVal = (long)solution["x"];
            long yVal = (long)solution["y"];
            long zVal = (long)solution["z"];

            Assert.IsTrue(xVal > 0);
            Assert.IsTrue(yVal > 0);
            Assert.IsTrue(zVal > 0);
            Assert.IsTrue((xVal + yVal) * zVal > 100);
        }

        [TestMethod]
        public void TestSolve_QuadraticEquation()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);

            // Create expression for x^2 - 5x + 6 = 0
            // This has solutions x=2 and x=3

            // x^2 term
            var xSquared = new SymbolicExpression(x, x, Operator.Multiply);

            // 5x term
            var five = new ConcreteValue<long>(5);
            var fiveX = new SymbolicExpression(five, x, Operator.Multiply);

            // x^2 - 5x
            var xSquaredMinus5x = new SymbolicExpression(xSquared, fiveX, Operator.Subtract);

            // x^2 - 5x + 6
            var six = new ConcreteValue<long>(6);
            var xSquaredMinus5xPlus6 = new SymbolicExpression(xSquaredMinus5x, six, Operator.Add);

            // x^2 - 5x + 6 = 0
            var constraint = new SymbolicExpression(xSquaredMinus5xPlus6, new ConcreteValue<long>(0), Operator.Equal);

            var constraints = new List<SymbolicExpression> { constraint };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            long xVal = (long)solution["x"];

            // Check that the solution satisfies the equation
            long result = (xVal * xVal) - (5 * xVal) + 6;
            Assert.AreEqual(0, result);

            // The solution should be either 2 or 3
            Assert.IsTrue(xVal == 2 || xVal == 3);
        }

        [TestMethod]
        public void TestSolve_ModuloConstraints()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);

            // Create constraint: x % 5 = 3
            var five = new ConcreteValue<long>(5);
            var three = new ConcreteValue<long>(3);
            var xMod5 = new SymbolicExpression(x, five, Operator.Modulo);
            var constraint = new SymbolicExpression(xMod5, three, Operator.Equal);

            // Add constraint: x > 0
            var xGt0 = new SymbolicExpression(x, new ConcreteValue<long>(0), Operator.GreaterThan);

            var constraints = new List<SymbolicExpression> { constraint, xGt0 };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            long xVal = (long)solution["x"];

            Assert.IsTrue(xVal > 0);
            Assert.AreEqual(3, xVal % 5);
        }

        [TestMethod]
        public void TestSolve_BooleanLogic()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var a = new SymbolicVariable("a", VM.Types.StackItemType.Boolean);
            var b = new SymbolicVariable("b", VM.Types.StackItemType.Boolean);
            var c = new SymbolicVariable("c", VM.Types.StackItemType.Boolean);

            // Create constraint: (a AND b) OR (NOT c) = TRUE
            var aAndB = new SymbolicExpression(a, b, Operator.And);
            var notC = new SymbolicExpression(c, null, Operator.Not);
            var aAndBOrNotC = new SymbolicExpression(aAndB, notC, Operator.Or);
            var constraint = new SymbolicExpression(aAndBOrNotC, new ConcreteValue<bool>(true), Operator.Equal);

            var constraints = new List<SymbolicExpression> { constraint };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("a"));
            Assert.IsTrue(solution.ContainsKey("b"));
            Assert.IsTrue(solution.ContainsKey("c"));

            bool aVal = (bool)solution["a"];
            bool bVal = (bool)solution["b"];
            bool cVal = (bool)solution["c"];

            // Verify the solution satisfies the constraint
            bool result = (aVal && bVal) || (!cVal);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestSolve_MixedTypeConstraints()
        {
            // Arrange
            var solver = new UnifiedConstraintSolver();
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var b = new SymbolicVariable("b", VM.Types.StackItemType.Boolean);

            // Create constraint: (x > 10) = b
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var constraint = new SymbolicExpression(xGt10, b, Operator.Equal);

            var constraints = new List<SymbolicExpression> { constraint };

            // Act
            var solution = solver.Solve(constraints);

            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("b"));

            long xVal = (long)solution["x"];
            bool bVal = (bool)solution["b"];

            // Verify the solution satisfies the constraint
            Assert.AreEqual(xVal > 10, bVal);
        }

        [TestMethod]
        public void TestPerformance_CompareWithSimpleConstraintSolver()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver(42); // Use fixed seed for reproducibility
            var simpleSolver = new SimpleConstraintSolver(42);

            // Create a complex set of constraints
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var y = new SymbolicVariable("y", VM.Types.StackItemType.Integer);
            var z = new SymbolicVariable("z", VM.Types.StackItemType.Integer);

            // x > 10
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);

            // y < 20
            var yLt20 = new SymbolicExpression(y, new ConcreteValue<long>(20), Operator.LessThan);

            // z = x + y
            var xPlusY = new SymbolicExpression(x, y, Operator.Add);
            var zEqXPlusY = new SymbolicExpression(z, xPlusY, Operator.Equal);

            var constraints = new List<SymbolicExpression> { xGt10, yLt20, zEqXPlusY };

            // Act - Measure performance of UnifiedConstraintSolver
            var unifiedStopwatch = new Stopwatch();
            unifiedStopwatch.Start();
            var unifiedSolution = unifiedSolver.Solve(constraints);
            unifiedStopwatch.Stop();
            var unifiedTime = unifiedStopwatch.ElapsedMilliseconds;

            // Act - Measure performance of SimpleConstraintSolver
            var simpleStopwatch = new Stopwatch();
            simpleStopwatch.Start();
            var simpleSolution = simpleSolver.Solve(constraints);
            simpleStopwatch.Stop();
            var simpleTime = simpleStopwatch.ElapsedMilliseconds;

            // Assert - Both solvers should find valid solutions
            Assert.IsTrue(unifiedSolution.ContainsKey("x"));
            Assert.IsTrue(unifiedSolution.ContainsKey("y"));
            Assert.IsTrue(unifiedSolution.ContainsKey("z"));

            Assert.IsTrue(simpleSolution.ContainsKey("x"));
            Assert.IsTrue(simpleSolution.ContainsKey("y"));
            Assert.IsTrue(simpleSolution.ContainsKey("z"));

            // Verify unified solver solution
            long xUnified = (long)unifiedSolution["x"];
            long yUnified = (long)unifiedSolution["y"];
            long zUnified = (long)unifiedSolution["z"];

            Assert.IsTrue(xUnified > 10);
            Assert.IsTrue(yUnified < 20);
            Assert.AreEqual(xUnified + yUnified, zUnified);

            // Verify simple solver solution
            long xSimple = (long)simpleSolution["x"];
            long ySimple = (long)simpleSolution["y"];
            long zSimple = (long)simpleSolution["z"];

            Assert.IsTrue(xSimple > 10);
            Assert.IsTrue(ySimple < 20);
            Assert.AreEqual(xSimple + ySimple, zSimple);

            // Log performance comparison
            Console.WriteLine($"UnifiedConstraintSolver time: {unifiedTime}ms");
            Console.WriteLine($"SimpleConstraintSolver time: {simpleTime}ms");
        }
    }
}
