using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Native : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Native"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""NEO_Decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""NEO_Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":6,""safe"":false},{""name"":""NEO_BalanceOf"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":19,""safe"":false},{""name"":""NEO_GetAccountState"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Any"",""offset"":29,""safe"":false},{""name"":""NEO_GetGasPerBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""NEO_UnclaimedGas"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""end"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":45,""safe"":false},{""name"":""NEO_RegisterCandidate"",""parameters"":[{""name"":""pubkey"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":56,""safe"":false},{""name"":""NEO_GetCandidates"",""parameters"":[],""returntype"":""Array"",""offset"":66,""safe"":false},{""name"":""GAS_Decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":72,""safe"":false},{""name"":""Policy_GetFeePerByte"",""parameters"":[],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""Policy_IsBlocked"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":84,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b"",""methods"":[""getFeePerByte"",""isBlocked""]},{""contract"":""0xd2a4cff31913016155e38e474a2c06d08be276cf"",""methods"":[""decimals""]},{""contract"":""0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"",""methods"":[""balanceOf"",""decimals"",""getAccountState"",""getCandidates"",""getGasPerBlock"",""registerCandidate"",""transfer"",""unclaimedGas""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAv1Y+pAvCg9TQ4FxI6jBbPyoHNA7whkZWNpbWFscwAAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7wh0cmFuc2ZlcgQAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7wliYWxhbmNlT2YBAAEP9WPqQLwoPU0OBcSOowWz8qBzQO8PZ2V0QWNjb3VudFN0YXRlAQABD/Vj6kC8KD1NDgXEjqMFs/Kgc0DvDmdldEdhc1BlckJsb2NrAAABD/Vj6kC8KD1NDgXEjqMFs/Kgc0DvDHVuY2xhaW1lZEdhcwIAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7xFyZWdpc3RlckNhbmRpZGF0ZQEAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7w1nZXRDYW5kaWRhdGVzAAABD8924ovQBixKR47jVWEBExnzz6TSCGRlY2ltYWxzAAABD3vGgcCh9x1UNFe2i7qNX5/dTl7MDWdldEZlZVBlckJ5dGUAAAEPe8aBwKH3HVQ0V7aLuo1fn91OXswJaXNCbG9ja2VkAQABDwAAXjcAACICQFcAAwt6eXg3AQAiAkBXAAF4NwIAIgJAVwABeDcDACICQDcEACICQFcAAnl4NwUAIgJAVwABeDcGACICQDcHACICQDcIACICQDcJACICQFcAAXg3CgAiAkBdQhft"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? GAS_Decimals();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? NEO_BalanceOf(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? NEO_Decimals();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract object? NEO_GetAccountState(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract IList<object>? NEO_GetCandidates();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? NEO_GetGasPerBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool? NEO_RegisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool? NEO_Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? NEO_UnclaimedGas(UInt160? account, BigInteger? end);

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract BigInteger? Policy_GetFeePerByte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract bool? Policy_IsBlocked(UInt160? account);

    #endregion

    #region Constructor for internal use only

    protected Contract_Native(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
