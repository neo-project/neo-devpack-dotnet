using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using System;
using System.Linq;
using Neo.VM.Types; // Added for StackItemType
using System.Text;

namespace Neo.SmartContract.Fuzzer.SymbolicExecution.Utils
{
    /// <summary>
    /// Provides utility methods for ByteString operations during symbolic execution.
    /// </summary>
    public static class ByteStringOperations
    {
        /// <summary>
        /// Converts a concrete object or an existing SymbolicValue into the appropriate SymbolicValue type.
        /// </summary>
        private static SymbolicValue ToSymbolicValue(object obj)
        {
            return obj switch
            {
                SymbolicValue sv => sv, // Already symbolic
                byte[] bytes => new ConcreteValue<byte[]>(bytes),
                int i => new ConcreteValue<int>(i),
                long l => new ConcreteValue<long>(l),
                bool b => new ConcreteValue<bool>(b),
                string s => new ConcreteValue<string>(s),
                // Add other concrete types handled by the VM as needed
                _ => throw new ArgumentException($"Cannot convert object of type {obj?.GetType().Name ?? "null"} to SymbolicValue")
            };
        }

        /// <summary>
        /// Concatenates two byte strings, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="left">The left byte string.</param>
        /// <param name="right">The right byte string.</param>
        /// <returns>The concatenated byte string or a symbolic expression representing the concatenation.</returns>
        public static object Concatenate(object left, object right)
        {
            // If both operands are concrete byte arrays, concatenate them
            if (left is byte[] leftBytes && right is byte[] rightBytes)
            {
                byte[] result = new byte[leftBytes.Length + rightBytes.Length];
                System.Buffer.BlockCopy(leftBytes, 0, result, 0, leftBytes.Length);
                System.Buffer.BlockCopy(rightBytes, 0, result, leftBytes.Length, rightBytes.Length);
                return result;
            }

            // If at least one operand is symbolic, create a symbolic function call
            var leftExpr = ToSymbolicValue(left);
            var rightExpr = ToSymbolicValue(right);

            // TODO: Decide how to represent Concat symbolically. Using Add operator as placeholder.
            return new SymbolicExpression(leftExpr, Operator.Add, rightExpr);
        }

        /// <summary>
        /// Slices a byte string, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="str">The byte string to slice.</param>
        /// <param name="index">The starting index.</param>
        /// <param name="count">The number of bytes to slice.</param>
        /// <returns>The sliced byte string or a symbolic expression representing the slice.</returns>
        public static object Slice(object str, object index, object count)
        {
            // If all operands are concrete, perform concrete slice operation
            if (str is byte[] bytes &&
                TryGetConcreteInt(index, out int startIndex) &&
                TryGetConcreteInt(count, out int length))
            {
                // Validate indices
                if (startIndex < 0 || startIndex >= bytes.Length ||
                    length < 0 || startIndex + length > bytes.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                byte[] result = new byte[length];
                System.Buffer.BlockCopy(bytes, startIndex, result, 0, length);
                return result;
            }

            // If any operand is symbolic, create a symbolic function call
            var strExpr = ToSymbolicValue(str);
            var indexExpr = ToSymbolicValue(index);
            var countExpr = ToSymbolicValue(count);

            // TODO: Symbolic slicing needs a proper representation (FunctionCall or dedicated type).
            // Using a placeholder variable for now.
            return new SymbolicVariable($"var_ByteString_Slice({strExpr}, {indexExpr}, {countExpr})", StackItemType.ByteString);
        }

        /// <summary>
        /// Compares two byte strings, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="left">The left byte string.</param>
        /// <param name="right">The right byte string.</param>
        /// <returns>
        /// -1 if left is less than right,
        /// 0 if left is equal to right,
        /// 1 if left is greater than right,
        /// or a symbolic expression representing the comparison.
        /// </returns>
        public static object Compare(object left, object right)
        {
            // If both operands are concrete byte arrays, compare them
            if (left is byte[] leftBytes && right is byte[] rightBytes)
            {
                return CompareByteArrays(leftBytes, rightBytes);
            }

            // If at least one operand is symbolic, create a symbolic function call
            var leftExpr = ToSymbolicValue(left);
            var rightExpr = ToSymbolicValue(right);

            // TODO: Decide how to represent Compare symbolically. Using Equal operator as placeholder.
            return new SymbolicExpression(leftExpr, Operator.Equal, rightExpr); // Placeholder operator!
        }

        /// <summary>
        /// Converts a byte string to a string representation, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="bytes">The byte string.</param>
        /// <returns>The string representation or a symbolic expression representing the conversion.</returns>
        public static object ToString(object bytes)
        {
            // If the operand is a concrete byte array, convert it to string
            if (bytes is byte[] concreteBytes)
            {
                try
                {
                    return Encoding.UTF8.GetString(concreteBytes);
                }
                catch
                {
                    // If conversion fails, return the hex representation
                    return BitConverter.ToString(concreteBytes).Replace("-", "");
                }
            }

            // If the operand is symbolic, create a symbolic function call
            var bytesExpr = ToSymbolicValue(bytes);

            // TODO: Unary operations need representation. Using placeholder variable.
            return new SymbolicVariable($"var_ByteString_ToString({bytesExpr})", StackItemType.ByteString); // Technically produces string, but represented as bytes internally?
        }

        /// <summary>
        /// Gets the hex string representation of a byte string, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="bytes">The byte string.</param>
        /// <returns>The hex string representation or a symbolic expression representing the conversion.</returns>
        public static object ToHexString(object bytes)
        {
            // If the operand is a concrete byte array, convert it to hex string
            if (bytes is byte[] concreteBytes)
            {
                return BitConverter.ToString(concreteBytes).Replace("-", "");
            }

            // If the operand is symbolic, create a symbolic function call
            var bytesExpr = ToSymbolicValue(bytes);

            // TODO: Unary operations need representation. Using placeholder variable.
            return new SymbolicVariable($"var_ByteString_ToHexString({bytesExpr})", StackItemType.ByteString); // Produces string, see above
        }

        /// <summary>
        /// Gets a single byte from a byte string, handling both concrete and symbolic values.
        /// </summary>
        /// <param name="bytes">The byte string.</param>
        /// <param name="index">The index.</param>
        /// <returns>The byte at the specified index or a symbolic expression representing the byte.</returns>
        public static object GetByte(object bytes, object index)
        {
            // If both operands are concrete, get the byte at the specified index
            if (bytes is byte[] concreteBytes && TryGetConcreteInt(index, out int concreteIndex))
            {
                // Validate index
                if (concreteIndex < 0 || concreteIndex >= concreteBytes.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return concreteBytes[concreteIndex];
            }

            // If at least one operand is symbolic, create a symbolic function call
            var bytesExpr = ToSymbolicValue(bytes);
            var indexExpr = ToSymbolicValue(index);

            // TODO: GetByte needs symbolic representation. Using placeholder variable.
            return new SymbolicVariable($"var_ByteString_GetByte({bytesExpr}, {indexExpr})", StackItemType.Integer); // GetByte returns an integer
        }

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>
        /// -1 if a is less than b,
        /// 0 if a is equal to b,
        /// 1 if a is greater than b.
        /// </returns>
        private static int CompareByteArrays(byte[] a, byte[] b)
        {
            // Compare lengths first
            int lengthComparison = a.Length.CompareTo(b.Length);
            if (lengthComparison != 0)
                return lengthComparison;

            // If lengths are equal, compare byte by byte
            for (int i = 0; i < a.Length; i++)
            {
                int byteComparison = a[i].CompareTo(b[i]);
                if (byteComparison != 0)
                    return byteComparison;
            }

            // Arrays are equal
            return 0;
        }

        /// <summary>
        /// Tries to convert an object to an integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="result">The converted integer value.</param>
        /// <returns>True if conversion was successful, false otherwise.</returns>
        private static bool TryGetConcreteInt(object value, out int result)
        {
            if (value is int intValue)
            {
                result = intValue;
                return true;
            }

            if (value is long longValue && longValue >= int.MinValue && longValue <= int.MaxValue)
            {
                result = (int)longValue;
                return true;
            }

            if (value is System.Numerics.BigInteger biValue &&
                biValue >= int.MinValue && biValue <= int.MaxValue)
            {
                result = (int)biValue;
                return true;
            }

            if (value is byte[] bytes && bytes.Length <= 4)
            {
                try
                {
                    result = BitConverter.ToInt32(bytes.Reverse().Concat(new byte[4 - bytes.Length]).ToArray(), 0);
                    return true;
                }
                catch
                {
                    result = 0;
                    return false;
                }
            }

            result = 0;
            return false;
        }
    }
}
