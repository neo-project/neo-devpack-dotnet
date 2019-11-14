using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Crypto : SmartContract.Framework.SmartContract
    {
        public static bool VerifySignature(byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaVerify(null, pubkey, signature);
        }

        public static bool VerifySignatureWithMessage(byte[] message, byte[] pubkey, byte[] signature)
        {
            return Crypto.ECDsaVerify(message, pubkey, signature);
        }

        public static bool VerifySignatures(byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaCheckMultiSig(null, pubkeys, signatures);
        }

        public static bool VerifySignaturesWithMessage(byte[] message, byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.ECDsaCheckMultiSig(message, pubkeys, signatures);
        }
    }
}
