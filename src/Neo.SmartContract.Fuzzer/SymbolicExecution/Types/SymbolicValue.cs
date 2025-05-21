using Neo.VM.Types;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Abstract base class for all symbolic value types.
    /// Represents a value that could be concrete or symbolic during execution.
    /// </summary>
    public abstract class SymbolicValue
    {
        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public abstract StackItemType Type { get; }

        /// <summary>
        /// Indicates whether the value is concrete (has a fixed known value).
        /// </summary>
        public virtual bool IsConcrete => true; // Default to true, override in symbolic types

        /// <summary>
        /// Attempts to get the concrete value as a BigInteger.
        /// Returns false if the value is not a concrete integer.
        /// </summary>
        public virtual bool TryGetInteger(out BigInteger value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a boolean.
        /// Returns false if the value is not a concrete boolean.
        /// </summary>
        public virtual bool TryGetBoolean(out bool value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to get the concrete value as a byte array.
        /// Returns false if the value is not a concrete byte array.
        /// </summary>
        public virtual bool TryGetBytes(out byte[] value)
        {
            value = default;
            return false;
        }

        // Add other common methods or properties as needed, e.g., ToStackItem()
        public abstract StackItem ToStackItem();
    }
}
