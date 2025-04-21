using Neo.VM.Types;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a concrete integer value during symbolic execution.
    /// </summary>
    public class SymbolicInteger : SymbolicValue
    {
        public BigInteger Value { get; }

        public override StackItemType Type => StackItemType.Integer;

        public SymbolicInteger(BigInteger value)
        {
            Value = value;
        }

        public static SymbolicInteger FromLong(long value)
        {
            return new SymbolicInteger(new BigInteger(value));
        }

        public override bool TryGetInteger(out BigInteger value)
        {
            value = Value;
            return true;
        }

        public override StackItem ToStackItem()
        {
            return new Integer(Value);
        }

        public override string ToString()
        {
            return $"Integer({Value})";
        }

        // Potentially add implicit/explicit conversions or operator overloads later
    }
}
