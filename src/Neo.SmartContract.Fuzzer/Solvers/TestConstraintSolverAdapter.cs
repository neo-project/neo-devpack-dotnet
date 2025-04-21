using System.Collections.Generic;
using System.Linq;
using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Adapter class that bridges between the Solvers.IConstraintSolver and SymbolicExecution.IConstraintSolver interfaces.
    /// Used primarily for testing.
    /// </summary>
    public class TestConstraintSolverAdapter : SymbolicExecution.Interfaces.IConstraintSolver
    {
        private readonly Solvers.IConstraintSolver _solver;

        /// <summary>
        /// Initializes a new instance of the TestConstraintSolverAdapter class.
        /// </summary>
        /// <param name="solver">The underlying solver to adapt.</param>
        public TestConstraintSolverAdapter(Solvers.IConstraintSolver solver)
        {
            _solver = solver;
        }

        /// <summary>
        /// Determines if the specified constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<PathConstraint> constraints)
        {
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            return _solver.IsSatisfiable(symbolicExpressions);
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<PathConstraint> constraints)
        {
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            return _solver.Solve(symbolicExpressions);
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<PathConstraint> constraints)
        {
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            _solver.UpdateConstraints(symbolicExpressions);
        }

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        public IEnumerable<PathConstraint> Simplify(IEnumerable<PathConstraint> constraints)
        {
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            var simplifiedExpressions = _solver.Simplify(symbolicExpressions);

            // Convert back to PathConstraint
            return simplifiedExpressions.Select((expr, i) =>
                new PathConstraint(expr, constraints.ElementAt(i).InstructionPointer)).ToList();
        }
    }
}
