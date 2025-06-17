using Microsoft.Z3;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// A unified constraint solver implementation using the Z3 theorem prover.
    /// This combines the best parts of SimpleConstraintSolver and DefaultConstraintSolver.
    /// </summary>
    public class UnifiedConstraintSolver : IConstraintSolver, IDisposable
    {
        private readonly Context _context;
        private readonly ExpressionTranslator _translator;
        private readonly Random _random;
        private Solver? _statefulSolver;

        /// <summary>
        /// Initializes a new instance of the UnifiedConstraintSolver class.
        /// </summary>
        public UnifiedConstraintSolver()
        {
            _context = new Context(new Dictionary<string, string>() { { "model", "true" } });
            _translator = new ExpressionTranslator(_context);
            _random = new Random();
        }

        /// <summary>
        /// Initializes a new instance of the UnifiedConstraintSolver class with a specified seed.
        /// </summary>
        /// <param name="seed">The seed for the random number generator.</param>
        public UnifiedConstraintSolver(int seed)
        {
            _context = new Context(new Dictionary<string, string>() { { "model", "true" } });
            _translator = new ExpressionTranslator(_context);
            _random = new Random(seed);
        }

        /// <summary>
        /// Checks if the given set of constraints is satisfiable.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                var solver = _context.MkSolver();

                foreach (var constraint in constraints)
                {
                    try
                    {
                        var z3Expr = _translator.Translate(constraint);
                        if (z3Expr != null)
                        {
                            solver.Assert((BoolExpr)z3Expr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error translating constraint {constraint}: {ex.Message}");
                        // If we can't translate a constraint, we conservatively assume the path might be satisfiable
                    }
                }

                return solver.Check() == Status.SATISFIABLE;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking satisfiability: {ex.Message}");
                // If there's an error, conservatively assume the constraints might be satisfiable
                return true;
            }
        }

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                var solver = _context.MkSolver();

                // Track variables for model extraction
                var variables = new Dictionary<string, SymbolicVariable>();

                foreach (var constraint in constraints)
                {
                    try
                    {
                        // Collect variables used in this constraint
                        CollectVariables(constraint, variables);

                        var z3Expr = _translator.Translate(constraint);
                        if (z3Expr != null)
                        {
                            solver.Assert((BoolExpr)z3Expr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error translating constraint {constraint}: {ex.Message}");
                    }
                }

                if (solver.Check() != Status.SATISFIABLE)
                {
                    return new Dictionary<string, object>();
                }

                var model = solver.Model;
                var result = new Dictionary<string, object>();
                var variableCache = _translator.GetVariableCache();

                foreach (var variable in variables.Values)
                {
                    if (variableCache.TryGetValue(variable.Name, out var expr))
                    {
                        try
                        {
                            var interpretation = model.Eval(expr, true);
                            if (interpretation is IntNum intNum)
                            {
                                result[variable.Name] = intNum.Int64;
                            }
                            else if (interpretation is BoolExpr boolExpr)
                            {
                                result[variable.Name] = boolExpr.BoolValue == Z3_lbool.Z3_L_TRUE;
                            }
                            // Handle other types as needed
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error extracting model value for {variable.Name}: {ex.Message}");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving constraints: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                // Create a stateful solver if it doesn't exist
                _statefulSolver ??= _context.MkSolver();

                // Add each constraint to the solver
                foreach (var constraint in constraints)
                {
                    try
                    {
                        var z3Expr = _translator.Translate(constraint);
                        if (z3Expr != null)
                        {
                            _statefulSolver.Assert((BoolExpr)z3Expr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating constraint {constraint}: {ex.Message}");
                    }
                }
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
        public IEnumerable<SymbolicExpression> Simplify(IEnumerable<SymbolicExpression> constraints)
        {
            var result = new List<SymbolicExpression>();
            var processed = new HashSet<string>(); // Track processed constraints by their string representation

            foreach (var constraint in constraints)
            {
                try
                {
                    // Convert to Z3 expression and simplify
                    var z3Expr = _translator.Translate(constraint);
                    if (z3Expr != null)
                    {
                        var simplified = z3Expr.Simplify();

                        // Skip tautologies (always true)
                        if (simplified.Equals(_context.MkTrue()))
                            continue;

                        // Skip contradictions (always false) - these should be caught by IsSatisfiable
                        if (simplified.Equals(_context.MkFalse()))
                            continue;

                        // Convert back to a string representation for deduplication
                        string exprString = simplified.ToString();
                        if (processed.Add(exprString))
                        {
                            // Keep the original constraint in the result
                            result.Add(constraint);
                        }
                    }
                    else
                    {
                        // If we can't translate, keep the original constraint
                        result.Add(constraint);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error simplifying constraint {constraint}: {ex.Message}");
                    // Keep the original constraint if simplification fails
                    result.Add(constraint);
                }
            }

            return result;
        }

        /// <summary>
        /// Disposes resources used by the constraint solver.
        /// </summary>
        public void Dispose()
        {
            _statefulSolver?.Dispose();
            _context.Dispose();
        }

        // Helper method to collect variables from symbolic expressions
        private void CollectVariables(SymbolicValue value, Dictionary<string, SymbolicVariable> variables)
        {
            if (value is SymbolicVariable symVar)
            {
                if (!variables.ContainsKey(symVar.Name))
                {
                    variables[symVar.Name] = symVar;
                }
            }
            else if (value is SymbolicExpression symExpr)
            {
                CollectVariables(symExpr.Left, variables);
                if (symExpr.Right != null)
                {
                    CollectVariables(symExpr.Right, variables);
                }
            }
        }
    }
}
