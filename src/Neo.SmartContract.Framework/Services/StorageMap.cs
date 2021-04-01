using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework.Services
{
    public class StorageMap
    {
        internal readonly StorageContext Context;
        internal readonly byte[] Prefix;

        public extern ByteString this[ByteString key]
        {
            [OpCode(OpCode.OVER)]
            [OpCode(OpCode.PUSH1)]
            [OpCode(OpCode.PICKITEM)]
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.PICKITEM)]
            [Syscall("System.Storage.Get")]
            get;
            [OpCode(OpCode.PUSH2)]
            [OpCode(OpCode.PICK)]
            [OpCode(OpCode.PUSH1)]
            [OpCode(OpCode.PICKITEM)]
            [OpCode(OpCode.ROT)]
            [OpCode(OpCode.CAT)]
            [OpCode(OpCode.ROT)]
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.PICKITEM)]
            [Syscall("System.Storage.Put")]
            set;
        }

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, byte[] prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, ByteString prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public extern StorageMap(StorageContext context, sbyte prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(ByteString prefix, FindOptions options = FindOptions.None);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.TUCK)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Find")]
        public extern Iterator Find(byte[] prefix, FindOptions options = FindOptions.None);

        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PICK)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, ByteString value);

        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PICK)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.ROT)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Put")]
        public extern void Put(ByteString key, BigInteger value);

        [OpCode(OpCode.OVER)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Delete")]
        public extern void Delete(ByteString key);
    }
}
