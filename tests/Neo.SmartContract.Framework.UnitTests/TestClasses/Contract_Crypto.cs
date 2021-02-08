using Neo.Cryptography.ECC;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Crypto : SmartContract.Framework.SmartContract
    {
        [DisplayName("SHA256")]
        public static byte[] SHA256(byte[] value)
        {
            return (byte[])CryptoLib.Sha256((ByteString)value);
        }

        [DisplayName("RIPEMD160")]
        public static byte[] RIPEMD160(byte[] value)
        {
            return (byte[])CryptoLib.RIPEMD160((ByteString)value);
        }

        public static byte[] Hash160(byte[] value)
        {
            return (byte[])CryptoLib.Hash160((ByteString)value);
        }

        public static byte[] Hash256(byte[] value)
        {
            return (byte[])CryptoLib.Hash256((ByteString)value);
        }

        public static bool Secp256r1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa(null, pubkey, (ByteString)signature, CryptoLib.NamedCurve.secp256r1);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, CryptoLib.NamedCurve.secp256r1);
        }

        public static bool Secp256k1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa(null, pubkey, (ByteString)signature, CryptoLib.NamedCurve.secp256k1);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return CryptoLib.VerifyWithECDsa((ByteString)message, pubkey, (ByteString)signature, CryptoLib.NamedCurve.secp256k1);
        }
    }
}
