using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Determines if the specified constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression> constraints)
        {
            return _solver.IsSatisfiable(constraints);
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
                var convertedConstraints = new List<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Add the expression from the path constraint
                        convertedConstraints.Add(pathConstraint.Expression);
                    }
                }

                // If no constraints, it's trivially satisfiable
                if (!convertedConstraints.Any())
                    return true;

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
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            return _solver.Solve(symbolicExpressions);
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression> constraints)
        {
            return _solver.Solve(constraints);
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
                var convertedConstraints = new List<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Add the expression from the path constraint
                        convertedConstraints.Add(pathConstraint.Expression);
                    }
                }

                // If no constraints, return an empty solution
                if (!convertedConstraints.Any())
                    return new Dictionary<string, object>();

                // Check if the constraints are satisfiable first
                if (!_solver.IsSatisfiable(convertedConstraints))
                    return new Dictionary<string, object>();

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
        /// Tries to solve the given path constraints.
        /// </summary>
        /// <param name="constraints">The path constraints to solve.</param>
        /// <param name="solution">The solution, if found.</param>
        /// <returns>True if a solution was found, false otherwise.</returns>
        public bool TrySolve(IEnumerable<PathConstraint> constraints, out Dictionary<string, object> solution)
        {
            solution = Solve(constraints);
            return _solver.IsSatisfiable(constraints.Select(c => c.Expression).ToList());
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
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression> constraints)
        {
            _solver.UpdateConstraints(constraints);
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
                var convertedConstraints = new List<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression>();

                foreach (var constraint in constraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Add the expression from the path constraint
                        convertedConstraints.Add(pathConstraint.Expression);
                    }
                }

                // If no constraints, nothing to do
                if (!convertedConstraints.Any())
                    return;

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
            // Convert PathConstraint to SymbolicExpression
            var symbolicExpressions = constraints.Select(c => c.Expression).ToList();
            var simplifiedExpressions = _solver.Simplify(symbolicExpressions);

            // Convert back to PathConstraint
            return simplifiedExpressions.Select((expr, i) =>
                new PathConstraint(expr, constraints.ElementAt(i).InstructionPointer)).ToList();
        }

        /// <summary>
        /// Simplifies the given constraints.
        /// </summary>
        /// <param name="constraints">The constraints to simplify.</param>
        /// <returns>Simplified constraints.</returns>
        public IEnumerable<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression> Simplify(IEnumerable<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression> constraints)
        {
            return _solver.Simplify(constraints);
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
                var convertedConstraints = new List<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression>();
                var originalConstraints = constraints.ToList();

                foreach (var constraint in originalConstraints)
                {
                    if (constraint is SymbolicExecution.SymbolicExpression symbolicExpr)
                    {
                        // Convert from SymbolicExecution.SymbolicExpression to SymbolicExecution.Types.SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToTypesExpression(symbolicExpr));
                    }
                    else if (constraint is Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression typesExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(typesExpr);
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Add the expression from the path constraint
                        convertedConstraints.Add(pathConstraint.Expression);
                    }
                }

                // If no constraints, return the original constraints
                if (!convertedConstraints.Any())
                    return constraints;

                // Use the solver to simplify constraints
                var simplifiedExpressions = _solver.Simplify(convertedConstraints).ToList();

                // Convert back to the original type if possible
                var result = new List<object>();
                for (int i = 0; i < simplifiedExpressions.Count; i++)
                {
                    var simplified = simplifiedExpressions[i];

                    // If the original was a SymbolicExecution.SymbolicExpression, convert back
                    if (i < originalConstraints.Count && originalConstraints[i] is SymbolicExecution.SymbolicExpression)
                    {
                        // Convert from Types.SymbolicExpression to SymbolicExecution.SymbolicExpression
                        result.Add(SymbolicExpressionConverter.ToSymbolicExpression(simplified));
                    }
                    // If the original was a PathConstraint, convert back
                    else if (i < originalConstraints.Count && originalConstraints[i] is PathConstraint pathConstraint)
                    {
                        // Create a new PathConstraint with the simplified expression
                        result.Add(new PathConstraint(simplified, pathConstraint.InstructionPointer));
                    }
                    else
                    {
                        // Otherwise, keep as Types.SymbolicExpression
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
