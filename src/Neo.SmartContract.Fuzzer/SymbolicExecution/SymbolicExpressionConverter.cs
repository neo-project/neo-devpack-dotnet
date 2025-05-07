using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Utility class for converting between different SymbolicExpression types
    /// </summary>
    public static class SymbolicExpressionConverter
    {
        /// <summary>
        /// Converts a SymbolicExpression from the SymbolicExecution namespace to a SymbolicExpression from the Types namespace
        /// </summary>
        /// <param name="expr">The expression to convert</param>
        /// <returns>The converted expression</returns>
        public static Types.SymbolicExpression? ToTypesExpression(this SymbolicExpression? expr)
        {
            if (expr == null)
                return null;

            // Convert the left operand
            var leftValue = ToTypesValue(expr.Left);
            if (leftValue == null)
                throw new InvalidOperationException("Left operand cannot be null");

            // Handle unary operations
            if (expr.Right == null)
            {
                return new Types.SymbolicExpression(leftValue, ToTypesOperator(expr.Operator));
            }

            // Convert the right operand for binary operations
            var rightValue = ToTypesValue(expr.Right);
            if (rightValue == null)
                throw new InvalidOperationException("Right operand cannot be null");

            // Create and return the converted expression
            return new Types.SymbolicExpression(leftValue, ToTypesOperator(expr.Operator), rightValue);
        }

        /// <summary>
        /// Converts a SymbolicValue from the SymbolicExecution namespace to a SymbolicValue from the Types namespace
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public static Types.SymbolicValue? ToTypesValue(SymbolicValue? value)
        {
            if (value == null)
                return null;

            // Handle different types of symbolic values
            if (value is SymbolicVariable variable)
            {
                return new Types.SymbolicVariable(variable.Name ?? "unknown", variable.Type);
            }
            else if (value is SymbolicExpression expression)
            {
                return ToTypesExpression(expression);
            }
            else if (value is ConstantValue constant)
            {
                return new Types.ConstantValue(constant.Value, constant.Type);
            }

            // Default case - create a symbolic variable with the string representation
            return new Types.SymbolicVariable(value.ToString() ?? "unknown", value.Type);
        }

        /// <summary>
        /// Converts a PathConstraint to a SymbolicExpression from the Types namespace
        /// </summary>
        /// <param name="constraint">The constraint to convert</param>
        /// <returns>The converted expression</returns>
        public static Types.SymbolicExpression? ToTypesExpression(PathConstraint? constraint)
        {
            if (constraint == null)
                return null;

            return constraint.Expression;
        }

        /// <summary>
        /// Converts a SymbolicExpression from the Types namespace to a SymbolicExpression from the SymbolicExecution namespace
        /// </summary>
        /// <param name="expr">The expression to convert</param>
        /// <returns>The converted expression</returns>
        public static SymbolicExpression? ToSymbolicExpression(Types.SymbolicExpression? expr)
        {
            if (expr == null)
                return null;

            // Convert the left operand
            var leftValue = ToSymbolicValue(expr.Left);
            if (leftValue == null)
                throw new InvalidOperationException("Left operand cannot be null");

            // Handle unary operations
            if (expr.Right == null)
            {
                return new SymbolicExpression(leftValue, ToOperator(expr.Operator), null);
            }

            // Convert the right operand for binary operations
            var rightValue = ToSymbolicValue(expr.Right);
            if (rightValue == null)
                throw new InvalidOperationException("Right operand cannot be null");

            // Create and return the converted expression
            return new SymbolicExpression(leftValue, ToOperator(expr.Operator), rightValue);
        }

        /// <summary>
        /// Converts a SymbolicValue from the Types namespace to a SymbolicValue from the SymbolicExecution namespace
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public static SymbolicValue? ToSymbolicValue(Types.SymbolicValue? value)
        {
            if (value == null)
                return null;

            // Handle different types of symbolic values
            if (value is Types.SymbolicVariable variable)
            {
                return new SymbolicVariable(variable.Name ?? "unknown", variable.Type);
            }
            else if (value is Types.SymbolicExpression expression)
            {
                return ToSymbolicExpression(expression);
            }
            else if (value is Types.ConstantValue constant)
            {
                return new ConstantValue(constant.Value, Utils.TypeConverter.ToSymbolicType(constant.Type));
            }

            // Default case - create a symbolic variable with the string representation
            return new SymbolicVariable(value.ToString() ?? "unknown", SymbolicType.Any);
        }

        /// <summary>
        /// Creates a PathConstraint from a SymbolicExpression
        /// </summary>
        /// <param name="expr">The expression to convert</param>
        /// <param name="instructionPointer">The instruction pointer</param>
        /// <returns>The created path constraint</returns>
        public static PathConstraint? ToPathConstraint(Types.SymbolicExpression? expr, int instructionPointer = 0)
        {
            if (expr == null)
                return null;

            return new PathConstraint(expr, instructionPointer);
        }

        /// <summary>
        /// Creates a PathConstraint from a SymbolicExpression
        /// </summary>
        /// <param name="expr">The expression to convert</param>
        /// <param name="instructionPointer">The instruction pointer</param>
        /// <returns>The created path constraint</returns>
        public static PathConstraint? ToPathConstraint(SymbolicExpression? expr, int instructionPointer = 0)
        {
            if (expr == null)
                return null;

            // Convert the expression to a Types.SymbolicExpression
            var typesExpr = ToTypesExpression(expr);
            if (typesExpr == null)
                return null;

            // Create and return the path constraint
            return new PathConstraint(typesExpr, instructionPointer);
        }

        /// <summary>
        /// Converts a Types.Operator to a SymbolicExecution.Operator
        /// </summary>
        /// <param name="op">The operator to convert</param>
        /// <returns>The converted operator</returns>
        public static Operator ToOperator(Types.Operator op)
        {
            return op switch
            {
                Types.Operator.Add => Operator.Add,
                Types.Operator.Subtract => Operator.Subtract,
                Types.Operator.Multiply => Operator.Multiply,
                Types.Operator.Divide => Operator.Divide,
                Types.Operator.Modulo => Operator.Modulo,
                Types.Operator.Equal => Operator.Equal,
                Types.Operator.NotEqual => Operator.NotEqual,
                Types.Operator.LessThan => Operator.LessThan,
                Types.Operator.LessThanOrEqual => Operator.LessThanOrEqual,
                Types.Operator.GreaterThan => Operator.GreaterThan,
                Types.Operator.GreaterThanOrEqual => Operator.GreaterThanOrEqual,
                Types.Operator.And => Operator.And,
                Types.Operator.Or => Operator.Or,
                Types.Operator.Not => Operator.Not,
                Types.Operator.BitwiseAnd => Operator.BitwiseAnd,
                Types.Operator.BitwiseOr => Operator.BitwiseOr,
                Types.Operator.BitwiseXor => Operator.BitwiseXor,
                Types.Operator.BitwiseNot => Operator.BitwiseNot,
                Types.Operator.LeftShift => Operator.LeftShift,
                Types.Operator.RightShift => Operator.RightShift,
                Types.Operator.Negate => Operator.Negate,
                Types.Operator.Abs => Operator.Abs,
                Types.Operator.Sign => Operator.Sign,
                Types.Operator.Min => Operator.Min,
                Types.Operator.Max => Operator.Max,
                Types.Operator.Within => Operator.Within,
                _ => Operator.Identity
            };
        }

        /// <summary>
        /// Converts a SymbolicExecution.Operator to a Types.Operator
        /// </summary>
        /// <param name="op">The operator to convert</param>
        /// <returns>The converted operator</returns>
        public static Types.Operator ToTypesOperator(Operator op)
        {
            return op switch
            {
                Operator.Add => Types.Operator.Add,
                Operator.Subtract => Types.Operator.Subtract,
                Operator.Multiply => Types.Operator.Multiply,
                Operator.Divide => Types.Operator.Divide,
                Operator.Modulo => Types.Operator.Modulo,
                Operator.Equal => Types.Operator.Equal,
                Operator.NotEqual => Types.Operator.NotEqual,
                Operator.LessThan => Types.Operator.LessThan,
                Operator.LessThanOrEqual => Types.Operator.LessThanOrEqual,
                Operator.GreaterThan => Types.Operator.GreaterThan,
                Operator.GreaterThanOrEqual => Types.Operator.GreaterThanOrEqual,
                Operator.And => Types.Operator.And,
                Operator.Or => Types.Operator.Or,
                Operator.Not => Types.Operator.Not,
                Operator.BitwiseAnd => Types.Operator.BitwiseAnd,
                Operator.BitwiseOr => Types.Operator.BitwiseOr,
                Operator.BitwiseXor => Types.Operator.BitwiseXor,
                Operator.BitwiseNot => Types.Operator.BitwiseNot,
                Operator.LeftShift => Types.Operator.LeftShift,
                Operator.RightShift => Types.Operator.RightShift,
                Operator.Negate => Types.Operator.Negate,
                Operator.Abs => Types.Operator.Abs,
                Operator.Sign => Types.Operator.Sign,
                Operator.Min => Types.Operator.Min,
                Operator.Max => Types.Operator.Max,
                Operator.Within => Types.Operator.Within,
                _ => Types.Operator.Identity
            };
        }
    }
}
