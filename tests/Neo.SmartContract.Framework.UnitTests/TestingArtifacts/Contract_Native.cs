using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Native(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Native"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""NEO_Decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""NEO_Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":4,""safe"":false},{""name"":""NEO_BalanceOf"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":15,""safe"":false},{""name"":""NEO_GetAccountState"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Any"",""offset"":23,""safe"":false},{""name"":""NEO_GetGasPerBlock"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""NEO_UnclaimedGas"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""end"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""NEO_RegisterCandidate"",""parameters"":[{""name"":""pubkey"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":44,""safe"":false},{""name"":""NEO_GetCandidates"",""parameters"":[],""returntype"":""Array"",""offset"":52,""safe"":false},{""name"":""GAS_Decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""Policy_GetFeePerByte"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""Policy_IsBlocked"",""parameters"":[{""name"":""account"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":64,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b"",""methods"":[""getFeePerByte"",""isBlocked""]},{""contract"":""0xd2a4cff31913016155e38e474a2c06d08be276cf"",""methods"":[""decimals""]},{""contract"":""0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"",""methods"":[""balanceOf"",""decimals"",""getAccountState"",""getCandidates"",""getGasPerBlock"",""registerCandidate"",""transfer"",""unclaimedGas""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAv1Y+pAvCg9TQ4FxI6jBbPyoHNA7whkZWNpbWFscwAAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7wh0cmFuc2ZlcgQAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7wliYWxhbmNlT2YBAAEP9WPqQLwoPU0OBcSOowWz8qBzQO8PZ2V0QWNjb3VudFN0YXRlAQABD/Vj6kC8KD1NDgXEjqMFs/Kgc0DvDmdldEdhc1BlckJsb2NrAAABD/Vj6kC8KD1NDgXEjqMFs/Kgc0DvDHVuY2xhaW1lZEdhcwIAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7xFyZWdpc3RlckNhbmRpZGF0ZQEAAQ/1Y+pAvCg9TQ4FxI6jBbPyoHNA7w1nZXRDYW5kaWRhdGVzAAABD8924ovQBixKR47jVWEBExnzz6TSCGRlY2ltYWxzAAABD3vGgcCh9x1UNFe2i7qNX5/dTl7MDWdldEZlZVBlckJ5dGUAAAEPe8aBwKH3HVQ0V7aLuo1fn91OXswJaXNCbG9ja2VkAQABDwAASDcAAEBXAAMLenl4NwEAQFcAAXg3AgBAVwABeDcDAEA3BABAVwACeXg3BQBAVwABeDcGAEA3BwBANwgAQDcJAEBXAAF4NwoAQL+gUWg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwgAQA==
    /// CALLT 0800 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? GAS_Decimals();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcCAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? NEO_BalanceOf(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwAAQA==
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? NEO_Decimals();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcDAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract object? NEO_GetAccountState(UInt160? account);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwcAQA==
    /// CALLT 0700 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract IList<object>? NEO_GetCandidates();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwQAQA==
    /// CALLT 0400 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? NEO_GetGasPerBlock();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcGAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0600 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract bool? NEO_RegisterCandidate(ECPoint? pubkey);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADC3p5eDcBAEA=
    /// INITSLOT 0003 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract bool? NEO_Transfer(UInt160? from, UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg3BQBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0500 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? NEO_UnclaimedGas(UInt160? account, BigInteger? end);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwkAQA==
    /// CALLT 0900 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract BigInteger? Policy_GetFeePerByte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcKAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0A00 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract bool? Policy_IsBlocked(UInt160? account);

    #endregion
}
