using Neo.SmartContract.Framework;

namespace Neo
{
    public abstract class UInt256 : ByteString
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        public extern bool IsZero
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            [OpCode(OpCode.BOOLAND)]
            get;
        }

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIF, "09")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator UInt256(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt256 value);
    }
}
