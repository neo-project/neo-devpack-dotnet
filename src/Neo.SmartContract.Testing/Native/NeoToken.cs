using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class NeoToken : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delCandidateStateChanged(ECPoint pubkey, bool registered, BigInteger votes);
    [DisplayName("CandidateStateChanged")]
    public event delCandidateStateChanged? OnCandidateStateChanged;
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;
    public delegate void delVote(UInt160 account, ECPoint from, ECPoint to, BigInteger amount);
    [DisplayName("Vote")]
    public event delVote? OnVote;
    #endregion
    #region Properties
    public abstract BigInteger Decimals { [DisplayName("decimals")] get; }
    public abstract object AllCandidates { [DisplayName("getAllCandidates")] get; }
    public abstract List<object> Candidates { [DisplayName("getCandidates")] get; }
    public abstract List<object> Committee { [DisplayName("getCommittee")] get; }
    public abstract BigInteger GasPerBlock { [DisplayName("getGasPerBlock")] get; [DisplayName("setGasPerBlock")] set; }
    public abstract List<object> NextBlockValidators { [DisplayName("getNextBlockValidators")] get; }
    public abstract BigInteger RegisterPrice { [DisplayName("getRegisterPrice")] get; [DisplayName("setRegisterPrice")] set; }
    public abstract string Symbol { [DisplayName("symbol")] get; }
    public abstract BigInteger TotalSupply { [DisplayName("totalSupply")] get; }
    #endregion
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger BalanceOf(UInt160 account);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getAccountState")]
    public abstract List<object> GetAccountState(UInt160 account);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getCandidateVote")]
    public abstract BigInteger GetCandidateVote(ECPoint pubKey);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("unclaimedGas")]
    public abstract BigInteger UnclaimedGas(UInt160 account, BigInteger end);
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("registerCandidate")]
    public abstract bool RegisterCandidate(ECPoint pubkey);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unregisterCandidate")]
    public abstract bool UnregisterCandidate(ECPoint pubkey);
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("vote")]
    public abstract bool Vote(UInt160 account, ECPoint voteTo);
    #endregion
    #region Constructor for internal use only
    protected NeoToken(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }
    #endregion
}
