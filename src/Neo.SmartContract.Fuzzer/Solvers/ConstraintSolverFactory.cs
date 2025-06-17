using System;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Factory for creating constraint solvers.
    /// </summary>
    public static class ConstraintSolverFactory
    {
        /// <summary>
        /// Creates a new constraint solver.
        /// </summary>
        /// <param name="type">The type of constraint solver to create.</param>
        /// <param name="seed">Optional seed for random number generation.</param>
        /// <returns>A new constraint solver.</returns>
        public static IConstraintSolver Create(ConstraintSolverType type = ConstraintSolverType.Unified, int? seed = null)
        {
            switch (type)
            {
                case ConstraintSolverType.Unified:
                    return seed.HasValue ? new UnifiedConstraintSolver(seed.Value) : new UnifiedConstraintSolver();
                
                case ConstraintSolverType.Z3:
                    return new Z3Solver();
                
                case ConstraintSolverType.Dummy:
                    return new DummySolver();
                
                default:
                    throw new ArgumentException($"Unknown constraint solver type: {type}", nameof(type));
            }
        }
    }

    /// <summary>
    /// Types of constraint solvers.
    /// </summary>
    public enum ConstraintSolverType
    {
        /// <summary>
        /// The unified constraint solver that combines the best parts of all solvers.
        /// </summary>
        Unified,
        
        /// <summary>
        /// The Z3-based constraint solver.
        /// </summary>
        Z3,
        
        /// <summary>
        /// A dummy constraint solver for testing.
        /// </summary>
        Dummy
    }
}
