using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_ComplexAssign(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ComplexAssign"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Add_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""unitTest_Add_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":67,""safe"":false},{""name"":""unitTest_Sub_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":172,""safe"":false},{""name"":""unitTest_Sub_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":231,""safe"":false},{""name"":""unitTest_Mul_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":328,""safe"":false},{""name"":""unitTest_Mul_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":397,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":504,""safe"":false},{""name"":""leftShiftAssignChecked"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":595,""safe"":false},{""name"":""unitTest_Left_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":633,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_Checked"",""parameters"":[],""returntype"":""Array"",""offset"":740,""safe"":false},{""name"":""unitTest_Right_Shift_Assign_UnChecked"",""parameters"":[],""returntype"":""Array"",""offset"":765,""safe"":false},{""name"":""testAndAssign"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":790,""safe"":false},{""name"":""testOrAssign"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":799,""safe"":false},{""name"":""testXorAssign"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""shift"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":808,""safe"":false},{""name"":""testBoolXorAssign"",""parameters"":[{""name"":""left"",""type"":""Boolean""},{""name"":""right"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":817,""safe"":false},{""name"":""testBoolAndAssign"",""parameters"":[{""name"":""left"",""type"":""Boolean""},{""name"":""right"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":828,""safe"":false},{""name"":""testBoolOrAssign"",""parameters"":[{""name"":""left"",""type"":""Boolean""},{""name"":""right"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":837,""safe"":false},{""name"":""unitTest_Member_Element_Complex_Assign"",""parameters"":[],""returntype"":""Void"",""offset"":846,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2FA1cCAAP/////AAAAAHAC////f3FonEoQLgM6SgP/////AAAAADIDOnBpnEoCAAAAgC4DOkoC////fzIDOnFoaVASv0BXAgAD/////wAAAABwAv///39xaJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGmcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FoaVASv0BXAgAQcAIAAACAcWidShAuAzpKA/////8AAAAAMgM6cGmdSgIAAACALgM6SgL///9/MgM6cWhpUBK/QFcCABBwAgAAAIBxaJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGmdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FoaVASv0BXAgAD/////wAAAABwAv///39xaBKgShAuAzpKA/////8AAAAAMgM6cGkSoEoCAAAAgC4DOkoC////fzIDOnFoaVASv0BXAgAD/////wAAAABwAv///39xaBKgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpUBK/QFcCAAP/////AAAAAHAC////f3FoEUoQLgM6SgAgMAM6qEoQLgM6SgP/////AAAAADIDOnBpEUoQLgM6SgAgMAM6qEoCAAAAgC4DOkoC////fzIDOnFoaVASv0BXAAJ4eUoQLgM6SgAgMAM6qEoCAAAAgC4DOkoC////fzIDOkqAQFcCAAP/////AAAAAHAC////f3FoEahKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGkRqEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaGlQEr9AVwIAEHACAAAAgHFoEalwaRGpcWhpUBK/QFcCABBwAgAAAIBxaBGpcGkRqXFoaVASv0BXAAJ4eZGAeEBXAAJ4eZKAeEBXAAJ4eZOAeEBXAAJ4eZPbIIB4QFcAAnh5q4B4QFcAAnh5rIB4QFcBAAwEAAECA9swFxLAcGhKEM4Tok4QUNBFaBDOEZc5aBHOE0tLzhSRSlRT0EVoEc4QzhCXOUCJuNsg").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKEC4DOkoAIDADOqhKAgAAAIAuAzpKAv///38yAzpKgEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// STARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leftShiftAssignChecked")]
    public abstract BigInteger? LeftShiftAssignChecked(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmRgHhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// AND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAndAssign")]
    public abstract BigInteger? TestAndAssign(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmrgHhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// BOOLAND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAndAssign")]
    public abstract bool? TestBoolAndAssign(bool? left, bool? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmsgHhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// BOOLOR [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOrAssign")]
    public abstract bool? TestBoolOrAssign(bool? left, bool? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmT2yCAeEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// XOR [8 datoshi]
    /// CONVERT 20 'Boolean' [8192 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolXorAssign")]
    public abstract bool? TestBoolXorAssign(bool? left, bool? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmSgHhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// OR [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOrAssign")]
    public abstract BigInteger? TestOrAssign(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmTgHhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// XOR [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testXorAssign")]
    public abstract BigInteger? TestXorAssign(BigInteger? value, BigInteger? shift);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWicShAuAzpKA/////8AAAAAMgM6cGmcSgIAAACALgM6SgL///9/MgM6cWhpUBK/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_Checked")]
    public abstract IList<object>? UnitTest_Add_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBpnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaGlQEr9A
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Add_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Add_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRShAuAzpKACAwAzqoShAuAzpKA/////8AAAAAMgM6cGkRShAuAzpKACAwAzqoSgIAAACALgM6SgL///9/MgM6cWhpUBK/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgRqEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRGoSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FoaVASv0A=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHL [8 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Left_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Left_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAQAAQID2zAXEsBwaEoQzhOiThBQ0EVoEM4RlzloEc4TS0vOFJFKVFPQRWgRzhDOEJc5QA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 00010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// MOD [8 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH4 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// ASSERT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Member_Element_Complex_Assign")]
    public abstract void UnitTest_Member_Element_Complex_Assign();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgM6SgP/////AAAAADIDOnBpEqBKAgAAAIAuAzpKAv///38yAzpxaGlQEr9A
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_Checked")]
    public abstract IList<object>? UnitTest_Mul_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAA/////8AAAAAcAL///9/cWgSoEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaRKgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FoaVASv0A=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Mul_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Mul_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalwaRGpcWhpUBK/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_Checked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFoEalwaRGpcWhpUBK/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Right_Shift_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Right_Shift_Assign_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFonUoQLgM6SgP/////AAAAADIDOnBpnUoCAAAAgC4DOkoC////fzIDOnFoaVASv0A=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_Checked")]
    public abstract IList<object>? UnitTest_Sub_Assign_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEHACAAAAgHFonUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpUBK/QA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Sub_Assign_UnChecked")]
    public abstract IList<object>? UnitTest_Sub_Assign_UnChecked();

    #endregion
}
