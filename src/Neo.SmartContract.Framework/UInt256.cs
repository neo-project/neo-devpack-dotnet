using Neo.SmartContract.Framework;

namespace Neo
{
    public class UInt256
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        public UInt256(byte[] _)
        {
        }

        [Script]
        public static extern implicit operator byte[](UInt256 value);
    }
}
