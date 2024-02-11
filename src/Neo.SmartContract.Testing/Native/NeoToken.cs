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
    #region Properties
    public abstract object AllCandidates { get; }
    public abstract List<object> Candidates { get; }
    public abstract List<object> Committee { get; }
    public abstract BigInteger GasPerBlock { get; set; }
    public abstract List<object> NextBlockValidators { get; }
    public abstract BigInteger RegisterPrice { get; set; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger balanceOf(UInt160 account);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger decimals();
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> getAccountState(UInt160 account);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger getCandidateVote(ECPoint pubKey);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string symbol();
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger totalSupply();
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger unclaimedGas(UInt160 account, BigInteger end);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool registerCandidate(ECPoint pubkey);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool unregisterCandidate(ECPoint pubkey);
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool vote(UInt160 account, ECPoint voteTo);
    #endregion
    #region Constructor for internal use only
    protected NeoToken(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}