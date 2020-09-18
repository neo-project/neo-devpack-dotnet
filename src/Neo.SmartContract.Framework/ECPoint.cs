namespace Neo.Cryptography.ECC
{
    public class ECPoint
    {
        public ECPoint()
        {
        }

        public ECPoint(byte[] _)
        {
        }

        [SmartContract.Framework.Script]
        public static extern implicit operator byte[](ECPoint value);
    }
}