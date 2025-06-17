using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Solvers;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Tests.Solvers
{
    [TestClass]
    public class ConstraintSolverAdapterTests
    {
        [TestMethod]
        public void TestAdapter_IsSatisfiable_PathConstraints()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create a path constraint
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var pathConstraint = new PathConstraint(xGt10, 0);
            
            var constraints = new List<PathConstraint> { pathConstraint };
            
            // Act
            bool result = adapter.IsSatisfiable(constraints);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void TestAdapter_IsSatisfiable_SymbolicExpressions()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create a symbolic expression
            var x = new SymbolicExecution.SymbolicExpression("x", VM.Types.StackItemType.Integer);
            var ten = new SymbolicExecution.SymbolicExpression(10);
            var xGt10 = new SymbolicExecution.SymbolicExpression(x, ten, SymbolicExecution.SymbolicExpressionType.GreaterThan);
            
            var constraints = new List<SymbolicExecution.SymbolicExpression> { xGt10 };
            
            // Act
            bool result = adapter.IsSatisfiable(constraints);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void TestAdapter_Solve_PathConstraints()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create path constraints
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var y = new SymbolicVariable("y", VM.Types.StackItemType.Integer);
            
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var yLt20 = new SymbolicExpression(y, new ConcreteValue<long>(20), Operator.LessThan);
            
            var pathConstraint1 = new PathConstraint(xGt10, 0);
            var pathConstraint2 = new PathConstraint(yLt20, 1);
            
            var constraints = new List<PathConstraint> { pathConstraint1, pathConstraint2 };
            
            // Act
            var solution = adapter.Solve(constraints);
            
            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("y"));
            
            long xVal = (long)solution["x"];
            long yVal = (long)solution["y"];
            
            Assert.IsTrue(xVal > 10);
            Assert.IsTrue(yVal < 20);
        }
        
        [TestMethod]
        public void TestAdapter_Solve_SymbolicExpressions()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create symbolic expressions
            var x = new SymbolicExecution.SymbolicExpression("x", VM.Types.StackItemType.Integer);
            var y = new SymbolicExecution.SymbolicExpression("y", VM.Types.StackItemType.Integer);
            
            var ten = new SymbolicExecution.SymbolicExpression(10);
            var twenty = new SymbolicExecution.SymbolicExpression(20);
            
            var xGt10 = new SymbolicExecution.SymbolicExpression(x, ten, SymbolicExecution.SymbolicExpressionType.GreaterThan);
            var yLt20 = new SymbolicExecution.SymbolicExpression(y, twenty, SymbolicExecution.SymbolicExpressionType.LessThan);
            
            var constraints = new List<SymbolicExecution.SymbolicExpression> { xGt10, yLt20 };
            
            // Act
            var solution = adapter.Solve(constraints);
            
            // Assert
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("y"));
            
            long xVal = (long)solution["x"];
            long yVal = (long)solution["y"];
            
            Assert.IsTrue(xVal > 10);
            Assert.IsTrue(yVal < 20);
        }
        
        [TestMethod]
        public void TestAdapter_UpdateConstraints_PathConstraints()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create initial path constraint
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var pathConstraint1 = new PathConstraint(xGt10, 0);
            
            var initialConstraints = new List<PathConstraint> { pathConstraint1 };
            
            // Act - Update with initial constraints
            adapter.UpdateConstraints(initialConstraints);
            
            // Create additional path constraint
            var y = new SymbolicVariable("y", VM.Types.StackItemType.Integer);
            var yLt20 = new SymbolicExpression(y, new ConcreteValue<long>(20), Operator.LessThan);
            var pathConstraint2 = new PathConstraint(yLt20, 1);
            
            var additionalConstraints = new List<PathConstraint> { pathConstraint2 };
            
            // Act - Update with additional constraints
            adapter.UpdateConstraints(additionalConstraints);
            
            // Combine all constraints
            var allConstraints = new List<PathConstraint> { pathConstraint1, pathConstraint2 };
            
            // Assert - The combined constraints should be satisfiable
            Assert.IsTrue(adapter.IsSatisfiable(allConstraints));
            
            // Solve the combined constraints
            var solution = adapter.Solve(allConstraints);
            
            // Verify the solution satisfies all constraints
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("y"));
            
            long xVal = (long)solution["x"];
            long yVal = (long)solution["y"];
            
            Assert.IsTrue(xVal > 10);
            Assert.IsTrue(yVal < 20);
        }
        
        [TestMethod]
        public void TestAdapter_UpdateConstraints_SymbolicExpressions()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create initial symbolic expression
            var x = new SymbolicExecution.SymbolicExpression("x", VM.Types.StackItemType.Integer);
            var ten = new SymbolicExecution.SymbolicExpression(10);
            var xGt10 = new SymbolicExecution.SymbolicExpression(x, ten, SymbolicExecution.SymbolicExpressionType.GreaterThan);
            
            var initialConstraints = new List<SymbolicExecution.SymbolicExpression> { xGt10 };
            
            // Act - Update with initial constraints
            adapter.UpdateConstraints(initialConstraints);
            
            // Create additional symbolic expression
            var y = new SymbolicExecution.SymbolicExpression("y", VM.Types.StackItemType.Integer);
            var twenty = new SymbolicExecution.SymbolicExpression(20);
            var yLt20 = new SymbolicExecution.SymbolicExpression(y, twenty, SymbolicExecution.SymbolicExpressionType.LessThan);
            
            var additionalConstraints = new List<SymbolicExecution.SymbolicExpression> { yLt20 };
            
            // Act - Update with additional constraints
            adapter.UpdateConstraints(additionalConstraints);
            
            // Combine all constraints
            var allConstraints = new List<SymbolicExecution.SymbolicExpression> { xGt10, yLt20 };
            
            // Assert - The combined constraints should be satisfiable
            Assert.IsTrue(adapter.IsSatisfiable(allConstraints));
            
            // Solve the combined constraints
            var solution = adapter.Solve(allConstraints);
            
            // Verify the solution satisfies all constraints
            Assert.IsTrue(solution.ContainsKey("x"));
            Assert.IsTrue(solution.ContainsKey("y"));
            
            long xVal = (long)solution["x"];
            long yVal = (long)solution["y"];
            
            Assert.IsTrue(xVal > 10);
            Assert.IsTrue(yVal < 20);
        }
        
        [TestMethod]
        public void TestAdapter_Simplify_PathConstraints()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create path constraints with a tautology
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var xEqX = new SymbolicExpression(x, x, Operator.Equal); // Tautology: x = x
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            
            var pathConstraint1 = new PathConstraint(xEqX, 0);
            var pathConstraint2 = new PathConstraint(xGt10, 1);
            
            var constraints = new List<PathConstraint> { pathConstraint1, pathConstraint2 };
            
            // Act
            var simplified = adapter.Simplify(constraints).ToList();
            
            // Assert - The tautology should be removed
            Assert.AreEqual(1, simplified.Count);
            Assert.AreEqual(1, simplified[0].InstructionPointer); // Should be the xGt10 constraint
        }
        
        [TestMethod]
        public void TestAdapter_Simplify_SymbolicExpressions()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create symbolic expressions with a tautology
            var x = new SymbolicExecution.SymbolicExpression("x", VM.Types.StackItemType.Integer);
            var xEqX = new SymbolicExecution.SymbolicExpression(x, x, SymbolicExecution.SymbolicExpressionType.Equal); // Tautology: x = x
            
            var ten = new SymbolicExecution.SymbolicExpression(10);
            var xGt10 = new SymbolicExecution.SymbolicExpression(x, ten, SymbolicExecution.SymbolicExpressionType.GreaterThan);
            
            var constraints = new List<SymbolicExecution.SymbolicExpression> { xEqX, xGt10 };
            
            // Act
            var simplified = adapter.Simplify(constraints).ToList();
            
            // Assert - The tautology should be removed
            Assert.AreEqual(1, simplified.Count);
            // The remaining constraint should be xGt10
            Assert.AreEqual(SymbolicExecution.SymbolicExpressionType.GreaterThan, simplified[0].Type);
        }
        
        [TestMethod]
        public void TestAdapter_TrySolve_Success()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create satisfiable path constraints
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var pathConstraint = new PathConstraint(xGt10, 0);
            
            var constraints = new List<PathConstraint> { pathConstraint };
            
            // Act
            bool result = adapter.TrySolve(constraints, out var solution);
            
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(solution);
            Assert.IsTrue(solution.ContainsKey("x"));
            
            long xVal = (long)solution["x"];
            Assert.IsTrue(xVal > 10);
        }
        
        [TestMethod]
        public void TestAdapter_TrySolve_Failure()
        {
            // Arrange
            var unifiedSolver = new UnifiedConstraintSolver();
            var adapter = new ConstraintSolverAdapter(unifiedSolver);
            
            // Create contradictory path constraints
            var x = new SymbolicVariable("x", VM.Types.StackItemType.Integer);
            var xGt10 = new SymbolicExpression(x, new ConcreteValue<long>(10), Operator.GreaterThan);
            var xLt5 = new SymbolicExpression(x, new ConcreteValue<long>(5), Operator.LessThan);
            
            var pathConstraint1 = new PathConstraint(xGt10, 0);
            var pathConstraint2 = new PathConstraint(xLt5, 1);
            
            var constraints = new List<PathConstraint> { pathConstraint1, pathConstraint2 };
            
            // Act
            bool result = adapter.TrySolve(constraints, out var solution);
            
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(solution);
            Assert.AreEqual(0, solution.Count);
        }
    }
}
