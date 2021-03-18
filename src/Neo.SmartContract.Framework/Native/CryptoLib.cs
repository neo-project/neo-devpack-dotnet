#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x726cb6e0cd8628a1350a611384688911ab75f51b")]
    public static class CryptoLib
    {
        public static extern UInt160 Hash { [ContractHash] get; }

        public static extern ByteString Sha256(ByteString value);

        public static extern ByteString ripemd160(ByteString value);

        public extern static bool VerifyWithECDsa(ByteString message, Cryptography.ECC.ECPoint pubkey, ByteString signature, NamedCurve curve);
    }
}
