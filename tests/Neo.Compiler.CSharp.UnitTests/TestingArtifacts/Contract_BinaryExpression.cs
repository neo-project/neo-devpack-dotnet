using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_BinaryExpression(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_BinaryExpression"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""binaryIs"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""binaryAs"",""parameters"":[],""returntype"":""Void"",""offset"":19,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAE5XAgAMAWFwaNkoOQwAcWnZKDlAVwEADBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBoStkoJARFCwwUAAAAAAAAAAAAAAAAAAAAAAAAAACXOUBlVuZG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADBQAAAAAAAAAAAAAAAAAAAAAAAAAAHBoStkoJARFCwwUAAAAAAAAAAAAAAAAAAAAAAAAAACXOUA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("binaryAs")]
    public abstract void BinaryAs();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAFhcGjZKDkMAHFp2Sg5QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 61 'a' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// ASSERT [1 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("binaryIs")]
    public abstract void BinaryIs();

    #endregion
}
