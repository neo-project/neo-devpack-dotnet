using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":68,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":174,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":234,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":332,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":400,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":506,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":574,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":680,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":740,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1GA1cCAAP/////AAAAAHAC////f3FoEZ5KEC4DOkoD/////wAAAAAyAzpwaRGeSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCABBwAgAAAIBxaBGfShAuAzpKA/////8AAAAAMgM6cGkRn0oCAAAAgC4DOkoC////fzIDOnFpaBK/QFcCABBwAgAAAIBxaBGfShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEZ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWloEr9AVwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxaWgSv0BXAgAD/////wAAAABwAv///39xaBKgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWloEr9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOnBpEahKAgAAAIAuAzpKAv///38yAzpxaWgSv0BXAgAD/////wAAAABwAv///39xaBGoShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEahKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWloEr9AVwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpwaRGpSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0Cow2c0").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgM6SgP/////AAAAADIDOnBpEZ5KAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : ADD [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : JMPLE 03 [2 datoshi]
    /// 27 : THROW [512 datoshi]
    /// 28 : STLOC0 [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : PUSH1 [1 datoshi]
    /// 2B : ADD [8 datoshi]
    /// 2C : DUP [2 datoshi]
    /// 2D : PUSHINT32 00000080 [1 datoshi]
    /// 32 : JMPGE 03 [2 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : JMPLE 03 [2 datoshi]
    /// 3D : THROW [512 datoshi]
    /// 3E : STLOC1 [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDLOC0 [2 datoshi]
    /// 41 : PUSH2 [1 datoshi]
    /// 42 : PACKSTRUCT [2048 datoshi]
    /// 43 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : ADD [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 04 [2 datoshi]
    /// 1A : JMP 0E [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : JMPLE 0C [2 datoshi]
    /// 28 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : STLOC0 [2 datoshi]
    /// 33 : LDLOC1 [2 datoshi]
    /// 34 : PUSH1 [1 datoshi]
    /// 35 : ADD [8 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHINT32 00000080 [1 datoshi]
    /// 3C : JMPGE 04 [2 datoshi]
    /// 3E : JMP 0A [2 datoshi]
    /// 40 : DUP [2 datoshi]
    /// 41 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : JMPLE 1E [2 datoshi]
    /// 48 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : AND [8 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : JMPLE 0C [2 datoshi]
    /// 5A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : SUB [8 datoshi]
    /// 64 : STLOC1 [2 datoshi]
    /// 65 : LDLOC1 [2 datoshi]
    /// 66 : LDLOC0 [2 datoshi]
    /// 67 : PUSH2 [1 datoshi]
    /// 68 : PACKSTRUCT [2048 datoshi]
    /// 69 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOnBpEahKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : SHL [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : JMPLE 03 [2 datoshi]
    /// 27 : THROW [512 datoshi]
    /// 28 : STLOC0 [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : PUSH1 [1 datoshi]
    /// 2B : SHL [8 datoshi]
    /// 2C : DUP [2 datoshi]
    /// 2D : PUSHINT32 00000080 [1 datoshi]
    /// 32 : JMPGE 03 [2 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : JMPLE 03 [2 datoshi]
    /// 3D : THROW [512 datoshi]
    /// 3E : STLOC1 [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDLOC0 [2 datoshi]
    /// 41 : PUSH2 [1 datoshi]
    /// 42 : PACKSTRUCT [2048 datoshi]
    /// 43 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : SHL [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 04 [2 datoshi]
    /// 1A : JMP 0E [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : JMPLE 0C [2 datoshi]
    /// 28 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : STLOC0 [2 datoshi]
    /// 33 : LDLOC1 [2 datoshi]
    /// 34 : PUSH1 [1 datoshi]
    /// 35 : SHL [8 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHINT32 00000080 [1 datoshi]
    /// 3C : JMPGE 04 [2 datoshi]
    /// 3E : JMP 0A [2 datoshi]
    /// 40 : DUP [2 datoshi]
    /// 41 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : JMPLE 1E [2 datoshi]
    /// 48 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : AND [8 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : JMPLE 0C [2 datoshi]
    /// 5A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : SUB [8 datoshi]
    /// 64 : STLOC1 [2 datoshi]
    /// 65 : LDLOC1 [2 datoshi]
    /// 66 : LDLOC0 [2 datoshi]
    /// 67 : PUSH2 [1 datoshi]
    /// 68 : PACKSTRUCT [2048 datoshi]
    /// 69 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH2 [1 datoshi]
    /// 15 : MUL [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : JMPLE 03 [2 datoshi]
    /// 27 : THROW [512 datoshi]
    /// 28 : STLOC0 [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : PUSH2 [1 datoshi]
    /// 2B : MUL [8 datoshi]
    /// 2C : DUP [2 datoshi]
    /// 2D : PUSHINT32 00000080 [1 datoshi]
    /// 32 : JMPGE 03 [2 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3B : JMPLE 03 [2 datoshi]
    /// 3D : THROW [512 datoshi]
    /// 3E : STLOC1 [2 datoshi]
    /// 3F : LDLOC1 [2 datoshi]
    /// 40 : LDLOC0 [2 datoshi]
    /// 41 : PUSH2 [1 datoshi]
    /// 42 : PACKSTRUCT [2048 datoshi]
    /// 43 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : PUSH2 [1 datoshi]
    /// 15 : MUL [8 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : JMPGE 04 [2 datoshi]
    /// 1A : JMP 0E [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : JMPLE 0C [2 datoshi]
    /// 28 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : STLOC0 [2 datoshi]
    /// 33 : LDLOC1 [2 datoshi]
    /// 34 : PUSH2 [1 datoshi]
    /// 35 : MUL [8 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHINT32 00000080 [1 datoshi]
    /// 3C : JMPGE 04 [2 datoshi]
    /// 3E : JMP 0A [2 datoshi]
    /// 40 : DUP [2 datoshi]
    /// 41 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 46 : JMPLE 1E [2 datoshi]
    /// 48 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 51 : AND [8 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 58 : JMPLE 0C [2 datoshi]
    /// 5A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 63 : SUB [8 datoshi]
    /// 64 : STLOC1 [2 datoshi]
    /// 65 : LDLOC1 [2 datoshi]
    /// 66 : LDLOC0 [2 datoshi]
    /// 67 : PUSH2 [1 datoshi]
    /// 68 : PACKSTRUCT [2048 datoshi]
    /// 69 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpwaRGpSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : STLOC1 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : SHR [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : JMPGE 03 [2 datoshi]
    /// 12 : THROW [512 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1D : JMPLE 03 [2 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : STLOC0 [2 datoshi]
    /// 21 : LDLOC1 [2 datoshi]
    /// 22 : PUSH1 [1 datoshi]
    /// 23 : SHR [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 00000080 [1 datoshi]
    /// 2A : JMPGE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : JMPLE 03 [2 datoshi]
    /// 35 : THROW [512 datoshi]
    /// 36 : STLOC1 [2 datoshi]
    /// 37 : LDLOC1 [2 datoshi]
    /// 38 : LDLOC0 [2 datoshi]
    /// 39 : PUSH2 [1 datoshi]
    /// 3A : PACKSTRUCT [2048 datoshi]
    /// 3B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : STLOC1 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : SHR [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : JMPGE 04 [2 datoshi]
    /// 12 : JMP 0E [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : JMPLE 0C [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : STLOC0 [2 datoshi]
    /// 2B : LDLOC1 [2 datoshi]
    /// 2C : PUSH1 [1 datoshi]
    /// 2D : SHR [8 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSHINT32 00000080 [1 datoshi]
    /// 34 : JMPGE 04 [2 datoshi]
    /// 36 : JMP 0A [2 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 1E [2 datoshi]
    /// 40 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : AND [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : JMPLE 0C [2 datoshi]
    /// 52 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : SUB [8 datoshi]
    /// 5C : STLOC1 [2 datoshi]
    /// 5D : LDLOC1 [2 datoshi]
    /// 5E : LDLOC0 [2 datoshi]
    /// 5F : PUSH2 [1 datoshi]
    /// 60 : PACKSTRUCT [2048 datoshi]
    /// 61 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4DOkoD/////wAAAAAyAzpwaRGfSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : STLOC1 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : SUB [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : JMPGE 03 [2 datoshi]
    /// 12 : THROW [512 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1D : JMPLE 03 [2 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : STLOC0 [2 datoshi]
    /// 21 : LDLOC1 [2 datoshi]
    /// 22 : PUSH1 [1 datoshi]
    /// 23 : SUB [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 00000080 [1 datoshi]
    /// 2A : JMPGE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 33 : JMPLE 03 [2 datoshi]
    /// 35 : THROW [512 datoshi]
    /// 36 : STLOC1 [2 datoshi]
    /// 37 : LDLOC1 [2 datoshi]
    /// 38 : LDLOC0 [2 datoshi]
    /// 39 : PUSH2 [1 datoshi]
    /// 3A : PACKSTRUCT [2048 datoshi]
    /// 3B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : STLOC1 [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : SUB [8 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : JMPGE 04 [2 datoshi]
    /// 12 : JMP 0E [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : JMPLE 0C [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : STLOC0 [2 datoshi]
    /// 2B : LDLOC1 [2 datoshi]
    /// 2C : PUSH1 [1 datoshi]
    /// 2D : SUB [8 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSHINT32 00000080 [1 datoshi]
    /// 34 : JMPGE 04 [2 datoshi]
    /// 36 : JMP 0A [2 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 1E [2 datoshi]
    /// 40 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 49 : AND [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 50 : JMPLE 0C [2 datoshi]
    /// 52 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 5B : SUB [8 datoshi]
    /// 5C : STLOC1 [2 datoshi]
    /// 5D : LDLOC1 [2 datoshi]
    /// 5E : LDLOC0 [2 datoshi]
    /// 5F : PUSH2 [1 datoshi]
    /// 60 : PACKSTRUCT [2048 datoshi]
    /// 61 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion
}
