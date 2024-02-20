using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract.Iterators;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Testing.Extensions;
using System.Linq;

namespace Neo.SmartContract.Testing.Native;

public abstract class NeoToken : SmartContract
{
    public class Candidate : IInteroperable
    {
        /// <summary>
        /// Public key
        /// </summary>
        public ECPoint? PublicKey { get; private set; }

        /// <summary>
        /// Votes
        /// </summary>
        public BigInteger Votes { get; private set; } = BigInteger.Zero;

        public void FromStackItem(StackItem stackItem)
        {
            if (stackItem is not CompoundType cp) throw new FormatException();
            if (cp.Count < 2) throw new FormatException();

            var items = cp.SubItems.ToArray();

            PublicKey = (ECPoint)items[0].ConvertTo(typeof(ECPoint))!;
            Votes = (BigInteger)items[1].ConvertTo(typeof(BigInteger))!;
        }

        public StackItem ToStackItem(ReferenceCounter referenceCounter)
        {
            return new VM.Types.Array(new StackItem[] { PublicKey.ToArray(), Votes });
        }
    }

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

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract IIterator AllCandidates { [DisplayName("getAllCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract Candidate[] Candidates { [DisplayName("getCandidates")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract ECPoint[] Committee { [DisplayName("getCommittee")] get; }

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
    public abstract BigInteger RegisterPrice { [DisplayName("getRegisterPrice")] get; [DisplayName("setRegisterPrice")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("balanceOf")]
    public abstract BigInteger BalanceOf(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getAccountState")]
    public abstract Neo.SmartContract.Native.NeoToken.NeoAccountState GetAccountState(UInt160? account);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getCandidateVote")]
    public abstract BigInteger GetCandidateVote(ECPoint? pubKey);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("unclaimedGas")]
    public abstract BigInteger UnclaimedGas(UInt160? account, BigInteger? end);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("registerCandidate")]
    public abstract bool RegisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract bool Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unregisterCandidate")]
    public abstract bool UnregisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("vote")]
    public abstract bool Vote(UInt160? account, ECPoint? voteTo);

    #endregion

    #region Constructor for internal use only

    protected NeoToken(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
