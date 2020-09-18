namespace Neo
{
    public class UInt256
    {
        public static readonly UInt256 Zero = new UInt256();

        public UInt256()
        {
        }

        public UInt256(byte[] _)
        {
        }

        [SmartContract.Framework.Script]
        public static extern implicit operator byte[](UInt256 value);
    }
}