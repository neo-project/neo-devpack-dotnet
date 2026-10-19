using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_StringConcatEmpty(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StringConcatEmpty"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""concatNeoRight"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""concatNeoLeft"",""parameters"":[],""returntype"":""String"",""offset"":13,""safe"":false},{""name"":""concatBoth"",""parameters"":[],""returntype"":""String"",""offset"":26,""safe"":false},{""name"":""concatNullRight"",""parameters"":[],""returntype"":""String"",""offset"":29,""safe"":false},{""name"":""concatNullLeft"",""parameters"":[],""returntype"":""String"",""offset"":38,""safe"":false},{""name"":""concatRight"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":47,""safe"":false},{""name"":""concatLeft"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""String"",""offset"":61,""safe"":false},{""name"":""concatNormal"",""parameters"":[],""returntype"":""String"",""offset"":75,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGcMA05lb0rYJgVFDABADANOZW9K2CYFRQwAQAwAQAtK2CYFRQwAQAtK2CYFRQwAQFcAAXhK2CYFRQwA2yhAVwABeErYJgVFDADbKEAMAk4zDANOZW9K2CYFRQwAUErYJgVFDACL2yhA9DUFDg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DABA
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatBoth")]
    public abstract string? ConcatBoth();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDADbKEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatLeft")]
    public abstract string? ConcatLeft(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANOZW9K2CYFRQwAQA==
    /// PUSHDATA1 4E656F 'Neo' [8 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatNeoLeft")]
    public abstract string? ConcatNeoLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANOZW9K2CYFRQwAQA==
    /// PUSHDATA1 4E656F 'Neo' [8 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatNeoRight")]
    public abstract string? ConcatNeoRight();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAJOMwwDTmVvStgmBUUMAFBK2CYFRQwAi9soQA==
    /// PUSHDATA1 4E33 'N3' [8 datoshi]
    /// PUSHDATA1 4E656F 'Neo' [8 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatNormal")]
    public abstract string? ConcatNormal();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0rYJgVFDABA
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatNullLeft")]
    public abstract string? ConcatNullLeft();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0rYJgVFDABA
    /// PUSHNULL [1 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatNullRight")]
    public abstract string? ConcatNullRight();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDADbKEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatRight")]
    public abstract string? ConcatRight(string? value);

    #endregion
}
