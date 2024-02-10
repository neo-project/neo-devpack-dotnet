using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class NeoToken : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delCandidateStateChanged(ECPoint pubkey, bool registered, BigInteger votes);
    public event delCandidateStateChanged? CandidateStateChanged;
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    public event delTransfer? Transfer;
    public delegate void delVote(UInt160 account, ECPoint from, ECPoint to, BigInteger amount);
    public event delVote? Vote;
    #endregion
    #region Safe methods
    public abstract BigInteger balanceOf(UInt160 account);
    public abstract BigInteger decimals();
    public abstract List<object> getAccountState(UInt160 account);
    public abstract object getAllCandidates();
    public abstract List<object> getCandidates();
    public abstract BigInteger getCandidateVote(ECPoint pubKey);
    public abstract List<object> getCommittee();
    public abstract BigInteger getGasPerBlock();
    public abstract List<object> getNextBlockValidators();
    public abstract BigInteger getRegisterPrice();
    public abstract string symbol();
    public abstract BigInteger totalSupply();
    public abstract BigInteger unclaimedGas(UInt160 account, BigInteger end);
    #endregion
    #region Unsafe methods
    public abstract bool registerCandidate(ECPoint pubkey);
    public abstract void setGasPerBlock(BigInteger gasPerBlock);
    public abstract void setRegisterPrice(BigInteger registerPrice);
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    public abstract bool unregisterCandidate(ECPoint pubkey);
    public abstract bool vote(UInt160 account, ECPoint voteTo);
    #endregion
    #region Constructor for internal use only
    protected NeoToken(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
