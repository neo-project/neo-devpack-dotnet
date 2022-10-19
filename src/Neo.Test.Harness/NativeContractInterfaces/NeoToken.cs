namespace NeoTestHarness.NativeContractInterfaces
{
    public interface NeoToken : Nep17Token
    {
        Neo.VM.Types.Array getAccountState(Neo.UInt160 account);
        Neo.VM.Types.Array getCandidates();
        Neo.VM.Types.Array getCommittee();
        System.Numerics.BigInteger getGasPerBlock();
        Neo.VM.Types.Array getNextBlockValidators();
        System.Numerics.BigInteger getRegisterPrice();
        bool registerCandidate(Neo.Cryptography.ECC.ECPoint pubkey);
        void setGasPerBlock(System.Numerics.BigInteger gasPerBlock);
        void setRegisterPrice(System.Numerics.BigInteger registerPrice);
        System.Numerics.BigInteger unclaimedGas(Neo.UInt160 account, System.Numerics.BigInteger end);
        bool unregisterCandidate(Neo.Cryptography.ECC.ECPoint pubkey);
        bool vote(Neo.UInt160 account, Neo.Cryptography.ECC.ECPoint voteTo);
    }
}