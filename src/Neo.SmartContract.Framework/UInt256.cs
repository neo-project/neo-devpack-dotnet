using Neo.SmartContract.Framework;

namespace Neo
{
    public class UInt256
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        public extern bool IsZero
        {
            [OpCode(OpCode.CONVERT, StackItemType.Integer)]
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator UInt256(byte[] value);

        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator UInt256(ByteString value);

        [Script]
        public static extern implicit operator byte[](UInt256 value);

        [Script]
        public static extern implicit operator ByteString(UInt256 value);
    }
}
