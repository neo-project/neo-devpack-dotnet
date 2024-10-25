using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":68,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":174,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":234,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":332,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":400,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":506,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":574,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":680,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":740,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x5d46269ff23ec37de131e4791d5e5c964b140704"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEBxRLllxeHXnkMeF9wz7ynyZGXQl0ZXN0QXJnczEBAAEPBAcUS5ZcXh155DHhfcM+8p8mRl0IdGVzdFZvaWQAAAAPAAD9RgNXAgAD/////wAAAABwAv///39xaBGeShAuAzpKA/////8AAAAAMgM6cGkRnkoCAAAAgC4DOkoC////fzIDOnFpaBK/QFcCAAP/////AAAAAHAC////f3FoEZ5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0BXAgAQcAIAAACAcWgRn0oQLgM6SgP/////AAAAADIDOnBpEZ9KAgAAAIAuAzpKAv///38yAzpxaWgSv0BXAgAQcAIAAACAcWgRn0oQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCAAP/////AAAAAHAC////f3FoEqBKEC4DOkoD/////wAAAAAyAzpwaRKgSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCAAP/////AAAAAHAC////f3FoEahKEC4DOkoD/////wAAAAAyAzpwaRGoSgIAAACALgM6SgL///9/MgM6cWloEr9AVwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QFcCABBwAgAAAIBxaBGpShAuAzpKA/////8AAAAAMgM6cGkRqUoCAAAAgC4DOkoC////fzIDOnFpaBK/QFcCABBwAgAAAIBxaBGpShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEalKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWloEr9ADFu5Rw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgM6SgP/////AAAAADIDOnBpEZ5KAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.ADD
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 25 : OpCode.JMPLE 03
    /// 27 : OpCode.THROW
    /// 28 : OpCode.STLOC0
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.PUSH1
    /// 2B : OpCode.ADD
    /// 2C : OpCode.DUP
    /// 2D : OpCode.PUSHINT32 00000080
    /// 32 : OpCode.JMPGE 03
    /// 34 : OpCode.THROW
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSHINT32 FFFFFF7F
    /// 3B : OpCode.JMPLE 03
    /// 3D : OpCode.THROW
    /// 3E : OpCode.STLOC1
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.PUSH2
    /// 42 : OpCode.PACKSTRUCT
    /// 43 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRnkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.ADD
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 04
    /// 1A : OpCode.JMP 0E
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 26 : OpCode.JMPLE 0C
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.STLOC0
    /// 33 : OpCode.LDLOC1
    /// 34 : OpCode.PUSH1
    /// 35 : OpCode.ADD
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHINT32 00000080
    /// 3C : OpCode.JMPGE 04
    /// 3E : OpCode.JMP 0A
    /// 40 : OpCode.DUP
    /// 41 : OpCode.PUSHINT32 FFFFFF7F
    /// 46 : OpCode.JMPLE 1E
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 51 : OpCode.AND
    /// 52 : OpCode.DUP
    /// 53 : OpCode.PUSHINT32 FFFFFF7F
    /// 58 : OpCode.JMPLE 0C
    /// 5A : OpCode.PUSHINT64 0000000001000000
    /// 63 : OpCode.SUB
    /// 64 : OpCode.STLOC1
    /// 65 : OpCode.LDLOC1
    /// 66 : OpCode.LDLOC0
    /// 67 : OpCode.PUSH2
    /// 68 : OpCode.PACKSTRUCT
    /// 69 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgM6SgP/////AAAAADIDOnBpEahKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.SHL
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 25 : OpCode.JMPLE 03
    /// 27 : OpCode.THROW
    /// 28 : OpCode.STLOC0
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.PUSH1
    /// 2B : OpCode.SHL
    /// 2C : OpCode.DUP
    /// 2D : OpCode.PUSHINT32 00000080
    /// 32 : OpCode.JMPGE 03
    /// 34 : OpCode.THROW
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSHINT32 FFFFFF7F
    /// 3B : OpCode.JMPLE 03
    /// 3D : OpCode.THROW
    /// 3E : OpCode.STLOC1
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.PUSH2
    /// 42 : OpCode.PACKSTRUCT
    /// 43 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.SHL
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 04
    /// 1A : OpCode.JMP 0E
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 26 : OpCode.JMPLE 0C
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.STLOC0
    /// 33 : OpCode.LDLOC1
    /// 34 : OpCode.PUSH1
    /// 35 : OpCode.SHL
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHINT32 00000080
    /// 3C : OpCode.JMPGE 04
    /// 3E : OpCode.JMP 0A
    /// 40 : OpCode.DUP
    /// 41 : OpCode.PUSHINT32 FFFFFF7F
    /// 46 : OpCode.JMPLE 1E
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 51 : OpCode.AND
    /// 52 : OpCode.DUP
    /// 53 : OpCode.PUSHINT32 FFFFFF7F
    /// 58 : OpCode.JMPLE 0C
    /// 5A : OpCode.PUSHINT64 0000000001000000
    /// 63 : OpCode.SUB
    /// 64 : OpCode.STLOC1
    /// 65 : OpCode.LDLOC1
    /// 66 : OpCode.LDLOC0
    /// 67 : OpCode.PUSH2
    /// 68 : OpCode.PACKSTRUCT
    /// 69 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH2
    /// 15 : OpCode.MUL
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 25 : OpCode.JMPLE 03
    /// 27 : OpCode.THROW
    /// 28 : OpCode.STLOC0
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.PUSH2
    /// 2B : OpCode.MUL
    /// 2C : OpCode.DUP
    /// 2D : OpCode.PUSHINT32 00000080
    /// 32 : OpCode.JMPGE 03
    /// 34 : OpCode.THROW
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSHINT32 FFFFFF7F
    /// 3B : OpCode.JMPLE 03
    /// 3D : OpCode.THROW
    /// 3E : OpCode.STLOC1
    /// 3F : OpCode.LDLOC1
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.PUSH2
    /// 42 : OpCode.PACKSTRUCT
    /// 43 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FpaBK/QA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.PUSHINT32 FFFFFF7F
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH2
    /// 15 : OpCode.MUL
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.JMPGE 04
    /// 1A : OpCode.JMP 0E
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 26 : OpCode.JMPLE 0C
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.STLOC0
    /// 33 : OpCode.LDLOC1
    /// 34 : OpCode.PUSH2
    /// 35 : OpCode.MUL
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHINT32 00000080
    /// 3C : OpCode.JMPGE 04
    /// 3E : OpCode.JMP 0A
    /// 40 : OpCode.DUP
    /// 41 : OpCode.PUSHINT32 FFFFFF7F
    /// 46 : OpCode.JMPLE 1E
    /// 48 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 51 : OpCode.AND
    /// 52 : OpCode.DUP
    /// 53 : OpCode.PUSHINT32 FFFFFF7F
    /// 58 : OpCode.JMPLE 0C
    /// 5A : OpCode.PUSHINT64 0000000001000000
    /// 63 : OpCode.SUB
    /// 64 : OpCode.STLOC1
    /// 65 : OpCode.LDLOC1
    /// 66 : OpCode.LDLOC0
    /// 67 : OpCode.PUSH2
    /// 68 : OpCode.PACKSTRUCT
    /// 69 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4DOkoD/////wAAAAAyAzpwaRGpSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.SHR
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH0
    /// 10 : OpCode.JMPGE 03
    /// 12 : OpCode.THROW
    /// 13 : OpCode.DUP
    /// 14 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1D : OpCode.JMPLE 03
    /// 1F : OpCode.THROW
    /// 20 : OpCode.STLOC0
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.PUSH1
    /// 23 : OpCode.SHR
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 00000080
    /// 2A : OpCode.JMPGE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 FFFFFF7F
    /// 33 : OpCode.JMPLE 03
    /// 35 : OpCode.THROW
    /// 36 : OpCode.STLOC1
    /// 37 : OpCode.LDLOC1
    /// 38 : OpCode.LDLOC0
    /// 39 : OpCode.PUSH2
    /// 3A : OpCode.PACKSTRUCT
    /// 3B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.SHR
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH0
    /// 10 : OpCode.JMPGE 04
    /// 12 : OpCode.JMP 0E
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1E : OpCode.JMPLE 0C
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 29 : OpCode.AND
    /// 2A : OpCode.STLOC0
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.PUSH1
    /// 2D : OpCode.SHR
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSHINT32 00000080
    /// 34 : OpCode.JMPGE 04
    /// 36 : OpCode.JMP 0A
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 1E
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 49 : OpCode.AND
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT32 FFFFFF7F
    /// 50 : OpCode.JMPLE 0C
    /// 52 : OpCode.PUSHINT64 0000000001000000
    /// 5B : OpCode.SUB
    /// 5C : OpCode.STLOC1
    /// 5D : OpCode.LDLOC1
    /// 5E : OpCode.LDLOC0
    /// 5F : OpCode.PUSH2
    /// 60 : OpCode.PACKSTRUCT
    /// 61 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4DOkoD/////wAAAAAyAzpwaRGfSgIAAACALgM6SgL///9/MgM6cWloEr9A
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.SUB
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH0
    /// 10 : OpCode.JMPGE 03
    /// 12 : OpCode.THROW
    /// 13 : OpCode.DUP
    /// 14 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1D : OpCode.JMPLE 03
    /// 1F : OpCode.THROW
    /// 20 : OpCode.STLOC0
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.PUSH1
    /// 23 : OpCode.SUB
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 00000080
    /// 2A : OpCode.JMPGE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 FFFFFF7F
    /// 33 : OpCode.JMPLE 03
    /// 35 : OpCode.THROW
    /// 36 : OpCode.STLOC1
    /// 37 : OpCode.LDLOC1
    /// 38 : OpCode.LDLOC0
    /// 39 : OpCode.PUSH2
    /// 3A : OpCode.PACKSTRUCT
    /// 3B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEZ9KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaWgSv0A=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.SUB
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH0
    /// 10 : OpCode.JMPGE 04
    /// 12 : OpCode.JMP 0E
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1E : OpCode.JMPLE 0C
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 29 : OpCode.AND
    /// 2A : OpCode.STLOC0
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.PUSH1
    /// 2D : OpCode.SUB
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSHINT32 00000080
    /// 34 : OpCode.JMPGE 04
    /// 36 : OpCode.JMP 0A
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 1E
    /// 40 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 49 : OpCode.AND
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT32 FFFFFF7F
    /// 50 : OpCode.JMPLE 0C
    /// 52 : OpCode.PUSHINT64 0000000001000000
    /// 5B : OpCode.SUB
    /// 5C : OpCode.STLOC1
    /// 5D : OpCode.LDLOC1
    /// 5E : OpCode.LDLOC0
    /// 5F : OpCode.PUSH2
    /// 60 : OpCode.PACKSTRUCT
    /// 61 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion
}
