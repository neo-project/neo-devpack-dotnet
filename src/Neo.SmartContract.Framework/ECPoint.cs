using Neo.SmartContract.Framework;

namespace Neo.Cryptography.ECC
{
    public class ECPoint
    {
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHDATA1, "0121")] // 0x01 == 1 byte data length, 0x21 == 33 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]       
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator ECPoint(byte[] value);


        [Script]
        public static extern implicit operator byte[](ECPoint value);
    }
}
