using Neo.Cryptography.ECC;

namespace Neo.SmartContract.Framework.Services
{
    public struct ContractGroup
    {
        public ECPoint PubKey;
        public ByteString Signature;
    }
}
