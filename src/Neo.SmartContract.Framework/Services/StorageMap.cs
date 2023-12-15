// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
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

        public extern ByteString this[ByteString key]
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

        public extern ByteString this[byte[] key]
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

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString Get(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetUInt160(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetUInt256(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetECPoint(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern int GetInteger(ByteString key);

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
        public extern int GetIntegerOrZero(ByteString key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString Get(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetUInt160(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetUInt256(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetECPoint(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public extern byte[] GetByteArray(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        public extern ByteString GetString(byte[] key);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Get")]
        [OpCode(OpCode.CONVERT, StackItemType.Integer)]
        public extern int GetInteger(byte[] key);

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
        public extern int GetIntegerOrZero(byte[] key);

        public object GetObject(ByteString key)
        {
            ByteString value = Get(key);
            if (value is null) return null;
            return StdLib.Deserialize(value);
        }

        public object GetObject(byte[] key)
        {
            ByteString value = Get(key);
            if (value is null) return null;
            return StdLib.Deserialize(value);
        }

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(ByteString prefix, FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(byte[] prefix, FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, ByteString value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, ByteString value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, BigInteger value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, BigInteger value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, bool value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, bool value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, byte[] value);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Put")]
        public extern void Put(byte[] key, byte[] value);

        public void PutObject(ByteString key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        public void PutObject(byte[] key, object value)
        {
            Put(key, StdLib.Serialize(value));
        }

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.UNPACK)]
        [OpCode(OpCode.DROP)]
        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(ByteString key);

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
