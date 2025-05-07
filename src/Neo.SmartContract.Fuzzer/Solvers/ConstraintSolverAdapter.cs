using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Adapter class that bridges between the Solvers.IConstraintSolver and SymbolicExecution.Interfaces.IConstraintSolver interfaces.
    /// </summary>
    public class ConstraintSolverAdapter : SymbolicExecution.Interfaces.IConstraintSolver
    {
        private readonly Solvers.IConstraintSolver _solver;

        /// <summary>
        /// Initializes a new instance of the ConstraintSolverAdapter class.
        /// </summary>
        /// <param name="solver">The constraint solver to adapt.</param>
        public ConstraintSolverAdapter(Solvers.IConstraintSolver solver)
        {
            _solver = solver ?? throw new ArgumentNullException(nameof(solver));
        }

        /// <summary>
        /// Tries to solve the given path constraints.
        /// </summary>
        /// <param name="constraints">The path constraints to solve.</param>
        /// <param name="solution">The solution, if found.</param>
        /// <returns>True if a solution was found, false otherwise.</returns>
        public bool TrySolve(IEnumerable<PathConstraint> constraints, out Dictionary<string, object> solution)
        {
            try
            {
                // Convert PathConstraint to TypesSymbolicExpression
                var convertedConstraints = constraints.Select(c => c.Expression).ToList();

                // Check if the constraints are satisfiable
                if (!_solver.IsSatisfiable(convertedConstraints))
                {
                    solution = new Dictionary<string, object>();
                    return false;
                }

                // Solve the constraints
                solution = _solver.Solve(convertedConstraints);
                return solution.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TrySolve: {ex.Message}");
                solution = new Dictionary<string, object>();
                return false;
            }
        }

        /// <summary>
        /// Determines if the specified constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<PathConstraint> constraints)
        {
            try
            {
                // Convert PathConstraint to TypesSymbolicExpression
                var convertedConstraints = constraints.Select(c => c.Expression).ToList();

                // Use the solver to check satisfiability
                return _solver.IsSatisfiable(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking satisfiability of path constraints: {ex.Message}");
                // If there's an error, conservatively assume the constraints might be satisfiable
                return true;
            }
        }

        /// <summary>
        /// Determines if the specified constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<object> constraints)
        {
            try
            {
                // Convert constraints to SymbolicExpression
                var convertedConstraints = new List<SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                }

                // Use the solver to check satisfiability
                return _solver.IsSatisfiable(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking satisfiability of constraints: {ex.Message}");
                // If there's an error, conservatively assume the constraints might be satisfiable
                return true;
            }
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<PathConstraint> constraints)
        {
            try
            {
                // Convert PathConstraint to TypesSymbolicExpression
                var convertedConstraints = constraints.Select(c => c.Expression).ToList();

                // Use the solver to solve the constraints
                return _solver.Solve(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving path constraints: {ex.Message}");
                // If there's an error, return an empty dictionary
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<object> constraints)
        {
            try
            {
                // Convert constraints to SymbolicExpression
                var convertedConstraints = new List<SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                }

                // Check if the constraints are satisfiable first
                if (!_solver.IsSatisfiable(convertedConstraints))
                {
                    return new Dictionary<string, object>();
                }

                // Use the solver to solve the constraints
                return _solver.Solve(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving constraints: {ex.Message}");
                // If there's an error, return an empty dictionary
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<PathConstraint> constraints)
        {
            try
            {
                // Convert PathConstraint to TypesSymbolicExpression
                var convertedConstraints = constraints.Select(c => c.Expression).ToList();

                // Use the solver to update constraints
                _solver.UpdateConstraints(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating path constraints: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<object> constraints)
        {
            try
            {
                // Convert constraints to SymbolicExpression
                var convertedConstraints = new List<SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                }

                // Use the solver to update constraints
                _solver.UpdateConstraints(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating constraints: {ex.Message}");
            }
        }

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        public IEnumerable<PathConstraint> Simplify(IEnumerable<PathConstraint> constraints)
        {
            try
            {
                // Convert PathConstraint to TypesSymbolicExpression
                var convertedConstraints = constraints.Select(c => c.Expression).ToList();

                // Use the solver to simplify constraints
                var simplifiedExpressions = _solver.Simplify(convertedConstraints);

                // Convert back to PathConstraint
                // Create a new list of PathConstraints from the simplified expressions
                return simplifiedExpressions.Select(expr => new PathConstraint(expr, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simplifying path constraints: {ex.Message}");
                // If there's an error, return the original constraints
                return constraints;
            }
        }

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        public IEnumerable<object> Simplify(IEnumerable<object> constraints)
        {
            try
            {
                // Convert constraints to SymbolicExpression
                var convertedConstraints = new List<SymbolicExpression>();
                var originalConstraints = constraints.ToList();

                foreach (var constraint in originalConstraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                }

                // Use the solver to simplify constraints
                var simplifiedExpressions = _solver.Simplify(convertedConstraints);

                // Convert back to the original type if possible
                var result = new List<object>();
                for (int i = 0; i < simplifiedExpressions.Count(); i++)
                {
                    var simplified = simplifiedExpressions.ElementAt(i);

                    // If the original was a SymbolicExecution.SymbolicExpression, convert back
                    if (i < originalConstraints.Count && originalConstraints[i] is SymbolicExecution.SymbolicExpression)
                    {
                        // Convert from SymbolicExecution.Types.SymbolicExpression to SymbolicExecution.SymbolicExpression
                        result.Add(SymbolicExpressionConverter.ToSymbolicExpression(simplified));
                    }
                    else
                    {
                        // Otherwise, keep as SymbolicExecution.Types.SymbolicExpression
                        result.Add(simplified);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simplifying constraints: {ex.Message}");
                // If there's an error, return the original constraints
                return constraints;
            }
        }
    }
}
