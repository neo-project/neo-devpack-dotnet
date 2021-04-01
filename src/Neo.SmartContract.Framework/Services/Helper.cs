using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework.Services
{
    public static class Helper
    {
        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public static extern StorageMap CreateMap(this StorageContext context, byte[] prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public static extern StorageMap CreateMap(this StorageContext context, ByteString prefix);

        [CallingConvention(CallingConvention.Cdecl)]
        [OpCode(OpCode.PUSH2)]
        [OpCode(OpCode.PACK)]
        public static extern StorageMap CreateMap(this StorageContext context, sbyte prefix);

        [OpCode(OpCode.OVER)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Delete")]
        public static extern void Delete(this StorageMap map, ByteString key);

        [OpCode(OpCode.OVER)]
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PICKITEM)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.CAT)]
        [OpCode(OpCode.SWAP)]
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.PICKITEM)]
        [Syscall("System.Storage.Get")]
        public static extern ByteString Get(this StorageMap map, ByteString key);

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
        public static extern void Put(this StorageMap map, ByteString key, ByteString value);

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
        public static extern void Put(this StorageMap map, ByteString key, BigInteger value);
    }
}
