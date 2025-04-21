using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Fuzzer.Utility
{
    /// <summary>
    /// Comparer for byte arrays based on sequence equality.
    /// </summary>
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj is null) return 0;
            unchecked
            {
                int hash = 17;
                foreach (byte b in obj)
                {
                    hash = hash * 31 + b;
                }
                return hash;
            }
        }

        public static readonly ByteArrayComparer Instance = new ByteArrayComparer();
    }
}
