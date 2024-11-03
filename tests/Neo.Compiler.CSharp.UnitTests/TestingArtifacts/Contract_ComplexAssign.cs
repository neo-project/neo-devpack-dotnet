using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":68,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":174,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":234,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":332,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":400,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":506,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":574,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":680,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":740,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x23c11410a8a4da12da47c53565745eef1587260e"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIOJocV7150ZTXFR9oS2qSoEBTBIwl0ZXN0QXJnczEBAAEPDiaHFe9edGU1xUfaEtqkqBAUwSMIdGVzdFZvaWQAAAAPAAD9RgNXAgAD/////wAAAABwAv///39xaBGeShAuAzpKA/////8AAAAAMgM6cGkRnkoCAAAAgC4DOkoC////fzIDOnFpaBK/QFcCAAP/////AAAAAHAC////f3FoEZ5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0BXAgAQcAIAAACAcWgRn0oQLgM6SgP/////AAAAADIDOnBpEZ9KAgAAAIAuAzpKAv///38yAzpxaWgSv0BXAgAQcAIAAACAcWgRn0oQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCAAP/////AAAAAHAC////f3FoEqBKEC4DOkoD/////wAAAAAyAzpwaRKgSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCAAP/////AAAAAHAC////f3FoEahKEC4DOkoD/////wAAAAAyAzpwaRGoSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCABBwAgAAAIBxaBGpShAuAzpKA/////8AAAAAMgM6cGkRqUoCAAAAgC4DOkoC////fzIDOnFpaBK/QFcCABBwAgAAAIBxaBGpShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEalKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWloEr9Ac17gyA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgM6SgP/////AAAAADIDOnBpEZ5KAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH1 [1 datoshi]
    /// 15 : OpCode.ADD [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : OpCode.JMPLE 03 [2 datoshi]
    /// 27 : OpCode.THROW [512 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.PUSH1 [1 datoshi]
    /// 2B : OpCode.ADD [8 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 32 : OpCode.JMPGE 03 [2 datoshi]
    /// 34 : OpCode.THROW [512 datoshi]
    /// 35 : OpCode.DUP [2 datoshi]
    /// 36 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : OpCode.JMPLE 03 [2 datoshi]
    /// 3D : OpCode.THROW [512 datoshi]
    /// 3E : OpCode.STLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.PUSH2 [1 datoshi]
    /// 42 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 43 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH1 [1 datoshi]
    /// 15 : OpCode.ADD [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 04 [2 datoshi]
    /// 1A : OpCode.JMP 0E [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : OpCode.JMPLE 0C [2 datoshi]
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : OpCode.AND [8 datoshi]
    /// 32 : OpCode.STLOC0 [2 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.PUSH1 [1 datoshi]
    /// 35 : OpCode.ADD [8 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 3C : OpCode.JMPGE 04 [2 datoshi]
    /// 3E : OpCode.JMP 0A [2 datoshi]
    /// 40 : OpCode.DUP [2 datoshi]
    /// 41 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : OpCode.JMPLE 1E [2 datoshi]
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : OpCode.AND [8 datoshi]
    /// 52 : OpCode.DUP [2 datoshi]
    /// 53 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : OpCode.JMPLE 0C [2 datoshi]
    /// 5A : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : OpCode.SUB [8 datoshi]
    /// 64 : OpCode.STLOC1 [2 datoshi]
    /// 65 : OpCode.LDLOC1 [2 datoshi]
    /// 66 : OpCode.LDLOC0 [2 datoshi]
    /// 67 : OpCode.PUSH2 [1 datoshi]
    /// 68 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 69 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOnBpEahKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH1 [1 datoshi]
    /// 15 : OpCode.SHL [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : OpCode.JMPLE 03 [2 datoshi]
    /// 27 : OpCode.THROW [512 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.PUSH1 [1 datoshi]
    /// 2B : OpCode.SHL [8 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 32 : OpCode.JMPGE 03 [2 datoshi]
    /// 34 : OpCode.THROW [512 datoshi]
    /// 35 : OpCode.DUP [2 datoshi]
    /// 36 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : OpCode.JMPLE 03 [2 datoshi]
    /// 3D : OpCode.THROW [512 datoshi]
    /// 3E : OpCode.STLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.PUSH2 [1 datoshi]
    /// 42 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 43 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH1 [1 datoshi]
    /// 15 : OpCode.SHL [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 04 [2 datoshi]
    /// 1A : OpCode.JMP 0E [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : OpCode.JMPLE 0C [2 datoshi]
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : OpCode.AND [8 datoshi]
    /// 32 : OpCode.STLOC0 [2 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.PUSH1 [1 datoshi]
    /// 35 : OpCode.SHL [8 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 3C : OpCode.JMPGE 04 [2 datoshi]
    /// 3E : OpCode.JMP 0A [2 datoshi]
    /// 40 : OpCode.DUP [2 datoshi]
    /// 41 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : OpCode.JMPLE 1E [2 datoshi]
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : OpCode.AND [8 datoshi]
    /// 52 : OpCode.DUP [2 datoshi]
    /// 53 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : OpCode.JMPLE 0C [2 datoshi]
    /// 5A : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : OpCode.SUB [8 datoshi]
    /// 64 : OpCode.STLOC1 [2 datoshi]
    /// 65 : OpCode.LDLOC1 [2 datoshi]
    /// 66 : OpCode.LDLOC0 [2 datoshi]
    /// 67 : OpCode.PUSH2 [1 datoshi]
    /// 68 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 69 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.MUL [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : OpCode.JMPLE 03 [2 datoshi]
    /// 27 : OpCode.THROW [512 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.PUSH2 [1 datoshi]
    /// 2B : OpCode.MUL [8 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 32 : OpCode.JMPGE 03 [2 datoshi]
    /// 34 : OpCode.THROW [512 datoshi]
    /// 35 : OpCode.DUP [2 datoshi]
    /// 36 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : OpCode.JMPLE 03 [2 datoshi]
    /// 3D : OpCode.THROW [512 datoshi]
    /// 3E : OpCode.STLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC1 [2 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.PUSH2 [1 datoshi]
    /// 42 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 43 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.MUL [8 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.JMPGE 04 [2 datoshi]
    /// 1A : OpCode.JMP 0E [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : OpCode.JMPLE 0C [2 datoshi]
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : OpCode.AND [8 datoshi]
    /// 32 : OpCode.STLOC0 [2 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.PUSH2 [1 datoshi]
    /// 35 : OpCode.MUL [8 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 3C : OpCode.JMPGE 04 [2 datoshi]
    /// 3E : OpCode.JMP 0A [2 datoshi]
    /// 40 : OpCode.DUP [2 datoshi]
    /// 41 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : OpCode.JMPLE 1E [2 datoshi]
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : OpCode.AND [8 datoshi]
    /// 52 : OpCode.DUP [2 datoshi]
    /// 53 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : OpCode.JMPLE 0C [2 datoshi]
    /// 5A : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : OpCode.SUB [8 datoshi]
    /// 64 : OpCode.STLOC1 [2 datoshi]
    /// 65 : OpCode.LDLOC1 [2 datoshi]
    /// 66 : OpCode.LDLOC0 [2 datoshi]
    /// 67 : OpCode.PUSH2 [1 datoshi]
    /// 68 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 69 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpwaRGpSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.SHR [8 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.JMPGE 03 [2 datoshi]
    /// 12 : OpCode.THROW [512 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1D : OpCode.JMPLE 03 [2 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.STLOC0 [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.PUSH1 [1 datoshi]
    /// 23 : OpCode.SHR [8 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 2A : OpCode.JMPGE 03 [2 datoshi]
    /// 2C : OpCode.THROW [512 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : OpCode.JMPLE 03 [2 datoshi]
    /// 35 : OpCode.THROW [512 datoshi]
    /// 36 : OpCode.STLOC1 [2 datoshi]
    /// 37 : OpCode.LDLOC1 [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.PUSH2 [1 datoshi]
    /// 3A : OpCode.PACKSTRUCT [2048 datoshi]
    /// 3B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.SHR [8 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.JMPGE 04 [2 datoshi]
    /// 12 : OpCode.JMP 0E [2 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : OpCode.JMPLE 0C [2 datoshi]
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : OpCode.AND [8 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.LDLOC1 [2 datoshi]
    /// 2C : OpCode.PUSH1 [1 datoshi]
    /// 2D : OpCode.SHR [8 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 34 : OpCode.JMPGE 04 [2 datoshi]
    /// 36 : OpCode.JMP 0A [2 datoshi]
    /// 38 : OpCode.DUP [2 datoshi]
    /// 39 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : OpCode.JMPLE 1E [2 datoshi]
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : OpCode.AND [8 datoshi]
    /// 4A : OpCode.DUP [2 datoshi]
    /// 4B : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : OpCode.JMPLE 0C [2 datoshi]
    /// 52 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : OpCode.SUB [8 datoshi]
    /// 5C : OpCode.STLOC1 [2 datoshi]
    /// 5D : OpCode.LDLOC1 [2 datoshi]
    /// 5E : OpCode.LDLOC0 [2 datoshi]
    /// 5F : OpCode.PUSH2 [1 datoshi]
    /// 60 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 61 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4DOkoD/////wAAAAAyAzpwaRGfSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.SUB [8 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.JMPGE 03 [2 datoshi]
    /// 12 : OpCode.THROW [512 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1D : OpCode.JMPLE 03 [2 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.STLOC0 [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.PUSH1 [1 datoshi]
    /// 23 : OpCode.SUB [8 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 2A : OpCode.JMPGE 03 [2 datoshi]
    /// 2C : OpCode.THROW [512 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : OpCode.JMPLE 03 [2 datoshi]
    /// 35 : OpCode.THROW [512 datoshi]
    /// 36 : OpCode.STLOC1 [2 datoshi]
    /// 37 : OpCode.LDLOC1 [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.PUSH2 [1 datoshi]
    /// 3A : OpCode.PACKSTRUCT [2048 datoshi]
    /// 3B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.SUB [8 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.JMPGE 04 [2 datoshi]
    /// 12 : OpCode.JMP 0E [2 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : OpCode.JMPLE 0C [2 datoshi]
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : OpCode.AND [8 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.LDLOC1 [2 datoshi]
    /// 2C : OpCode.PUSH1 [1 datoshi]
    /// 2D : OpCode.SUB [8 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 34 : OpCode.JMPGE 04 [2 datoshi]
    /// 36 : OpCode.JMP 0A [2 datoshi]
    /// 38 : OpCode.DUP [2 datoshi]
    /// 39 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : OpCode.JMPLE 1E [2 datoshi]
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : OpCode.AND [8 datoshi]
    /// 4A : OpCode.DUP [2 datoshi]
    /// 4B : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : OpCode.JMPLE 0C [2 datoshi]
    /// 52 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : OpCode.SUB [8 datoshi]
    /// 5C : OpCode.STLOC1 [2 datoshi]
    /// 5D : OpCode.LDLOC1 [2 datoshi]
    /// 5E : OpCode.LDLOC0 [2 datoshi]
    /// 5F : OpCode.PUSH2 [1 datoshi]
    /// 60 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 61 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion
}
