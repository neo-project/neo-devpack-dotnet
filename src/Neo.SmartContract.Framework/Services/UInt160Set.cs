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
    /// A persistent set of <see cref="UInt160"/> values stored in the contract storage, keyed by value.
    /// Each element is stored once (inside its storage key), so it is the cheapest option when only membership
    /// and enumeration are needed. Use <see cref="UInt160IndexedSet"/> if <c>At(index)</c> is also required.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Storage layout (all keys are relative to the configured prefix <c>P</c>):
    /// <code>
    /// P | 0x00              -> element count (integer; absent when the set is empty)
    /// P | 0x01 | value[20]  -> true (presence marker; the value is the key itself)
    /// </code>
    /// </para>
    /// <para>
    /// <see cref="Values"/> iterates in storage-key order, which is deterministic. Removal does not move other elements.
    /// </para>
    /// <para>
    /// Prefixes: sets in the same contract must use different prefixes that are not a prefix of one another, and
    /// must not share a prefix with any other storage. A prefix may be 1 to 43 bytes so that the longest key
    /// (prefix + tag + 20 bytes) stays within the 64 bytes storage key limit.
    /// </para>
    /// <para>
    /// Any failure (invalid value) aborts the execution, which reverts all storage writes.
    /// </para>
    /// </remarks>
    public class UInt160Set
    {
        /// <summary>
        /// The maximum prefix length, so that prefix + tag + 20 bytes is not greater than the 64 bytes key limit.
        /// </summary>
        public const int MaxPrefixLength = 43;

        private const byte TAG_COUNT = 0x00;
        private const byte TAG_VALUE = 0x01;

        private readonly LocalStorageMap map;

        /// <summary>
        /// Creates a set that stores its data under the specified single byte prefix.
        /// </summary>
        public UInt160Set(byte prefix)
        {
            map = new LocalStorageMap(prefix);
        }

        /// <summary>
        /// Creates a set that stores its data under the specified prefix (1 to <see cref="MaxPrefixLength"/> bytes).
        /// </summary>
        public UInt160Set(byte[] prefix)
        {
            ExecutionEngine.Assert(prefix.Length > 0 && prefix.Length <= MaxPrefixLength, "invalid prefix length");
            map = new LocalStorageMap(prefix);
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

            map.Put(valueKey, true);
            map.Put(new byte[] { TAG_COUNT }, Length + 1);
            return true;
        }

        /// <summary>
        /// Removes the value from the set. Returns false (and writes nothing) if it is not present.
        /// The execution fails if the value is not a valid UInt160.
        /// </summary>
        public bool Remove(UInt160 value)
        {
            ExecutionEngine.Assert(value.IsValid, "invalid value");
            byte[] valueKey = ValueKey(value);
            if (map.Get(valueKey) == null) return false;

            map.Delete(valueKey);
            BigInteger remaining = Length - 1;
            if (remaining == 0)
                map.Delete(new byte[] { TAG_COUNT });
            else
                map.Put(new byte[] { TAG_COUNT }, remaining);
            return true;
        }

        /// <summary>
        /// Gets a keys-only iterator over all elements, in storage-key order (deterministic).
        /// Each iterator item is a UInt160. The set must not be modified while the iterator is being consumed.
        /// </summary>
        public Iterator Values()
        {
            return map.Find(new byte[] { TAG_VALUE }, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        private static byte[] ValueKey(UInt160 value)
            => new byte[] { TAG_VALUE }.Concat((byte[])value);
    }
}
