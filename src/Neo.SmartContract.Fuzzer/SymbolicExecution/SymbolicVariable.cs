using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution
{
    /// <summary>
    /// Represents a symbolic variable in symbolic execution.
    /// </summary>
    public class SymbolicVariable : SymbolicValue
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the symbolic type of the variable.
        /// </summary>
        public SymbolicType SymbolicType { get; }

        /// <summary>
        /// Gets the underlying Neo VM StackItem type.
        /// </summary>
        public override StackItemType Type => ConvertType(SymbolicType);

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="type">The type of the variable.</param>
        public SymbolicVariable(string name, SymbolicType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SymbolicType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="type">The type of the variable as a StackItemType.</param>
        public SymbolicVariable(string name, StackItemType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SymbolicType = Utils.TypeConverter.ToSymbolicType(type);
        }

        /// <summary>
        /// Converts the symbolic variable to a Neo VM StackItem.
        /// </summary>
        /// <returns>A StackItem representation of the variable.</returns>
        public override StackItem ToStackItem()
        {
            // Create a placeholder stack item based on the type
            switch (SymbolicType)
            {
                case SymbolicType.Integer:
                    return new VM.Types.Integer(0); // Default placeholder
                case SymbolicType.Boolean:
                    return VM.Types.Boolean.False; // Default placeholder
                case SymbolicType.String:
                    return new VM.Types.ByteString(System.Text.Encoding.UTF8.GetBytes(Name)); // Use name as placeholder
                case SymbolicType.ByteArray:
                    return new VM.Types.Buffer(System.Text.Encoding.UTF8.GetBytes(Name)); // Use name as placeholder
                case SymbolicType.Array:
                    return new VM.Types.Array(); // Empty array placeholder
                case SymbolicType.Map:
                    return new VM.Types.Map(); // Empty map placeholder
                default:
                    return VM.Types.Null.Null; // Default fallback
            }
        }

        /// <summary>
        /// Converts a SymbolicType to a StackItemType.
        /// </summary>
        /// <param name="type">The symbolic type to convert.</param>
        /// <returns>The corresponding StackItemType.</returns>
        private static StackItemType ConvertType(SymbolicType type)
        {
            return Utils.TypeConverter.ToStackItemType(type);
        }

        /// <summary>
        /// Returns a string representation of the variable.
        /// </summary>
        /// <returns>A string representation of the variable.</returns>
        public override string ToString()
        {
            return $"Var({Name}, {Type})";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is SymbolicVariable other)
            {
                return Name == other.Name && Type == other.Type;
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }
    }
}
