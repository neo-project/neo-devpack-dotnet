namespace Neo
{
    public class UInt160
    {
        public static readonly UInt160 Zero = new UInt160();

        public UInt160()
        {
        }

        public UInt160(byte[] _)
        {
        }

        [SmartContract.Framework.Script]
        public static extern implicit operator byte[](UInt160 value);
    }
}