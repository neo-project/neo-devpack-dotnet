using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":180,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":248,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":299,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":370,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":429,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":508,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":557,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":626,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":675,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":744,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":796,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":904,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":956,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1064,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1119,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1230,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1285,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1396,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1445,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1550,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1599,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1704,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1750,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x5d46269ff23ec37de131e4791d5e5c964b140704"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEBxRLllxeHXnkMeF9wz7ynyZGXQl0ZXN0QXJnczEBAAEPBAcUS5ZcXh155DHhfcM+8p8mRl0IdGVzdFZvaWQAAAAPAAD92QYD/////wAAAABgWJxKEC4DOkoD/////wAAAAAyAzpgWEqcShAuAzpKA/////8AAAAAMgM6YEVYQAP/////AAAAAGBYnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgWEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBFWEAQYFidShAuAzpKA/////8AAAAAMgM6YFhKnUoQLgM6SgP/////AAAAADIDOmBFWEAQYFidShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBYSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYQFcBABBwaJ1KEC4DOkoD/////wAAAAAyAzpwaEqdShAuAzpKA/////8AAAAAMgM6cEVoQFcBABBwaJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWhAVwEAA/////8AAAAAcGicShAuAzpKA/////8AAAAAMgM6cGhKnEoQLgM6SgP/////AAAAADIDOnBFaEBXAQAD/////wAAAABwaJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcGhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWhAVwABeJ1KEC4DOkoD/////wAAAAAyAzqAeEqdShAuAzpKA/////8AAAAAMgM6gEV4QFcAAXidShAuBCIOSgP/////AAAAADIMA/////8AAAAAkYB4Sp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgEV4QFcAAXicShAuAzpKA/////8AAAAAMgM6gHhKnEoQLgM6SgP/////AAAAADIDOoBFeEBXAAF4nEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGAeEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkYBFeEAC////f2FZnEoCAAAAgC4DOkoC////fzIDOmFZSpxKAgAAAIAuAzpKAv///38yAzphRVlAAv///39hWZxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlAAgAAAIBhWZ1KAgAAAIAuAzpKAv///38yAzphWUqdSgIAAACALgM6SgL///9/MgM6YUVZQAIAAACAYVmdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FZSp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUVZQFcBAAL///9/cGicSgIAAACALgM6SgL///9/MgM6cGhKnEoCAAAAgC4DOkoC////fzIDOnBFaEBXAQAC////f3BonEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaEBXAQACAAAAgHBonUoCAAAAgC4DOkoC////fzIDOnBoSp1KAgAAAIAuAzpKAv///38yAzpwRWhAVwEAAgAAAIBwaJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhAVwABeJxKAgAAAIAuAzpKAv///38yAzqAeEqcSgIAAACALgM6SgL///9/MgM6gEV4QFcAAXicSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4SpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4QFcAAXidSgIAAACALgM6SgL///9/MgM6gHhKnUoCAAAAgC4DOkoC////fzIDOoBFeEBXAAF4nUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4BFeEBXAQAVcCIjaEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaBe1JNxAVgJAt1Dg/Q=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonUoQLgM6SgP/////AAAAADIDOnBoSp1KEC4DOkoD/////wAAAAAyAzpwRWhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.DEC
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.JMPGE 03
    /// 0B : OpCode.THROW
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 16 : OpCode.JMPLE 03
    /// 18 : OpCode.THROW
    /// 19 : OpCode.STLOC0
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.DUP
    /// 1C : OpCode.DEC
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.JMPGE 03
    /// 21 : OpCode.THROW
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2C : OpCode.JMPLE 03
    /// 2E : OpCode.THROW
    /// 2F : OpCode.STLOC0
    /// 30 : OpCode.DROP
    /// 31 : OpCode.LDLOC0
    /// 32 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KAgAAAIAuAzpKAv///38yAzpwaEqdSgIAAACALgM6SgL///9/MgM6cEVoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT32 00000080
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.DEC
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT32 00000080
    /// 11 : OpCode.JMPGE 03
    /// 13 : OpCode.THROW
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT32 FFFFFF7F
    /// 1A : OpCode.JMPLE 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.STLOC0
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.DUP
    /// 20 : OpCode.DEC
    /// 21 : OpCode.DUP
    /// 22 : OpCode.PUSHINT32 00000080
    /// 27 : OpCode.JMPGE 03
    /// 29 : OpCode.THROW
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT32 FFFFFF7F
    /// 30 : OpCode.JMPLE 03
    /// 32 : OpCode.THROW
    /// 33 : OpCode.STLOC0
    /// 34 : OpCode.DROP
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaEA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.DEC
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.JMPGE 04
    /// 0B : OpCode.JMP 0E
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 17 : OpCode.JMPLE 0C
    /// 19 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 22 : OpCode.AND
    /// 23 : OpCode.STLOC0
    /// 24 : OpCode.LDLOC0
    /// 25 : OpCode.DUP
    /// 26 : OpCode.DEC
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSH0
    /// 29 : OpCode.JMPGE 04
    /// 2B : OpCode.JMP 0E
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.JMPLE 0C
    /// 39 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 42 : OpCode.AND
    /// 43 : OpCode.STLOC0
    /// 44 : OpCode.DROP
    /// 45 : OpCode.LDLOC0
    /// 46 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT32 00000080
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.DEC
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT32 00000080
    /// 11 : OpCode.JMPGE 04
    /// 13 : OpCode.JMP 0A
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHINT32 FFFFFF7F
    /// 1B : OpCode.JMPLE 1E
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 26 : OpCode.AND
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 0C
    /// 2F : OpCode.PUSHINT64 0000000001000000
    /// 38 : OpCode.SUB
    /// 39 : OpCode.STLOC0
    /// 3A : OpCode.LDLOC0
    /// 3B : OpCode.DUP
    /// 3C : OpCode.DEC
    /// 3D : OpCode.DUP
    /// 3E : OpCode.PUSHINT32 00000080
    /// 43 : OpCode.JMPGE 04
    /// 45 : OpCode.JMP 0A
    /// 47 : OpCode.DUP
    /// 48 : OpCode.PUSHINT32 FFFFFF7F
    /// 4D : OpCode.JMPLE 1E
    /// 4F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 58 : OpCode.AND
    /// 59 : OpCode.DUP
    /// 5A : OpCode.PUSHINT32 FFFFFF7F
    /// 5F : OpCode.JMPLE 0C
    /// 61 : OpCode.PUSHINT64 0000000001000000
    /// 6A : OpCode.SUB
    /// 6B : OpCode.STLOC0
    /// 6C : OpCode.DROP
    /// 6D : OpCode.LDLOC0
    /// 6E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicShAuAzpKA/////8AAAAAMgM6cGhKnEoQLgM6SgP/////AAAAADIDOnBFaEA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSH0
    /// 11 : OpCode.JMPGE 03
    /// 13 : OpCode.THROW
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1E : OpCode.JMPLE 03
    /// 20 : OpCode.THROW
    /// 21 : OpCode.STLOC0
    /// 22 : OpCode.LDLOC0
    /// 23 : OpCode.DUP
    /// 24 : OpCode.INC
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSH0
    /// 27 : OpCode.JMPGE 03
    /// 29 : OpCode.THROW
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 34 : OpCode.JMPLE 03
    /// 36 : OpCode.THROW
    /// 37 : OpCode.STLOC0
    /// 38 : OpCode.DROP
    /// 39 : OpCode.LDLOC0
    /// 3A : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKAgAAAIAuAzpKAv///38yAzpwaEqcSgIAAACALgM6SgL///9/MgM6cEVoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT32 FFFFFF7F
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.INC
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT32 00000080
    /// 11 : OpCode.JMPGE 03
    /// 13 : OpCode.THROW
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT32 FFFFFF7F
    /// 1A : OpCode.JMPLE 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.STLOC0
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.DUP
    /// 20 : OpCode.INC
    /// 21 : OpCode.DUP
    /// 22 : OpCode.PUSHINT32 00000080
    /// 27 : OpCode.JMPGE 03
    /// 29 : OpCode.THROW
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT32 FFFFFF7F
    /// 30 : OpCode.JMPLE 03
    /// 32 : OpCode.THROW
    /// 33 : OpCode.STLOC0
    /// 34 : OpCode.DROP
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBoSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0C : OpCode.STLOC0
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.INC
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSH0
    /// 11 : OpCode.JMPGE 04
    /// 13 : OpCode.JMP 0E
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1F : OpCode.JMPLE 0C
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.AND
    /// 2B : OpCode.STLOC0
    /// 2C : OpCode.LDLOC0
    /// 2D : OpCode.DUP
    /// 2E : OpCode.INC
    /// 2F : OpCode.DUP
    /// 30 : OpCode.PUSH0
    /// 31 : OpCode.JMPGE 04
    /// 33 : OpCode.JMP 0E
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 4A : OpCode.AND
    /// 4B : OpCode.STLOC0
    /// 4C : OpCode.DROP
    /// 4D : OpCode.LDLOC0
    /// 4E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHINT32 FFFFFF7F
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.INC
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT32 00000080
    /// 11 : OpCode.JMPGE 04
    /// 13 : OpCode.JMP 0A
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHINT32 FFFFFF7F
    /// 1B : OpCode.JMPLE 1E
    /// 1D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 26 : OpCode.AND
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 0C
    /// 2F : OpCode.PUSHINT64 0000000001000000
    /// 38 : OpCode.SUB
    /// 39 : OpCode.STLOC0
    /// 3A : OpCode.LDLOC0
    /// 3B : OpCode.DUP
    /// 3C : OpCode.INC
    /// 3D : OpCode.DUP
    /// 3E : OpCode.PUSHINT32 00000080
    /// 43 : OpCode.JMPGE 04
    /// 45 : OpCode.JMP 0A
    /// 47 : OpCode.DUP
    /// 48 : OpCode.PUSHINT32 FFFFFF7F
    /// 4D : OpCode.JMPLE 1E
    /// 4F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 58 : OpCode.AND
    /// 59 : OpCode.DUP
    /// 5A : OpCode.PUSHINT32 FFFFFF7F
    /// 5F : OpCode.JMPLE 0C
    /// 61 : OpCode.PUSHINT64 0000000001000000
    /// 6A : OpCode.SUB
    /// 6B : OpCode.STLOC0
    /// 6C : OpCode.DROP
    /// 6D : OpCode.LDLOC0
    /// 6E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAFXAiI2hKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWgXtSTcQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH5
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.JMP 23
    /// 07 : OpCode.LDLOC0
    /// 08 : OpCode.DUP
    /// 09 : OpCode.DEC
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0E
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1A : OpCode.JMPLE 0C
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 25 : OpCode.AND
    /// 26 : OpCode.STLOC0
    /// 27 : OpCode.DROP
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.PUSH7
    /// 2A : OpCode.LT
    /// 2B : OpCode.JMPIF DC
    /// 2D : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KEC4DOkoD/////wAAAAAyAzqAeEqdShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DEC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.JMPGE 03
    /// 09 : OpCode.THROW
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 14 : OpCode.JMPLE 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.STARG0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.DEC
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.JMPGE 03
    /// 1F : OpCode.THROW
    /// 20 : OpCode.DUP
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.JMPLE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.STARG0
    /// 2E : OpCode.DROP
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KAgAAAIAuAzpKAv///38yAzqAeEqdSgIAAACALgM6SgL///9/MgM6gEV4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DEC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSHINT32 00000080
    /// 0B : OpCode.JMPGE 03
    /// 0D : OpCode.THROW
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSHINT32 FFFFFF7F
    /// 14 : OpCode.JMPLE 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.STARG0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.DEC
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 00000080
    /// 21 : OpCode.JMPGE 03
    /// 23 : OpCode.THROW
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.STARG0
    /// 2E : OpCode.DROP
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgHhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DEC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.JMPGE 04
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 15 : OpCode.JMPLE 0C
    /// 17 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 20 : OpCode.AND
    /// 21 : OpCode.STARG0
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.DUP
    /// 24 : OpCode.DEC
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSH0
    /// 27 : OpCode.JMPGE 04
    /// 29 : OpCode.JMP 0E
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 35 : OpCode.JMPLE 0C
    /// 37 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 40 : OpCode.AND
    /// 41 : OpCode.STARG0
    /// 42 : OpCode.DROP
    /// 43 : OpCode.LDARG0
    /// 44 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DEC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSHINT32 00000080
    /// 0B : OpCode.JMPGE 04
    /// 0D : OpCode.JMP 0A
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 FFFFFF7F
    /// 15 : OpCode.JMPLE 1E
    /// 17 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 20 : OpCode.AND
    /// 21 : OpCode.DUP
    /// 22 : OpCode.PUSHINT32 FFFFFF7F
    /// 27 : OpCode.JMPLE 0C
    /// 29 : OpCode.PUSHINT64 0000000001000000
    /// 32 : OpCode.SUB
    /// 33 : OpCode.STARG0
    /// 34 : OpCode.LDARG0
    /// 35 : OpCode.DUP
    /// 36 : OpCode.DEC
    /// 37 : OpCode.DUP
    /// 38 : OpCode.PUSHINT32 00000080
    /// 3D : OpCode.JMPGE 04
    /// 3F : OpCode.JMP 0A
    /// 41 : OpCode.DUP
    /// 42 : OpCode.PUSHINT32 FFFFFF7F
    /// 47 : OpCode.JMPLE 1E
    /// 49 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 52 : OpCode.AND
    /// 53 : OpCode.DUP
    /// 54 : OpCode.PUSHINT32 FFFFFF7F
    /// 59 : OpCode.JMPLE 0C
    /// 5B : OpCode.PUSHINT64 0000000001000000
    /// 64 : OpCode.SUB
    /// 65 : OpCode.STARG0
    /// 66 : OpCode.DROP
    /// 67 : OpCode.LDARG0
    /// 68 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKEC4DOkoD/////wAAAAAyAzqAeEqcShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.INC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.JMPGE 03
    /// 09 : OpCode.THROW
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 14 : OpCode.JMPLE 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.STARG0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.INC
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.JMPGE 03
    /// 1F : OpCode.THROW
    /// 20 : OpCode.DUP
    /// 21 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2A : OpCode.JMPLE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.STARG0
    /// 2E : OpCode.DROP
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKAgAAAIAuAzpKAv///38yAzqAeEqcSgIAAACALgM6SgL///9/MgM6gEV4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.INC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSHINT32 00000080
    /// 0B : OpCode.JMPGE 03
    /// 0D : OpCode.THROW
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSHINT32 FFFFFF7F
    /// 14 : OpCode.JMPLE 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.STARG0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.INC
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 00000080
    /// 21 : OpCode.JMPGE 03
    /// 23 : OpCode.THROW
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 03
    /// 2C : OpCode.THROW
    /// 2D : OpCode.STARG0
    /// 2E : OpCode.DROP
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgHhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.INC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.JMPGE 04
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 15 : OpCode.JMPLE 0C
    /// 17 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 20 : OpCode.AND
    /// 21 : OpCode.STARG0
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.DUP
    /// 24 : OpCode.INC
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSH0
    /// 27 : OpCode.JMPGE 04
    /// 29 : OpCode.JMP 0E
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 35 : OpCode.JMPLE 0C
    /// 37 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 40 : OpCode.AND
    /// 41 : OpCode.STARG0
    /// 42 : OpCode.DROP
    /// 43 : OpCode.LDARG0
    /// 44 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.INC
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSHINT32 00000080
    /// 0B : OpCode.JMPGE 04
    /// 0D : OpCode.JMP 0A
    /// 0F : OpCode.DUP
    /// 10 : OpCode.PUSHINT32 FFFFFF7F
    /// 15 : OpCode.JMPLE 1E
    /// 17 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 20 : OpCode.AND
    /// 21 : OpCode.DUP
    /// 22 : OpCode.PUSHINT32 FFFFFF7F
    /// 27 : OpCode.JMPLE 0C
    /// 29 : OpCode.PUSHINT64 0000000001000000
    /// 32 : OpCode.SUB
    /// 33 : OpCode.STARG0
    /// 34 : OpCode.LDARG0
    /// 35 : OpCode.DUP
    /// 36 : OpCode.INC
    /// 37 : OpCode.DUP
    /// 38 : OpCode.PUSHINT32 00000080
    /// 3D : OpCode.JMPGE 04
    /// 3F : OpCode.JMP 0A
    /// 41 : OpCode.DUP
    /// 42 : OpCode.PUSHINT32 FFFFFF7F
    /// 47 : OpCode.JMPLE 1E
    /// 49 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 52 : OpCode.AND
    /// 53 : OpCode.DUP
    /// 54 : OpCode.PUSHINT32 FFFFFF7F
    /// 59 : OpCode.JMPLE 0C
    /// 5B : OpCode.PUSHINT64 0000000001000000
    /// 64 : OpCode.SUB
    /// 65 : OpCode.STARG0
    /// 66 : OpCode.DROP
    /// 67 : OpCode.LDARG0
    /// 68 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnUoQLgM6SgP/////AAAAADIDOmBYSp1KEC4DOkoD/////wAAAAAyAzpgRVhA
    /// 00 : OpCode.PUSH0
    /// 01 : OpCode.STSFLD0
    /// 02 : OpCode.LDSFLD0
    /// 03 : OpCode.DEC
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 13 : OpCode.JMPLE 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.STSFLD0
    /// 17 : OpCode.LDSFLD0
    /// 18 : OpCode.DUP
    /// 19 : OpCode.DEC
    /// 1A : OpCode.DUP
    /// 1B : OpCode.PUSH0
    /// 1C : OpCode.JMPGE 03
    /// 1E : OpCode.THROW
    /// 1F : OpCode.DUP
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 29 : OpCode.JMPLE 03
    /// 2B : OpCode.THROW
    /// 2C : OpCode.STSFLD0
    /// 2D : OpCode.DROP
    /// 2E : OpCode.LDSFLD0
    /// 2F : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KAgAAAIAuAzpKAv///38yAzphWUqdSgIAAACALgM6SgL///9/MgM6YUVZQA==
    /// 00 : OpCode.PUSHINT32 00000080
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD1
    /// 07 : OpCode.DEC
    /// 08 : OpCode.DUP
    /// 09 : OpCode.PUSHINT32 00000080
    /// 0E : OpCode.JMPGE 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT32 FFFFFF7F
    /// 17 : OpCode.JMPLE 03
    /// 19 : OpCode.THROW
    /// 1A : OpCode.STSFLD1
    /// 1B : OpCode.LDSFLD1
    /// 1C : OpCode.DUP
    /// 1D : OpCode.DEC
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSHINT32 00000080
    /// 24 : OpCode.JMPGE 03
    /// 26 : OpCode.THROW
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 03
    /// 2F : OpCode.THROW
    /// 30 : OpCode.STSFLD1
    /// 31 : OpCode.DROP
    /// 32 : OpCode.LDSFLD1
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgWEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBFWEA=
    /// 00 : OpCode.PUSH0
    /// 01 : OpCode.STSFLD0
    /// 02 : OpCode.LDSFLD0
    /// 03 : OpCode.DEC
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 04
    /// 08 : OpCode.JMP 0E
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 14 : OpCode.JMPLE 0C
    /// 16 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1F : OpCode.AND
    /// 20 : OpCode.STSFLD0
    /// 21 : OpCode.LDSFLD0
    /// 22 : OpCode.DUP
    /// 23 : OpCode.DEC
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSH0
    /// 26 : OpCode.JMPGE 04
    /// 28 : OpCode.JMP 0E
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 34 : OpCode.JMPLE 0C
    /// 36 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3F : OpCode.AND
    /// 40 : OpCode.STSFLD0
    /// 41 : OpCode.DROP
    /// 42 : OpCode.LDSFLD0
    /// 43 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlA
    /// 00 : OpCode.PUSHINT32 00000080
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD1
    /// 07 : OpCode.DEC
    /// 08 : OpCode.DUP
    /// 09 : OpCode.PUSHINT32 00000080
    /// 0E : OpCode.JMPGE 04
    /// 10 : OpCode.JMP 0A
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSHINT32 FFFFFF7F
    /// 18 : OpCode.JMPLE 1E
    /// 1A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 23 : OpCode.AND
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 0C
    /// 2C : OpCode.PUSHINT64 0000000001000000
    /// 35 : OpCode.SUB
    /// 36 : OpCode.STSFLD1
    /// 37 : OpCode.LDSFLD1
    /// 38 : OpCode.DUP
    /// 39 : OpCode.DEC
    /// 3A : OpCode.DUP
    /// 3B : OpCode.PUSHINT32 00000080
    /// 40 : OpCode.JMPGE 04
    /// 42 : OpCode.JMP 0A
    /// 44 : OpCode.DUP
    /// 45 : OpCode.PUSHINT32 FFFFFF7F
    /// 4A : OpCode.JMPLE 1E
    /// 4C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 55 : OpCode.AND
    /// 56 : OpCode.DUP
    /// 57 : OpCode.PUSHINT32 FFFFFF7F
    /// 5C : OpCode.JMPLE 0C
    /// 5E : OpCode.PUSHINT64 0000000001000000
    /// 67 : OpCode.SUB
    /// 68 : OpCode.STSFLD1
    /// 69 : OpCode.DROP
    /// 6A : OpCode.LDSFLD1
    /// 6B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicShAuAzpKA/////8AAAAAMgM6YFhKnEoQLgM6SgP/////AAAAADIDOmBFWEA=
    /// 00 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 09 : OpCode.STSFLD0
    /// 0A : OpCode.LDSFLD0
    /// 0B : OpCode.INC
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.JMPGE 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1B : OpCode.JMPLE 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.STSFLD0
    /// 1F : OpCode.LDSFLD0
    /// 20 : OpCode.DUP
    /// 21 : OpCode.INC
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSH0
    /// 24 : OpCode.JMPGE 03
    /// 26 : OpCode.THROW
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 31 : OpCode.JMPLE 03
    /// 33 : OpCode.THROW
    /// 34 : OpCode.STSFLD0
    /// 35 : OpCode.DROP
    /// 36 : OpCode.LDSFLD0
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKAgAAAIAuAzpKAv///38yAzphWUqcSgIAAACALgM6SgL///9/MgM6YUVZQA==
    /// 00 : OpCode.PUSHINT32 FFFFFF7F
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD1
    /// 07 : OpCode.INC
    /// 08 : OpCode.DUP
    /// 09 : OpCode.PUSHINT32 00000080
    /// 0E : OpCode.JMPGE 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT32 FFFFFF7F
    /// 17 : OpCode.JMPLE 03
    /// 19 : OpCode.THROW
    /// 1A : OpCode.STSFLD1
    /// 1B : OpCode.LDSFLD1
    /// 1C : OpCode.DUP
    /// 1D : OpCode.INC
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSHINT32 00000080
    /// 24 : OpCode.JMPGE 03
    /// 26 : OpCode.THROW
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 03
    /// 2F : OpCode.THROW
    /// 30 : OpCode.STSFLD1
    /// 31 : OpCode.DROP
    /// 32 : OpCode.LDSFLD1
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBYSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYQA==
    /// 00 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 09 : OpCode.STSFLD0
    /// 0A : OpCode.LDSFLD0
    /// 0B : OpCode.INC
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.JMPGE 04
    /// 10 : OpCode.JMP 0E
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1C : OpCode.JMPLE 0C
    /// 1E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 27 : OpCode.AND
    /// 28 : OpCode.STSFLD0
    /// 29 : OpCode.LDSFLD0
    /// 2A : OpCode.DUP
    /// 2B : OpCode.INC
    /// 2C : OpCode.DUP
    /// 2D : OpCode.PUSH0
    /// 2E : OpCode.JMPGE 04
    /// 30 : OpCode.JMP 0E
    /// 32 : OpCode.DUP
    /// 33 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3C : OpCode.JMPLE 0C
    /// 3E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 47 : OpCode.AND
    /// 48 : OpCode.STSFLD0
    /// 49 : OpCode.DROP
    /// 4A : OpCode.LDSFLD0
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlA
    /// 00 : OpCode.PUSHINT32 FFFFFF7F
    /// 05 : OpCode.STSFLD1
    /// 06 : OpCode.LDSFLD1
    /// 07 : OpCode.INC
    /// 08 : OpCode.DUP
    /// 09 : OpCode.PUSHINT32 00000080
    /// 0E : OpCode.JMPGE 04
    /// 10 : OpCode.JMP 0A
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSHINT32 FFFFFF7F
    /// 18 : OpCode.JMPLE 1E
    /// 1A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 23 : OpCode.AND
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT32 FFFFFF7F
    /// 2A : OpCode.JMPLE 0C
    /// 2C : OpCode.PUSHINT64 0000000001000000
    /// 35 : OpCode.SUB
    /// 36 : OpCode.STSFLD1
    /// 37 : OpCode.LDSFLD1
    /// 38 : OpCode.DUP
    /// 39 : OpCode.INC
    /// 3A : OpCode.DUP
    /// 3B : OpCode.PUSHINT32 00000080
    /// 40 : OpCode.JMPGE 04
    /// 42 : OpCode.JMP 0A
    /// 44 : OpCode.DUP
    /// 45 : OpCode.PUSHINT32 FFFFFF7F
    /// 4A : OpCode.JMPLE 1E
    /// 4C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 55 : OpCode.AND
    /// 56 : OpCode.DUP
    /// 57 : OpCode.PUSHINT32 FFFFFF7F
    /// 5C : OpCode.JMPLE 0C
    /// 5E : OpCode.PUSHINT64 0000000001000000
    /// 67 : OpCode.SUB
    /// 68 : OpCode.STSFLD1
    /// 69 : OpCode.DROP
    /// 6A : OpCode.LDSFLD1
    /// 6B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();

    #endregion
}
