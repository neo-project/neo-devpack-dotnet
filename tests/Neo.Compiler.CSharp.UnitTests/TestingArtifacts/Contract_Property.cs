using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testStaticPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""testPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""testPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":45,""safe"":false},{""name"":""incTestStaticFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":57,""safe"":false},{""name"":""incTestStaticFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""incTestFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":71,""safe"":false},{""name"":""incTestFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""publicStaticProperty"",""parameters"":[],""returntype"":""String"",""offset"":84,""safe"":false},{""name"":""setPublicStaticProperty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":94,""safe"":false},{""name"":""publicProperty"",""parameters"":[],""returntype"":""String"",""offset"":2270,""safe"":false},{""name"":""setPublicProperty"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Void"",""offset"":2288,""safe"":false},{""name"":""uninitializedProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":2306,""safe"":false},{""name"":""setUninitializedProperty"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":2324,""safe"":false},{""name"":""uninitializedStaticProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":141,""safe"":false},{""name"":""setUninitializedStaticProperty"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":143,""safe"":false},{""name"":""computedProperty"",""parameters"":[],""returntype"":""Integer"",""offset"":2342,""safe"":false},{""name"":""testStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":155,""safe"":false},{""name"":""testStaticPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":194,""safe"":false},{""name"":""testStaticPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":233,""safe"":false},{""name"":""testPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2360,""safe"":false},{""name"":""testPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2378,""safe"":false},{""name"":""testPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2396,""safe"":false},{""name"":""uninitializedPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2414,""safe"":false},{""name"":""uninitializedPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2432,""safe"":false},{""name"":""uninitializedPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2450,""safe"":false},{""name"":""uninitializedStaticPropertyInc"",""parameters"":[],""returntype"":""Integer"",""offset"":2468,""safe"":false},{""name"":""uninitializedStaticPropertyDec"",""parameters"":[],""returntype"":""Integer"",""offset"":2486,""safe"":false},{""name"":""uninitializedStaticPropertyMul"",""parameters"":[],""returntype"":""Integer"",""offset"":2504,""safe"":false},{""name"":""concatPublicProperties"",""parameters"":[],""returntype"":""String"",""offset"":2522,""safe"":false},{""name"":""toggleProtectedProperty"",""parameters"":[],""returntype"":""Boolean"",""offset"":2540,""safe"":false},{""name"":""getComputedValue"",""parameters"":[],""returntype"":""Integer"",""offset"":2558,""safe"":false},{""name"":""reset"",""parameters"":[],""returntype"":""Void"",""offset"":2576,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2229,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0jCgwLVG9rZW5TeW1ib2xAWEqcYEVZSpxhRVhAWUqcYUVZQFpKnGJFWkqcYkVaQFtKnGNFW0qcY0VbQFxKnGRFXEBdSpxlRV1AXkqcZkVeQBJnB18HQAwHSW5pdGlhbEBnCUARzkBXAAF4EBDQeBEMBFRlc3TQeBIJ0HgTACrQeBQQ0EARUdBAFM5AFFHQQBBAZwtAVwABeBDOEqBAXwhKnGcIRV8ISpxnCEVfCEqcZwhFXwicZwhfCJxnCF8InGcIXwhAXwhKnWcIRV8ISp1nCEVfCEqdZwhFXwidZwhfCJ1nCF8InWcIXwhAXwgSoGcIXwgSoGcIXwgSoGcIXwhAVwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeEoQzpxOUBBR0EV4ShDOnE5QEFHQRXhKEM6cTlAQUdBFeBDOQFcAAXhKEM5OnVAQUdBFeEoQzk6dUBBR0EV4ShDOTp1QEFHQRXhKEM6dTlAQUdBFeEoQzp1OUBBR0EV4ShDOnU5QEFHQRXgQzkBXAAF4ShDOEqBOUBBR0EV4ShDOEqBOUBBR0EV4ShDOEqBOUBBR0EV4EM5AVwABeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQFcAAXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM5OnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXgUzkBXAAF4ShTOEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzhKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM4SoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OUBRR0EV4FM5AVwABXwtKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABXwtKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABXwsSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC18LEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtfCxKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtAVwABeBHODAdJbml0aWFsi9soQFcAAXgSzqpKeBJR0EV4Es5AVwABeDUi+P//QFcAARBnCAwHSW5pdGlhbGcJAGRnChBKeBBR0EUMBFRlc3RKeBFR0EUJSngSUdBFACpKeBNR0EUQSngUUdBFQFYMEGAaYRBiG2MQZBFlEGYSZwcQZwgMB0luaXRpYWxnCQBkZwoQZwtAEBAJCxAVwEo1fvf//yN29///EBAJCxAVwEo1bPf//yOF9///EBAJCxAVwEo1Wvf//yN39///EBAJCxAVwEo1SPf//yNo9///EBAJCxAVwEo1Nvf//yNf9///EBAJCxAVwEo1JPf//yO59///EBAJCxAVwEo1Evf//yPw9///EBAJCxAVwEo1APf//yMn+P//EBAJCxAVwEo17vb//yNA+P//EBAJCxAVwEo13Pb//yOL+f//EBAJCxAVwEo1yvb//yPW+v//EBAJCxAVwEo1uPb//yN5+///EBAJCxAVwEo1pvb//yOl/P//EBAJCxAVwEo1lPb//yPR/f//EBAJCxAVwEo1gvb//yNh/v//EBAJCxAVwEo1cPb//yNi/v//EBAJCxAVwEo1Xvb//yNh/v//EBAJCxAVwEo1TPb//yNZ/v//QMK/nI4=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBDOEqBA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PICKITEM [64 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : MUL [8 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("computedProperty")]
    public abstract BigInteger? ComputedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBHODAdJbml0aWFsi9soQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : PICKITEM [64 datoshi]
    /// 06 : PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// 0F : CAT [2048 datoshi]
    /// 10 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatPublicProperties")]
    public abstract string? ConcatPublicProperties();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDUi+P//QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL_L 22F8FFFF [512 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getComputedValue")]
    public abstract BigInteger? GetComputedValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XkqcZkVeQA==
    /// 00 : LDSFLD6 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD6 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD6 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldDefault")]
    public abstract BigInteger? IncTestFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EmcHXwdA
    /// 00 : PUSH2 [1 datoshi]
    /// 01 : STSFLD 07 [2 datoshi]
    /// 03 : LDSFLD 07 [2 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldValue")]
    public abstract BigInteger? IncTestFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XEqcZEVcQA==
    /// 00 : LDSFLD4 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD4 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD4 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldDefault")]
    public abstract BigInteger? IncTestStaticFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XUqcZUVdQA==
    /// 00 : LDSFLD5 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD5 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD5 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldValue")]
    public abstract BigInteger? IncTestStaticFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Ec5A
    /// 00 : PUSH1 [1 datoshi]
    /// 01 : PICKITEM [64 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("publicProperty")]
    public abstract string? PublicProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAdJbml0aWFsQA==
    /// 00 : PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("publicStaticProperty")]
    public abstract string? PublicStaticProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEGcIDAdJbml0aWFsZwkAZGcKEEp4EFHQRQwEVGVzdEp4EVHQRQlKeBJR0EUAKkp4E1HQRRBKeBRR0EVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STSFLD 08 [2 datoshi]
    /// 06 : PUSHDATA1 496E697469616C 'Initial' [8 datoshi]
    /// 0F : STSFLD 09 [2 datoshi]
    /// 11 : PUSHINT8 64 [1 datoshi]
    /// 13 : STSFLD 0A [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : PUSH0 [1 datoshi]
    /// 19 : ROT [2 datoshi]
    /// 1A : SETITEM [8192 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : PUSHDATA1 54657374 'Test' [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : LDARG0 [2 datoshi]
    /// 24 : PUSH1 [1 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SETITEM [8192 datoshi]
    /// 27 : DROP [2 datoshi]
    /// 28 : PUSHF [1 datoshi]
    /// 29 : DUP [2 datoshi]
    /// 2A : LDARG0 [2 datoshi]
    /// 2B : PUSH2 [1 datoshi]
    /// 2C : ROT [2 datoshi]
    /// 2D : SETITEM [8192 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHINT8 2A [1 datoshi]
    /// 31 : DUP [2 datoshi]
    /// 32 : LDARG0 [2 datoshi]
    /// 33 : PUSH3 [1 datoshi]
    /// 34 : ROT [2 datoshi]
    /// 35 : SETITEM [8192 datoshi]
    /// 36 : DROP [2 datoshi]
    /// 37 : PUSH0 [1 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : LDARG0 [2 datoshi]
    /// 3A : PUSH4 [1 datoshi]
    /// 3B : ROT [2 datoshi]
    /// 3C : SETITEM [8192 datoshi]
    /// 3D : DROP [2 datoshi]
    /// 3E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("reset")]
    public abstract void Reset();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EVHQQA==
    /// 00 : PUSH1 [1 datoshi]
    /// 01 : ROT [2 datoshi]
    /// 02 : SETITEM [8192 datoshi]
    /// 03 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("setPublicProperty")]
    public abstract void SetPublicProperty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ZwlA
    /// 00 : STSFLD 09 [2 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("setPublicStaticProperty")]
    public abstract void SetPublicStaticProperty(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FFHQQA==
    /// 00 : PUSH4 [1 datoshi]
    /// 01 : ROT [2 datoshi]
    /// 02 : SETITEM [8192 datoshi]
    /// 03 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("setUninitializedProperty")]
    public abstract void SetUninitializedProperty(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ZwtA
    /// 00 : STSFLD 0B [2 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("setUninitializedStaticProperty")]
    public abstract void SetUninitializedStaticProperty(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtUb2tlblN5bWJvbEA=
    /// 00 : PUSHDATA1 546F6B656E53796D626F6C 'TokenSymbol' [8 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6dUBBR0EV4ShDOTp1QEFHQRXhKEM5OnVAQUdBFeEoQzp1OUBBR0EV4ShDOnU5QEFHQRXhKEM6dTlAQUdBFeBDOQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : TUCK [2 datoshi]
    /// 08 : DEC [4 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : SETITEM [8192 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : LDARG0 [2 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSH0 [1 datoshi]
    /// 11 : PICKITEM [64 datoshi]
    /// 12 : TUCK [2 datoshi]
    /// 13 : DEC [4 datoshi]
    /// 14 : SWAP [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDARG0 [2 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSH0 [1 datoshi]
    /// 1C : PICKITEM [64 datoshi]
    /// 1D : TUCK [2 datoshi]
    /// 1E : DEC [4 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : PUSH0 [1 datoshi]
    /// 21 : ROT [2 datoshi]
    /// 22 : SETITEM [8192 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : LDARG0 [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : PICKITEM [64 datoshi]
    /// 28 : DEC [4 datoshi]
    /// 29 : TUCK [2 datoshi]
    /// 2A : SWAP [2 datoshi]
    /// 2B : PUSH0 [1 datoshi]
    /// 2C : ROT [2 datoshi]
    /// 2D : SETITEM [8192 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : DUP [2 datoshi]
    /// 31 : PUSH0 [1 datoshi]
    /// 32 : PICKITEM [64 datoshi]
    /// 33 : DEC [4 datoshi]
    /// 34 : TUCK [2 datoshi]
    /// 35 : SWAP [2 datoshi]
    /// 36 : PUSH0 [1 datoshi]
    /// 37 : ROT [2 datoshi]
    /// 38 : SETITEM [8192 datoshi]
    /// 39 : DROP [2 datoshi]
    /// 3A : LDARG0 [2 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : PUSH0 [1 datoshi]
    /// 3D : PICKITEM [64 datoshi]
    /// 3E : DEC [4 datoshi]
    /// 3F : TUCK [2 datoshi]
    /// 40 : SWAP [2 datoshi]
    /// 41 : PUSH0 [1 datoshi]
    /// 42 : ROT [2 datoshi]
    /// 43 : SETITEM [8192 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : LDARG0 [2 datoshi]
    /// 46 : PUSH0 [1 datoshi]
    /// 47 : PICKITEM [64 datoshi]
    /// 48 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDec")]
    public abstract BigInteger? TestPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkqcYkVaSpxiRVpA
    /// 00 : LDSFLD2 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD2 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD2 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD2 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD2 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDefaultInc")]
    public abstract BigInteger? TestPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzk6cUBBR0EV4ShDOTpxQEFHQRXhKEM5OnFAQUdBFeEoQzpxOUBBR0EV4ShDOnE5QEFHQRXhKEM6cTlAQUdBFeBDOQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : TUCK [2 datoshi]
    /// 08 : INC [4 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : SETITEM [8192 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : LDARG0 [2 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSH0 [1 datoshi]
    /// 11 : PICKITEM [64 datoshi]
    /// 12 : TUCK [2 datoshi]
    /// 13 : INC [4 datoshi]
    /// 14 : SWAP [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDARG0 [2 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSH0 [1 datoshi]
    /// 1C : PICKITEM [64 datoshi]
    /// 1D : TUCK [2 datoshi]
    /// 1E : INC [4 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : PUSH0 [1 datoshi]
    /// 21 : ROT [2 datoshi]
    /// 22 : SETITEM [8192 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : LDARG0 [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : PICKITEM [64 datoshi]
    /// 28 : INC [4 datoshi]
    /// 29 : TUCK [2 datoshi]
    /// 2A : SWAP [2 datoshi]
    /// 2B : PUSH0 [1 datoshi]
    /// 2C : ROT [2 datoshi]
    /// 2D : SETITEM [8192 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : DUP [2 datoshi]
    /// 31 : PUSH0 [1 datoshi]
    /// 32 : PICKITEM [64 datoshi]
    /// 33 : INC [4 datoshi]
    /// 34 : TUCK [2 datoshi]
    /// 35 : SWAP [2 datoshi]
    /// 36 : PUSH0 [1 datoshi]
    /// 37 : ROT [2 datoshi]
    /// 38 : SETITEM [8192 datoshi]
    /// 39 : DROP [2 datoshi]
    /// 3A : LDARG0 [2 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : PUSH0 [1 datoshi]
    /// 3D : PICKITEM [64 datoshi]
    /// 3E : INC [4 datoshi]
    /// 3F : TUCK [2 datoshi]
    /// 40 : SWAP [2 datoshi]
    /// 41 : PUSH0 [1 datoshi]
    /// 42 : ROT [2 datoshi]
    /// 43 : SETITEM [8192 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : LDARG0 [2 datoshi]
    /// 46 : PUSH0 [1 datoshi]
    /// 47 : PICKITEM [64 datoshi]
    /// 48 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInc")]
    public abstract BigInteger? TestPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQzhKgTlAQUdBFeEoQzhKgTlAQUdBFeEoQzhKgTlAQUdBFeBDOQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : PUSH2 [1 datoshi]
    /// 08 : MUL [8 datoshi]
    /// 09 : TUCK [2 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : ROT [2 datoshi]
    /// 0D : SETITEM [8192 datoshi]
    /// 0E : DROP [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PICKITEM [64 datoshi]
    /// 13 : PUSH2 [1 datoshi]
    /// 14 : MUL [8 datoshi]
    /// 15 : TUCK [2 datoshi]
    /// 16 : SWAP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : SETITEM [8192 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSH0 [1 datoshi]
    /// 1E : PICKITEM [64 datoshi]
    /// 1F : PUSH2 [1 datoshi]
    /// 20 : MUL [8 datoshi]
    /// 21 : TUCK [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : PUSH0 [1 datoshi]
    /// 24 : ROT [2 datoshi]
    /// 25 : SETITEM [8192 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : LDARG0 [2 datoshi]
    /// 28 : PUSH0 [1 datoshi]
    /// 29 : PICKITEM [64 datoshi]
    /// 2A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyMul")]
    public abstract BigInteger? TestPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0qcY0VbSpxjRVtA
    /// 00 : LDSFLD3 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD3 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD3 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD3 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD3 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyValueInc")]
    public abstract BigInteger? TestPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwhKnWcIRV8ISp1nCEVfCEqdZwhFXwidZwhfCJ1nCF8InWcIXwhA
    /// 00 : LDSFLD 08 [2 datoshi]
    /// 02 : DUP [2 datoshi]
    /// 03 : DEC [4 datoshi]
    /// 04 : STSFLD 08 [2 datoshi]
    /// 06 : DROP [2 datoshi]
    /// 07 : LDSFLD 08 [2 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : DEC [4 datoshi]
    /// 0B : STSFLD 08 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : LDSFLD 08 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : DEC [4 datoshi]
    /// 12 : STSFLD 08 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : LDSFLD 08 [2 datoshi]
    /// 17 : DEC [4 datoshi]
    /// 18 : STSFLD 08 [2 datoshi]
    /// 1A : LDSFLD 08 [2 datoshi]
    /// 1C : DEC [4 datoshi]
    /// 1D : STSFLD 08 [2 datoshi]
    /// 1F : LDSFLD 08 [2 datoshi]
    /// 21 : DEC [4 datoshi]
    /// 22 : STSFLD 08 [2 datoshi]
    /// 24 : LDSFLD 08 [2 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDec")]
    public abstract BigInteger? TestStaticPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEqcYEVZSpxhRVhA
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD0 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : STSFLD1 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : LDSFLD0 [2 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDefaultInc")]
    public abstract BigInteger? TestStaticPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwhKnGcIRV8ISpxnCEVfCEqcZwhFXwicZwhfCJxnCF8InGcIXwhA
    /// 00 : LDSFLD 08 [2 datoshi]
    /// 02 : DUP [2 datoshi]
    /// 03 : INC [4 datoshi]
    /// 04 : STSFLD 08 [2 datoshi]
    /// 06 : DROP [2 datoshi]
    /// 07 : LDSFLD 08 [2 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : INC [4 datoshi]
    /// 0B : STSFLD 08 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : LDSFLD 08 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : INC [4 datoshi]
    /// 12 : STSFLD 08 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : LDSFLD 08 [2 datoshi]
    /// 17 : INC [4 datoshi]
    /// 18 : STSFLD 08 [2 datoshi]
    /// 1A : LDSFLD 08 [2 datoshi]
    /// 1C : INC [4 datoshi]
    /// 1D : STSFLD 08 [2 datoshi]
    /// 1F : LDSFLD 08 [2 datoshi]
    /// 21 : INC [4 datoshi]
    /// 22 : STSFLD 08 [2 datoshi]
    /// 24 : LDSFLD 08 [2 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyInc")]
    public abstract BigInteger? TestStaticPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XwgSoGcIXwgSoGcIXwgSoGcIXwhA
    /// 00 : LDSFLD 08 [2 datoshi]
    /// 02 : PUSH2 [1 datoshi]
    /// 03 : MUL [8 datoshi]
    /// 04 : STSFLD 08 [2 datoshi]
    /// 06 : LDSFLD 08 [2 datoshi]
    /// 08 : PUSH2 [1 datoshi]
    /// 09 : MUL [8 datoshi]
    /// 0A : STSFLD 08 [2 datoshi]
    /// 0C : LDSFLD 08 [2 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : MUL [8 datoshi]
    /// 10 : STSFLD 08 [2 datoshi]
    /// 12 : LDSFLD 08 [2 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyMul")]
    public abstract BigInteger? TestStaticPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUqcYUVZQA==
    /// 00 : LDSFLD1 [2 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : INC [4 datoshi]
    /// 03 : STSFLD1 [2 datoshi]
    /// 04 : DROP [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyValueInc")]
    public abstract BigInteger? TestStaticPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBLOqkp4ElHQRXgSzkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : PICKITEM [64 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : ROT [2 datoshi]
    /// 0B : SETITEM [8192 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PICKITEM [64 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("toggleProtectedProperty")]
    public abstract bool? ToggleProtectedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FM5A
    /// 00 : PUSH4 [1 datoshi]
    /// 01 : PICKITEM [64 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedProperty")]
    public abstract BigInteger? UninitializedProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6dSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// 0000 : INITSLOT 0001 [64 datoshi]
    /// 0003 : LDARG0 [2 datoshi]
    /// 0004 : DUP [2 datoshi]
    /// 0005 : PUSH4 [1 datoshi]
    /// 0006 : PICKITEM [64 datoshi]
    /// 0007 : TUCK [2 datoshi]
    /// 0008 : DEC [4 datoshi]
    /// 0009 : DUP [2 datoshi]
    /// 000A : PUSHINT32 00000080 [1 datoshi]
    /// 000F : JMPGE 04 [2 datoshi]
    /// 0011 : JMP 0A [2 datoshi]
    /// 0013 : DUP [2 datoshi]
    /// 0014 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0019 : JMPLE 1E [2 datoshi]
    /// 001B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0024 : AND [8 datoshi]
    /// 0025 : DUP [2 datoshi]
    /// 0026 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 002B : JMPLE 0C [2 datoshi]
    /// 002D : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0036 : SUB [8 datoshi]
    /// 0037 : SWAP [2 datoshi]
    /// 0038 : PUSH4 [1 datoshi]
    /// 0039 : ROT [2 datoshi]
    /// 003A : SETITEM [8192 datoshi]
    /// 003B : DROP [2 datoshi]
    /// 003C : LDARG0 [2 datoshi]
    /// 003D : DUP [2 datoshi]
    /// 003E : PUSH4 [1 datoshi]
    /// 003F : PICKITEM [64 datoshi]
    /// 0040 : TUCK [2 datoshi]
    /// 0041 : DEC [4 datoshi]
    /// 0042 : DUP [2 datoshi]
    /// 0043 : PUSHINT32 00000080 [1 datoshi]
    /// 0048 : JMPGE 04 [2 datoshi]
    /// 004A : JMP 0A [2 datoshi]
    /// 004C : DUP [2 datoshi]
    /// 004D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0052 : JMPLE 1E [2 datoshi]
    /// 0054 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 005D : AND [8 datoshi]
    /// 005E : DUP [2 datoshi]
    /// 005F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0064 : JMPLE 0C [2 datoshi]
    /// 0066 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 006F : SUB [8 datoshi]
    /// 0070 : SWAP [2 datoshi]
    /// 0071 : PUSH4 [1 datoshi]
    /// 0072 : ROT [2 datoshi]
    /// 0073 : SETITEM [8192 datoshi]
    /// 0074 : DROP [2 datoshi]
    /// 0075 : LDARG0 [2 datoshi]
    /// 0076 : DUP [2 datoshi]
    /// 0077 : PUSH4 [1 datoshi]
    /// 0078 : PICKITEM [64 datoshi]
    /// 0079 : TUCK [2 datoshi]
    /// 007A : DEC [4 datoshi]
    /// 007B : DUP [2 datoshi]
    /// 007C : PUSHINT32 00000080 [1 datoshi]
    /// 0081 : JMPGE 04 [2 datoshi]
    /// 0083 : JMP 0A [2 datoshi]
    /// 0085 : DUP [2 datoshi]
    /// 0086 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 008B : JMPLE 1E [2 datoshi]
    /// 008D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0096 : AND [8 datoshi]
    /// 0097 : DUP [2 datoshi]
    /// 0098 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 009D : JMPLE 0C [2 datoshi]
    /// 009F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00A8 : SUB [8 datoshi]
    /// 00A9 : SWAP [2 datoshi]
    /// 00AA : PUSH4 [1 datoshi]
    /// 00AB : ROT [2 datoshi]
    /// 00AC : SETITEM [8192 datoshi]
    /// 00AD : DROP [2 datoshi]
    /// 00AE : LDARG0 [2 datoshi]
    /// 00AF : DUP [2 datoshi]
    /// 00B0 : PUSH4 [1 datoshi]
    /// 00B1 : PICKITEM [64 datoshi]
    /// 00B2 : DEC [4 datoshi]
    /// 00B3 : DUP [2 datoshi]
    /// 00B4 : PUSHINT32 00000080 [1 datoshi]
    /// 00B9 : JMPGE 04 [2 datoshi]
    /// 00BB : JMP 0A [2 datoshi]
    /// 00BD : DUP [2 datoshi]
    /// 00BE : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00C3 : JMPLE 1E [2 datoshi]
    /// 00C5 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00CE : AND [8 datoshi]
    /// 00CF : DUP [2 datoshi]
    /// 00D0 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00D5 : JMPLE 0C [2 datoshi]
    /// 00D7 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00E0 : SUB [8 datoshi]
    /// 00E1 : TUCK [2 datoshi]
    /// 00E2 : SWAP [2 datoshi]
    /// 00E3 : PUSH4 [1 datoshi]
    /// 00E4 : ROT [2 datoshi]
    /// 00E5 : SETITEM [8192 datoshi]
    /// 00E6 : DROP [2 datoshi]
    /// 00E7 : LDARG0 [2 datoshi]
    /// 00E8 : DUP [2 datoshi]
    /// 00E9 : PUSH4 [1 datoshi]
    /// 00EA : PICKITEM [64 datoshi]
    /// 00EB : DEC [4 datoshi]
    /// 00EC : DUP [2 datoshi]
    /// 00ED : PUSHINT32 00000080 [1 datoshi]
    /// 00F2 : JMPGE 04 [2 datoshi]
    /// 00F4 : JMP 0A [2 datoshi]
    /// 00F6 : DUP [2 datoshi]
    /// 00F7 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00FC : JMPLE 1E [2 datoshi]
    /// 00FE : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0107 : AND [8 datoshi]
    /// 0108 : DUP [2 datoshi]
    /// 0109 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 010E : JMPLE 0C [2 datoshi]
    /// 0110 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0119 : SUB [8 datoshi]
    /// 011A : TUCK [2 datoshi]
    /// 011B : SWAP [2 datoshi]
    /// 011C : PUSH4 [1 datoshi]
    /// 011D : ROT [2 datoshi]
    /// 011E : SETITEM [8192 datoshi]
    /// 011F : DROP [2 datoshi]
    /// 0120 : LDARG0 [2 datoshi]
    /// 0121 : DUP [2 datoshi]
    /// 0122 : PUSH4 [1 datoshi]
    /// 0123 : PICKITEM [64 datoshi]
    /// 0124 : DEC [4 datoshi]
    /// 0125 : DUP [2 datoshi]
    /// 0126 : PUSHINT32 00000080 [1 datoshi]
    /// 012B : JMPGE 04 [2 datoshi]
    /// 012D : JMP 0A [2 datoshi]
    /// 012F : DUP [2 datoshi]
    /// 0130 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0135 : JMPLE 1E [2 datoshi]
    /// 0137 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0140 : AND [8 datoshi]
    /// 0141 : DUP [2 datoshi]
    /// 0142 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0147 : JMPLE 0C [2 datoshi]
    /// 0149 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0152 : SUB [8 datoshi]
    /// 0153 : TUCK [2 datoshi]
    /// 0154 : SWAP [2 datoshi]
    /// 0155 : PUSH4 [1 datoshi]
    /// 0156 : ROT [2 datoshi]
    /// 0157 : SETITEM [8192 datoshi]
    /// 0158 : DROP [2 datoshi]
    /// 0159 : LDARG0 [2 datoshi]
    /// 015A : PUSH4 [1 datoshi]
    /// 015B : PICKITEM [64 datoshi]
    /// 015C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyDec")]
    public abstract BigInteger? UninitializedPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzk6cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn1AUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeEoUzpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// 0000 : INITSLOT 0001 [64 datoshi]
    /// 0003 : LDARG0 [2 datoshi]
    /// 0004 : DUP [2 datoshi]
    /// 0005 : PUSH4 [1 datoshi]
    /// 0006 : PICKITEM [64 datoshi]
    /// 0007 : TUCK [2 datoshi]
    /// 0008 : INC [4 datoshi]
    /// 0009 : DUP [2 datoshi]
    /// 000A : PUSHINT32 00000080 [1 datoshi]
    /// 000F : JMPGE 04 [2 datoshi]
    /// 0011 : JMP 0A [2 datoshi]
    /// 0013 : DUP [2 datoshi]
    /// 0014 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0019 : JMPLE 1E [2 datoshi]
    /// 001B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0024 : AND [8 datoshi]
    /// 0025 : DUP [2 datoshi]
    /// 0026 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 002B : JMPLE 0C [2 datoshi]
    /// 002D : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0036 : SUB [8 datoshi]
    /// 0037 : SWAP [2 datoshi]
    /// 0038 : PUSH4 [1 datoshi]
    /// 0039 : ROT [2 datoshi]
    /// 003A : SETITEM [8192 datoshi]
    /// 003B : DROP [2 datoshi]
    /// 003C : LDARG0 [2 datoshi]
    /// 003D : DUP [2 datoshi]
    /// 003E : PUSH4 [1 datoshi]
    /// 003F : PICKITEM [64 datoshi]
    /// 0040 : TUCK [2 datoshi]
    /// 0041 : INC [4 datoshi]
    /// 0042 : DUP [2 datoshi]
    /// 0043 : PUSHINT32 00000080 [1 datoshi]
    /// 0048 : JMPGE 04 [2 datoshi]
    /// 004A : JMP 0A [2 datoshi]
    /// 004C : DUP [2 datoshi]
    /// 004D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0052 : JMPLE 1E [2 datoshi]
    /// 0054 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 005D : AND [8 datoshi]
    /// 005E : DUP [2 datoshi]
    /// 005F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0064 : JMPLE 0C [2 datoshi]
    /// 0066 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 006F : SUB [8 datoshi]
    /// 0070 : SWAP [2 datoshi]
    /// 0071 : PUSH4 [1 datoshi]
    /// 0072 : ROT [2 datoshi]
    /// 0073 : SETITEM [8192 datoshi]
    /// 0074 : DROP [2 datoshi]
    /// 0075 : LDARG0 [2 datoshi]
    /// 0076 : DUP [2 datoshi]
    /// 0077 : PUSH4 [1 datoshi]
    /// 0078 : PICKITEM [64 datoshi]
    /// 0079 : TUCK [2 datoshi]
    /// 007A : INC [4 datoshi]
    /// 007B : DUP [2 datoshi]
    /// 007C : PUSHINT32 00000080 [1 datoshi]
    /// 0081 : JMPGE 04 [2 datoshi]
    /// 0083 : JMP 0A [2 datoshi]
    /// 0085 : DUP [2 datoshi]
    /// 0086 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 008B : JMPLE 1E [2 datoshi]
    /// 008D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0096 : AND [8 datoshi]
    /// 0097 : DUP [2 datoshi]
    /// 0098 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 009D : JMPLE 0C [2 datoshi]
    /// 009F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00A8 : SUB [8 datoshi]
    /// 00A9 : SWAP [2 datoshi]
    /// 00AA : PUSH4 [1 datoshi]
    /// 00AB : ROT [2 datoshi]
    /// 00AC : SETITEM [8192 datoshi]
    /// 00AD : DROP [2 datoshi]
    /// 00AE : LDARG0 [2 datoshi]
    /// 00AF : DUP [2 datoshi]
    /// 00B0 : PUSH4 [1 datoshi]
    /// 00B1 : PICKITEM [64 datoshi]
    /// 00B2 : INC [4 datoshi]
    /// 00B3 : DUP [2 datoshi]
    /// 00B4 : PUSHINT32 00000080 [1 datoshi]
    /// 00B9 : JMPGE 04 [2 datoshi]
    /// 00BB : JMP 0A [2 datoshi]
    /// 00BD : DUP [2 datoshi]
    /// 00BE : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00C3 : JMPLE 1E [2 datoshi]
    /// 00C5 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00CE : AND [8 datoshi]
    /// 00CF : DUP [2 datoshi]
    /// 00D0 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00D5 : JMPLE 0C [2 datoshi]
    /// 00D7 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00E0 : SUB [8 datoshi]
    /// 00E1 : TUCK [2 datoshi]
    /// 00E2 : SWAP [2 datoshi]
    /// 00E3 : PUSH4 [1 datoshi]
    /// 00E4 : ROT [2 datoshi]
    /// 00E5 : SETITEM [8192 datoshi]
    /// 00E6 : DROP [2 datoshi]
    /// 00E7 : LDARG0 [2 datoshi]
    /// 00E8 : DUP [2 datoshi]
    /// 00E9 : PUSH4 [1 datoshi]
    /// 00EA : PICKITEM [64 datoshi]
    /// 00EB : INC [4 datoshi]
    /// 00EC : DUP [2 datoshi]
    /// 00ED : PUSHINT32 00000080 [1 datoshi]
    /// 00F2 : JMPGE 04 [2 datoshi]
    /// 00F4 : JMP 0A [2 datoshi]
    /// 00F6 : DUP [2 datoshi]
    /// 00F7 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00FC : JMPLE 1E [2 datoshi]
    /// 00FE : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0107 : AND [8 datoshi]
    /// 0108 : DUP [2 datoshi]
    /// 0109 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 010E : JMPLE 0C [2 datoshi]
    /// 0110 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0119 : SUB [8 datoshi]
    /// 011A : TUCK [2 datoshi]
    /// 011B : SWAP [2 datoshi]
    /// 011C : PUSH4 [1 datoshi]
    /// 011D : ROT [2 datoshi]
    /// 011E : SETITEM [8192 datoshi]
    /// 011F : DROP [2 datoshi]
    /// 0120 : LDARG0 [2 datoshi]
    /// 0121 : DUP [2 datoshi]
    /// 0122 : PUSH4 [1 datoshi]
    /// 0123 : PICKITEM [64 datoshi]
    /// 0124 : INC [4 datoshi]
    /// 0125 : DUP [2 datoshi]
    /// 0126 : PUSHINT32 00000080 [1 datoshi]
    /// 012B : JMPGE 04 [2 datoshi]
    /// 012D : JMP 0A [2 datoshi]
    /// 012F : DUP [2 datoshi]
    /// 0130 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0135 : JMPLE 1E [2 datoshi]
    /// 0137 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0140 : AND [8 datoshi]
    /// 0141 : DUP [2 datoshi]
    /// 0142 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0147 : JMPLE 0C [2 datoshi]
    /// 0149 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0152 : SUB [8 datoshi]
    /// 0153 : TUCK [2 datoshi]
    /// 0154 : SWAP [2 datoshi]
    /// 0155 : PUSH4 [1 datoshi]
    /// 0156 : ROT [2 datoshi]
    /// 0157 : SETITEM [8192 datoshi]
    /// 0158 : DROP [2 datoshi]
    /// 0159 : LDARG0 [2 datoshi]
    /// 015A : PUSH4 [1 datoshi]
    /// 015B : PICKITEM [64 datoshi]
    /// 015C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyInc")]
    public abstract BigInteger? UninitializedPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoUzhKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn05QFFHQRXhKFM4SoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9OUBRR0EV4ShTOEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfTlAUUdBFeBTOQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH4 [1 datoshi]
    /// 06 : PICKITEM [64 datoshi]
    /// 07 : PUSH2 [1 datoshi]
    /// 08 : MUL [8 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSHINT32 00000080 [1 datoshi]
    /// 0F : JMPGE 04 [2 datoshi]
    /// 11 : JMP 0A [2 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 19 : JMPLE 1E [2 datoshi]
    /// 1B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 24 : AND [8 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2B : JMPLE 0C [2 datoshi]
    /// 2D : PUSHINT64 0000000001000000 [1 datoshi]
    /// 36 : SUB [8 datoshi]
    /// 37 : TUCK [2 datoshi]
    /// 38 : SWAP [2 datoshi]
    /// 39 : PUSH4 [1 datoshi]
    /// 3A : ROT [2 datoshi]
    /// 3B : SETITEM [8192 datoshi]
    /// 3C : DROP [2 datoshi]
    /// 3D : LDARG0 [2 datoshi]
    /// 3E : DUP [2 datoshi]
    /// 3F : PUSH4 [1 datoshi]
    /// 40 : PICKITEM [64 datoshi]
    /// 41 : PUSH2 [1 datoshi]
    /// 42 : MUL [8 datoshi]
    /// 43 : DUP [2 datoshi]
    /// 44 : PUSHINT32 00000080 [1 datoshi]
    /// 49 : JMPGE 04 [2 datoshi]
    /// 4B : JMP 0A [2 datoshi]
    /// 4D : DUP [2 datoshi]
    /// 4E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 53 : JMPLE 1E [2 datoshi]
    /// 55 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 5E : AND [8 datoshi]
    /// 5F : DUP [2 datoshi]
    /// 60 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 65 : JMPLE 0C [2 datoshi]
    /// 67 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 70 : SUB [8 datoshi]
    /// 71 : TUCK [2 datoshi]
    /// 72 : SWAP [2 datoshi]
    /// 73 : PUSH4 [1 datoshi]
    /// 74 : ROT [2 datoshi]
    /// 75 : SETITEM [8192 datoshi]
    /// 76 : DROP [2 datoshi]
    /// 77 : LDARG0 [2 datoshi]
    /// 78 : DUP [2 datoshi]
    /// 79 : PUSH4 [1 datoshi]
    /// 7A : PICKITEM [64 datoshi]
    /// 7B : PUSH2 [1 datoshi]
    /// 7C : MUL [8 datoshi]
    /// 7D : DUP [2 datoshi]
    /// 7E : PUSHINT32 00000080 [1 datoshi]
    /// 83 : JMPGE 04 [2 datoshi]
    /// 85 : JMP 0A [2 datoshi]
    /// 87 : DUP [2 datoshi]
    /// 88 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 8D : JMPLE 1E [2 datoshi]
    /// 8F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 98 : AND [8 datoshi]
    /// 99 : DUP [2 datoshi]
    /// 9A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 9F : JMPLE 0C [2 datoshi]
    /// A1 : PUSHINT64 0000000001000000 [1 datoshi]
    /// AA : SUB [8 datoshi]
    /// AB : TUCK [2 datoshi]
    /// AC : SWAP [2 datoshi]
    /// AD : PUSH4 [1 datoshi]
    /// AE : ROT [2 datoshi]
    /// AF : SETITEM [8192 datoshi]
    /// B0 : DROP [2 datoshi]
    /// B1 : LDARG0 [2 datoshi]
    /// B2 : PUSH4 [1 datoshi]
    /// B3 : PICKITEM [64 datoshi]
    /// B4 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedPropertyMul")]
    public abstract BigInteger? UninitializedPropertyMul();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EEA=
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticProperty")]
    public abstract BigInteger? UninitializedStaticProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwtKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwudSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// 0000 : INITSLOT 0001 [64 datoshi]
    /// 0003 : LDSFLD 0B [2 datoshi]
    /// 0005 : DUP [2 datoshi]
    /// 0006 : DEC [4 datoshi]
    /// 0007 : DUP [2 datoshi]
    /// 0008 : PUSHINT32 00000080 [1 datoshi]
    /// 000D : JMPGE 04 [2 datoshi]
    /// 000F : JMP 0A [2 datoshi]
    /// 0011 : DUP [2 datoshi]
    /// 0012 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0017 : JMPLE 1E [2 datoshi]
    /// 0019 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0022 : AND [8 datoshi]
    /// 0023 : DUP [2 datoshi]
    /// 0024 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0029 : JMPLE 0C [2 datoshi]
    /// 002B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0034 : SUB [8 datoshi]
    /// 0035 : STSFLD 0B [2 datoshi]
    /// 0037 : DROP [2 datoshi]
    /// 0038 : LDSFLD 0B [2 datoshi]
    /// 003A : DUP [2 datoshi]
    /// 003B : DEC [4 datoshi]
    /// 003C : DUP [2 datoshi]
    /// 003D : PUSHINT32 00000080 [1 datoshi]
    /// 0042 : JMPGE 04 [2 datoshi]
    /// 0044 : JMP 0A [2 datoshi]
    /// 0046 : DUP [2 datoshi]
    /// 0047 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 004C : JMPLE 1E [2 datoshi]
    /// 004E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0057 : AND [8 datoshi]
    /// 0058 : DUP [2 datoshi]
    /// 0059 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 005E : JMPLE 0C [2 datoshi]
    /// 0060 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0069 : SUB [8 datoshi]
    /// 006A : STSFLD 0B [2 datoshi]
    /// 006C : DROP [2 datoshi]
    /// 006D : LDSFLD 0B [2 datoshi]
    /// 006F : DUP [2 datoshi]
    /// 0070 : DEC [4 datoshi]
    /// 0071 : DUP [2 datoshi]
    /// 0072 : PUSHINT32 00000080 [1 datoshi]
    /// 0077 : JMPGE 04 [2 datoshi]
    /// 0079 : JMP 0A [2 datoshi]
    /// 007B : DUP [2 datoshi]
    /// 007C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0081 : JMPLE 1E [2 datoshi]
    /// 0083 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 008C : AND [8 datoshi]
    /// 008D : DUP [2 datoshi]
    /// 008E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0093 : JMPLE 0C [2 datoshi]
    /// 0095 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 009E : SUB [8 datoshi]
    /// 009F : STSFLD 0B [2 datoshi]
    /// 00A1 : DROP [2 datoshi]
    /// 00A2 : LDSFLD 0B [2 datoshi]
    /// 00A4 : DEC [4 datoshi]
    /// 00A5 : DUP [2 datoshi]
    /// 00A6 : PUSHINT32 00000080 [1 datoshi]
    /// 00AB : JMPGE 04 [2 datoshi]
    /// 00AD : JMP 0A [2 datoshi]
    /// 00AF : DUP [2 datoshi]
    /// 00B0 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00B5 : JMPLE 1E [2 datoshi]
    /// 00B7 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00C0 : AND [8 datoshi]
    /// 00C1 : DUP [2 datoshi]
    /// 00C2 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00C7 : JMPLE 0C [2 datoshi]
    /// 00C9 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00D2 : SUB [8 datoshi]
    /// 00D3 : STSFLD 0B [2 datoshi]
    /// 00D5 : LDSFLD 0B [2 datoshi]
    /// 00D7 : DEC [4 datoshi]
    /// 00D8 : DUP [2 datoshi]
    /// 00D9 : PUSHINT32 00000080 [1 datoshi]
    /// 00DE : JMPGE 04 [2 datoshi]
    /// 00E0 : JMP 0A [2 datoshi]
    /// 00E2 : DUP [2 datoshi]
    /// 00E3 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00E8 : JMPLE 1E [2 datoshi]
    /// 00EA : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00F3 : AND [8 datoshi]
    /// 00F4 : DUP [2 datoshi]
    /// 00F5 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00FA : JMPLE 0C [2 datoshi]
    /// 00FC : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0105 : SUB [8 datoshi]
    /// 0106 : STSFLD 0B [2 datoshi]
    /// 0108 : LDSFLD 0B [2 datoshi]
    /// 010A : DEC [4 datoshi]
    /// 010B : DUP [2 datoshi]
    /// 010C : PUSHINT32 00000080 [1 datoshi]
    /// 0111 : JMPGE 04 [2 datoshi]
    /// 0113 : JMP 0A [2 datoshi]
    /// 0115 : DUP [2 datoshi]
    /// 0116 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 011B : JMPLE 1E [2 datoshi]
    /// 011D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0126 : AND [8 datoshi]
    /// 0127 : DUP [2 datoshi]
    /// 0128 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 012D : JMPLE 0C [2 datoshi]
    /// 012F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0138 : SUB [8 datoshi]
    /// 0139 : STSFLD 0B [2 datoshi]
    /// 013B : LDSFLD 0B [2 datoshi]
    /// 013D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyDec")]
    public abstract BigInteger? UninitializedStaticPropertyDec();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwtKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC0VfC0qcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLRV8LSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtFXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwucSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// 0000 : INITSLOT 0001 [64 datoshi]
    /// 0003 : LDSFLD 0B [2 datoshi]
    /// 0005 : DUP [2 datoshi]
    /// 0006 : INC [4 datoshi]
    /// 0007 : DUP [2 datoshi]
    /// 0008 : PUSHINT32 00000080 [1 datoshi]
    /// 000D : JMPGE 04 [2 datoshi]
    /// 000F : JMP 0A [2 datoshi]
    /// 0011 : DUP [2 datoshi]
    /// 0012 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0017 : JMPLE 1E [2 datoshi]
    /// 0019 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0022 : AND [8 datoshi]
    /// 0023 : DUP [2 datoshi]
    /// 0024 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0029 : JMPLE 0C [2 datoshi]
    /// 002B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0034 : SUB [8 datoshi]
    /// 0035 : STSFLD 0B [2 datoshi]
    /// 0037 : DROP [2 datoshi]
    /// 0038 : LDSFLD 0B [2 datoshi]
    /// 003A : DUP [2 datoshi]
    /// 003B : INC [4 datoshi]
    /// 003C : DUP [2 datoshi]
    /// 003D : PUSHINT32 00000080 [1 datoshi]
    /// 0042 : JMPGE 04 [2 datoshi]
    /// 0044 : JMP 0A [2 datoshi]
    /// 0046 : DUP [2 datoshi]
    /// 0047 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 004C : JMPLE 1E [2 datoshi]
    /// 004E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0057 : AND [8 datoshi]
    /// 0058 : DUP [2 datoshi]
    /// 0059 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 005E : JMPLE 0C [2 datoshi]
    /// 0060 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0069 : SUB [8 datoshi]
    /// 006A : STSFLD 0B [2 datoshi]
    /// 006C : DROP [2 datoshi]
    /// 006D : LDSFLD 0B [2 datoshi]
    /// 006F : DUP [2 datoshi]
    /// 0070 : INC [4 datoshi]
    /// 0071 : DUP [2 datoshi]
    /// 0072 : PUSHINT32 00000080 [1 datoshi]
    /// 0077 : JMPGE 04 [2 datoshi]
    /// 0079 : JMP 0A [2 datoshi]
    /// 007B : DUP [2 datoshi]
    /// 007C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0081 : JMPLE 1E [2 datoshi]
    /// 0083 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 008C : AND [8 datoshi]
    /// 008D : DUP [2 datoshi]
    /// 008E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0093 : JMPLE 0C [2 datoshi]
    /// 0095 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 009E : SUB [8 datoshi]
    /// 009F : STSFLD 0B [2 datoshi]
    /// 00A1 : DROP [2 datoshi]
    /// 00A2 : LDSFLD 0B [2 datoshi]
    /// 00A4 : INC [4 datoshi]
    /// 00A5 : DUP [2 datoshi]
    /// 00A6 : PUSHINT32 00000080 [1 datoshi]
    /// 00AB : JMPGE 04 [2 datoshi]
    /// 00AD : JMP 0A [2 datoshi]
    /// 00AF : DUP [2 datoshi]
    /// 00B0 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00B5 : JMPLE 1E [2 datoshi]
    /// 00B7 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00C0 : AND [8 datoshi]
    /// 00C1 : DUP [2 datoshi]
    /// 00C2 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00C7 : JMPLE 0C [2 datoshi]
    /// 00C9 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 00D2 : SUB [8 datoshi]
    /// 00D3 : STSFLD 0B [2 datoshi]
    /// 00D5 : LDSFLD 0B [2 datoshi]
    /// 00D7 : INC [4 datoshi]
    /// 00D8 : DUP [2 datoshi]
    /// 00D9 : PUSHINT32 00000080 [1 datoshi]
    /// 00DE : JMPGE 04 [2 datoshi]
    /// 00E0 : JMP 0A [2 datoshi]
    /// 00E2 : DUP [2 datoshi]
    /// 00E3 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00E8 : JMPLE 1E [2 datoshi]
    /// 00EA : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 00F3 : AND [8 datoshi]
    /// 00F4 : DUP [2 datoshi]
    /// 00F5 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 00FA : JMPLE 0C [2 datoshi]
    /// 00FC : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0105 : SUB [8 datoshi]
    /// 0106 : STSFLD 0B [2 datoshi]
    /// 0108 : LDSFLD 0B [2 datoshi]
    /// 010A : INC [4 datoshi]
    /// 010B : DUP [2 datoshi]
    /// 010C : PUSHINT32 00000080 [1 datoshi]
    /// 0111 : JMPGE 04 [2 datoshi]
    /// 0113 : JMP 0A [2 datoshi]
    /// 0115 : DUP [2 datoshi]
    /// 0116 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 011B : JMPLE 1E [2 datoshi]
    /// 011D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0126 : AND [8 datoshi]
    /// 0127 : DUP [2 datoshi]
    /// 0128 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 012D : JMPLE 0C [2 datoshi]
    /// 012F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 0138 : SUB [8 datoshi]
    /// 0139 : STSFLD 0B [2 datoshi]
    /// 013B : LDSFLD 0B [2 datoshi]
    /// 013D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyInc")]
    public abstract BigInteger? UninitializedStaticPropertyInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABXwsSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9nC18LEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfZwtfCxKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2cLXwtA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDSFLD 0B [2 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : MUL [8 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSHINT32 00000080 [1 datoshi]
    /// 0D : JMPGE 04 [2 datoshi]
    /// 0F : JMP 0A [2 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 17 : JMPLE 1E [2 datoshi]
    /// 19 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 22 : AND [8 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 29 : JMPLE 0C [2 datoshi]
    /// 2B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 34 : SUB [8 datoshi]
    /// 35 : STSFLD 0B [2 datoshi]
    /// 37 : LDSFLD 0B [2 datoshi]
    /// 39 : PUSH2 [1 datoshi]
    /// 3A : MUL [8 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : PUSHINT32 00000080 [1 datoshi]
    /// 41 : JMPGE 04 [2 datoshi]
    /// 43 : JMP 0A [2 datoshi]
    /// 45 : DUP [2 datoshi]
    /// 46 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4B : JMPLE 1E [2 datoshi]
    /// 4D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 56 : AND [8 datoshi]
    /// 57 : DUP [2 datoshi]
    /// 58 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5D : JMPLE 0C [2 datoshi]
    /// 5F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 68 : SUB [8 datoshi]
    /// 69 : STSFLD 0B [2 datoshi]
    /// 6B : LDSFLD 0B [2 datoshi]
    /// 6D : PUSH2 [1 datoshi]
    /// 6E : MUL [8 datoshi]
    /// 6F : DUP [2 datoshi]
    /// 70 : PUSHINT32 00000080 [1 datoshi]
    /// 75 : JMPGE 04 [2 datoshi]
    /// 77 : JMP 0A [2 datoshi]
    /// 79 : DUP [2 datoshi]
    /// 7A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 7F : JMPLE 1E [2 datoshi]
    /// 81 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 8A : AND [8 datoshi]
    /// 8B : DUP [2 datoshi]
    /// 8C : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 91 : JMPLE 0C [2 datoshi]
    /// 93 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 9C : SUB [8 datoshi]
    /// 9D : STSFLD 0B [2 datoshi]
    /// 9F : LDSFLD 0B [2 datoshi]
    /// A1 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uninitializedStaticPropertyMul")]
    public abstract BigInteger? UninitializedStaticPropertyMul();

    #endregion
}
