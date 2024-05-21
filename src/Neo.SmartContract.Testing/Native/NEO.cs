using Neo.Cryptography.ECC;
using Neo.SmartContract.Iterators;
using Neo.SmartContract.Native;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class NEO : SmartContract, TestingStandards.INep17Standard
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.NEO.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Events

    public delegate void delCandidateStateChanged(ECPoint pubkey, bool registered, BigInteger votes);

    [DisplayName("CandidateStateChanged")]
#pragma warning disable CS0067 // Event is never used
    public event delCandidateStateChanged? OnCandidateStateChanged;

    [DisplayName("Transfer")]
    public event TestingStandards.INep17Standard.delTransfer? OnTransfer;

    public delegate void delCommitteeChanged(ECPoint[] old, ECPoint[] @new);

    [DisplayName("CommitteeChanged")]
    public event delCommitteeChanged? OnCommitteeChanged;
    public delegate void delVote(UInt160 account, ECPoint? from, ECPoint? to, BigInteger amount);

    [DisplayName("Vote")]
    public event delVote? OnVote;
#pragma warning restore CS0067 // Event is never used
    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract IIterator AllCandidates { [DisplayName("getAllCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract Models.Candidate[] Candidates { [DisplayName("getCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract ECPoint[] Committee { [DisplayName("getCommittee")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? CommitteeAddress { [DisplayName("getCommitteeAddress")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger GasPerBlock { [DisplayName("getGasPerBlock")] get; [DisplayName("setGasPerBlock")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract ECPoint[] NextBlockValidators { [DisplayName("getNextBlockValidators")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract long RegisterPrice { [DisplayName("getRegisterPrice")] get; [DisplayName("setRegisterPrice")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getAccountState")]
    public abstract Neo.SmartContract.Native.NeoToken.NeoAccountState GetAccountState(UInt160 account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getCandidateVote")]
    public abstract BigInteger GetCandidateVote(ECPoint pubKey);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("unclaimedGas")]
    public abstract BigInteger UnclaimedGas(UInt160 account, uint end);

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
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unregisterCandidate")]
    public abstract bool UnregisterCandidate(ECPoint pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("vote")]
    public abstract bool Vote(UInt160 account, ECPoint? voteTo);

    #endregion

    #region Constructor for internal use only

    protected NEO(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
