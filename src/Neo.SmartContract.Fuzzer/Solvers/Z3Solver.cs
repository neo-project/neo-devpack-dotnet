using Microsoft.Z3;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Constraint solver implementation using the Z3 theorem prover.
    /// </summary>
    public class Z3Solver : IConstraintSolver, IDisposable
    {
        private readonly Context _context;
        private readonly Dictionary<string, Expr> _variableCache;

        public Z3Solver()
        {
            _context = new Context(new Dictionary<string, string>() { { "model", "true" } });
            _variableCache = new Dictionary<string, Expr>();
        }

        /// <summary>
        /// Checks if the given path constraints are satisfiable.
        /// </summary>
        /// <param name="constraints">The collection of symbolic expressions representing the path constraints.</param>
        /// <returns>True if the constraints are satisfiable, false otherwise.</returns>
        public bool IsSatisfiable(IEnumerable<SymbolicExpression> constraints)
        {
            var solver = _context.MkSolver();

            foreach (var constraint in constraints)
            {
                try
                {
                    var z3Expr = TranslateExpression(constraint);
                    if (z3Expr != null)
                    {
                        solver.Assert((BoolExpr)z3Expr);
                    }
                    else
                    {
                        // Handle cases where translation might fail or result in null
                        Console.WriteLine($"Warning: Z3 translation failed for constraint: {constraint}");
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

        /// <summary>
        /// Solves the specified constraints and returns a model of variable assignments.
        /// </summary>
        /// <param name="constraints">The constraints to solve.</param>
        /// <returns>A dictionary mapping variable names to concrete values that satisfy the constraints.</returns>
        public Dictionary<string, object> Solve(IEnumerable<SymbolicExpression> constraints)
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

                    var z3Expr = TranslateExpression(constraint);
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

            foreach (var variable in variables.Values)
            {
                if (_variableCache.TryGetValue(variable.Name, out var expr))
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

        private Expr? TranslateExpression(SymbolicValue value)
        {
            if (value is ConcreteValue<int> intVal)
            {
                return _context.MkInt(intVal.Value);
            }
            else if (value is ConcreteValue<long> longVal)
            {
                return _context.MkInt(longVal.Value);
            }
            else if (value is ConcreteValue<BigInteger> bigIntVal)
            {
                // Handle BigInteger values
                return _context.MkInt(bigIntVal.Value.ToString());
            }
            else if (value is ConcreteValue<bool> boolVal)
            {
                return boolVal.Value ? _context.MkTrue() : _context.MkFalse();
            }
            else if (value is ConcreteValue<byte[]> byteArrayVal)
            {
                // Handle byte arrays by converting to a string
                return TranslateByteArray(byteArrayVal.Value);
            }
            else if (value is ConcreteValue<UInt160> uint160Val)
            {
                // Handle UInt160 by converting to string for now
                return _context.MkString(uint160Val.Value.ToString());
            }
            else if (value is ConcreteValue<UInt256> uint256Val)
            {
                // Handle UInt256 by converting to string for now
                return _context.MkString(uint256Val.Value.ToString());
            }
            else if (value is ConcreteValue<string> stringVal)
            {
                // Handle strings
                return _context.MkString(stringVal.Value);
            }
            else if (value is SymbolicVariable symVar)
            {
                if (!_variableCache.TryGetValue(symVar.Name, out var varExpr))
                {
                    // Create appropriate type based on variable type
                    if (symVar.Type != Neo.VM.Types.StackItemType.Any)
                    {
                        varExpr = CreateTypedVariable(symVar.Name, symVar.Type);
                    }
                    else
                    {
                        // Assume int by default
                        varExpr = _context.MkIntConst(symVar.Name);
                    }

                    _variableCache[symVar.Name] = varExpr;
                }
                return varExpr;
            }
            else if (value is SymbolicExpression symExpr)
            {
                var left = TranslateExpression(symExpr.Left);
                if (left == null) return null;

                // Handle unary operations
                if (symExpr.Right == null)
                {
                    switch (symExpr.Operator)
                    {
                        case Operator.Not:
                            if (left is BoolExpr boolExpr)
                            {
                                return _context.MkNot(boolExpr);
                            }
                            // Try to convert to boolean if possible
                            try
                            {
                                var eqExpr = _context.MkEq(left, _context.MkInt(0));
                                return _context.MkNot(eqExpr);
                            }
                            catch
                            {
                                throw new InvalidOperationException("NOT operator requires boolean operand");
                            }

                        case Operator.Negate:
                            if (left is ArithExpr arithExpr)
                            {
                                return _context.MkUnaryMinus(arithExpr);
                            }
                            throw new InvalidOperationException("NEGATE operator requires arithmetic operand");

                        case Operator.BitwiseNot:
                            if (left is IntExpr intExpr)
                            {
                                // Bitwise NOT is equivalent to -x-1 in two's complement
                                return _context.MkSub(_context.MkUnaryMinus(intExpr), _context.MkInt(1));
                            }
                            throw new InvalidOperationException("BITWISE_NOT operator requires integer operand");

                        case Operator.Abs:
                            if (left is ArithExpr absExpr)
                            {
                                // abs(x) = if x >= 0 then x else -x
                                var condition = _context.MkGe(absExpr, _context.MkInt(0));
                                return _context.MkITE(condition, absExpr, _context.MkUnaryMinus(absExpr));
                            }
                            throw new InvalidOperationException("ABS operator requires arithmetic operand");

                        case Operator.Sign:
                            if (left is ArithExpr signExpr)
                            {
                                // sign(x) = if x > 0 then 1 else if x < 0 then -1 else 0
                                var positiveCondition = _context.MkGt(signExpr, _context.MkInt(0));
                                var negativeCondition = _context.MkLt(signExpr, _context.MkInt(0));
                                return _context.MkITE(
                                    positiveCondition,
                                    _context.MkInt(1),
                                    _context.MkITE(negativeCondition, _context.MkInt(-1), _context.MkInt(0))
                                );
                            }
                            throw new InvalidOperationException("SIGN operator requires arithmetic operand");

                        default:
                            throw new NotImplementedException($"Unary operator {symExpr.Operator} not implemented");
                    }
                }

                // Handle binary operations
                var right = TranslateExpression(symExpr.Right);
                if (right == null) return null;

                switch (symExpr.Operator)
                {
                    case Operator.Equal:
                        return _context.MkEq(left, right);

                    case Operator.NotEqual:
                        return _context.MkNot(_context.MkEq(left, right));

                    case Operator.Add:
                        if (left is ArithExpr leftArith && right is ArithExpr rightArith)
                        {
                            return _context.MkAdd(leftArith, rightArith);
                        }
                        throw new InvalidOperationException("ADD operator requires arithmetic operands");

                    case Operator.Subtract:
                        if (left is ArithExpr leftSub && right is ArithExpr rightSub)
                        {
                            return _context.MkSub(leftSub, rightSub);
                        }
                        throw new InvalidOperationException("SUBTRACT operator requires arithmetic operands");

                    case Operator.Multiply:
                        if (left is ArithExpr leftMul && right is ArithExpr rightMul)
                        {
                            return _context.MkMul(leftMul, rightMul);
                        }
                        throw new InvalidOperationException("MULTIPLY operator requires arithmetic operands");

                    case Operator.Divide:
                        if (left is ArithExpr leftDiv && right is ArithExpr rightDiv)
                        {
                            return _context.MkDiv(leftDiv, rightDiv);
                        }
                        throw new InvalidOperationException("DIVIDE operator requires arithmetic operands");

                    case Operator.Modulo:
                        if (left is ArithExpr leftMod && right is ArithExpr rightMod)
                        {
                            return _context.MkMod((IntExpr)leftMod, (IntExpr)rightMod);
                        }
                        throw new InvalidOperationException("MODULO operator requires integer operands");

                    case Operator.LessThan:
                        if (left is ArithExpr leftLt && right is ArithExpr rightLt)
                        {
                            return _context.MkLt(leftLt, rightLt);
                        }
                        throw new InvalidOperationException("LESS_THAN operator requires arithmetic operands");

                    case Operator.LessThanOrEqual:
                        if (left is ArithExpr leftLe && right is ArithExpr rightLe)
                        {
                            return _context.MkLe(leftLe, rightLe);
                        }
                        throw new InvalidOperationException("LESS_THAN_OR_EQUAL operator requires arithmetic operands");

                    case Operator.GreaterThan:
                        if (left is ArithExpr leftGt && right is ArithExpr rightGt)
                        {
                            return _context.MkGt(leftGt, rightGt);
                        }
                        throw new InvalidOperationException("GREATER_THAN operator requires arithmetic operands");

                    case Operator.GreaterThanOrEqual:
                        if (left is ArithExpr leftGe && right is ArithExpr rightGe)
                        {
                            return _context.MkGe(leftGe, rightGe);
                        }
                        throw new InvalidOperationException("GREATER_THAN_OR_EQUAL operator requires arithmetic operands");

                    case Operator.And:
                        if (left is BoolExpr leftAnd && right is BoolExpr rightAnd)
                        {
                            return _context.MkAnd(leftAnd, rightAnd);
                        }
                        throw new InvalidOperationException("AND operator requires boolean operands");

                    case Operator.Or:
                        if (left is BoolExpr leftOr && right is BoolExpr rightOr)
                        {
                            return _context.MkOr(leftOr, rightOr);
                        }
                        throw new InvalidOperationException("OR operator requires boolean operands");

                    case Operator.BitwiseAnd:
                        if (left is IntExpr leftBitAnd && right is IntExpr rightBitAnd)
                        {
                            // Z3 doesn't have direct bitwise operations for integers
                            // We can use the BV (bit vector) sort for this
                            var bvSize = 64u; // Use 64-bit vectors
                            var leftBv = _context.MkInt2BV(bvSize, leftBitAnd);
                            var rightBv = _context.MkInt2BV(bvSize, rightBitAnd);
                            var result = _context.MkBVAND(leftBv, rightBv);
                            return _context.MkBV2Int(result, true);
                        }
                        throw new InvalidOperationException("BITWISE_AND operator requires integer operands");

                    case Operator.BitwiseOr:
                        if (left is IntExpr leftBitOr && right is IntExpr rightBitOr)
                        {
                            var bvSize = 64u;
                            var leftBv = _context.MkInt2BV(bvSize, leftBitOr);
                            var rightBv = _context.MkInt2BV(bvSize, rightBitOr);
                            var result = _context.MkBVOR(leftBv, rightBv);
                            return _context.MkBV2Int(result, true);
                        }
                        throw new InvalidOperationException("BITWISE_OR operator requires integer operands");

                    case Operator.BitwiseXor:
                        if (left is IntExpr leftBitXor && right is IntExpr rightBitXor)
                        {
                            var bvSize = 64u;
                            var leftBv = _context.MkInt2BV(bvSize, leftBitXor);
                            var rightBv = _context.MkInt2BV(bvSize, rightBitXor);
                            var result = _context.MkBVXOR(leftBv, rightBv);
                            return _context.MkBV2Int(result, true);
                        }
                        throw new InvalidOperationException("BITWISE_XOR operator requires integer operands");

                    case Operator.LeftShift:
                        if (left is IntExpr leftShift && right is IntExpr rightShift)
                        {
                            var bvSize = 64u;
                            var leftBv = _context.MkInt2BV(bvSize, leftShift);
                            // Convert shift amount to BV
                            var rightBvExpr = _context.MkInt2BV(bvSize, rightShift);
                            var result = _context.MkBVSHL(leftBv, rightBvExpr);
                            return _context.MkBV2Int(result, true);
                        }
                        throw new InvalidOperationException("LEFT_SHIFT operator requires integer operands");

                    case Operator.RightShift:
                        if (left is IntExpr leftRShift && right is IntExpr rightRShift)
                        {
                            var bvSize = 64u;
                            var leftBv = _context.MkInt2BV(bvSize, leftRShift);
                            var rightBvExpr = _context.MkInt2BV(bvSize, rightRShift);
                            // Use logical shift right (unsigned)
                            var result = _context.MkBVLSHR(leftBv, rightBvExpr);
                            return _context.MkBV2Int(result, true);
                        }
                        throw new InvalidOperationException("RIGHT_SHIFT operator requires integer operands");

                    case Operator.Min:
                        if (left is ArithExpr leftMin && right is ArithExpr rightMin)
                        {
                            // min(x, y) = if x <= y then x else y
                            var condition = _context.MkLe(leftMin, rightMin);
                            return _context.MkITE(condition, leftMin, rightMin);
                        }
                        throw new InvalidOperationException("MIN operator requires arithmetic operands");

                    case Operator.Max:
                        if (left is ArithExpr leftMax && right is ArithExpr rightMax)
                        {
                            // max(x, y) = if x >= y then x else y
                            var condition = _context.MkGe(leftMax, rightMax);
                            return _context.MkITE(condition, leftMax, rightMax);
                        }
                        throw new InvalidOperationException("MAX operator requires arithmetic operands");

                    case Operator.Within:
                        if (left is ArithExpr x && right is ArithExpr y)
                        {
                            // We need a third parameter for within
                            // For now, we'll just check if x is between 0 and y
                            var lowerBound = _context.MkGe(x, _context.MkInt(0));
                            var upperBound = _context.MkLe(x, y);
                            return _context.MkAnd(lowerBound, upperBound);
                        }
                        throw new InvalidOperationException("WITHIN operator requires arithmetic operands");

                    default:
                        // Log the unsupported operator and return a default value
                        Console.WriteLine($"Warning: Binary operator {symExpr.Operator} not implemented, returning default value");
                        return _context.MkBool(true); // Default to true as a fallback
                }
            }

            // Default case for unsupported values
            Console.WriteLine($"Warning: Unsupported symbolic value type: {value.GetType().Name}");
            return null;
        }

        // Solver instance for maintaining state between calls
        private Solver? _statefulSolver;

        /// <summary>
        /// Updates the solver with new constraints.
        /// </summary>
        /// <param name="constraints">The constraints to update.</param>
        public void UpdateConstraints(IEnumerable<SymbolicExpression> constraints)
        {
            // Create a stateful solver if it doesn't exist
            _statefulSolver ??= _context.MkSolver();

            // Add each constraint to the solver
            foreach (var constraint in constraints)
            {
                try
                {
                    var z3Expr = TranslateExpression(constraint);
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
                    var z3Expr = TranslateExpression(constraint);
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
        /// Translates a byte array to a Z3 integer expression
        /// </summary>
        private Expr TranslateByteArray(byte[] bytes)
        {
            // Convert byte array to integer for simplicity
            // Use the first 8 bytes (or less) to create a 64-bit integer
            long value = 0;
            for (int i = 0; i < Math.Min(bytes.Length, 8); i++)
            {
                value = (value << 8) | bytes[i];
            }
            return _context.MkInt(value);
        }

        /// <summary>
        /// Creates a variable with the appropriate Z3 type based on the Neo VM type
        /// </summary>
        private Expr CreateTypedVariable(string name, Neo.VM.Types.StackItemType type)
        {
            switch (type)
            {
                case Neo.VM.Types.StackItemType.Integer:
                    return _context.MkIntConst(name);
                case Neo.VM.Types.StackItemType.Boolean:
                    return _context.MkBoolConst(name);
                case Neo.VM.Types.StackItemType.ByteString:
                    return _context.MkIntConst(name); // Use int as a simplification
                case Neo.VM.Types.StackItemType.Buffer:
                    return _context.MkIntConst(name); // Use int as a simplification
                case Neo.VM.Types.StackItemType.Array:
                    // For arrays, we use integers as a simplification
                    return _context.MkIntConst(name);
                case Neo.VM.Types.StackItemType.Map:
                    // For maps, we use integers as a simplification
                    return _context.MkIntConst(name);
                case Neo.VM.Types.StackItemType.Struct:
                    // For structs, we use integers as a simplification
                    return _context.MkIntConst(name);
                case Neo.VM.Types.StackItemType.Pointer:
                    // For pointers, we use integers
                    return _context.MkIntConst(name);
                case Neo.VM.Types.StackItemType.InteropInterface:
                    // For interop interfaces, we use integers as a simplification
                    return _context.MkIntConst(name);
                default:
                    // Default to int for unknown types
                    return _context.MkIntConst(name);
            }
        }

        /// <summary>
        /// Checks if a symbolic expression is always true
        /// </summary>
        /// <param name="expr">The expression to check</param>
        /// <returns>True if the expression is always true, false otherwise</returns>
        public bool IsAlwaysTrue(SymbolicExpression expr)
        {
            try
            {
                var solver = _context.MkSolver();

                // Translate the expression to Z3
                var z3Expr = TranslateExpression(expr);
                if (z3Expr == null)
                    return false;

                // Check if the negation is unsatisfiable
                var negation = _context.MkNot((BoolExpr)z3Expr);
                solver.Assert(negation);

                // If the negation is unsatisfiable, the expression is always true
                return solver.Check() == Status.UNSATISFIABLE;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if expression is always true: {ex.Message}");
                // If there's an error, conservatively return false
                return false;
            }
        }

        /// <summary>
        /// Checks if one expression implies another
        /// </summary>
        /// <param name="antecedent">The antecedent expression</param>
        /// <param name="consequent">The consequent expression</param>
        /// <returns>True if antecedent implies consequent, false otherwise</returns>
        public bool IsImplication(SymbolicExpression antecedent, SymbolicExpression consequent)
        {
            try
            {
                var solver = _context.MkSolver();

                // Translate the expressions to Z3
                var z3Antecedent = TranslateExpression(antecedent);
                var z3Consequent = TranslateExpression(consequent);

                if (z3Antecedent == null || z3Consequent == null)
                    return false;

                // Check if (antecedent AND NOT consequent) is unsatisfiable
                // This is equivalent to checking if (antecedent => consequent) is valid
                var implication = _context.MkImplies((BoolExpr)z3Antecedent, (BoolExpr)z3Consequent);
                var negation = _context.MkNot(implication);
                solver.Assert(negation);

                // If the negation is unsatisfiable, the implication is valid
                return solver.Check() == Status.UNSATISFIABLE;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking implication: {ex.Message}");
                // If there's an error, conservatively return false
                return false;
            }
        }

        public void Dispose()
        {
            _statefulSolver?.Dispose();
            _context.Dispose();
        }
    }
}
