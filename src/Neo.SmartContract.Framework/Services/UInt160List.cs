// Copyright (C) 2015-2026 The Neo Project.
//
// UInt160List.cs file belongs to the neo project and is free
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
    /// A persistent, index-addressable list of <see cref="UInt160"/> values stored in the contract storage.
    /// Each element is stored once, so it is the cheapest option when only <see cref="At(BigInteger)"/> and
    /// enumeration are needed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Storage layout (all keys are relative to the configured prefix <c>P</c>):
    /// <code>
    /// P | 0x00           -> element count (integer; absent when the list is empty)
    /// P | 0x01 | index   -> value (index is the minimal little-endian integer encoding)
    /// </code>
    /// </para>
    /// <para>
    /// The list does not check uniqueness, because that would require scanning it. Use <see cref="UInt160Set"/>
    /// or <see cref="UInt160IndexedSet"/> when duplicates must be avoided.
    /// </para>
    /// <para>
    /// <see cref="RemoveAt(BigInteger)"/> uses swap-and-pop: the last element takes the removed position, so
    /// <b>removal may change the order</b> of the remaining elements. <see cref="Values"/> iterates in
    /// storage-key order, which is deterministic but not guaranteed to match the order of <see cref="At(BigInteger)"/>.
    /// </para>
    /// <para>
    /// Prefixes: lists in the same contract must use different prefixes that are not a prefix of one another, and
    /// must not share a prefix with any other storage. A prefix may be 1 to 43 bytes.
    /// </para>
    /// <para>
    /// Any failure (invalid value, index out of range) aborts the execution, which reverts all storage writes.
    /// </para>
    /// </remarks>
    public class UInt160List
    {
        /// <summary>
        /// The maximum prefix length, so that prefix + tag + index stays within the 64 bytes key limit.
        /// </summary>
        public const int MaxPrefixLength = 43;

        private const byte TAG_COUNT = 0x00;
        private const byte TAG_INDEX = 0x01;

        private readonly LocalStorageMap map;

        /// <summary>
        /// Creates a list that stores its data under the specified single byte prefix.
        /// </summary>
        public UInt160List(byte prefix)
        {
            map = new LocalStorageMap(prefix);
        }

        /// <summary>
        /// Creates a list that stores its data under the specified prefix (1 to <see cref="MaxPrefixLength"/> bytes).
        /// </summary>
        public UInt160List(byte[] prefix)
        {
            ExecutionEngine.Assert(prefix.Length > 0 && prefix.Length <= MaxPrefixLength, "invalid prefix length");
            map = new LocalStorageMap(prefix);
        }

        /// <summary>
        /// Gets the number of elements in the list.
        /// </summary>
        public BigInteger Length => map.GetIntegerOrZero(new byte[] { TAG_COUNT });

        /// <summary>
        /// Appends the value to the list. Duplicates are allowed.
        /// The execution fails if the value is not a valid UInt160.
        /// </summary>
        public void Add(UInt160 value)
        {
            ExecutionEngine.Assert(value.IsValid, "invalid value");
            BigInteger index = Length;
            map.Put(IndexKey(index), value);
            map.Put(new byte[] { TAG_COUNT }, index + 1);
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
        /// Removes the element at the specified index using swap-and-pop and returns it.
        /// The last element takes the position of the removed one. The execution fails if the index is out of range.
        /// </summary>
        public UInt160 RemoveAt(BigInteger index)
        {
            BigInteger last = Length - 1;
            ExecutionEngine.Assert(index >= 0 && index <= last, "index out of range");
            UInt160 removed = map.GetUInt160(IndexKey(index));
            byte[] lastKey = IndexKey(last);
            if (index != last)
                map.Put(IndexKey(index), map.GetUInt160(lastKey));
            map.Delete(lastKey);
            if (last == 0)
                map.Delete(new byte[] { TAG_COUNT });
            else
                map.Put(new byte[] { TAG_COUNT }, last);
            return removed;
        }

        /// <summary>
        /// Gets a values-only iterator over all elements, in storage-key order (deterministic, but not
        /// guaranteed to match <see cref="At(BigInteger)"/> order). Each iterator item is a UInt160.
        /// The list must not be modified while the iterator is being consumed.
        /// </summary>
        public Iterator Values()
        {
            return map.Find(new byte[] { TAG_INDEX }, FindOptions.ValuesOnly);
        }

        private static byte[] IndexKey(BigInteger index)
            => new byte[] { TAG_INDEX }.Concat((ByteString)index);
    }
}
