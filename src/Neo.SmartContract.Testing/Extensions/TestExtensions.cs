using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Testing.Extensions
{
    public static class TestExtensions
    {
        /// <summary>
        /// Convert Array stack item to dotnet array
        /// </summary>
        /// <param name="state">Item</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Object</returns>
        public static object?[]? ConvertTo(this VM.Types.Array state, ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                object?[] args = new object[parameters.Length];

                for (int x = 0; x < parameters.Length; x++)
                {
                    args[x] = state[x].ConvertTo(parameters[x].ParameterType);
                }

                return args;
            }

            return null;
        }

        /// <summary>
        /// Convert stack item to dotnet
        /// </summary>
        /// <param name="stackItem">Item</param>
        /// <param name="type">Type</param>
        /// <returns>Object</returns>
        public static object? ConvertTo(this StackItem stackItem, Type type)
        {
            if (stackItem is null || stackItem.IsNull) return null;

            return type switch
            {
                _ when type == typeof(string) => Utility.StrictUTF8.GetString(stackItem.GetSpan()),
                _ when type == typeof(byte[]) => stackItem.GetSpan().ToArray(),

                _ when type == typeof(bool) => stackItem.GetBoolean(),
                _ when type == typeof(bool?) => stackItem.GetBoolean(),
                _ when type == typeof(byte) => (byte)stackItem.GetInteger(),
                _ when type == typeof(byte?) => (byte)stackItem.GetInteger(),
                _ when type == typeof(sbyte) => (sbyte)stackItem.GetInteger(),
                _ when type == typeof(sbyte?) => (sbyte)stackItem.GetInteger(),
                _ when type == typeof(short) => (short)stackItem.GetInteger(),
                _ when type == typeof(short?) => (short)stackItem.GetInteger(),
                _ when type == typeof(ushort) => (ushort)stackItem.GetInteger(),
                _ when type == typeof(ushort?) => (ushort)stackItem.GetInteger(),
                _ when type == typeof(int) => (int)stackItem.GetInteger(),
                _ when type == typeof(int?) => (int)stackItem.GetInteger(),
                _ when type == typeof(uint) => (uint)stackItem.GetInteger(),
                _ when type == typeof(uint?) => (uint)stackItem.GetInteger(),
                _ when type == typeof(long) => (long)stackItem.GetInteger(),
                _ when type == typeof(long?) => (long)stackItem.GetInteger(),
                _ when type == typeof(ulong) => (ulong)stackItem.GetInteger(),
                _ when type == typeof(ulong?) => (ulong)stackItem.GetInteger(),

                _ when type == typeof(BigInteger) => stackItem.GetInteger(),
                _ when type == typeof(BigInteger?) => stackItem.GetInteger(),
                _ when type == typeof(UInt160) => new UInt160(stackItem.GetSpan().ToArray()),
                _ when type == typeof(UInt256) => new UInt256(stackItem.GetSpan().ToArray()),
                _ when type == typeof(Cryptography.ECC.ECPoint) => Cryptography.ECC.ECPoint.FromBytes(stackItem.GetSpan().ToArray(), Cryptography.ECC.ECCurve.Secp256r1),
                _ when type == typeof(List<object>) && stackItem is CompoundType cp => new List<object>(cp.SubItems), // SubItems in StackItem type
                _ when typeof(IInteroperable).IsAssignableFrom(type) => CreateInteroperable(stackItem, type),

                _ => throw new FormatException($"Impossible to convert {stackItem} to {type}"),
            };
        }

        private static IInteroperable CreateInteroperable(StackItem stackItem, Type type)
        {
            var interoperable = (IInteroperable)Activator.CreateInstance(type)!;
            interoperable.FromStackItem(stackItem);
            return interoperable;
        }
    }
}
