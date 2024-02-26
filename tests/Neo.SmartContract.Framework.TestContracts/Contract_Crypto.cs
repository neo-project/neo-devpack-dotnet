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
            return (byte[])CryptoLib.Ripemd160((ByteString)value);
        }

        public static byte[] Murmur32(byte[] value, uint seed)
        {
            return (byte[])CryptoLib.Murmur32((ByteString)value, seed);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, NamedCurve.secp256r1);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, NamedCurve.secp256k1);
        }

        public static byte[] Bls12381Serialize(object data)
        {
            return CryptoLib.Bls12381Serialize(data);
        }

        public static object Bls12381Deserialize(byte[] data)
        {
            return CryptoLib.Bls12381Deserialize(data);
        }

        public static object Bls12381Equal(object x, object y)
        {
            return CryptoLib.Bls12381Equal(x, y);
        }

        public static object Bls12381Add(object x, object y)
        {
            return CryptoLib.Bls12381Add(x, y);
        }

        public static object Bls12381Mul(object x, byte[] mul, bool neg)
        {
            return CryptoLib.Bls12381Mul(x, mul, neg);
        }

        public static object Bls12381Pairing(object g1, object g2)
        {
            return CryptoLib.Bls12381Pairing(g1, g2);
        }
    }
}
