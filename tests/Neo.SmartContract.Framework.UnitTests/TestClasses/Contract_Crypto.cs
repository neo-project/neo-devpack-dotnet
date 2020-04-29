using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Crypto : SmartContract.Framework.SmartContract
    {
        public static byte[] SHA256(byte[] value)
        {
            return Crypto.SHA256(value);
        }

        public static bool Secp256r1VerifySignature(byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaSecp256r1Verify(null, pubkey, signature);
        }

        public static bool Secp256r1VerifySignatureWithMessage(byte[] message, byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaSecp256r1Verify(message, pubkey, signature);
        }

        public static bool Secp256r1VerifySignatures(byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaSecp256r1CheckMultiSig(null, pubkeys, signatures);
        }

        public static bool Secp256r1VerifySignaturesWithMessage(byte[] message, byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaSecp256r1CheckMultiSig(message, pubkeys, signatures);
        }

        public static bool Secp256k1VerifySignature(byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaSecp256k1Verify(null, pubkey, signature);
        }

        public static bool Secp256k1VerifySignatureWithMessage(byte[] message, byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaSecp256k1Verify(message, pubkey, signature);
        }

        public static bool Secp256k1VerifySignatures(byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaSecp256k1CheckMultiSig(null, pubkeys, signatures);
        }

        public static bool Secp256k1VerifySignaturesWithMessage(byte[] message, byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaSecp256k1CheckMultiSig(message, pubkeys, signatures);
        }
    }
}
