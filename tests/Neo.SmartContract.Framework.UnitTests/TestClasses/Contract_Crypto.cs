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
            return (byte[])Crypto.SHA256((ByteString)value);
        }

        [DisplayName("RIPEMD160")]
        public static byte[] RIPEMD160(byte[] value)
        {
            return (byte[])Crypto.RIPEMD160((ByteString)value);
        }

        public static byte[] Hash160(byte[] value)
        {
            return (byte[])Crypto.Hash160((ByteString)value);
        }

        public static byte[] Hash256(byte[] value)
        {
            return (byte[])Crypto.Hash256((ByteString)value);
        }

        public static bool Secp256r1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256r1.Verify(null, pubkey, (ByteString)signature);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256r1.Verify((ByteString)message, pubkey, (ByteString)signature);
        }

        public static bool Secp256r1VerifySignatures(ECPoint[] pubkeys, object[] signatures)
        {
            return Crypto.ECDsa.Secp256r1.CheckMultiSig(null, pubkeys, (ByteString[])signatures);
        }

        public static bool Secp256r1VerifySignaturesWithMessage(byte[] message, ECPoint[] pubkeys, object[] signatures)
        {
            return Crypto.ECDsa.Secp256r1.CheckMultiSig((ByteString)message, pubkeys, (ByteString[])signatures);
        }

        public static bool Secp256k1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256k1.Verify(null, pubkey, (ByteString)signature);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256k1.Verify((ByteString)message, pubkey, (ByteString)signature);
        }

        public static bool Secp256k1VerifySignatures(ECPoint[] pubkeys, object[] signatures)
        {
            return Crypto.ECDsa.Secp256k1.CheckMultiSig(null, pubkeys, (ByteString[])signatures);
        }

        public static bool Secp256k1VerifySignaturesWithMessage(byte[] message, ECPoint[] pubkeys, object[] signatures)
        {
            return Crypto.ECDsa.Secp256k1.CheckMultiSig((ByteString)message, pubkeys, (ByteString[])signatures);
        }
    }
}
