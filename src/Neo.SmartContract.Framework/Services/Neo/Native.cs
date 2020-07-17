using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
    public class NEO
    {
        public static extern int id();
        public static extern string name();
        public static extern string symbol();
        public static extern byte decimals();
        public static extern BigInteger totalSupply();
        public static extern BigInteger balanceOf(byte[] account);
        public static extern void onPersist();
        public static extern BigInteger unclaimedGas(byte[] account, uint end);
        public static extern bool registerCandidate(byte[] pubkey);
        public static extern bool unRegisterCandidate(byte[] pubkey);
        public static extern bool vote(byte[] account, byte[] voteTo);
        public static extern object getCandidates();
        public static extern object getValidators();
        public static extern object getCommittee();
        public static extern object getNextBlockValidators();
    }

    [Contract("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
    public class GAS
    {
        public static extern int id();
        public static extern string name();
        public static extern string symbol();
        public static extern byte decimals();
        public static extern BigInteger totalSupply();
        public static extern BigInteger balanceOf(byte[] account);
    }

    [Contract("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
    public class Policy
    {
        public static extern string name();
        public static extern void onPersist();
        public static extern uint getMaxTransactionsPerBlock();
        public static extern uint getMaxBlockSize();
        public static extern long getMaxBlockSystemFee();
        public static extern BigInteger getFeePerByte();
        public static extern object getBlockedAccounts();
        public static extern bool setMaxBlockSize(uint value);
        public static extern bool setMaxTransactionsPerBlock(uint value);
        public static extern bool setMaxBlockSystemFee(long value);
        public static extern bool setFeePerByte(long value);
        public static extern bool blockAccount(byte[] account);
        public static extern bool unblockAccount(byte[] account);
    }
}
