using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_InitialValue(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_InitialValue"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getDefaultName"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""getDefaultAge"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""getInferredInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":17,""safe"":false},{""name"":""getInferredBytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":20,""safe"":false},{""name"":""getDefaultOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":27,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADIMC0RlZmF1bHROYW1lQAASQAAqQAwEABEiM0AMFH7uGqvrZ+0deR1E5PX8866RcahxQNFOImM=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ABJA
    /// PUSHINT8 12 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getDefaultAge")]
    public abstract BigInteger? GetDefaultAge();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtEZWZhdWx0TmFtZUA=
    /// PUSHDATA1 44656661756C744E616D65 'DefaultName' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getDefaultName")]
    public abstract string? GetDefaultName();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBR+7hqr62ftHXkdROT1/POukXGocUA=
    /// PUSHDATA1 7EEE1AABEB67ED1D791D44E4F5FCF3AE9171A871 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getDefaultOwner")]
    public abstract UInt160? GetDefaultOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAQAESIzQA==
    /// PUSHDATA1 00112233 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getInferredBytes")]
    public abstract byte[]? GetInferredBytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ACpA
    /// PUSHINT8 2A [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getInferredInteger")]
    public abstract BigInteger? GetInferredInteger();

    #endregion
}
