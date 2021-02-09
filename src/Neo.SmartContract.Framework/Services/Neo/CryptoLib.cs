#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x726cb6e0cd8628a1350a611384688911ab75f51b")]
    public static class CryptoLib
    {
        /// <summary>
        /// RFC 4492
        /// </summary>
        /// <remarks>
        /// https://tools.ietf.org/html/rfc4492#section-5.1.1
        /// </remarks>
        public enum NamedCurve : byte
        {
            secp256k1 = 22,
            secp256r1 = 23
        }

        public static extern UInt160 Hash { [ContractHash] get; }

        public static extern ByteString Sha256(ByteString value);

        public static extern ByteString ripemd160(ByteString value);

        public extern static bool VerifyWithECDsa(ByteString message, Cryptography.ECC.ECPoint pubkey, ByteString signature, NamedCurve curve);
    }
}
