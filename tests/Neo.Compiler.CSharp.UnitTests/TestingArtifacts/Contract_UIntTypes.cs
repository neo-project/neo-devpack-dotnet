using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_UIntTypes(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UIntTypes"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkOwner"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""checkZeroStatic"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""constructUInt160"",""parameters"":[{""name"":""bytes"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":56,""safe"":false},{""name"":""validateAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":74,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGJXAAF4DBT2ZENJjTh40yuZTk4Sg8aTRCHa/pdAVwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXQFcAAXjbKErYJAlKygAUKAM6QFcAAXhK2SgkBkUJIgbKABSzJAQJQHixQDMGjmI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAwU9mRDSY04eNMrmU5OEoPGk0Qh2v6XQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 F66443498D3878D32B994E4E1283C6934421DAFE [8 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkOwner")]
    public abstract bool? CheckOwner(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZeroStatic")]
    public abstract bool? CheckZeroStatic(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoStgkCUrKABQoAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("constructUInt160")]
    public abstract UInt160? ConstructUInt160(byte[]? bytes);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErZKCQGRQkiBsoAFLMkBAlAeLFA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("validateAddress")]
    public abstract bool? ValidateAddress(UInt160? address);

    #endregion
}
