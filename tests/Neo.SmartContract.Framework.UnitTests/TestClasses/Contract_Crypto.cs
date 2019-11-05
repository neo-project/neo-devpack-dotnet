using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Crypto : SmartContract.Framework.SmartContract
    {
        public static bool VerifySignature(byte[] pubkey, byte[] signature)
        {
            return Crypto.VerifySignature(pubkey, signature);
        }

        public static bool VerifySignatureWithMessage(byte[] message, byte[] pubkey, byte[] signature)
        {
            return Crypto.VerifySignature(message, pubkey, signature);
        }

        public static bool VerifySignatures(byte[][] pubkeys, byte[][] signatures)
        {
            return Crypto.VerifySignatures(pubkeys, signatures);
        }
    }
}
