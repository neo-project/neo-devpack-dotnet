using System.Collections.Generic;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for a constraint solver used in symbolic execution.
    /// </summary>
    public interface IConstraintSolver
    {
        /// <summary>
        /// Determines if the specified constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        bool IsSatisfiable(IEnumerable<PathConstraint> constraints);

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        Dictionary<string, object> Solve(IEnumerable<PathConstraint> constraints);

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        void UpdateConstraints(IEnumerable<PathConstraint> constraints);

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        IEnumerable<PathConstraint> Simplify(IEnumerable<PathConstraint> constraints);
    }
}
