using Neo.IO;
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
        /// Convert dotnet type to stack item
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>StackItem</returns>
        public static StackItem ConvertToStackItem(this object data)
        {
            if (data is null) return StackItem.Null;
            if (data is bool b) return (VM.Types.Boolean)b;
            if (data is string s) return (ByteString)s;
            if (data is byte[] d) return (ByteString)d;
            if (data is ReadOnlyMemory<byte> r) return (ByteString)r;

            if (data is byte by) return (Integer)by;
            if (data is sbyte sby) return (Integer)sby;
            if (data is short i16) return (Integer)i16;
            if (data is ushort ui16) return (Integer)ui16;
            if (data is int i32) return (Integer)i32;
            if (data is uint ui32) return (Integer)ui32;
            if (data is long i64) return (Integer)i64;
            if (data is ulong ui64) return (Integer)ui64;
            if (data is BigInteger bi) return (Integer)bi;

            if (data is UInt160 u160) return (ByteString)u160.ToArray();
            if (data is UInt256 u256) return (ByteString)u256.ToArray();
            if (data is Cryptography.ECC.ECPoint ec) return (ByteString)ec.ToArray();

            if (data is object[] arr)
            {
                VM.Types.Array ar = new();

                foreach (object o in arr)
                {
                    ar.Add(o.ConvertToStackItem());
                }

                return ar;
            }

            if (data is IEnumerable<object> iarr)
            {
                VM.Types.Array ar = new();

                foreach (object o in iarr)
                {
                    ar.Add(o.ConvertToStackItem());
                }

                return ar;
            }

            return StackItem.Null;
        }

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

            if (type == typeof(bool)) return stackItem.GetBoolean();
            if (type == typeof(string)) return stackItem.ToString();
            if (type == typeof(byte[])) return stackItem.GetSpan().ToArray();

            if (type == typeof(byte)) return (byte)stackItem.GetInteger();
            if (type == typeof(sbyte)) return (sbyte)stackItem.GetInteger();
            if (type == typeof(short)) return (short)stackItem.GetInteger();
            if (type == typeof(ushort)) return (ushort)stackItem.GetInteger();
            if (type == typeof(int)) return (int)stackItem.GetInteger();
            if (type == typeof(uint)) return (uint)stackItem.GetInteger();
            if (type == typeof(long)) return (long)stackItem.GetInteger();
            if (type == typeof(ulong)) return (ulong)stackItem.GetInteger();
            if (type == typeof(BigInteger)) return stackItem.GetInteger();

            if (type == typeof(UInt160)) return new UInt160(stackItem.GetSpan().ToArray());
            if (type == typeof(UInt256)) return new UInt256(stackItem.GetSpan().ToArray());
            if (type == typeof(Cryptography.ECC.ECPoint))
                return Cryptography.ECC.ECPoint.FromBytes(stackItem.GetSpan().ToArray(), Cryptography.ECC.ECCurve.Secp256r1);

            if (type == typeof(List<object>) && stackItem is CompoundType cp)
            {
                return new List<object>(cp.SubItems);
            }

            throw new FormatException($"Impossible to convert {stackItem} to {type}");
        }
    }
}
