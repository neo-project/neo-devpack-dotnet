namespace Neo.Cryptography.ECC
{
    public class ECPoint
    {
        [SmartContract.Framework.Script]
        public static extern implicit operator byte[](ECPoint value);
    }
}
