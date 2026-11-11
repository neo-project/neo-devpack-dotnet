// Copyright (C) 2015-2026 The Neo Project.
//
// UInt160Set.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.SmartContract.Framework.Services
{
    /// <summary>
    /// A persistent, enumerable set of <see cref="UInt160"/> values stored in the contract storage.
    /// Membership, insertion and removal are O(1) storage operations and never rewrite the whole collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Storage layout (all keys are relative to the configured prefix <c>P</c>):
    /// <code>
    /// P | 0x00                -> element count (integer; absent when the set is empty)
    /// P | 0x01 | index        -> value            (index is the minimal little-endian integer encoding)
    /// P | 0x02 | value[20]    -> index + 1        (position; absent means "not a member")
    /// </code>
    /// Positions are stored as index + 1 so that a missing value can be distinguished from index zero.
    /// </para>
    /// <para>
    /// Removal uses swap-and-pop: the last element is moved into the removed slot, so
    /// <b>removal may change the enumeration order</b> of the remaining elements. Do not rely on a stable order
    /// across <see cref="Remove(UInt160)"/> calls. <see cref="Values"/> iterates in storage-key order, which is
    /// deterministic but is not guaranteed to match the order of <see cref="At(BigInteger)"/>.
    /// </para>
    /// <para>
    /// Prefixes: two sets in the same contract must use different prefixes, and the prefixes must not be a
    /// prefix of one another (use prefixes of the same length to be safe). Do not share a prefix with any other
    /// storage in the contract. A prefix may be 1 to 43 bytes so that the longest key (prefix + tag + 20 bytes)
    /// stays within the 64 byte storage key limit.
    /// </para>
    /// <para>
    /// Any failure (invalid value, index out of range) aborts the execution, which reverts all storage writes.
    /// </para>
    /// </remarks>
    public class UInt160Set
    {
        /// <summary>
        /// The maximum prefix length, so that prefix + tag + 20 bytes is not greater than the 64 bytes key limit.
        /// </summary>
        public const int MaxPrefixLength = 43;

        private const byte TAG_COUNT = 0x00;
        private const byte TAG_INDEX = 0x01;
        private const byte TAG_VALUE = 0x02;

        private readonly StorageMap map;

        /// <summary>
        /// Creates a set that stores its data under the specified single byte prefix.
        /// </summary>
        public UInt160Set(byte prefix)
        {
            map = new StorageMap(Storage.CurrentContext, prefix);
        }

        /// <summary>
        /// Creates a set that stores its data under the specified prefix (1 to <see cref="MaxPrefixLength"/> bytes).
        /// </summary>
        public UInt160Set(byte[] prefix)
        {
            ExecutionEngine.Assert(prefix.Length > 0 && prefix.Length <= MaxPrefixLength, "invalid prefix length");
            map = new StorageMap(Storage.CurrentContext, prefix);
        }

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        public BigInteger Length => map.GetIntegerOrZero(new byte[] { TAG_COUNT });

        /// <summary>
        /// Checks whether the value is in the set. The execution fails if the value is not a valid UInt160.
        /// </summary>
        public bool Contains(UInt160 value)
        {
            ExecutionEngine.Assert(value.IsValid, "invalid value");
            return map.Get(ValueKey(value)) != null;
        }

        /// <summary>
        /// Adds the value to the set. Returns false (and writes nothing) if it is already present.
        /// The execution fails if the value is not a valid UInt160.
        /// </summary>
        public bool Add(UInt160 value)
        {
            ExecutionEngine.Assert(value.IsValid, "invalid value");
            byte[] valueKey = ValueKey(value);
            if (map.Get(valueKey) != null) return false;

            BigInteger index = Length;
            map.Put(IndexKey(index), value);
            map.Put(valueKey, index + 1);
            map.Put(new byte[] { TAG_COUNT }, index + 1);
            return true;
        }

        /// <summary>
        /// Removes the value from the set using swap-and-pop. Returns false (and writes nothing) if it is not present.
        /// The last element takes the position of the removed one, so the enumeration order may change.
        /// The execution fails if the value is not a valid UInt160.
        /// </summary>
        public bool Remove(UInt160 value)
        {
            ExecutionEngine.Assert(value.IsValid, "invalid value");
            byte[] valueKey = ValueKey(value);
            BigInteger position = map.GetIntegerOrZero(valueKey);
            if (position == 0) return false;

            BigInteger index = position - 1;
            BigInteger last = Length - 1;
            byte[] lastKey = IndexKey(last);
            if (index != last)
            {
                UInt160 lastValue = map.GetUInt160(lastKey);
                map.Put(IndexKey(index), lastValue);
                map.Put(ValueKey(lastValue), position);
            }
            map.Delete(lastKey);
            map.Delete(valueKey);
            if (last == 0)
                map.Delete(new byte[] { TAG_COUNT });
            else
                map.Put(new byte[] { TAG_COUNT }, last);
            return true;
        }

        /// <summary>
        /// Gets the element at the specified index. The execution fails if the index is out of range.
        /// </summary>
        public UInt160 At(BigInteger index)
        {
            ExecutionEngine.Assert(index >= 0 && index < Length, "index out of range");
            return map.GetUInt160(IndexKey(index));
        }

        /// <summary>
        /// Gets a values-only iterator over all elements, in storage-key order (deterministic, but not
        /// guaranteed to match <see cref="At(BigInteger)"/> order). Each iterator item is a UInt160.
        /// The set must not be modified while the iterator is being consumed.
        /// </summary>
        public Iterator Values()
        {
            return map.Find(new byte[] { TAG_INDEX }, FindOptions.ValuesOnly);
        }

        private static byte[] IndexKey(BigInteger index)
            => new byte[] { TAG_INDEX }.Concat((ByteString)index);

        private static byte[] ValueKey(UInt160 value)
            => new byte[] { TAG_VALUE }.Concat((byte[])value);
    }
}
