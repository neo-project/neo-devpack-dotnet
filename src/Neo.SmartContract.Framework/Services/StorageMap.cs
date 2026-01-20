// Copyright (C) 2015-2026 The Neo Project.
//
// StorageMap.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0169
#pragma warning disable IDE0051

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework.Services
{
    public class StorageMap
    {
        private readonly StorageContext context;
        private readonly byte[] prefix;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// <para>
        /// For setting, the execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        public extern ByteString? this[ByteString key]
        {
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Get")]
            get;
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Put")]
            set;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// <para>
        /// For setting, the execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        public extern ByteString? this[byte[] key]
        {
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Get")]
            get;
            [CallingConvention(CallingConvention.Cdecl)]
            [OpCode(OpCode.UNPACK)]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.REVERSE3)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [Syscall("System.Storage.Put")]
            set;
        }

        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(byte[] prefix);

        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(ByteString prefix);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NEWBUFFER)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.SETITEM)]
        [Syscall("System.Storage.GetContext")]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(byte prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, byte[] prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, ByteString prefix);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.NEWBUFFER)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.SETITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, byte prefix);

        /// <summary>
        /// Gets the value associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString? Get(ByteString key);

        /// <summary>
        /// Gets the value as UInt160 associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt160 GetUInt160(ByteString key);

        /// <summary>
        /// Gets the value as UInt256 associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt256 GetUInt256(ByteString key);

        /// <summary>
        /// Gets the value as ECPoint associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ECPoint GetECPoint(ByteString key);

        /// <summary>
        /// Gets the value as byte[] associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(ByteString key);

        /// <summary>
        /// Gets the value as ByteString associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(ByteString key);

        /// <summary>
        /// Gets the value as BigInteger associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetInteger(ByteString key);

        /// <summary>
        /// Gets the value as boolean associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.NOT)]
        [OpCode(OpCode.NOT)]
        public extern bool GetBoolean(ByteString key);

        /// <summary>
        /// Gets the value as BigInteger associated with the specified key, 0 if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIFNOT, "0x06")]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.JMP, "0x04")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetIntegerOrZero(ByteString key);

        /// <summary>
        /// Gets the value associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString? Get(byte[] key);

        /// <summary>
        /// Gets the value as UInt160 associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt160 GetUInt160(byte[] key);

        /// <summary>
        /// Gets the value as UInt256 associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern UInt256 GetUInt256(byte[] key);

        /// <summary>
        /// Gets the value as ECPoint associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ECPoint GetECPoint(byte[] key);

        /// <summary>
        /// Gets the value as byte[] associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(byte[] key);

        /// <summary>
        /// Gets the value as ByteString associated with the specified key, null if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(byte[] key);

        /// <summary>
        /// Gets the value as BigInteger associated with the specified key, 0 if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetInteger(byte[] key);

        /// <summary>
        /// Gets the value as boolean associated with the specified key, false if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.NOT)]
        [OpCode(OpCode.NOT)]
        public extern bool GetBoolean(byte[] key);

        /// <summary>
        /// Gets the value as BigInteger associated with the specified key, 0 if the key is not found.
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIFNOT, "0x06")]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.JMP, "0x04")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern BigInteger GetIntegerOrZero(byte[] key);

        /// <summary>
        /// Gets the value as object(deserialized from the value) associated with the specified key, null if the key is not found.
        /// <para>
        /// The execution will fail if the value is not a valid neo binary serilization format.
        /// </para>
        /// </summary>
        public object? GetObject(ByteString key)
        {
            ByteString? value = Get(key);
            if (value is null) return null;
            return StdLib.Deserialize(value);
        }

        /// <summary>
        /// Gets the value as object(deserialized from the value) associated with the specified key, null if the key is not found.
        /// <para>
        /// The execution will fail if the value is not a valid neo binary serilization format.
        /// </para>
        /// </summary>
        public object? GetObject(byte[] key)
        {
            ByteString? value = Get(key);
            if (value is null) return null;
            return StdLib.Deserialize(value);
        }

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and increase it by 1 then put back the new value.
        /// If the key is not found, the value will be set to 0 and then increased by 1.
        /// </summary>
        public BigInteger Increase(byte[] key) => Increase((ByteString)key, BigInteger.One);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and increase it by 1 then put back the new value.
        /// If the key is not found, the value will be set to 0 and then increased by 1.
        /// </summary>
        public BigInteger Increase(ByteString key) => Increase(key, BigInteger.One);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and increase it by the specified amount then put back the new value.
        /// If the key is not found, the value will be set to 0 and then increased by the specified amount.
        /// </summary>
        public BigInteger Increase(byte[] key, BigInteger amount) => Increase((ByteString)key, amount);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and increase it by the specified amount then put back the new value.
        /// If the key is not found, the value will be set to 0 and then increased by the specified amount.
        /// </summary>
        public BigInteger Increase(ByteString key, BigInteger amount)
        {
            var i = GetIntegerOrZero(key) + amount;
            Put(key, i);
            return i;
        }

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and decrease it by 1 then put back the new value.
        /// If the key is not found, the value will be set to 0 and then decreased by 1.
        /// If the result is 0, the key will be deleted.
        /// </summary>
        public BigInteger Decrease(byte[] key) => Decrease((ByteString)key, BigInteger.One);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and decrease it by 1 then put back the new value.
        /// If the key is not found, the value will be set to 0 and then decreased by 1.
        /// If the result is 0, the key will be deleted.
        /// </summary>
        public BigInteger Decrease(ByteString key) => Decrease(key, BigInteger.One);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and decrease it by the specified amount then put back the new value.
        /// If the key is not found, the value will be set to 0 and then decreased by the specified amount.
        /// If the result is 0, the key will be deleted.
        /// </summary>
        public BigInteger Decrease(byte[] key, BigInteger amount) => Decrease((ByteString)key, amount);

        /// <summary>
        /// Get the value as BigInteger associated with the specified key, and decrease it by the specified amount then put back the new value.
        /// If the key is not found, the value will be set to 0 and then decreased by the specified amount.
        /// If the result is 0, the key will be deleted.
        /// </summary>
        public BigInteger Decrease(ByteString key, BigInteger amount)
        {
            var i = GetIntegerOrZero(key) - amount;
            if (i == 0)
            {
                Delete(key);
            }
            else
            {
                Put(key, i);
            }
            return i;
        }

        /// <summary>
        /// Finds the keys and values in the storage map.
        /// <para>
        /// The execution will fail if the options are invalid. (see <see cref="FindOptions"/>).
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(FindOptions options = FindOptions.None);

        /// <summary>
        /// Finds the keys and values in the storage map.
        /// <para>
        /// The execution will fail if the options are invalid. (see <see cref="FindOptions"/>).
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(ByteString prefix, FindOptions options = FindOptions.None);

        /// <summary>
        /// Finds the keys and values in the storage map.
        /// <para>
        /// The execution will fail if the options are invalid. (see <see cref="FindOptions"/>).
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(byte[] prefix, FindOptions options = FindOptions.None);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, ByteString value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, ByteString value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, BigInteger value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, BigInteger value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, bool value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, bool value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, byte[] value);

        /// <summary>
        /// Puts the value associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, byte[] value);

        /// <summary>
        /// Puts the value(serialized from the object) associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        public void PutObject(ByteString key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        /// <summary>
        /// Puts the value(serialized from the object) associated with the specified key.
        /// <para>
        /// The execution will fail if:
        ///  1. The key is null or key.Length > MaxStorageKeySize(the default value is 64).
        ///  2. The value is null or value.Length > MaxStorageValueSize(the default value is 65535).
        ///  3. The context is read-only.
        /// </para>
        /// </summary>
        public void PutObject(byte[] key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        /// <summary>
        /// Deletes the entry from the storage map.
        /// <para>
        /// The execution will fail if the context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(ByteString key);

        /// <summary>
        /// Deletes the entry from the storage map.
        /// <para>
        /// The execution will fail if the context is read-only.
        /// </para>
        /// </summary>
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(byte[] key);
    }
}
