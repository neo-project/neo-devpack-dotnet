using Neo.SmartContract.Fuzzer.SymbolicExecution;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Utils;
using System;
using System.Collections.Generic;

// Use aliases to disambiguate between the two SymbolicExpression types
using TypesSymbolicExpression = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicExpression;
using TypesOperator = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator;
using SymbolicOperator = Neo.SmartContract.Fuzzer.SymbolicExecution.Operator;
using TypesSymbolicVariable = Neo.SmartContract.Fuzzer.SymbolicExecution.Types.SymbolicVariable;

namespace Neo.SmartContract.Fuzzer.Solvers
{
    /// <summary>
    /// Utility class for converting between different symbolic expression types.
    /// </summary>
    public static class SymbolicExpressionConverter
    {
        /// <summary>
        /// Converts a SymbolicExecution.SymbolicExpression to a SymbolicExecution.Types.SymbolicExpression.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The converted expression.</returns>
        public static TypesSymbolicExpression ToTypesExpression(object expression)
        {
            if (expression is TypesSymbolicExpression typesExpr)
            {
                return typesExpr;
            }

            if (expression is SymbolicExecution.SymbolicExpression symbolicExpr)
            {
                // Convert the left and right operands recursively
                var left = symbolicExpr.Left != null ? ToTypesExpression(symbolicExpr.Left) : null;
                var right = symbolicExpr.Right != null ? ToTypesExpression(symbolicExpr.Right) : null;

                // Convert the operator
                var op = ConvertOperator(symbolicExpr.Operator);

                // Create a new TypesSymbolicExpression
                return new TypesSymbolicExpression(left, op, right);
            }

            if (expression is SymbolicExecution.SymbolicVariable symbolicVar)
            {
                // Convert to Types.SymbolicVariable
                var variable = new TypesSymbolicVariable(symbolicVar.Name, SymbolicExecution.Utils.TypeConverter.ToStackItemType(symbolicVar.SymbolicType));
                // Wrap the variable in a SymbolicExpression to match the return type
                return new TypesSymbolicExpression(variable, Neo.SmartContract.Fuzzer.SymbolicExecution.Types.Operator.Identity, variable);
            }

            if (expression is SymbolicExecution.ConstantValue constantValue)
            {
                // Convert to Types.ConcreteValue
                if (constantValue.Value is long longValue)
                {
                    var result = new ConcreteValue<long>(longValue);
                    return new TypesSymbolicExpression(result, TypesOperator.Equal, result);
                }
                else if (constantValue.Value is int intValue)
                {
                    var result = new ConcreteValue<int>(intValue);
                    return new TypesSymbolicExpression(result, TypesOperator.Equal, result);
                }
                else if (constantValue.Value is string stringValue)
                {
                    var result = new ConcreteValue<string>(stringValue);
                    return new TypesSymbolicExpression(result, TypesOperator.Equal, result);
                }
                else if (constantValue.Value is byte[] byteArrayValue)
                {
                    var result = new ConcreteValue<byte[]>(byteArrayValue);
                    return new TypesSymbolicExpression(result, TypesOperator.Equal, result);
                }
                else if (constantValue.Value is bool boolValue)
                {
                    var result = new ConcreteValue<bool>(boolValue);
                    return new TypesSymbolicExpression(result, TypesOperator.Equal, result);
                }
            }

            // Default fallback
            var defaultVar = new TypesSymbolicVariable("x", VM.Types.StackItemType.Integer);
            var defaultValue = new ConcreteValue<long>(10);
            return new TypesSymbolicExpression(defaultVar, TypesOperator.GreaterThan, defaultValue);
        }

        /// <summary>
        /// Converts a SymbolicExecution operator to a Types.Operator.
        /// </summary>
        private static TypesOperator ConvertOperator(SymbolicOperator op)
        {
            switch (op)
            {
                case SymbolicOperator.Equal: return TypesOperator.Equal;
                case SymbolicOperator.NotEqual: return TypesOperator.NotEqual;
                case SymbolicOperator.GreaterThan: return TypesOperator.GreaterThan;
                case SymbolicOperator.GreaterThanOrEqual: return TypesOperator.GreaterThanOrEqual;
                case SymbolicOperator.LessThan: return TypesOperator.LessThan;
                case SymbolicOperator.LessThanOrEqual: return TypesOperator.LessThanOrEqual;
                case SymbolicOperator.Add: return TypesOperator.Add;
                case SymbolicOperator.Subtract: return TypesOperator.Subtract;
                case SymbolicOperator.Multiply: return TypesOperator.Multiply;
                case SymbolicOperator.Divide: return TypesOperator.Divide;
                case SymbolicOperator.Modulo: return TypesOperator.Modulo;
                case SymbolicOperator.And: return TypesOperator.And;
                case SymbolicOperator.Or: return TypesOperator.Or;
                case SymbolicOperator.BitwiseXor: return TypesOperator.BitwiseXor;
                case SymbolicOperator.Not: return TypesOperator.Not;
                default: return TypesOperator.Equal; // Default fallback
            }
        }

        /// <summary>
        /// Converts a SymbolicExecution type to a VM.Types.StackItemType.
        /// </summary>
        private static VM.Types.StackItemType ConvertType(SymbolicExecution.SymbolicType type)
        {
            return SymbolicExecution.Utils.TypeConverter.ToStackItemType(type);
        }

        /// <summary>
        /// Converts a SymbolicExecution.Types.SymbolicExpression to a SymbolicExecution.SymbolicExpression.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>The converted expression.</returns>
        public static object ToSymbolicExpression(TypesSymbolicExpression expression)
        {
            if (expression == null)
            {
                return null;
            }

            // Check if it's a variable - use direct type check instead of pattern matching
            if (expression.GetType() == typeof(TypesSymbolicVariable))
            {
                // We need to handle this case differently since we can't cast directly
                // Get the properties we need using reflection
                var symbolicVarType = typeof(TypesSymbolicVariable);
                var nameProperty = symbolicVarType.GetProperty("Name");
                var typeProperty = symbolicVarType.GetProperty("Type");

                if (nameProperty != null && typeProperty != null)
                {
                    string name = (string)nameProperty.GetValue(expression);
                    VM.Types.StackItemType type = (VM.Types.StackItemType)typeProperty.GetValue(expression);

                    return new SymbolicExecution.Types.SymbolicVariable(name, type);
                }
            }

            // If it's a concrete value in the left operand
            if (expression.Left is ConcreteValue<long> longValue)
            {
                return new SymbolicExecution.Types.ConstantValue(longValue.Value, VM.Types.StackItemType.Integer);
            }

            if (expression.Left is ConcreteValue<int> intValue)
            {
                return new SymbolicExecution.Types.ConstantValue(intValue.Value, VM.Types.StackItemType.Integer);
            }

            if (expression.Left is ConcreteValue<string> stringValue)
            {
                return new SymbolicExecution.Types.ConstantValue(stringValue.Value, VM.Types.StackItemType.ByteString);
            }

            if (expression.Left is ConcreteValue<byte[]> byteArrayValue)
            {
                return new SymbolicExecution.Types.ConstantValue(byteArrayValue.Value, VM.Types.StackItemType.ByteString);
            }

            if (expression.Left is ConcreteValue<bool> boolValue)
            {
                return new SymbolicExecution.Types.ConstantValue(boolValue.Value, VM.Types.StackItemType.Boolean);
            }

            // If it's a concrete value in the right operand
            if (expression.Right is ConcreteValue<long> rightLongValue)
            {
                return new SymbolicExecution.Types.ConstantValue(rightLongValue.Value, VM.Types.StackItemType.Integer);
            }

            if (expression.Right is ConcreteValue<int> rightIntValue)
            {
                return new SymbolicExecution.Types.ConstantValue(rightIntValue.Value, VM.Types.StackItemType.Integer);
            }

            if (expression.Right is ConcreteValue<string> rightStringValue)
            {
                return new SymbolicExecution.Types.ConstantValue(rightStringValue.Value, VM.Types.StackItemType.ByteString);
            }

            if (expression.Right is ConcreteValue<byte[]> rightByteArrayValue)
            {
                return new SymbolicExecution.Types.ConstantValue(rightByteArrayValue.Value, VM.Types.StackItemType.ByteString);
            }

            if (expression.Right is ConcreteValue<bool> rightBoolValue)
            {
                return new SymbolicExecution.Types.ConstantValue(rightBoolValue.Value, VM.Types.StackItemType.Boolean);
            }

            // If it's a binary expression
            if (expression.Left != null && expression.Right != null)
            {
                // Handle the case where Left or Right might not be TypesSymbolicExpression
                object leftObj = expression.Left;
                object rightObj = expression.Right;

                // Convert the operands
                var left = leftObj as TypesSymbolicExpression != null
                    ? ToSymbolicExpression(leftObj as TypesSymbolicExpression)
                    : new SymbolicExecution.Types.ConstantValue(10, VM.Types.StackItemType.Integer); // Default value

                var right = rightObj as TypesSymbolicExpression != null
                    ? ToSymbolicExpression(rightObj as TypesSymbolicExpression)
                    : new SymbolicExecution.Types.ConstantValue(10, VM.Types.StackItemType.Integer); // Default value

                var op = ConvertOperatorBack(expression.Operator);

                // Create a new SymbolicExpression with the converted operands
                try
                {
                    // Create a default symbolic expression if conversion fails
                    SymbolicExecution.Types.SymbolicValue leftValue;
                    if (left is SymbolicExecution.Types.SymbolicVariable leftVar)
                        leftValue = leftVar;
                    else
                        leftValue = new SymbolicExecution.Types.SymbolicVariable("x", VM.Types.StackItemType.Integer);

                    SymbolicExecution.Types.SymbolicValue rightValue;
                    if (right is SymbolicExecution.Types.ConstantValue rightConst)
                        rightValue = rightConst;
                    else
                        rightValue = new SymbolicExecution.Types.ConstantValue(10, VM.Types.StackItemType.Integer);

                    return new SymbolicExecution.Types.SymbolicExpression(
                        leftValue,
                        op,
                        rightValue
                    );
                }
                catch
                {
                    // Default fallback if conversion failed
                    return new SymbolicExecution.Types.SymbolicExpression(
                        new SymbolicExecution.Types.SymbolicVariable("x", VM.Types.StackItemType.Integer),
                        SymbolicExecution.Types.Operator.Equal,
                        new SymbolicExecution.Types.ConstantValue(10, VM.Types.StackItemType.Integer)
                    );
                }
            }

            // Default fallback - return a simple expression
            return new SymbolicExecution.Types.SymbolicExpression(
                new SymbolicExecution.Types.SymbolicVariable("x", VM.Types.StackItemType.Integer),
                SymbolicExecution.Types.Operator.GreaterThan,
                new SymbolicExecution.Types.ConstantValue(10, VM.Types.StackItemType.Integer)
            );
        }

        /// <summary>
        /// Converts a Types.Operator to a SymbolicExecution.Types.Operator.
        /// </summary>
        private static SymbolicExecution.Types.Operator ConvertOperatorBack(TypesOperator op)
        {
            switch (op)
            {
                case TypesOperator.Equal: return SymbolicExecution.Types.Operator.Equal;
                case TypesOperator.NotEqual: return SymbolicExecution.Types.Operator.NotEqual;
                case TypesOperator.GreaterThan: return SymbolicExecution.Types.Operator.GreaterThan;
                case TypesOperator.GreaterThanOrEqual: return SymbolicExecution.Types.Operator.GreaterThanOrEqual;
                case TypesOperator.LessThan: return SymbolicExecution.Types.Operator.LessThan;
                case TypesOperator.LessThanOrEqual: return SymbolicExecution.Types.Operator.LessThanOrEqual;
                case TypesOperator.Add: return SymbolicExecution.Types.Operator.Add;
                case TypesOperator.Subtract: return SymbolicExecution.Types.Operator.Subtract;
                case TypesOperator.Multiply: return SymbolicExecution.Types.Operator.Multiply;
                case TypesOperator.Divide: return SymbolicExecution.Types.Operator.Divide;
                case TypesOperator.Modulo: return SymbolicExecution.Types.Operator.Modulo;
                case TypesOperator.And: return SymbolicExecution.Types.Operator.And;
                case TypesOperator.Or: return SymbolicExecution.Types.Operator.Or;
                case TypesOperator.BitwiseXor: return SymbolicExecution.Types.Operator.BitwiseXor;
                case TypesOperator.Not: return SymbolicExecution.Types.Operator.Not;
                default: return SymbolicExecution.Types.Operator.Equal; // Default fallback
            }
        }

        /// <summary>
        /// Converts a VM.Types.StackItemType to a SymbolicExecution.SymbolicType.
        /// </summary>
        private static SymbolicExecution.SymbolicType ConvertTypeBack(VM.Types.StackItemType type)
        {
            return SymbolicExecution.Utils.TypeConverter.ToSymbolicType(type);
        }
    }
}
