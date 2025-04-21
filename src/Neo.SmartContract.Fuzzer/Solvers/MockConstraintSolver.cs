using System.Collections.Generic;
using System.Linq;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// A mock implementation of the IConstraintSolver interface for testing.
    /// </summary>
    public class MockConstraintSolver : IConstraintSolver
    {
        private readonly bool _defaultSatisfiability;
        private readonly Dictionary<string, object> _model;

        /// <summary>
        /// Initializes a new instance of the MockConstraintSolver class.
        /// </summary>
        /// <param name="defaultSatisfiability">The default satisfiability result to return.</param>
        /// <param name="model">The model to return when solving constraints.</param>
        public MockConstraintSolver(bool defaultSatisfiability = true, Dictionary<string, object>? model = null)
        {
            _defaultSatisfiability = defaultSatisfiability;
            _model = model ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Checks if the given set of path constraints is satisfiable.
        /// </summary>
        /// <param name="constraints">The list of symbolic expressions representing path constraints.</param>
        /// <returns>The default satisfiability result.</returns>
        public bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints)
        {
            return _defaultSatisfiability;
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>The model provided during initialization.</returns>
        public Dictionary<string, object> Solve(IEnumerable<SymbolicExpression> constraints)
        {
            return new Dictionary<string, object>(_model);
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<SymbolicExpression> constraints)
        {
            // No-op for mock implementation
        }

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>The original constraints.</returns>
        public IEnumerable<SymbolicExpression> Simplify(IEnumerable<SymbolicExpression> constraints)
        {
            return constraints.ToList();
        }
    }
}
