using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.IntegrationTests
{
    [TestClass]
    public class SimpleConstraintSolverIntegrationTests
    {
        private SimpleConstraintSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new SimpleConstraintSolver();
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
            var const10 = new SymbolicConstant(10);
            var const20 = new SymbolicConstant(20);
            var const30 = new SymbolicConstant(30);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, const10, Operator.GreaterThan);
            
            // y < 20
            var yLt20 = new SymbolicExpression(y, const20, Operator.LessThan);
            
            // z == 30
            var zEq30 = new SymbolicExpression(z, const30, Operator.Equal);
            
            // Create path constraints
            var constraint1 = new PathConstraint(xGt10, 0);
            var constraint2 = new PathConstraint(yLt20, 1);
            var constraint3 = new PathConstraint(zEq30, 2);
            
            // Link constraints
            constraint1.Next = constraint2;
            constraint2.Next = constraint3;

            // Act
            var solution = _solver.Solve(constraint1);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("x", "y", "z");
            
            // Verify solution satisfies constraints
            ((int)solution["x"]).Should().BeGreaterThan(10);
            ((int)solution["y"]).Should().BeLessThan(20);
            ((int)solution["z"]).Should().Be(30);
        }

        [TestMethod]
        public void Solve_ArithmeticConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            
            // Create constants
            var const5 = new SymbolicConstant(5);
            var const10 = new SymbolicConstant(10);
            
            // Create expressions
            // x + 5 == y
            var xPlus5 = new SymbolicExpression(x, const5, Operator.Add);
            var xPlus5EqY = new SymbolicExpression(xPlus5, y, Operator.Equal);
            
            // y < 10
            var yLt10 = new SymbolicExpression(y, const10, Operator.LessThan);
            
            // Create path constraints
            var constraint1 = new PathConstraint(xPlus5EqY, 0);
            var constraint2 = new PathConstraint(yLt10, 1);
            
            // Link constraints
            constraint1.Next = constraint2;

            // Act
            var solution = _solver.Solve(constraint1);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("x", "y");
            
            // Verify solution satisfies constraints
            ((int)solution["x"] + 5).Should().Be((int)solution["y"]);
            ((int)solution["y"]).Should().BeLessThan(10);
        }

        [TestMethod]
        public void Solve_BooleanConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var a = new SymbolicVariable("a", StackItemType.Boolean);
            var b = new SymbolicVariable("b", StackItemType.Boolean);
            
            // Create constants
            var constTrue = new SymbolicConstant(true);
            var constFalse = new SymbolicConstant(false);
            
            // Create expressions
            // a == true
            var aEqTrue = new SymbolicExpression(a, constTrue, Operator.Equal);
            
            // b == false
            var bEqFalse = new SymbolicExpression(b, constFalse, Operator.Equal);
            
            // Create path constraints
            var constraint1 = new PathConstraint(aEqTrue, 0);
            var constraint2 = new PathConstraint(bEqFalse, 1);
            
            // Link constraints
            constraint1.Next = constraint2;

            // Act
            var solution = _solver.Solve(constraint1);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKeys("a", "b");
            
            // Verify solution satisfies constraints
            solution["a"].Should().Be(true);
            solution["b"].Should().Be(false);
        }

        [TestMethod]
        public void Solve_StringConstraints_ReturnsValidSolution()
        {
            // Arrange
            // Create variables
            var s = new SymbolicVariable("s", StackItemType.ByteString);
            
            // Create constants
            var constHello = new SymbolicConstant("hello");
            
            // Create expressions
            // s == "hello"
            var sEqHello = new SymbolicExpression(s, constHello, Operator.Equal);
            
            // Create path constraints
            var constraint = new PathConstraint(sEqHello, 0);

            // Act
            var solution = _solver.Solve(constraint);

            // Assert
            solution.Should().NotBeNull();
            solution.Should().ContainKey("s");
            
            // Verify solution satisfies constraints
            solution["s"].Should().Be("hello");
        }

        [TestMethod]
        public void Solve_ContradictoryConstraints_ReturnsNull()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new SymbolicConstant(10);
            var const20 = new SymbolicConstant(20);
            
            // Create expressions
            // x == 10
            var xEq10 = new SymbolicExpression(x, const10, Operator.Equal);
            
            // x == 20
            var xEq20 = new SymbolicExpression(x, const20, Operator.Equal);
            
            // Create path constraints
            var constraint1 = new PathConstraint(xEq10, 0);
            var constraint2 = new PathConstraint(xEq20, 1);
            
            // Link constraints
            constraint1.Next = constraint2;

            // Act
            var solution = _solver.Solve(constraint1);

            // Assert
            solution.Should().BeNull();
        }

        [TestMethod]
        public void AreContradictory_ComplexConstraints_ReturnsExpectedResult()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const10 = new SymbolicConstant(10);
            var const20 = new SymbolicConstant(20);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, const10, Operator.GreaterThan);
            
            // x < 20
            var xLt20 = new SymbolicExpression(x, const20, Operator.LessThan);
            
            // x == 10
            var xEq10 = new SymbolicExpression(x, const10, Operator.Equal);
            
            // Create path constraints
            var constraint1 = new PathConstraint(xGt10, 0);
            var constraint2 = new PathConstraint(xLt20, 1);
            var constraint3 = new PathConstraint(xEq10, 2);

            // Act
            var result1 = _solver.AreContradictory(constraint1, constraint2);
            var result2 = _solver.AreContradictory(constraint1, constraint3);

            // Assert
            result1.Should().BeFalse(); // x > 10 and x < 20 are not contradictory
            result2.Should().BeTrue();  // x > 10 and x == 10 are contradictory
        }

        [TestMethod]
        public void IsSymbolicExpressionSubsumedBy_ComplexExpressions_ReturnsExpectedResult()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            
            // Create constants
            var const5 = new SymbolicConstant(5);
            var const10 = new SymbolicConstant(10);
            var const15 = new SymbolicConstant(15);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, const10, Operator.GreaterThan);
            
            // x > 5
            var xGt5 = new SymbolicExpression(x, const5, Operator.GreaterThan);
            
            // x < 15
            var xLt15 = new SymbolicExpression(x, const15, Operator.LessThan);

            // Act
            var result1 = _solver.IsSymbolicExpressionSubsumedBy(xGt10, xGt5);
            var result2 = _solver.IsSymbolicExpressionSubsumedBy(xGt5, xGt10);
            var result3 = _solver.IsSymbolicExpressionSubsumedBy(xGt10, xLt15);

            // Assert
            result1.Should().BeTrue();  // x > 10 implies x > 5
            result2.Should().BeFalse(); // x > 5 does not imply x > 10
            result3.Should().BeFalse(); // x > 10 does not imply x < 15
        }

        [TestMethod]
        public void GatherVariables_ComplexExpression_ReturnsAllVariables()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            var z = new SymbolicVariable("z", StackItemType.Integer);
            
            // Create constants
            var const5 = new SymbolicConstant(5);
            
            // Create expressions
            // (x + y) > (z + 5)
            var xPlusY = new SymbolicExpression(x, y, Operator.Add);
            var zPlus5 = new SymbolicExpression(z, const5, Operator.Add);
            var complexExpr = new SymbolicExpression(xPlusY, zPlus5, Operator.GreaterThan);

            // Act
            var variables = _solver.GatherVariables(complexExpr);

            // Assert
            variables.Should().NotBeNull();
            variables.Should().HaveCount(3);
            variables.Should().Contain("x");
            variables.Should().Contain("y");
            variables.Should().Contain("z");
        }

        [TestMethod]
        public void UpdateConstraints_WithNewConstraint_ReturnsUpdatedConstraints()
        {
            // Arrange
            // Create variables
            var x = new SymbolicVariable("x", StackItemType.Integer);
            var y = new SymbolicVariable("y", StackItemType.Integer);
            
            // Create constants
            var const10 = new SymbolicConstant(10);
            var const20 = new SymbolicConstant(20);
            
            // Create expressions
            // x > 10
            var xGt10 = new SymbolicExpression(x, const10, Operator.GreaterThan);
            
            // y < 20
            var yLt20 = new SymbolicExpression(y, const20, Operator.LessThan);
            
            // Create path constraints
            var constraint1 = new PathConstraint(xGt10, 0);
            var constraint2 = new PathConstraint(yLt20, 1);
            
            // Link constraints
            constraint1.Next = constraint2;

            // Act
            var updatedConstraints = _solver.UpdateConstraints(constraint1, constraint2);

            // Assert
            updatedConstraints.Should().NotBeNull();
            
            // Verify the updated constraints contain both original constraints
            var constraints = new List<PathConstraint>();
            var current = updatedConstraints;
            while (current != null)
            {
                constraints.Add(current);
                current = current.Next;
            }
            
            constraints.Should().HaveCount(2);
            constraints[0].Expression.Should().Be(xGt10);
            constraints[1].Expression.Should().Be(yLt20);
        }
    }
}
