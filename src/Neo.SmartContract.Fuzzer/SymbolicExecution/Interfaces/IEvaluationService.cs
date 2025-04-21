using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Interfaces
{
    /// <summary>
    /// Interface for a service that evaluates operations on SymbolicValues.
    /// </summary>
    public interface IEvaluationService
    {
        // --- Unary Operations ---
        SymbolicValue Increment(SymbolicValue value);
        SymbolicValue Decrement(SymbolicValue value);
        SymbolicValue Negate(SymbolicValue value);
        SymbolicValue Abs(SymbolicValue value);
        SymbolicValue Sign(SymbolicValue value);
        SymbolicValue Not(SymbolicValue value);
        SymbolicValue IsNonZero(SymbolicValue value);

        // --- Binary Operations ---
        SymbolicValue Add(SymbolicValue left, SymbolicValue right);
        SymbolicValue Subtract(SymbolicValue left, SymbolicValue right);
        SymbolicValue Multiply(SymbolicValue left, SymbolicValue right);
        SymbolicValue Divide(SymbolicValue left, SymbolicValue right);
        SymbolicValue Modulo(SymbolicValue left, SymbolicValue right);
        SymbolicValue ShiftLeft(SymbolicValue value, SymbolicValue shiftAmount);
        SymbolicValue ShiftRight(SymbolicValue value, SymbolicValue shiftAmount);
        SymbolicValue BoolAnd(SymbolicValue left, SymbolicValue right);
        SymbolicValue BoolOr(SymbolicValue left, SymbolicValue right);
        SymbolicValue Equal(SymbolicValue left, SymbolicValue right);
        SymbolicValue NotEqual(SymbolicValue left, SymbolicValue right);
        SymbolicValue NumericEquals(SymbolicValue left, SymbolicValue right);
        SymbolicValue NumericNotEquals(SymbolicValue left, SymbolicValue right);
        SymbolicValue LessThan(SymbolicValue left, SymbolicValue right);
        SymbolicValue LessThanOrEqual(SymbolicValue left, SymbolicValue right);
        SymbolicValue GreaterThan(SymbolicValue left, SymbolicValue right);
        SymbolicValue GreaterThanOrEqual(SymbolicValue left, SymbolicValue right);
        SymbolicValue Min(SymbolicValue left, SymbolicValue right);
        SymbolicValue Max(SymbolicValue left, SymbolicValue right);

        // --- Ternary Operations ---
        SymbolicValue Within(SymbolicValue value, SymbolicValue min_bound, SymbolicValue max_bound);
    }
}
