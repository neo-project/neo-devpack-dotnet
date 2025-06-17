using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Types
{
    /// <summary>
    /// Represents a symbolic variable in symbolic execution.
    /// Inherits from SymbolicValue but represents a variable, not an expression.
    /// </summary>
    public class SymbolicVariable : SymbolicValue // Changed base class if SymbolicVariable itself is a value representation
    {
        /// <summary>
        /// Gets the unique name of this symbolic variable.
        /// </summary>
        public string Name { get; }

        private readonly StackItemType _variableType;

        /// <summary>
        /// Creates a new symbolic variable with the specified name and type.
        /// </summary>
        public SymbolicVariable(string name, StackItemType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _variableType = type;
        }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => _variableType;

        /// <summary>
        /// Indicates whether the value is concrete (always false for SymbolicVariable).
        /// </summary>
        public override bool IsConcrete => false;

        /// <summary>
        /// Converts this symbolic variable to a concrete stack item.
        /// </summary>
        /// <remarks>
        /// This will throw an exception since a symbolic variable cannot be directly
        /// converted to a concrete value without a model/assignment.
        /// </remarks>
        public override StackItem ToStackItem()
        {
            throw new InvalidOperationException($"Cannot convert symbolic variable '{Name}' to concrete StackItem without a model");
        }

        /// <summary>
        /// Returns a string representation of this symbolic variable.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        // Override Equals and GetHashCode for proper dictionary/set usage if needed
        public override bool Equals(object? obj)
        {
            return obj is SymbolicVariable variable && Name == variable.Name && Type == variable.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }
    }
}
