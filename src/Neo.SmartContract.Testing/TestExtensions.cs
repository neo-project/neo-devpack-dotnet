using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Testing
{
    public static class TestExtensions
    {
        /// <summary>
        /// Convert stack item to dotnet
        /// </summary>
        /// <param name="stackItem">Item</param>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static object? ConvertTo(this StackItem stackItem, Type type)
        {
            if (stackItem is null) return null;

            if (type == typeof(string)) return stackItem.ToString();
            if (type == typeof(byte[])) return stackItem.GetSpan().ToArray();

            if (type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong) ||
                type == typeof(BigInteger)
                )
            {
                return stackItem.GetInteger();
            }

            if (type == typeof(UInt160)) return new UInt160(stackItem.GetSpan().ToArray());
            if (type == typeof(UInt256)) return new UInt256(stackItem.GetSpan().ToArray());
            if (type == typeof(Cryptography.ECC.ECPoint))
                return Cryptography.ECC.ECPoint.FromBytes(stackItem.GetSpan().ToArray(), Cryptography.ECC.ECCurve.Secp256r1);

            return null;
        }
    }
}
