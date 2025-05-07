using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Solvers;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    [TestClass]
    public class UnifiedConstraintSolverIntegrationTests
    {
        private UnifiedConstraintSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new UnifiedConstraintSolver(42); // Use fixed seed for reproducibility
        }

        [TestMethod]
        public void Solve_ComplexConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            var z = new SymbolicVariable("z", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            var const20 = new ConcreteValue<long>(20);
            var const30 = new ConcreteValue<long>(30);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, Operator.GreaterThan, const10);
            
            // y < 20
            var yLt20 = new SymbolicExpression(y, Operator.LessThan, const20);
            
            // z == 30
            var zEq30 = new SymbolicExpression(z, Operator.Equal, const30);
            
            // Create list of constraints
            var constraints = new List<SymbolicExpression> { xGt10, yLt20, zEq30 };

            // Act
            var solution = _solver.Solve(constraints);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("x", "y", "z");
            
            // Verify solution satisfies constraints
            ((long)solution["x"]).Should().BeGreaterThan(10);
            ((long)solution["y"]).Should().BeLessThan(20);
            ((long)solution["z"]).Should().Be(30);
        }

        [TestMethod]
        public void Solve_ArithmeticConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            
            // Create constants
            var const5 = new ConcreteValue<long>(5);
            var const10 = new ConcreteValue<long>(10);
            
            // Create expressions
            // x + 5 == y
            var xPlus5 = new SymbolicExpression(x, Operator.Add, const5);
            var xPlus5EqY = new SymbolicExpression(xPlus5, Operator.Equal, y);
            
            // y < 10
            var yLt10 = new SymbolicExpression(y, Operator.LessThan, const10);
            
            // Create list of constraints
            var constraints = new List<SymbolicExpression> { xPlus5EqY, yLt10 };

            // Act
            var solution = _solver.Solve(constraints);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("x", "y");
            
            // Verify solution satisfies constraints
            ((long)solution["x"] + 5).Should().Be((long)solution["y"]);
            ((long)solution["y"]).Should().BeLessThan(10);
        }

        [TestMethod]
        public void Solve_BooleanConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var a = new SymbolicVariable("a", StackItemType.Boolean);
            var b = new SymbolicVariable("b", StackItemType.Boolean);
            
            // Create constants
            var constTrue = new ConcreteValue<bool>(true);
            var constFalse = new ConcreteValue<bool>(false);
            
            // Create expressions
            // a == true
            var aEqTrue = new SymbolicExpression(a, Operator.Equal, constTrue);
            
            // b == false
            var bEqFalse = new SymbolicExpression(b, Operator.Equal, constFalse);
            
            // Create list of constraints
            var constraints = new List<SymbolicExpression> { aEqTrue, bEqFalse };

            // Act
            var solution = _solver.Solve(constraints);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("a", "b");
            
            // Verify solution satisfies constraints
            solution["a"].Should().Be(true);
            solution["b"].Should().Be(false);
        }

        [TestMethod]
        public void Solve_ContradictoryConstraints_ReturnsEmptyDictionary()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            var const20 = new ConcreteValue<long>(20);
            
            // Create expressions
            // x == 10
            var xEq10 = new SymbolicExpression(x, Operator.Equal, const10);
            
            // x == 20
            var xEq20 = new SymbolicExpression(x, Operator.Equal, const20);
            
            // Create list of constraints
            var constraints = new List<SymbolicExpression> { xEq10, xEq20 };

            // Act
            var solution = _solver.Solve(constraints);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().BeEmpty();
        }

        [TestMethod]
        public void IsSatisfiable_ComplexConstraints_ReturnsExpectedResult()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            var const20 = new ConcreteValue<long>(20);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, Operator.GreaterThan, const10);
            
            // x < 20
            var xLt20 = new SymbolicExpression(x, Operator.LessThan, const20);
            
            // x == 10
            var xEq10 = new SymbolicExpression(x, Operator.Equal, const10);
            
            // Create constraint lists
            var satisfiableConstraints = new List<SymbolicExpression> { xGt10, xLt20 };
            var unsatisfiableConstraints = new List<SymbolicExpression> { xGt10, xEq10 };

            // Act
            var result1 = _solver.IsSatisfiable(satisfiableConstraints);
            var result2 = _solver.IsSatisfiable(unsatisfiableConstraints);

            // Assert
            result1.Should().BeTrue(); // x > 10 and x < 20 are satisfiable
            result2.Should().BeFalse(); // x > 10 and x == 10 are not satisfiable
        }

        [TestMethod]
        public void Simplify_ComplexConstraints_RemovesTautologies()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, Operator.GreaterThan, const10);
            
            // x == x (tautology)
            var xEqX = new SymbolicExpression(x, Operator.Equal, x);
            
            // Create list of constraints
            var constraints = new List<SymbolicExpression> { xGt10, xEqX };

            // Act
            var simplified = _solver.Simplify(constraints).ToList();

            // Assert
            simplified.Should().NotBeNull();
            simplified.Should().HaveCount(1); // The tautology should be removed
            simplified[0].Should().Be(xGt10);
        }

        [TestMethod]
        public void UpdateConstraints_AddsNewConstraints()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            var const20 = new ConcreteValue<long>(20);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, Operator.GreaterThan, const10);
            
            // y < 20
            var yLt20 = new SymbolicExpression(y, Operator.LessThan, const20);
            
            // Create initial constraints
            var initialConstraints = new List<SymbolicExpression> { xGt10 };
            
            // Act - Update with initial constraints
            _solver.UpdateConstraints(initialConstraints);
            
            // Create additional constraints
            var additionalConstraints = new List<SymbolicExpression> { yLt20 };
            
            // Act - Update with additional constraints
            _solver.UpdateConstraints(additionalConstraints);
            
            // Combine all constraints
            var allConstraints = new List<SymbolicExpression> { xGt10, yLt20 };
            
            // Assert - The combined constraints should be satisfiable
            _solver.IsSatisfiable(allConstraints).Should().BeTrue();
            
            // Solve the combined constraints
            var solution = _solver.Solve(allConstraints);
            
            // Verify the solution satisfies all constraints
            solution.Should().ContainKeys("x", "y");
            ((long)solution["x"]).Should().BeGreaterThan(10);
            ((long)solution["y"]).Should().BeLessThan(20);
        }

        [TestMethod]
        public void TrySolve_WithAdapter_ReturnsExpectedResult()
        {
            // Arrange
            var adapter = new ConstraintSolverAdapter(_solver);
            
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new ConcreteValue<long>(10);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, Operator.GreaterThan, const10);
            
            // Create path constraints
            var pathConstraint = new PathConstraint(xGt10, 0);
            var constraints = new List<PathConstraint> { pathConstraint };
            
            // Act
            bool result = adapter.TrySolve(constraints, out var solution);
            
            // Assert
            result.Should().BeTrue();
            solution.Should().NotBeNull();
            solution.Should().ContainKey("x");
            ((long)solution["x"]).Should().BeGreaterThan(10);
        }
    }
}
