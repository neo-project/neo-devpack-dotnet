using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testStaticPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""testPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""testPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":45,""safe"":false},{""name"":""incTestStaticFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":57,""safe"":false},{""name"":""incTestStaticFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""incTestFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":71,""safe"":false},{""name"":""incTestFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""publicStaticProperty"",""parameters"":[],""returntype"":""String"",""offset"":84,""safe"":false},{""name"":""setPublicStaticProperty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":94,""safe"":false},{""name"":""publicProperty"",""parameters"":[],""returntype"":""String"",""offset"":2240,""safe"":false},{""name"":""setPublicProperty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":2258,""safe"":false},{""name"":""uninitializedProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":2276,""safe"":false},{""name"":""setUninitializedProperty"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":2294,""safe"":false},{""name"":""uninitializedStaticProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":111,""safe"":false},{""name"":""setUninitializedStaticProperty"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":113,""safe"":false},{""name"":""computedProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":2312,""safe"":false},{""name"":""testStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":125,""safe"":false},{""name"":""testStaticPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":164,""safe"":false},{""name"":""testStaticPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":203,""safe"":false},{""name"":""testPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2330,""safe"":false},{""name"":""testPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2348,""safe"":false},{""name"":""testPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2366,""safe"":false},{""name"":""uninitializedPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2384,""safe"":false},{""name"":""uninitializedPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2402,""safe"":false},{""name"":""uninitializedPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2420,""safe"":false},{""name"":""uninitializedStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2438,""safe"":false},{""name"":""uninitializedStaticPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2456,""safe"":false},{""name"":""uninitializedStaticPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2474,""safe"":false},{""name"":""concatPublicProperties"",""parameters"":[],""returntype"":""String"",""offset"":2492,""safe"":false},{""name"":""toggleProtectedProperty"",""parameters"":[],""returntype"":""Boolean"",""offset"":2510,""safe"":false},{""name"":""getComputedValue"",""parameters"":[],""returntype"":""Integer"",""offset"":2528,""safe"":false},{""name"":""reset"",""parameters"":[],""returntype"":""Void"",""offset"":2546,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2199,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0FCgwLVG9rZW5TeW1ib2xAWEqcYEVZSpxhRVhAWUqcYUVZQFpKnGJFWkqcYkVaQFtKnGNFW0qcY0VbQFxKnGRFXEBdSpxlRV1AXkqcZkVeQBJnB18HQAwHSW5pdGlhbEBnCUARzkARUdBAFM5AFFHQQBBAZwtAVwABeBDOEqBAXwhKnGcIRV8ISpxnCEVfCEqcZwhFXwicZwhfCJxnCF8InGcIXwhAXwhKnWcIRV8ISp1nCEVfCEqdZwhFXwidZwhfCJ1nCF8InWcIXwhAXwgSoGcIXwgSoGcIXwgSoGcIXwhAVwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeEoQzpxOUBBR0EV4ShDOnE5QEFHQRXhKEM6cTlAQUdBFeBDOQFcAAXhKEM5OnVAQUdBFeEoQzk6dUBBR0EV4ShDOTp1QEFHQRXhKEM6dTlAQUdBFeEoQzp1OUBBR0EV4ShDOnU5QEFHQRXgQzkBXAAF4ShDOEqBOUBBR0EV4ShDOEqBOUBBR0EV4ShDOEqBOUBBR0EV4EM5AVwABeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQFcAAXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXgUzkBXAAF4ShTOEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzhKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM4SoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OUBRR0EV4FM5AVwABXwtKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABXwtKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABXwsSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC18LEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtfCxKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABeBHODAdJbml0aWFsi9soQFcAAXgSzqpKeBJR0EV4Es5AVwABeDUi+P//QFcAARBnCAwHSW5pdGlhbGcJAGRnChBKeBBR0EUMBFRlc3RKeBFR0EUJSngSUdBFACpKeBNR0EUQSngUUdBFQFYMEGAaYRBiG2MQZBFlEGYSZwcQZwgMB0luaXRpYWxnCQBkZwoQZwtAEAAqCQwEVGVzdBAVwCOU9///EAAqCQwEVGVzdBAVwCOF9///EAAqCQwEVGVzdBAVwCN39///EAAqCQwEVGVzdBAVwCNo9///EAAqCQwEVGVzdBAVwCNf9///EAAqCQwEVGVzdBAVwCO59///EAAqCQwEVGVzdBAVwCPw9///EAAqCQwEVGVzdBAVwCMn+P//EAAqCQwEVGVzdBAVwCNA+P//EAAqCQwEVGVzdBAVwCOL+f//EAAqCQwEVGVzdBAVwCPW+v//EAAqCQwEVGVzdBAVwCN5+///EAAqCQwEVGVzdBAVwCOl/P//EAAqCQwEVGVzdBAVwCPR/f//EAAqCQwEVGVzdBAVwCNh/v//EAAqCQwEVGVzdBAVwCNi/v//EAAqCQwEVGVzdBAVwCNh/v//EAAqCQwEVGVzdBAVwCNZ/v//QBOg6vQ="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBDOEqBA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("computedProperty")]
    public abstract BigInteger? ComputedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBHODAdJbml0aWFsi9soQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatPublicProperties")]
    public abstract string? ConcatPublicProperties();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUi+P//QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 22F8FFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getComputedValue")]
    public abstract BigInteger? GetComputedValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XkqcZkVeQA==
    /// LDSFLD6 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD6 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD6 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldDefault")]
    public abstract BigInteger? IncTestFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EmcHXwdA
    /// PUSH2 [1 datoshi]
    /// STSFLD 07 [2 datoshi]
    /// LDSFLD 07 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldValue")]
    public abstract BigInteger? IncTestFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XEqcZEVcQA==
    /// LDSFLD4 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD4 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD4 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldDefault")]
    public abstract BigInteger? IncTestStaticFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XUqcZUVdQA==
    /// LDSFLD5 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD5 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD5 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldValue")]
    public abstract BigInteger? IncTestStaticFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Ec5A
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("publicProperty")]
    public abstract string? PublicProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAdJbml0aWFsQA==
    /// PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("publicStaticProperty")]
    public abstract string? PublicStaticProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEGcIDAdJbml0aWFsZwkAZGcKEEp4EFHQRQwEVGVzdEp4EVHQRQlKeBJR0EUAKkp4E1HQRRBKeBRR0EVA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// STSFLD 09 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// STSFLD 0A [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 54657374 'Test' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("reset")]
    public abstract void Reset();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EVHQQA==
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("setPublicProperty")]
    public abstract void SetPublicProperty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ZwlA
    /// STSFLD 09 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("setPublicStaticProperty")]
    public abstract void SetPublicStaticProperty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FFHQQA==
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("setUninitializedProperty")]
    public abstract void SetUninitializedProperty(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ZwtA
    /// STSFLD 0B [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("setUninitializedStaticProperty")]
    public abstract void SetUninitializedStaticProperty(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtUb2tlblN5bWJvbEA=
    /// PUSHDATA1 546F6B656E53796D626F6C 'TokenSymbol' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6dUBBR0EV4ShDOTp1QEFHQRXhKEM5OnVAQUdBFeEoQzp1OUBBR0EV4ShDOnU5QEFHQRXhKEM6dTlAQUdBFeBDOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDec")]
    public abstract BigInteger? TestPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkqcYkVaSpxiRVpA
    /// LDSFLD2 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD2 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDefaultInc")]
    public abstract BigInteger? TestPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeEoQzpxOUBBR0EV4ShDOnE5QEFHQRXhKEM6cTlAQUdBFeBDOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInc")]
    public abstract BigInteger? TestPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzhKgTlAQUdBFeEoQzhKgTlAQUdBFeEoQzhKgTlAQUdBFeBDOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyMul")]
    public abstract BigInteger? TestPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0qcY0VbSpxjRVtA
    /// LDSFLD3 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD3 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD3 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyValueInc")]
    public abstract BigInteger? TestPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwhKnWcIRV8ISp1nCEVfCEqdZwhFXwidZwhfCJ1nCF8InWcIXwhA
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DEC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDec")]
    public abstract BigInteger? TestStaticPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVZSpxhRVhA
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDefaultInc")]
    public abstract BigInteger? TestStaticPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwhKnGcIRV8ISpxnCEVfCEqcZwhFXwicZwhfCJxnCF8InGcIXwhA
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyInc")]
    public abstract BigInteger? TestStaticPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwgSoGcIXwgSoGcIXwgSoGcIXwhA
    /// LDSFLD 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// STSFLD 08 [2 datoshi]
    /// LDSFLD 08 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyMul")]
    public abstract BigInteger? TestStaticPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUqcYUVZQA==
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyValueInc")]
    public abstract BigInteger? TestStaticPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBLOqkp4ElHQRXgSzkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// NOT [4 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toggleProtectedProperty")]
    public abstract bool? ToggleProtectedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FM5A
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedProperty")]
    public abstract BigInteger? UninitializedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyDec")]
    public abstract BigInteger? UninitializedPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyInc")]
    public abstract BigInteger? UninitializedPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzhKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM4SoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OUBRR0EV4ShTOEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// TUCK [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyMul")]
    public abstract BigInteger? UninitializedPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEA=
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticProperty")]
    public abstract BigInteger? UninitializedStaticProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwtKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyDec")]
    public abstract BigInteger? UninitializedStaticPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwtKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyInc")]
    public abstract BigInteger? UninitializedStaticPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwsSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC18LEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtfCxKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD 0B [2 datoshi]
    /// LDSFLD 0B [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyMul")]
    public abstract BigInteger? UninitializedStaticPropertyMul();

    #endregion
}
