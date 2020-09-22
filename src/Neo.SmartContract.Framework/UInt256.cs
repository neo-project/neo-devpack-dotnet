using Neo.SmartContract.Framework;

namespace Neo
{
    public class UInt256
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHDATA1, "0120")] // 0x01 == 1 byte data length, 0x20 == 32 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator UInt256(byte[] value);

        [Script]
        public static extern implicit operator byte[](UInt256 value);
    }
}
