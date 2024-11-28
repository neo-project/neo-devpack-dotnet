using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_WriteInTry(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_WriteInTry"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""baseTry"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""tryWrite"",""parameters"":[],""returntype"":""Void"",""offset"":102,""safe"":false},{""name"":""tryWriteWithVulnerability"",""parameters"":[],""returntype"":""Void"",""offset"":167,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALVXAQA7Byk0ND0xcDsABzRDPQAMF3Rocm93IGluIG5lc3RlZCBmaW5hbGx5OjsKAAwBADQlPQRwOD9AEAwBADQDQFcAAnl4QZv2Z85B5j8YhEAMAQA0A0BXAAF4QZv2Z85BL1jF7UBXAQA7HQA0zgwVdGhyb3cgaW4gVHJ5V3JpdGUgdHJ5OnA7AB80xwwXdGhyb3cgaW4gVHJ5V3JpdGUgY2F0Y2g6P1cBADsHADSkPQVwPQJAQC9ZHQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOwcpNDQ9MXA7AAc0Qz0ADBd0aHJvdyBpbiBuZXN0ZWQgZmluYWxseTo7CgAMAQA0JT0EcDg/QA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 0729 [4 datoshi]
    /// 06 : CALL 34 [512 datoshi]
    /// 08 : ENDTRY 31 [4 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : TRY 0007 [4 datoshi]
    /// 0E : CALL 43 [512 datoshi]
    /// 10 : ENDTRY 00 [4 datoshi]
    /// 12 : PUSHDATA1 7468726F7720696E206E65737465642066696E616C6C79 [8 datoshi]
    /// 2B : THROW [512 datoshi]
    /// 2C : TRY 0A00 [4 datoshi]
    /// 2F : PUSHDATA1 00 [8 datoshi]
    /// 32 : CALL 25 [512 datoshi]
    /// 34 : ENDTRY 04 [4 datoshi]
    /// 36 : STLOC0 [2 datoshi]
    /// 37 : ABORT [0 datoshi]
    /// 38 : ENDFINALLY [4 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("baseTry")]
    public abstract void BaseTry();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOx0ANM4MFXRocm93IGluIFRyeVdyaXRlIHRyeTpwOwAfNMcMF3Rocm93IGluIFRyeVdyaXRlIGNhdGNoOj8=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 1D00 [4 datoshi]
    /// 06 : CALL CE [512 datoshi]
    /// 08 : PUSHDATA1 7468726F7720696E20547279577269746520747279 [8 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : STLOC0 [2 datoshi]
    /// 21 : TRY 001F [4 datoshi]
    /// 24 : CALL C7 [512 datoshi]
    /// 26 : PUSHDATA1 7468726F7720696E205472795772697465206361746368 [8 datoshi]
    /// 3F : THROW [512 datoshi]
    /// 40 : ENDFINALLY [4 datoshi]
    /// </remarks>
    [DisplayName("tryWrite")]
    public abstract void TryWrite();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAOwcANKQ9BXA9AkA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : TRY 0700 [4 datoshi]
    /// 06 : CALL A4 [512 datoshi]
    /// 08 : ENDTRY 05 [4 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : ENDTRY 02 [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWriteWithVulnerability")]
    public abstract void TryWriteWithVulnerability();

    #endregion
}
