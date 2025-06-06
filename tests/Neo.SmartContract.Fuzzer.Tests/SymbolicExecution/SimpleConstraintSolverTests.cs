using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System.Collections.Generic;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution
{
    [TestClass]
    public class SimpleConstraintSolverTests
    {
        private SimpleConstraintSolver _solver;

        [TestInitialize]
        public void Setup()
        {
            _solver = new SimpleConstraintSolver(42); // Use a fixed seed for reproducibility
        }

        [TestMethod]
        public void IsSatisfiable_ReturnsTrue()
        {
            // Arrange
            var constraints = new List<PathConstraint>
            {
                new PathConstraint(new SymbolicVariable("x", "Integer"), false)
            };

            // Act
            bool result = _solver.IsSatisfiable(constraints);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void TrySolve_ReturnsTrue()
        {
            // Arrange
            var constraints = new List<PathConstraint>
            {
                new PathConstraint(new SymbolicVariable("x", "Integer"), false)
            };

            // Act
            bool result = _solver.TrySolve(constraints, out var solution);

            // Assert
            result.Should().BeTrue();
            solution.Should().NotBeNull();
            solution.Should().ContainKey("x");
        }

        [TestMethod]
        public void TrySolve_ExtractsVariableNames()
        {
            // Arrange
            var x = new SymbolicVariable("x", "Integer");
            var y = new SymbolicVariable("y", "Integer");
            var binaryExpr = new SymbolicBinaryExpression(x, y, "+", "Integer");
            var constraints = new List<PathConstraint>
            {
                new PathConstraint(binaryExpr, false)
            };

            // Act
            bool result = _solver.TrySolve(constraints, out var solution);

            // Assert
            result.Should().BeTrue();
            solution.Should().NotBeNull();
            solution.Should().ContainKey("x");
            solution.Should().ContainKey("y");
        }

        [TestMethod]
        public void TrySolve_HandlesUnaryExpressions()
        {
            // Arrange
            var x = new SymbolicVariable("x", "Integer");
            var unaryExpr = new SymbolicUnaryExpression(x, "-", "Integer");
            var constraints = new List<PathConstraint>
            {
                new PathConstraint(unaryExpr, false)
            };

            // Act
            bool result = _solver.TrySolve(constraints, out var solution);

            // Assert
            result.Should().BeTrue();
            solution.Should().NotBeNull();
            solution.Should().ContainKey("x");
        }

        [TestMethod]
        public void TrySolve_GeneratesRandomValues()
        {
            // Arrange
            var constraints = new List<PathConstraint>
            {
                new PathConstraint(new SymbolicVariable("x", "Integer"), false)
            };

            // Act
            bool result1 = _solver.TrySolve(constraints, out var solution1);
            
            // Create a new solver with a different seed
            var solver2 = new SimpleConstraintSolver(43);
            bool result2 = solver2.TrySolve(constraints, out var solution2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            solution1.Should().NotBeNull();
            solution2.Should().NotBeNull();
            solution1.Should().ContainKey("x");
            solution2.Should().ContainKey("x");
            
            // The values should be different due to different seeds
            solution1["x"].Should().NotBe(solution2["x"]);
        }
    }
}
