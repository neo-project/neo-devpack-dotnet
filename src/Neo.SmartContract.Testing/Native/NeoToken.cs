using Neo.Cryptography.ECC;
using Neo.SmartContract.Iterators;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract partial class NeoToken : SmartContract, TestingStandards.INep17Standard
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""NeoToken"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""balanceOf"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""getAccountState"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Array"",""offset"":14,""safe"":true},{""name"":""getAllCandidates"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":21,""safe"":true},{""name"":""getCandidateVote"",""parameters"":[{""name"":""pubKey"",""type"":""PublicKey""}],""returntype"":""Integer"",""offset"":28,""safe"":true},{""name"":""getCandidates"",""parameters"":[],""returntype"":""Array"",""offset"":35,""safe"":true},{""name"":""getCommittee"",""parameters"":[],""returntype"":""Array"",""offset"":42,""safe"":true},{""name"":""getGasPerBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":49,""safe"":true},{""name"":""getNextBlockValidators"",""parameters"":[],""returntype"":""Array"",""offset"":56,""safe"":true},{""name"":""getRegisterPrice"",""parameters"":[],""returntype"":""Integer"",""offset"":63,""safe"":true},{""name"":""registerCandidate"",""parameters"":[{""name"":""pubkey"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":70,""safe"":false},{""name"":""setGasPerBlock"",""parameters"":[{""name"":""gasPerBlock"",""type"":""Integer""}],""returntype"":""Void"",""offset"":77,""safe"":false},{""name"":""setRegisterPrice"",""parameters"":[{""name"":""registerPrice"",""type"":""Integer""}],""returntype"":""Void"",""offset"":84,""safe"":false},{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":91,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":98,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":105,""safe"":false},{""name"":""unclaimedGas"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""end"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":112,""safe"":true},{""name"":""unregisterCandidate"",""parameters"":[{""name"":""pubkey"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":119,""safe"":false},{""name"":""vote"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""voteTo"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":126,""safe"":false}],""events"":[{""name"":""CandidateStateChanged"",""parameters"":[{""name"":""pubkey"",""type"":""PublicKey""},{""name"":""registered"",""type"":""Boolean""},{""name"":""votes"",""type"":""Integer""}]},{""name"":""Vote"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""from"",""type"":""PublicKey""},{""name"":""to"",""type"":""PublicKey""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""CommitteeChanged"",""parameters"":[{""name"":""old"",""type"":""Array""},{""name"":""new"",""type"":""Array""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Events

    public delegate void delCandidateStateChanged(ECPoint? pubkey, bool? registered, BigInteger? votes);

    [DisplayName("CandidateStateChanged")]
    public event delCandidateStateChanged? OnCandidateStateChanged;

    [DisplayName("Transfer")]
    public event TestingStandards.INep17Standard.delTransfer? OnTransfer;


    public delegate void delCommitteeChanged(ECPoint[]? old, ECPoint[]? @new);

    [DisplayName("CommitteeChanged")]
    public event delCommitteeChanged? OnCommitteeChanged;

    public delegate void delVote(UInt160? account, ECPoint? from, ECPoint? to, BigInteger? amount);

    [DisplayName("Vote")]
    public event delVote? OnVote;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract IIterator? AllCandidates { [DisplayName("getAllCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract Models.Candidate[] Candidates { [DisplayName("getCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract ECPoint[]? Committee { [DisplayName("getCommittee")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? GasPerBlock { [DisplayName("getGasPerBlock")] get; [DisplayName("setGasPerBlock")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract ECPoint[]? NextBlockValidators { [DisplayName("getNextBlockValidators")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? RegisterPrice { [DisplayName("getRegisterPrice")] get; [DisplayName("setRegisterPrice")] set; }

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
    public abstract Neo.SmartContract.Native.NeoToken.NeoAccountState? GetAccountState(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getCandidateVote")]
    public abstract BigInteger? GetCandidateVote(ECPoint? pubKey);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("unclaimedGas")]
    public abstract BigInteger? UnclaimedGas(UInt160? account, BigInteger? end);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("registerCandidate")]
    public abstract bool? RegisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unregisterCandidate")]
    public abstract bool? UnregisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("vote")]
    public abstract bool? Vote(UInt160? account, ECPoint? voteTo);

    #endregion

    #region Constructor for internal use only

    protected NeoToken(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
