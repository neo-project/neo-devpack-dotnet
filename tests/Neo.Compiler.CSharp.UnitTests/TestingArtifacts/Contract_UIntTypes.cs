using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGJXAAF4DBT2ZENJjTh40yuZTk4Sg8aTRCHa/pdAVwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXQFcAAXjbKErYJAlKygAUKAM6QFcAAXhK2SgkBkUJIgbKABSzJAQJQHixQDMGjmI=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAwU9mRDSY04eNMrmU5OEoPGk0Qh2v6XQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHDATA1 F66443498D3878D32B994E4E1283C6934421DAFE [8 datoshi]
    /// 1A : EQUAL [32 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkOwner")]
    public abstract bool? CheckOwner(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAwUAAAAAAAAAAAAAAAAAAAAAAAAAACXQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1A : EQUAL [32 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkZeroStatic")]
    public abstract bool? CheckZeroStatic(UInt160? owner);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNsoStgkCUrKABQoAzpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : ISNULL [2 datoshi]
    /// 08 : JMPIF 09 [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : SIZE [4 datoshi]
    /// 0C : PUSHINT8 14 [1 datoshi]
    /// 0E : JMPEQ 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("constructUInt160")]
    public abstract UInt160? ConstructUInt160(byte[]? bytes);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErZKCQGRQkiBsoAFLMkBAlAeLFA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 04 [2 datoshi]
    /// 13 : PUSHF [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// 15 : LDARG0 [2 datoshi]
    /// 16 : NZ [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("validateAddress")]
    public abstract bool? ValidateAddress(UInt160? address);

    #endregion
}
