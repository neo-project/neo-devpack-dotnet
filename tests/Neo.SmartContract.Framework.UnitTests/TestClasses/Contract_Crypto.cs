using Neo.Cryptography.ECC;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Crypto : SmartContract
    {
        [DisplayName("SHA256")]
        public static byte[] SHA256(byte[] value)
        {
            return (byte[])CryptoLib.Sha256((ByteString)value);
        }

        [DisplayName("RIPEMD160")]
        public static byte[] RIPEMD160(byte[] value)
        {
            return (byte[])CryptoLib.ripemd160((ByteString)value);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, NamedCurve.secp256r1);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, NamedCurve.secp256k1);
        }
    }
}
