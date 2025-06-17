using Neo.SmartContract.Fuzzer.Solvers;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Linq;

using TypesSymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using FuzzerSymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.SymbolicExpression;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// A simple implementation of a constraint solver for symbolic execution.
    /// Uses Z3 solver for constraint solving.
    /// </summary>
    public class SimpleConstraintSolver : Interfaces.IConstraintSolver, IDisposable
    {
        private readonly Z3Solver _z3Solver;
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
                // Convert PathConstraint to SymbolicExpression
                var convertedConstraints = constraints.Select(c => SymbolicExpressionConverter.ToSymbolicExpression(c.Expression)).ToList();

                // Check if the constraints are satisfiable
                if (!IsSatisfiable(convertedConstraints))
                {
                    solution = new Dictionary<string, object>();
                    return false;
                }

                // Solve the constraints
                solution = Solve(convertedConstraints);
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
        /// Random number generator for generating test values.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConstraintSolver"/> class.
        /// </summary>
        public SimpleConstraintSolver()
        {
            _random = new Random();
            _z3Solver = new Z3Solver();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConstraintSolver"/> class with a specified seed.
        /// </summary>
        /// <param name="seed">The seed for the random number generator.</param>
        public SimpleConstraintSolver(int seed)
        {
            _random = new Random(seed);
            _z3Solver = new Z3Solver();
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
                // Convert PathConstraint to SymbolicExpression
                var convertedConstraints = constraints.Select(c => SymbolicExpressionConverter.ToSymbolicExpression(c.Expression)).ToList();

                // Use the other IsSatisfiable method that takes SymbolicExpression
                return IsSatisfiable(convertedConstraints);
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
        public bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                // Convert SymbolicExpression to Types.SymbolicExpression
                var typesConstraints = constraints.Select(c => c.ToTypesExpression()).ToList();

                // Use the Z3 solver to check satisfiability
                return _z3Solver.IsSatisfiable(typesConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking satisfiability: {ex.Message}");
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
                    if (constraint is SymbolicExpression symbolicExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(symbolicExpr);
                    }
                    else if (constraint is Types.SymbolicExpression typesExpr)
                    {
                        // Convert from Types.SymbolicExpression to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(typesExpr));
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Convert from PathConstraint to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(pathConstraint.Expression));
                    }
                }

                // If no constraints, it's trivially satisfiable
                if (!convertedConstraints.Any())
                    return true;

                // Use the other IsSatisfiable method that takes SymbolicExpression
                return IsSatisfiable(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking satisfiability of constraints: {ex.Message}");
                // If there's an error, conservatively assume the constraints might be satisfiable
                return true;
            }
        }

        /// <summary>
        /// Determines if two constraints are contradictory.
        /// </summary>
        /// <param name="a">The first constraint.</param>
        /// <param name="b">The second constraint.</param>
        /// <returns>True if the constraints are contradictory, false otherwise.</returns>
        private bool AreContradictory(PathConstraint a, PathConstraint b)
        {
            // If either constraint is inactive, they can't be contradictory
            if (!a.IsActive || !b.IsActive)
                return false;

            try
            {
                // Convert to SymbolicExpression
                var exprA = SymbolicExpressionConverter.ToSymbolicExpression(a.Expression);
                var exprB = SymbolicExpressionConverter.ToSymbolicExpression(b.Expression);

                // Check if the conjunction of the two constraints is unsatisfiable
                var conjunction = new List<SymbolicExpression> { exprA, exprB };
                return !IsSatisfiable(conjunction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking contradiction: {ex.Message}");
                // If there's an error, conservatively assume the constraints are not contradictory
                return false;
            }
        }

        /// <summary>
        /// Comparer for SymbolicVariable objects based on their name
        /// </summary>
        private class SymbolicVariableComparer : IEqualityComparer<SymbolicVariable>
        {
            public bool Equals(SymbolicVariable? x, SymbolicVariable? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(SymbolicVariable obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        /// <summary>
        /// Determines if two symbolic expressions are equal.
        /// </summary>
        /// <param name="a">The first expression.</param>
        /// <param name="b">The second expression.</param>
        /// <returns>True if the expressions are equal, false otherwise.</returns>
        private bool ExpressionsEqual(SymbolicValue a, SymbolicValue b)
        {
            // Check for reference equality first
            if (ReferenceEquals(a, b))
                return true;

            // If either is null, they're not equal
            if (a == null || b == null)
                return false;

            // Check for constant values
            if (a is ConstantValue constA && b is ConstantValue constB)
            {
                // Compare the constant values
                if (constA.Value == null && constB.Value == null)
                    return true;

                if (constA.Value == null || constB.Value == null)
                    return false;

                return constA.Value.Equals(constB.Value);
            }

            // Check for symbolic variables
            if (a is SymbolicVariable varA && b is SymbolicVariable varB)
            {
                // Compare variable names
                return varA.Name == varB.Name;
            }

            // Check for symbolic expressions
            if (a is SymbolicExpression exprA && b is SymbolicExpression exprB)
            {
                // Compare operators
                if (exprA.Operator != exprB.Operator)
                    return false;

                // Compare left and right operands recursively
                bool leftEqual = ExpressionsEqual(exprA.Left, exprB.Left);

                // For unary operations, we're done
                if (exprA.Right == null && exprB.Right == null)
                    return leftEqual;

                // For binary operations, check right operands too
                bool rightEqual = ExpressionsEqual(exprA.Right, exprB.Right);

                return leftEqual && rightEqual;
            }

            // For different types, use string representation as a fallback
            return a.ToString() == b.ToString();
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
                // Filter out inactive constraints
                var activeConstraints = constraints.Where(c => c.IsActive).ToList();

                // If there are no active constraints, return an empty solution
                if (!activeConstraints.Any())
                    return new Dictionary<string, object>();

                // Convert PathConstraint to SymbolicExpression
                var convertedConstraints = activeConstraints
                    .Select(c => SymbolicExpressionConverter.ToSymbolicExpression(c.Expression))
                    .ToList();

                // Use the other Solve method that takes SymbolicExpression
                return Solve(convertedConstraints);
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
        public Dictionary<string, object> Solve(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                // Convert SymbolicExpression to Types.SymbolicExpression
                var typesConstraints = constraints.Select(c => c.ToTypesExpression()).ToList();

                // Use the Z3 solver to solve the constraints
                return _z3Solver.Solve(typesConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving constraints: {ex.Message}");
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
                    if (constraint is SymbolicExpression symbolicExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(symbolicExpr);
                    }
                    else if (constraint is Types.SymbolicExpression typesExpr)
                    {
                        // Convert from Types.SymbolicExpression to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(typesExpr));
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Convert from PathConstraint to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(pathConstraint.Expression));
                    }
                }

                // If no constraints, return an empty solution
                if (!convertedConstraints.Any())
                    return new Dictionary<string, object>();

                // Check if the constraints are satisfiable first
                if (!IsSatisfiable(convertedConstraints))
                    return new Dictionary<string, object>();

                // Use the other Solve method that takes SymbolicExpression
                return Solve(convertedConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving constraints: {ex.Message}");
                // If there's an error, return an empty dictionary
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Gathers all symbolic variables from the constraints.
        /// </summary>
        /// <param name="constraints">The constraints to analyze.</param>
        /// <returns>A set of symbolic variables.</returns>
        private HashSet<SymbolicVariable> GatherVariables(IEnumerable<PathConstraint> constraints)
        {
            var variables = new HashSet<SymbolicVariable>(new SymbolicVariableComparer());

            foreach (var constraint in constraints)
            {
                if (constraint.IsActive && constraint.Expression != null)
                {
                    // Convert to SymbolicExpression and extract variables
                    var expr = SymbolicExpressionConverter.ToSymbolicExpression(constraint.Expression);
                    var exprVariables = ExtractVariables(expr);

                    // Add to the set of variables
                    foreach (var variable in exprVariables)
                    {
                        variables.Add(variable);
                    }
                }
            }

            return variables;
        }

        /// <summary>
        /// Gathers all symbolic variables from the constraints.
        /// </summary>
        /// <param name="constraints">The constraints to analyze.</param>
        /// <returns>A set of symbolic variables.</returns>
        private HashSet<SymbolicVariable> GatherVariables(IEnumerable<SymbolicExpression> constraints)
        {
            var variables = new HashSet<SymbolicVariable>(new SymbolicVariableComparer());

            foreach (var constraint in constraints)
            {
                if (constraint != null)
                {
                    // Extract variables from the expression
                    var exprVariables = ExtractVariables(constraint);

                    // Add to the set of variables
                    foreach (var variable in exprVariables)
                    {
                        variables.Add(variable);
                    }
                }
            }

            return variables;
        }

        /// <summary>
        /// Extracts all unique symbolic variables referenced within a symbolic expression.
        /// </summary>
        /// <param name="value">The expression to analyze.</param>
        /// <returns>A set of symbolic variables.</returns>
        public HashSet<SymbolicVariable> ExtractVariables(SymbolicValue? value)
        {
            var variables = new HashSet<SymbolicVariable>(new SymbolicVariableComparer());

            if (value == null)
                return variables;

            // If the value is a symbolic variable, add it to the set
            if (value is SymbolicVariable variable)
            {
                variables.Add(variable);
                return variables;
            }

            // If the value is a symbolic expression, extract variables from both sides
            if (value is SymbolicExpression symExpr)
            {
                var leftVariables = ExtractVariables(symExpr.Left);

                foreach (var leftVar in leftVariables)
                    variables.Add(leftVar);

                if (symExpr.Right != null)
                {
                    var rightVariables = ExtractVariables(symExpr.Right);

                    foreach (var rightVar in rightVariables)
                        variables.Add(rightVar);
                }

                return variables;
            }

            // For other types of symbolic values, return an empty set
            return variables;
        }

        /// <summary>
        /// Generates a random string of the specified length.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns>A random string.</returns>
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<PathConstraint> constraints)
        {
            try
            {
                // Filter out inactive constraints
                var activeConstraints = constraints.Where(c => c.IsActive).ToList();

                // If there are no active constraints, nothing to do
                if (!activeConstraints.Any())
                    return;

                // Convert PathConstraint to SymbolicExpression
                var convertedConstraints = activeConstraints
                    .Select(c => SymbolicExpressionConverter.ToSymbolicExpression(c.Expression))
                    .ToList();

                // Use the other UpdateConstraints method that takes SymbolicExpression
                UpdateConstraints(convertedConstraints);
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
        public void UpdateConstraints(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                // Convert SymbolicExpression to Types.SymbolicExpression
                var typesConstraints = constraints.Select(c => c.ToTypesExpression()).ToList();

                // Use the Z3 solver to update constraints
                _z3Solver.UpdateConstraints(typesConstraints);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating constraints: {ex.Message}");
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
                    if (constraint is SymbolicExpression symbolicExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(symbolicExpr);
                    }
                    else if (constraint is Types.SymbolicExpression typesExpr)
                    {
                        // Convert from Types.SymbolicExpression to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(typesExpr));
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Convert from PathConstraint to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(pathConstraint.Expression));
                    }
                }

                // If no constraints, nothing to do
                if (!convertedConstraints.Any())
                    return;

                // Use the other UpdateConstraints method that takes SymbolicExpression
                UpdateConstraints(convertedConstraints);
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
                var result = new List<PathConstraint>();
                var constraintsList = constraints.ToList();

                // Filter out inactive constraints
                var activeConstraints = constraintsList.Where(c => c.IsActive).ToList();

                // Add inactive constraints directly to the result
                result.AddRange(constraintsList.Where(c => !c.IsActive));

                // If there are no active constraints, return the result
                if (!activeConstraints.Any())
                    return result;

                // Filter out tautologies (constraints that are always satisfied)
                var nonTautologies = activeConstraints.Where(c => !IsAlwaysSatisfied(c)).ToList();

                // Filter out subsumed constraints
                var nonSubsumed = new List<PathConstraint>();
                foreach (var constraint in nonTautologies)
                {
                    bool isSubsumed = false;
                    foreach (var other in nonTautologies)
                    {
                        if (constraint != other && IsSubsumedBy(constraint, other))
                        {
                            isSubsumed = true;
                            break;
                        }
                    }

                    if (!isSubsumed)
                        nonSubsumed.Add(constraint);
                }

                // Use the Z3 solver to simplify each expression
                foreach (var constraint in nonSubsumed)
                {
                    if (constraint.Expression is TypesSymbolicExpression typesExpr)
                    {
                        // Use the Z3 solver to simplify the expression
                        // Since Z3Solver doesn't have a SimplifyExpression method, we'll use Simplify on a list with one expression
                        var typesExprList = new List<Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression>();
                        typesExprList.Add(typesExpr);
                        var simplifiedList = _z3Solver.Simplify(typesExprList);
                        var simplified = simplifiedList.FirstOrDefault() ?? typesExpr;

                        // Create a new PathConstraint with the simplified expression
                        result.Add(new PathConstraint(simplified, constraint.InstructionPointer));
                    }
                    else
                    {
                        // If we can't simplify, add the original constraint
                        result.Add(constraint);
                    }
                }

                return result;
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
        public IEnumerable<SymbolicExpression> Simplify(IEnumerable<SymbolicExpression> constraints)
        {
            try
            {
                // Convert SymbolicExpression to Types.SymbolicExpression
                var typesConstraints = constraints.Select(c => c.ToTypesExpression()).ToList();

                // Use the Z3 solver to simplify constraints
                var simplifiedTypesConstraints = _z3Solver.Simplify(typesConstraints);

                // Convert back to SymbolicExpression
                return simplifiedTypesConstraints.Select(c => SymbolicExpressionConverter.ToSymbolicExpression(c)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simplifying constraints: {ex.Message}");
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
                    if (constraint is SymbolicExpression symbolicExpr)
                    {
                        // Already in the right format
                        convertedConstraints.Add(symbolicExpr);
                    }
                    else if (constraint is Types.SymbolicExpression typesExpr)
                    {
                        // Convert from Types.SymbolicExpression to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(typesExpr));
                    }
                    else if (constraint is PathConstraint pathConstraint)
                    {
                        // Convert from PathConstraint to SymbolicExpression
                        convertedConstraints.Add(SymbolicExpressionConverter.ToSymbolicExpression(pathConstraint.Expression));
                    }
                }

                // If no constraints, return the original constraints
                if (!convertedConstraints.Any())
                    return constraints;

                // Use the other Simplify method that takes SymbolicExpression
                var simplifiedExpressions = Simplify(convertedConstraints).ToList();

                // Convert back to the original type if possible
                var result = new List<object>();
                for (int i = 0; i < simplifiedExpressions.Count; i++)
                {
                    var simplified = simplifiedExpressions[i];

                    // If the original was a PathConstraint, convert back
                    if (i < originalConstraints.Count && originalConstraints[i] is PathConstraint pathConstraint)
                    {
                        // Convert from SymbolicExpression to Types.SymbolicExpression
                        var typesExpr = simplified.ToTypesExpression();

                        // Create a new PathConstraint with the simplified expression
                        result.Add(new PathConstraint(typesExpr, pathConstraint.InstructionPointer));
                    }
                    // If the original was a Types.SymbolicExpression, convert back
                    else if (i < originalConstraints.Count && originalConstraints[i] is Types.SymbolicExpression)
                    {
                        // Convert from SymbolicExpression to Types.SymbolicExpression
                        result.Add(simplified.ToTypesExpression());
                    }
                    else
                    {
                        // Otherwise, keep as SymbolicExpression
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

        /// <summary>
        /// Checks if a constraint is always satisfied (tautology)
        /// </summary>
        private bool IsAlwaysSatisfied(PathConstraint constraint)
        {
            if (!constraint.IsActive)
                return true;

            if (constraint.Expression is SymbolicBinaryExpression binaryExpr)
            {
                // Check for simple tautologies like (x == x)
                if (binaryExpr.Operator == Types.Operator.Equal &&
                    ExpressionsEqual(binaryExpr.Left, binaryExpr.Right))
                {
                    return true;
                }

                // Check for constant expressions that are always true
                if (binaryExpr.Left is ConstantValue c1 && binaryExpr.Right is ConstantValue c2)
                {
                    if (c1.Value is long v1 && c2.Value is long v2)
                    {
                        var op = binaryExpr.Operator;
                        if (op == Types.Operator.Equal)
                            return v1 == v2;
                        else if (op == Types.Operator.NotEqual)
                            return v1 != v2;
                        else if (op == Types.Operator.GreaterThan)
                            return v1 > v2;
                        else if (op == Types.Operator.GreaterThanOrEqual)
                            return v1 >= v2;
                        else if (op == Types.Operator.LessThan)
                            return v1 < v2;
                        else if (op == Types.Operator.LessThanOrEqual)
                            return v1 <= v2;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if one constraint is subsumed by another
        /// </summary>
        private bool IsSubsumedBy(PathConstraint a, PathConstraint b)
        {
            // If either constraint is inactive, no subsumption
            if (!a.IsActive || !b.IsActive)
                return false;

            if (a.Expression is TypesSymbolicExpression exprA && b.Expression is TypesSymbolicExpression exprB)
            {
                return IsSymbolicExpressionSubsumedBy(exprA, exprB);
            }

            return false;
        }

        /// <summary>
        /// Checks if a symbolic expression is always true
        /// </summary>
        private bool IsAlwaysTrue(SymbolicValue expr)
        {
            try
            {
                // Check for constant true value
                if (expr is ConstantValue constVal)
                {
                    if (constVal.Value is bool boolVal)
                        return boolVal;

                    if (constVal.Value is long longVal)
                        return longVal != 0;

                    // For other types, conservatively return false
                    return false;
                }

                // Check for binary expressions that are always true
                if (expr is TypesSymbolicExpression symExpr)
                {
                    // Check for equality of identical expressions (x == x)
                    if (symExpr.Operator.Equals(Types.Operator.Equal) &&
                        ExpressionsEqual(symExpr.Left, symExpr.Right))
                    {
                        return true;
                    }

                    // Check for inequality of different constants (1 != 2)
                    if (symExpr.Operator.Equals(Types.Operator.NotEqual) &&
                        symExpr.Left is ConstantValue leftConst &&
                        symExpr.Right is ConstantValue rightConst)
                    {
                        if (leftConst.Value is long leftVal && rightConst.Value is long rightVal)
                            return leftVal != rightVal;
                    }

                    // Check for other constant expressions that are always true
                    if (symExpr.Left is ConstantValue c1 && symExpr.Right is ConstantValue c2)
                    {
                        if (c1.Value is long v1 && c2.Value is long v2)
                        {
                            var op = symExpr.Operator;
                            if (op.Equals(Types.Operator.Equal))
                                return v1 == v2;
                            else if (op.Equals(Types.Operator.NotEqual))
                                return v1 != v2;
                            else if (op.Equals(Types.Operator.GreaterThan))
                                return v1 > v2;
                            else if (op.Equals(Types.Operator.GreaterThanOrEqual))
                                return v1 >= v2;
                            else if (op.Equals(Types.Operator.LessThan))
                                return v1 < v2;
                            else if (op.Equals(Types.Operator.LessThanOrEqual))
                                return v1 <= v2;
                        }
                    }

                    // For more complex expressions, use the Z3 solver
                    return _z3Solver.IsAlwaysTrue(symExpr);
                }

                // If we can't determine, conservatively return false
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if expression is always true: {ex.Message}");
                // If there's an error, conservatively return false
                return false;
            }
        }

        /// <summary>
        /// Checks if one symbolic expression is subsumed by another
        /// </summary>
        private bool IsSymbolicExpressionSubsumedBy(TypesSymbolicExpression expr1, TypesSymbolicExpression expr2)
        {
            try
            {
                // Check if they operate on the same variables
                if (!ExpressionsEqual(expr1.Left, expr2.Left))
                    return false;

                // Check for subsumption based on operator and constants
                if (expr1.Operator.Equals(expr2.Operator) &&
                    expr1.Right is ConstantValue constA && expr2.Right is ConstantValue constB)
                {
                    if (constA.Value is long valueA && constB.Value is long valueB)
                    {
                        var op = expr1.Operator;
                        if (op.Equals(Types.Operator.GreaterThan) || op.Equals(Types.Operator.GreaterThanOrEqual))
                        {
                            // (x > 5) is subsumed by (x > 10)
                            return valueA < valueB;
                        }
                        else if (op.Equals(Types.Operator.LessThan) || op.Equals(Types.Operator.LessThanOrEqual))
                        {
                            // (x < 20) is subsumed by (x < 10)
                            return valueA > valueB;
                        }
                        else if (op.Equals(Types.Operator.Equal))
                        {
                            // (x == 5) is not subsumed by (x == 10)
                            return valueA == valueB;
                        }
                        else if (op.Equals(Types.Operator.NotEqual))
                        {
                            // (x != 5) is not subsumed by (x != 10)
                            return valueA == valueB;
                        }
                    }
                }

                // For more complex expressions, use the Z3 solver
                // Check if (expr1 => expr2) is always true
                return _z3Solver.IsImplication(expr1, expr2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if expression is subsumed: {ex.Message}");
                // If there's an error, conservatively return false
                return false;
            }
        }

        /// <summary>
        /// Disposes resources used by the constraint solver.
        /// </summary>
        public void Dispose()
        {
            _z3Solver.Dispose();
        }
    }
}
