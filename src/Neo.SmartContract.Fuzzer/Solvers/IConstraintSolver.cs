using System.Collections.Generic;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Interface for a constraint solver capable of checking satisfiability of symbolic path constraints.
    /// </summary>
    public interface IConstraintSolver
    {
        /// <summary>
        /// Checks if the given set of path constraints is satisfiable.
        /// </summary>
        /// <param name="constraints">The list of symbolic expressions representing path constraints.</param>
        /// <returns>True if the constraints might be satisfiable, False if they are proven unsatisfiable.</returns>
        /// <remarks>
        /// Implementations may return True if the satisfiability is unknown or cannot be determined (e.g., due to solver limitations or timeouts).
        /// </remarks>
        bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints);

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        Dictionary<string, object> Solve(IEnumerable<SymbolicExpression> constraints);

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        void UpdateConstraints(IEnumerable<SymbolicExpression> constraints);

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        IEnumerable<SymbolicExpression> Simplify(IEnumerable<SymbolicExpression> constraints);
    }
}
