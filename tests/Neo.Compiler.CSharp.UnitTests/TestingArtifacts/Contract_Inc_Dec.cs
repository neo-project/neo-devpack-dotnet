using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":180,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":248,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":299,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":370,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":429,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":508,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":557,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":626,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":675,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":744,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":796,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":904,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":956,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1064,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1119,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1230,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":1285,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1396,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1445,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1550,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1599,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1704,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1750,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3ZBgP/////AAAAAGBYnEoQLgM6SgP/////AAAAADIDOmBYSpxKEC4DOkoD/////wAAAAAyAzpgRVhAA/////8AAAAAYFicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBYSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYQBBgWJ1KEC4DOkoD/////wAAAAAyAzpgWEqdShAuAzpKA/////8AAAAAMgM6YEVYQBBgWJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYFhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgRVhAVwEAEHBonUoQLgM6SgP/////AAAAADIDOnBoSp1KEC4DOkoD/////wAAAAAyAzpwRWhAVwEAEHBonUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaEBXAQAD/////wAAAABwaJxKEC4DOkoD/////wAAAAAyAzpwaEqcShAuAzpKA/////8AAAAAMgM6cEVoQFcBAAP/////AAAAAHBonEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaEqcShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaEBXAAF4nUoQLgM6SgP/////AAAAADIDOoB4Sp1KEC4DOkoD/////wAAAAAyAzqARXhAVwABeJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgHhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhAVwABeJxKEC4DOkoD/////wAAAAAyAzqAeEqcShAuAzpKA/////8AAAAAMgM6gEV4QFcAAXicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkYB4SpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgEV4QAL///9/YVmcSgIAAACALgM6SgL///9/MgM6YVlKnEoCAAAAgC4DOkoC////fzIDOmFFWUAC////f2FZnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hWUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWUACAAAAgGFZnUoCAAAAgC4DOkoC////fzIDOmFZSp1KAgAAAIAuAzpKAv///38yAzphRVlAAgAAAIBhWZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlAVwEAAv///39waJxKAgAAAIAuAzpKAv///38yAzpwaEqcSgIAAACALgM6SgL///9/MgM6cEVoQFcBAAL///9/cGicSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoQFcBAAIAAACAcGidSgIAAACALgM6SgL///9/MgM6cGhKnUoCAAAAgC4DOkoC////fzIDOnBFaEBXAQACAAAAgHBonUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaEBXAAF4nEoCAAAAgC4DOkoC////fzIDOoB4SpxKAgAAAIAuAzpKAv///38yAzqARXhAVwABeJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhAVwABeJ1KAgAAAIAuAzpKAv///38yAzqAeEqdSgIAAACALgM6SgL///9/MgM6gEV4QFcAAXidSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4Sp1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4QFcBABVwIiNoSp1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoF7Uk3EBWAkBwSuOc"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonUoQLgM6SgP/////AAAAADIDOnBoSp1KEC4DOkoD/////wAAAAAyAzpwRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : DEC [4 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : JMPGE 03 [2 datoshi]
    /// 0B : THROW [512 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 16 : JMPLE 03 [2 datoshi]
    /// 18 : THROW [512 datoshi]
    /// 19 : STLOC0 [2 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : DEC [4 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : PUSH0 [1 datoshi]
    /// 1F : JMPGE 03 [2 datoshi]
    /// 21 : THROW [512 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2C : JMPLE 03 [2 datoshi]
    /// 2E : THROW [512 datoshi]
    /// 2F : STLOC0 [2 datoshi]
    /// 30 : DROP [2 datoshi]
    /// 31 : LDLOC0 [2 datoshi]
    /// 32 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KAgAAAIAuAzpKAv///38yAzpwaEqdSgIAAACALgM6SgL///9/MgM6cEVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT32 00000080 [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : DEC [4 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT32 00000080 [1 datoshi]
    /// 11 : JMPGE 03 [2 datoshi]
    /// 13 : THROW [512 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1A : JMPLE 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : STLOC0 [2 datoshi]
    /// 1E : LDLOC0 [2 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : DEC [4 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 00000080 [1 datoshi]
    /// 27 : JMPGE 03 [2 datoshi]
    /// 29 : THROW [512 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 30 : JMPLE 03 [2 datoshi]
    /// 32 : THROW [512 datoshi]
    /// 33 : STLOC0 [2 datoshi]
    /// 34 : DROP [2 datoshi]
    /// 35 : LDLOC0 [2 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwaEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBFaEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : DEC [4 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : JMPGE 04 [2 datoshi]
    /// 0B : JMP 0E [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 17 : JMPLE 0C [2 datoshi]
    /// 19 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 22 : AND [8 datoshi]
    /// 23 : STLOC0 [2 datoshi]
    /// 24 : LDLOC0 [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : DEC [4 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSH0 [1 datoshi]
    /// 29 : JMPGE 04 [2 datoshi]
    /// 2B : JMP 0E [2 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : JMPLE 0C [2 datoshi]
    /// 39 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 42 : AND [8 datoshi]
    /// 43 : STLOC0 [2 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : LDLOC0 [2 datoshi]
    /// 46 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT32 00000080 [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : DEC [4 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT32 00000080 [1 datoshi]
    /// 11 : JMPGE 04 [2 datoshi]
    /// 13 : JMP 0A [2 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1B : JMPLE 1E [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : AND [8 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 0C [2 datoshi]
    /// 2F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 38 : SUB [8 datoshi]
    /// 39 : STLOC0 [2 datoshi]
    /// 3A : LDLOC0 [2 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : DEC [4 datoshi]
    /// 3D : DUP [2 datoshi]
    /// 3E : PUSHINT32 00000080 [1 datoshi]
    /// 43 : JMPGE 04 [2 datoshi]
    /// 45 : JMP 0A [2 datoshi]
    /// 47 : DUP [2 datoshi]
    /// 48 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4D : JMPLE 1E [2 datoshi]
    /// 4F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 58 : AND [8 datoshi]
    /// 59 : DUP [2 datoshi]
    /// 5A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5F : JMPLE 0C [2 datoshi]
    /// 61 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 6A : SUB [8 datoshi]
    /// 6B : STLOC0 [2 datoshi]
    /// 6C : DROP [2 datoshi]
    /// 6D : LDLOC0 [2 datoshi]
    /// 6E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicShAuAzpKA/////8AAAAAMgM6cGhKnEoQLgM6SgP/////AAAAADIDOnBFaEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSH0 [1 datoshi]
    /// 11 : JMPGE 03 [2 datoshi]
    /// 13 : THROW [512 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : JMPLE 03 [2 datoshi]
    /// 20 : THROW [512 datoshi]
    /// 21 : STLOC0 [2 datoshi]
    /// 22 : LDLOC0 [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : INC [4 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : JMPGE 03 [2 datoshi]
    /// 29 : THROW [512 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 34 : JMPLE 03 [2 datoshi]
    /// 36 : THROW [512 datoshi]
    /// 37 : STLOC0 [2 datoshi]
    /// 38 : DROP [2 datoshi]
    /// 39 : LDLOC0 [2 datoshi]
    /// 3A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKAgAAAIAuAzpKAv///38yAzpwaEqcSgIAAACALgM6SgL///9/MgM6cEVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : INC [4 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT32 00000080 [1 datoshi]
    /// 11 : JMPGE 03 [2 datoshi]
    /// 13 : THROW [512 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1A : JMPLE 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : STLOC0 [2 datoshi]
    /// 1E : LDLOC0 [2 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : INC [4 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 00000080 [1 datoshi]
    /// 27 : JMPGE 03 [2 datoshi]
    /// 29 : THROW [512 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 30 : JMPLE 03 [2 datoshi]
    /// 32 : THROW [512 datoshi]
    /// 33 : STLOC0 [2 datoshi]
    /// 34 : DROP [2 datoshi]
    /// 35 : LDLOC0 [2 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkXBoSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRcEVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0C : STLOC0 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSH0 [1 datoshi]
    /// 11 : JMPGE 04 [2 datoshi]
    /// 13 : JMP 0E [2 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1F : JMPLE 0C [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : STLOC0 [2 datoshi]
    /// 2C : LDLOC0 [2 datoshi]
    /// 2D : DUP [2 datoshi]
    /// 2E : INC [4 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : PUSH0 [1 datoshi]
    /// 31 : JMPGE 04 [2 datoshi]
    /// 33 : JMP 0E [2 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3F : JMPLE 0C [2 datoshi]
    /// 41 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 4A : AND [8 datoshi]
    /// 4B : STLOC0 [2 datoshi]
    /// 4C : DROP [2 datoshi]
    /// 4D : LDLOC0 [2 datoshi]
    /// 4E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : INC [4 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT32 00000080 [1 datoshi]
    /// 11 : JMPGE 04 [2 datoshi]
    /// 13 : JMP 0A [2 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 1B : JMPLE 1E [2 datoshi]
    /// 1D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 26 : AND [8 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 0C [2 datoshi]
    /// 2F : PUSHINT64 0000000001000000 [1 datoshi]
    /// 38 : SUB [8 datoshi]
    /// 39 : STLOC0 [2 datoshi]
    /// 3A : LDLOC0 [2 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : INC [4 datoshi]
    /// 3D : DUP [2 datoshi]
    /// 3E : PUSHINT32 00000080 [1 datoshi]
    /// 43 : JMPGE 04 [2 datoshi]
    /// 45 : JMP 0A [2 datoshi]
    /// 47 : DUP [2 datoshi]
    /// 48 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4D : JMPLE 1E [2 datoshi]
    /// 4F : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 58 : AND [8 datoshi]
    /// 59 : DUP [2 datoshi]
    /// 5A : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5F : JMPLE 0C [2 datoshi]
    /// 61 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 6A : SUB [8 datoshi]
    /// 6B : STLOC0 [2 datoshi]
    /// 6C : DROP [2 datoshi]
    /// 6D : LDLOC0 [2 datoshi]
    /// 6E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAFXAiI2hKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFwRWgXtSTcQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH5 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : JMP 23 [2 datoshi]
    /// 07 : LDLOC0 [2 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : DEC [4 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0E [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1A : JMPLE 0C [2 datoshi]
    /// 1C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : AND [8 datoshi]
    /// 26 : STLOC0 [2 datoshi]
    /// 27 : DROP [2 datoshi]
    /// 28 : LDLOC0 [2 datoshi]
    /// 29 : PUSH7 [1 datoshi]
    /// 2A : LT [8 datoshi]
    /// 2B : JMPIF DC [2 datoshi]
    /// 2D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KEC4DOkoD/////wAAAAAyAzqAeEqdShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DEC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : JMPGE 03 [2 datoshi]
    /// 09 : THROW [512 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 14 : JMPLE 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : STARG0 [2 datoshi]
    /// 18 : LDARG0 [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : DEC [4 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSH0 [1 datoshi]
    /// 1D : JMPGE 03 [2 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : JMPLE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : STARG0 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KAgAAAIAuAzpKAv///38yAzqAeEqdSgIAAACALgM6SgL///9/MgM6gEV4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DEC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSHINT32 00000080 [1 datoshi]
    /// 0B : JMPGE 03 [2 datoshi]
    /// 0D : THROW [512 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 14 : JMPLE 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : STARG0 [2 datoshi]
    /// 18 : LDARG0 [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : DEC [4 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT32 00000080 [1 datoshi]
    /// 21 : JMPGE 03 [2 datoshi]
    /// 23 : THROW [512 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : STARG0 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgHhKnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DEC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : JMPGE 04 [2 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 15 : JMPLE 0C [2 datoshi]
    /// 17 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 20 : AND [8 datoshi]
    /// 21 : STARG0 [2 datoshi]
    /// 22 : LDARG0 [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : DEC [4 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : JMPGE 04 [2 datoshi]
    /// 29 : JMP 0E [2 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 35 : JMPLE 0C [2 datoshi]
    /// 37 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 40 : AND [8 datoshi]
    /// 41 : STARG0 [2 datoshi]
    /// 42 : DROP [2 datoshi]
    /// 43 : LDARG0 [2 datoshi]
    /// 44 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DEC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSHINT32 00000080 [1 datoshi]
    /// 0B : JMPGE 04 [2 datoshi]
    /// 0D : JMP 0A [2 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 15 : JMPLE 1E [2 datoshi]
    /// 17 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 20 : AND [8 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 27 : JMPLE 0C [2 datoshi]
    /// 29 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 32 : SUB [8 datoshi]
    /// 33 : STARG0 [2 datoshi]
    /// 34 : LDARG0 [2 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : DEC [4 datoshi]
    /// 37 : DUP [2 datoshi]
    /// 38 : PUSHINT32 00000080 [1 datoshi]
    /// 3D : JMPGE 04 [2 datoshi]
    /// 3F : JMP 0A [2 datoshi]
    /// 41 : DUP [2 datoshi]
    /// 42 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 47 : JMPLE 1E [2 datoshi]
    /// 49 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 52 : AND [8 datoshi]
    /// 53 : DUP [2 datoshi]
    /// 54 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 59 : JMPLE 0C [2 datoshi]
    /// 5B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 64 : SUB [8 datoshi]
    /// 65 : STARG0 [2 datoshi]
    /// 66 : DROP [2 datoshi]
    /// 67 : LDARG0 [2 datoshi]
    /// 68 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKEC4DOkoD/////wAAAAAyAzqAeEqcShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : INC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : JMPGE 03 [2 datoshi]
    /// 09 : THROW [512 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 14 : JMPLE 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : STARG0 [2 datoshi]
    /// 18 : LDARG0 [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : INC [4 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSH0 [1 datoshi]
    /// 1D : JMPGE 03 [2 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2A : JMPLE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : STARG0 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKAgAAAIAuAzpKAv///38yAzqAeEqcSgIAAACALgM6SgL///9/MgM6gEV4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : INC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSHINT32 00000080 [1 datoshi]
    /// 0B : JMPGE 03 [2 datoshi]
    /// 0D : THROW [512 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 14 : JMPLE 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : STARG0 [2 datoshi]
    /// 18 : LDARG0 [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : INC [4 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : PUSHINT32 00000080 [1 datoshi]
    /// 21 : JMPGE 03 [2 datoshi]
    /// 23 : THROW [512 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 03 [2 datoshi]
    /// 2C : THROW [512 datoshi]
    /// 2D : STARG0 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRgHhKnEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJGARXhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : INC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : JMPGE 04 [2 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 15 : JMPLE 0C [2 datoshi]
    /// 17 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 20 : AND [8 datoshi]
    /// 21 : STARG0 [2 datoshi]
    /// 22 : LDARG0 [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : INC [4 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : JMPGE 04 [2 datoshi]
    /// 29 : JMP 0E [2 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 35 : JMPLE 0C [2 datoshi]
    /// 37 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 40 : AND [8 datoshi]
    /// 41 : STARG0 [2 datoshi]
    /// 42 : DROP [2 datoshi]
    /// 43 : LDARG0 [2 datoshi]
    /// 44 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+ARXhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : INC [4 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSHINT32 00000080 [1 datoshi]
    /// 0B : JMPGE 04 [2 datoshi]
    /// 0D : JMP 0A [2 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 15 : JMPLE 1E [2 datoshi]
    /// 17 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 20 : AND [8 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 27 : JMPLE 0C [2 datoshi]
    /// 29 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 32 : SUB [8 datoshi]
    /// 33 : STARG0 [2 datoshi]
    /// 34 : LDARG0 [2 datoshi]
    /// 35 : DUP [2 datoshi]
    /// 36 : INC [4 datoshi]
    /// 37 : DUP [2 datoshi]
    /// 38 : PUSHINT32 00000080 [1 datoshi]
    /// 3D : JMPGE 04 [2 datoshi]
    /// 3F : JMP 0A [2 datoshi]
    /// 41 : DUP [2 datoshi]
    /// 42 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 47 : JMPLE 1E [2 datoshi]
    /// 49 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 52 : AND [8 datoshi]
    /// 53 : DUP [2 datoshi]
    /// 54 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 59 : JMPLE 0C [2 datoshi]
    /// 5B : PUSHINT64 0000000001000000 [1 datoshi]
    /// 64 : SUB [8 datoshi]
    /// 65 : STARG0 [2 datoshi]
    /// 66 : DROP [2 datoshi]
    /// 67 : LDARG0 [2 datoshi]
    /// 68 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnUoQLgM6SgP/////AAAAADIDOmBYSp1KEC4DOkoD/////wAAAAAyAzpgRVhA
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : STSFLD0 [2 datoshi]
    /// 02 : LDSFLD0 [2 datoshi]
    /// 03 : DEC [4 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 13 : JMPLE 03 [2 datoshi]
    /// 15 : THROW [512 datoshi]
    /// 16 : STSFLD0 [2 datoshi]
    /// 17 : LDSFLD0 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : DEC [4 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : PUSH0 [1 datoshi]
    /// 1C : JMPGE 03 [2 datoshi]
    /// 1E : THROW [512 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : JMPLE 03 [2 datoshi]
    /// 2B : THROW [512 datoshi]
    /// 2C : STSFLD0 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : LDSFLD0 [2 datoshi]
    /// 2F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KAgAAAIAuAzpKAv///38yAzphWUqdSgIAAACALgM6SgL///9/MgM6YUVZQA==
    /// 00 : PUSHINT32 00000080 [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD1 [2 datoshi]
    /// 07 : DEC [4 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 17 : JMPLE 03 [2 datoshi]
    /// 19 : THROW [512 datoshi]
    /// 1A : STSFLD1 [2 datoshi]
    /// 1B : LDSFLD1 [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : DEC [4 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSHINT32 00000080 [1 datoshi]
    /// 24 : JMPGE 03 [2 datoshi]
    /// 26 : THROW [512 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 03 [2 datoshi]
    /// 2F : THROW [512 datoshi]
    /// 30 : STSFLD1 [2 datoshi]
    /// 31 : DROP [2 datoshi]
    /// 32 : LDSFLD1 [2 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnUoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFgWEqdShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBFWEA=
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : STSFLD0 [2 datoshi]
    /// 02 : LDSFLD0 [2 datoshi]
    /// 03 : DEC [4 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 04 [2 datoshi]
    /// 08 : JMP 0E [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 14 : JMPLE 0C [2 datoshi]
    /// 16 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1F : AND [8 datoshi]
    /// 20 : STSFLD0 [2 datoshi]
    /// 21 : LDSFLD0 [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : DEC [4 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSH0 [1 datoshi]
    /// 26 : JMPGE 04 [2 datoshi]
    /// 28 : JMP 0E [2 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 34 : JMPLE 0C [2 datoshi]
    /// 36 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3F : AND [8 datoshi]
    /// 40 : STSFLD0 [2 datoshi]
    /// 41 : DROP [2 datoshi]
    /// 42 : LDSFLD0 [2 datoshi]
    /// 43 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlA
    /// 00 : PUSHINT32 00000080 [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD1 [2 datoshi]
    /// 07 : DEC [4 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 04 [2 datoshi]
    /// 10 : JMP 0A [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : JMPLE 1E [2 datoshi]
    /// 1A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : AND [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 0C [2 datoshi]
    /// 2C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : SUB [8 datoshi]
    /// 36 : STSFLD1 [2 datoshi]
    /// 37 : LDSFLD1 [2 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : DEC [4 datoshi]
    /// 3A : DUP [2 datoshi]
    /// 3B : PUSHINT32 00000080 [1 datoshi]
    /// 40 : JMPGE 04 [2 datoshi]
    /// 42 : JMP 0A [2 datoshi]
    /// 44 : DUP [2 datoshi]
    /// 45 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4A : JMPLE 1E [2 datoshi]
    /// 4C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 55 : AND [8 datoshi]
    /// 56 : DUP [2 datoshi]
    /// 57 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5C : JMPLE 0C [2 datoshi]
    /// 5E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 67 : SUB [8 datoshi]
    /// 68 : STSFLD1 [2 datoshi]
    /// 69 : DROP [2 datoshi]
    /// 6A : LDSFLD1 [2 datoshi]
    /// 6B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicShAuAzpKA/////8AAAAAMgM6YFhKnEoQLgM6SgP/////AAAAADIDOmBFWEA=
    /// 00 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 09 : STSFLD0 [2 datoshi]
    /// 0A : LDSFLD0 [2 datoshi]
    /// 0B : INC [4 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : JMPGE 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1B : JMPLE 03 [2 datoshi]
    /// 1D : THROW [512 datoshi]
    /// 1E : STSFLD0 [2 datoshi]
    /// 1F : LDSFLD0 [2 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : INC [4 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSH0 [1 datoshi]
    /// 24 : JMPGE 03 [2 datoshi]
    /// 26 : THROW [512 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 31 : JMPLE 03 [2 datoshi]
    /// 33 : THROW [512 datoshi]
    /// 34 : STSFLD0 [2 datoshi]
    /// 35 : DROP [2 datoshi]
    /// 36 : LDSFLD0 [2 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKAgAAAIAuAzpKAv///38yAzphWUqcSgIAAACALgM6SgL///9/MgM6YUVZQA==
    /// 00 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD1 [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 17 : JMPLE 03 [2 datoshi]
    /// 19 : THROW [512 datoshi]
    /// 1A : STSFLD1 [2 datoshi]
    /// 1B : LDSFLD1 [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : INC [4 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSHINT32 00000080 [1 datoshi]
    /// 24 : JMPGE 03 [2 datoshi]
    /// 26 : THROW [512 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2D : JMPLE 03 [2 datoshi]
    /// 2F : THROW [512 datoshi]
    /// 30 : STSFLD1 [2 datoshi]
    /// 31 : DROP [2 datoshi]
    /// 32 : LDSFLD1 [2 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicShAuBCIOSgP/////AAAAADIMA/////8AAAAAkWBYSpxKEC4EIg5KA/////8AAAAAMgwD/////wAAAACRYEVYQA==
    /// 00 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 09 : STSFLD0 [2 datoshi]
    /// 0A : LDSFLD0 [2 datoshi]
    /// 0B : INC [4 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : JMPGE 04 [2 datoshi]
    /// 10 : JMP 0E [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1C : JMPLE 0C [2 datoshi]
    /// 1E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 27 : AND [8 datoshi]
    /// 28 : STSFLD0 [2 datoshi]
    /// 29 : LDSFLD0 [2 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : INC [4 datoshi]
    /// 2C : DUP [2 datoshi]
    /// 2D : PUSH0 [1 datoshi]
    /// 2E : JMPGE 04 [2 datoshi]
    /// 30 : JMP 0E [2 datoshi]
    /// 32 : DUP [2 datoshi]
    /// 33 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3C : JMPLE 0C [2 datoshi]
    /// 3E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 47 : AND [8 datoshi]
    /// 48 : STSFLD0 [2 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : LDSFLD0 [2 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlA
    /// 00 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 05 : STSFLD1 [2 datoshi]
    /// 06 : LDSFLD1 [2 datoshi]
    /// 07 : INC [4 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : PUSHINT32 00000080 [1 datoshi]
    /// 0E : JMPGE 04 [2 datoshi]
    /// 10 : JMP 0A [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 18 : JMPLE 1E [2 datoshi]
    /// 1A : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 23 : AND [8 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2A : JMPLE 0C [2 datoshi]
    /// 2C : PUSHINT64 0000000001000000 [1 datoshi]
    /// 35 : SUB [8 datoshi]
    /// 36 : STSFLD1 [2 datoshi]
    /// 37 : LDSFLD1 [2 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : INC [4 datoshi]
    /// 3A : DUP [2 datoshi]
    /// 3B : PUSHINT32 00000080 [1 datoshi]
    /// 40 : JMPGE 04 [2 datoshi]
    /// 42 : JMP 0A [2 datoshi]
    /// 44 : DUP [2 datoshi]
    /// 45 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 4A : JMPLE 1E [2 datoshi]
    /// 4C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 55 : AND [8 datoshi]
    /// 56 : DUP [2 datoshi]
    /// 57 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 5C : JMPLE 0C [2 datoshi]
    /// 5E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 67 : SUB [8 datoshi]
    /// 68 : STSFLD1 [2 datoshi]
    /// 69 : DROP [2 datoshi]
    /// 6A : LDSFLD1 [2 datoshi]
    /// 6B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();

    #endregion
}
