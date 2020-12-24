using Neo.Cryptography.ECC;
using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Crypto : SmartContract.Framework.SmartContract
    {
        [DisplayName("SHA256")]
        public static byte[] SHA256(byte[] value)
        {
            return Crypto.SHA256(value);
        }

        [DisplayName("RIPEMD160")]
        public static byte[] RIPEMD160(byte[] value)
        {
            return Crypto.RIPEMD160(value);
        }

        public static byte[] Hash160(byte[] value)
        {
            return Crypto.Hash160(value);
        }

        public static byte[] Hash256(byte[] value)
        {
            return Crypto.Hash256(value);
        }

        public static bool Secp256r1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256r1.Verify(null, pubkey, signature);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256r1.Verify(message, pubkey, signature);
        }

        public static bool Secp256r1VerifySignatures(ECPoint[] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsa.Secp256r1.CheckMultiSig(null, pubkeys, signatures);
        }

        public static bool Secp256r1VerifySignaturesWithMessage(byte[] message, ECPoint[] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsa.Secp256r1.CheckMultiSig(message, pubkeys, signatures);
        }

        public static bool Secp256k1VerifySignature(ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256k1.Verify(null, pubkey, signature);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, ECPoint pubkey, byte[] signature)
        {
            return Crypto.ECDsa.Secp256k1.Verify(message, pubkey, signature);
        }

        public static bool Secp256k1VerifySignatures(ECPoint[] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsa.Secp256k1.CheckMultiSig(null, pubkeys, signatures);
        }

        public static bool Secp256k1VerifySignaturesWithMessage(byte[] message, ECPoint[] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsa.Secp256k1.CheckMultiSig(message, pubkeys, signatures);
        }
    }
}
