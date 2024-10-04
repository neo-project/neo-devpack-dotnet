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
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.DEC
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.JMPGE 03
    /// 000B : OpCode.THROW
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0016 : OpCode.JMPLE 03
    /// 0018 : OpCode.THROW
    /// 0019 : OpCode.STLOC0
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.DUP
    /// 001C : OpCode.DEC
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSH0
    /// 001F : OpCode.JMPGE 03
    /// 0021 : OpCode.THROW
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002C : OpCode.JMPLE 03
    /// 002E : OpCode.THROW
    /// 002F : OpCode.STLOC0
    /// 0030 : OpCode.DROP
    /// 0031 : OpCode.LDLOC0
    /// 0032 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT32 00000080
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.DEC
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT32 00000080
    /// 0011 : OpCode.JMPGE 03
    /// 0013 : OpCode.THROW
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT32 FFFFFF7F
    /// 001A : OpCode.JMPLE 03
    /// 001C : OpCode.THROW
    /// 001D : OpCode.STLOC0
    /// 001E : OpCode.LDLOC0
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.DEC
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.PUSHINT32 00000080
    /// 0027 : OpCode.JMPGE 03
    /// 0029 : OpCode.THROW
    /// 002A : OpCode.DUP
    /// 002B : OpCode.PUSHINT32 FFFFFF7F
    /// 0030 : OpCode.JMPLE 03
    /// 0032 : OpCode.THROW
    /// 0033 : OpCode.STLOC0
    /// 0034 : OpCode.DROP
    /// 0035 : OpCode.LDLOC0
    /// 0036 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.DEC
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.JMPGE 04
    /// 000B : OpCode.JMP 0E
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0017 : OpCode.JMPLE 0C
    /// 0019 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0022 : OpCode.AND
    /// 0023 : OpCode.STLOC0
    /// 0024 : OpCode.LDLOC0
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.DEC
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSH0
    /// 0029 : OpCode.JMPGE 04
    /// 002B : OpCode.JMP 0E
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0037 : OpCode.JMPLE 0C
    /// 0039 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0042 : OpCode.AND
    /// 0043 : OpCode.STLOC0
    /// 0044 : OpCode.DROP
    /// 0045 : OpCode.LDLOC0
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT32 00000080
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.DEC
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT32 00000080
    /// 0011 : OpCode.JMPGE 04
    /// 0013 : OpCode.JMP 0A
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSHINT32 FFFFFF7F
    /// 001B : OpCode.JMPLE 1E
    /// 001D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0026 : OpCode.AND
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 0C
    /// 002F : OpCode.PUSHINT64 0000000001000000
    /// 0038 : OpCode.SUB
    /// 0039 : OpCode.STLOC0
    /// 003A : OpCode.LDLOC0
    /// 003B : OpCode.DUP
    /// 003C : OpCode.DEC
    /// 003D : OpCode.DUP
    /// 003E : OpCode.PUSHINT32 00000080
    /// 0043 : OpCode.JMPGE 04
    /// 0045 : OpCode.JMP 0A
    /// 0047 : OpCode.DUP
    /// 0048 : OpCode.PUSHINT32 FFFFFF7F
    /// 004D : OpCode.JMPLE 1E
    /// 004F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0058 : OpCode.AND
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.PUSHINT32 FFFFFF7F
    /// 005F : OpCode.JMPLE 0C
    /// 0061 : OpCode.PUSHINT64 0000000001000000
    /// 006A : OpCode.SUB
    /// 006B : OpCode.STLOC0
    /// 006C : OpCode.DROP
    /// 006D : OpCode.LDLOC0
    /// 006E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSH0
    /// 0011 : OpCode.JMPGE 03
    /// 0013 : OpCode.THROW
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001E : OpCode.JMPLE 03
    /// 0020 : OpCode.THROW
    /// 0021 : OpCode.STLOC0
    /// 0022 : OpCode.LDLOC0
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.INC
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSH0
    /// 0027 : OpCode.JMPGE 03
    /// 0029 : OpCode.THROW
    /// 002A : OpCode.DUP
    /// 002B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0034 : OpCode.JMPLE 03
    /// 0036 : OpCode.THROW
    /// 0037 : OpCode.STLOC0
    /// 0038 : OpCode.DROP
    /// 0039 : OpCode.LDLOC0
    /// 003A : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT32 FFFFFF7F
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.INC
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT32 00000080
    /// 0011 : OpCode.JMPGE 03
    /// 0013 : OpCode.THROW
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT32 FFFFFF7F
    /// 001A : OpCode.JMPLE 03
    /// 001C : OpCode.THROW
    /// 001D : OpCode.STLOC0
    /// 001E : OpCode.LDLOC0
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.INC
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.PUSHINT32 00000080
    /// 0027 : OpCode.JMPGE 03
    /// 0029 : OpCode.THROW
    /// 002A : OpCode.DUP
    /// 002B : OpCode.PUSHINT32 FFFFFF7F
    /// 0030 : OpCode.JMPLE 03
    /// 0032 : OpCode.THROW
    /// 0033 : OpCode.STLOC0
    /// 0034 : OpCode.DROP
    /// 0035 : OpCode.LDLOC0
    /// 0036 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000C : OpCode.STLOC0
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.INC
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSH0
    /// 0011 : OpCode.JMPGE 04
    /// 0013 : OpCode.JMP 0E
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001F : OpCode.JMPLE 0C
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.AND
    /// 002B : OpCode.STLOC0
    /// 002C : OpCode.LDLOC0
    /// 002D : OpCode.DUP
    /// 002E : OpCode.INC
    /// 002F : OpCode.DUP
    /// 0030 : OpCode.PUSH0
    /// 0031 : OpCode.JMPGE 04
    /// 0033 : OpCode.JMP 0E
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 004A : OpCode.AND
    /// 004B : OpCode.STLOC0
    /// 004C : OpCode.DROP
    /// 004D : OpCode.LDLOC0
    /// 004E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHINT32 FFFFFF7F
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.INC
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT32 00000080
    /// 0011 : OpCode.JMPGE 04
    /// 0013 : OpCode.JMP 0A
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSHINT32 FFFFFF7F
    /// 001B : OpCode.JMPLE 1E
    /// 001D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0026 : OpCode.AND
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 0C
    /// 002F : OpCode.PUSHINT64 0000000001000000
    /// 0038 : OpCode.SUB
    /// 0039 : OpCode.STLOC0
    /// 003A : OpCode.LDLOC0
    /// 003B : OpCode.DUP
    /// 003C : OpCode.INC
    /// 003D : OpCode.DUP
    /// 003E : OpCode.PUSHINT32 00000080
    /// 0043 : OpCode.JMPGE 04
    /// 0045 : OpCode.JMP 0A
    /// 0047 : OpCode.DUP
    /// 0048 : OpCode.PUSHINT32 FFFFFF7F
    /// 004D : OpCode.JMPLE 1E
    /// 004F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0058 : OpCode.AND
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.PUSHINT32 FFFFFF7F
    /// 005F : OpCode.JMPLE 0C
    /// 0061 : OpCode.PUSHINT64 0000000001000000
    /// 006A : OpCode.SUB
    /// 006B : OpCode.STLOC0
    /// 006C : OpCode.DROP
    /// 006D : OpCode.LDLOC0
    /// 006E : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH5
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.JMP 23
    /// 0007 : OpCode.LDLOC0
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.DEC
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0E
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001A : OpCode.JMPLE 0C
    /// 001C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0025 : OpCode.AND
    /// 0026 : OpCode.STLOC0
    /// 0027 : OpCode.DROP
    /// 0028 : OpCode.LDLOC0
    /// 0029 : OpCode.PUSH7
    /// 002A : OpCode.LT
    /// 002B : OpCode.JMPIF DC
    /// 002D : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DEC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.JMPGE 03
    /// 0009 : OpCode.THROW
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0014 : OpCode.JMPLE 03
    /// 0016 : OpCode.THROW
    /// 0017 : OpCode.STARG0
    /// 0018 : OpCode.LDARG0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.DEC
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSH0
    /// 001D : OpCode.JMPGE 03
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.JMPLE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.STARG0
    /// 002E : OpCode.DROP
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DEC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSHINT32 00000080
    /// 000B : OpCode.JMPGE 03
    /// 000D : OpCode.THROW
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSHINT32 FFFFFF7F
    /// 0014 : OpCode.JMPLE 03
    /// 0016 : OpCode.THROW
    /// 0017 : OpCode.STARG0
    /// 0018 : OpCode.LDARG0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.DEC
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 00000080
    /// 0021 : OpCode.JMPGE 03
    /// 0023 : OpCode.THROW
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.STARG0
    /// 002E : OpCode.DROP
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DEC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.JMPGE 04
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0015 : OpCode.JMPLE 0C
    /// 0017 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0020 : OpCode.AND
    /// 0021 : OpCode.STARG0
    /// 0022 : OpCode.LDARG0
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.DEC
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSH0
    /// 0027 : OpCode.JMPGE 04
    /// 0029 : OpCode.JMP 0E
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0035 : OpCode.JMPLE 0C
    /// 0037 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0040 : OpCode.AND
    /// 0041 : OpCode.STARG0
    /// 0042 : OpCode.DROP
    /// 0043 : OpCode.LDARG0
    /// 0044 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DEC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSHINT32 00000080
    /// 000B : OpCode.JMPGE 04
    /// 000D : OpCode.JMP 0A
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 FFFFFF7F
    /// 0015 : OpCode.JMPLE 1E
    /// 0017 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0020 : OpCode.AND
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.PUSHINT32 FFFFFF7F
    /// 0027 : OpCode.JMPLE 0C
    /// 0029 : OpCode.PUSHINT64 0000000001000000
    /// 0032 : OpCode.SUB
    /// 0033 : OpCode.STARG0
    /// 0034 : OpCode.LDARG0
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.DEC
    /// 0037 : OpCode.DUP
    /// 0038 : OpCode.PUSHINT32 00000080
    /// 003D : OpCode.JMPGE 04
    /// 003F : OpCode.JMP 0A
    /// 0041 : OpCode.DUP
    /// 0042 : OpCode.PUSHINT32 FFFFFF7F
    /// 0047 : OpCode.JMPLE 1E
    /// 0049 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0052 : OpCode.AND
    /// 0053 : OpCode.DUP
    /// 0054 : OpCode.PUSHINT32 FFFFFF7F
    /// 0059 : OpCode.JMPLE 0C
    /// 005B : OpCode.PUSHINT64 0000000001000000
    /// 0064 : OpCode.SUB
    /// 0065 : OpCode.STARG0
    /// 0066 : OpCode.DROP
    /// 0067 : OpCode.LDARG0
    /// 0068 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.INC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.JMPGE 03
    /// 0009 : OpCode.THROW
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0014 : OpCode.JMPLE 03
    /// 0016 : OpCode.THROW
    /// 0017 : OpCode.STARG0
    /// 0018 : OpCode.LDARG0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.INC
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSH0
    /// 001D : OpCode.JMPGE 03
    /// 001F : OpCode.THROW
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002A : OpCode.JMPLE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.STARG0
    /// 002E : OpCode.DROP
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.INC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSHINT32 00000080
    /// 000B : OpCode.JMPGE 03
    /// 000D : OpCode.THROW
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSHINT32 FFFFFF7F
    /// 0014 : OpCode.JMPLE 03
    /// 0016 : OpCode.THROW
    /// 0017 : OpCode.STARG0
    /// 0018 : OpCode.LDARG0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.INC
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 00000080
    /// 0021 : OpCode.JMPGE 03
    /// 0023 : OpCode.THROW
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 03
    /// 002C : OpCode.THROW
    /// 002D : OpCode.STARG0
    /// 002E : OpCode.DROP
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.INC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH0
    /// 0007 : OpCode.JMPGE 04
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0015 : OpCode.JMPLE 0C
    /// 0017 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0020 : OpCode.AND
    /// 0021 : OpCode.STARG0
    /// 0022 : OpCode.LDARG0
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.INC
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSH0
    /// 0027 : OpCode.JMPGE 04
    /// 0029 : OpCode.JMP 0E
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0035 : OpCode.JMPLE 0C
    /// 0037 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0040 : OpCode.AND
    /// 0041 : OpCode.STARG0
    /// 0042 : OpCode.DROP
    /// 0043 : OpCode.LDARG0
    /// 0044 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.INC
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSHINT32 00000080
    /// 000B : OpCode.JMPGE 04
    /// 000D : OpCode.JMP 0A
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.PUSHINT32 FFFFFF7F
    /// 0015 : OpCode.JMPLE 1E
    /// 0017 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0020 : OpCode.AND
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.PUSHINT32 FFFFFF7F
    /// 0027 : OpCode.JMPLE 0C
    /// 0029 : OpCode.PUSHINT64 0000000001000000
    /// 0032 : OpCode.SUB
    /// 0033 : OpCode.STARG0
    /// 0034 : OpCode.LDARG0
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.INC
    /// 0037 : OpCode.DUP
    /// 0038 : OpCode.PUSHINT32 00000080
    /// 003D : OpCode.JMPGE 04
    /// 003F : OpCode.JMP 0A
    /// 0041 : OpCode.DUP
    /// 0042 : OpCode.PUSHINT32 FFFFFF7F
    /// 0047 : OpCode.JMPLE 1E
    /// 0049 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0052 : OpCode.AND
    /// 0053 : OpCode.DUP
    /// 0054 : OpCode.PUSHINT32 FFFFFF7F
    /// 0059 : OpCode.JMPLE 0C
    /// 005B : OpCode.PUSHINT64 0000000001000000
    /// 0064 : OpCode.SUB
    /// 0065 : OpCode.STARG0
    /// 0066 : OpCode.DROP
    /// 0067 : OpCode.LDARG0
    /// 0068 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSH0
    /// 0001 : OpCode.STSFLD0
    /// 0002 : OpCode.LDSFLD0
    /// 0003 : OpCode.DEC
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0013 : OpCode.JMPLE 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.STSFLD0
    /// 0017 : OpCode.LDSFLD0
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.DEC
    /// 001A : OpCode.DUP
    /// 001B : OpCode.PUSH0
    /// 001C : OpCode.JMPGE 03
    /// 001E : OpCode.THROW
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0029 : OpCode.JMPLE 03
    /// 002B : OpCode.THROW
    /// 002C : OpCode.STSFLD0
    /// 002D : OpCode.DROP
    /// 002E : OpCode.LDSFLD0
    /// 002F : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT32 00000080
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD1
    /// 0007 : OpCode.DEC
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.PUSHINT32 00000080
    /// 000E : OpCode.JMPGE 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT32 FFFFFF7F
    /// 0017 : OpCode.JMPLE 03
    /// 0019 : OpCode.THROW
    /// 001A : OpCode.STSFLD1
    /// 001B : OpCode.LDSFLD1
    /// 001C : OpCode.DUP
    /// 001D : OpCode.DEC
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSHINT32 00000080
    /// 0024 : OpCode.JMPGE 03
    /// 0026 : OpCode.THROW
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 03
    /// 002F : OpCode.THROW
    /// 0030 : OpCode.STSFLD1
    /// 0031 : OpCode.DROP
    /// 0032 : OpCode.LDSFLD1
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSH0
    /// 0001 : OpCode.STSFLD0
    /// 0002 : OpCode.LDSFLD0
    /// 0003 : OpCode.DEC
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 04
    /// 0008 : OpCode.JMP 0E
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0014 : OpCode.JMPLE 0C
    /// 0016 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001F : OpCode.AND
    /// 0020 : OpCode.STSFLD0
    /// 0021 : OpCode.LDSFLD0
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.DEC
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSH0
    /// 0026 : OpCode.JMPGE 04
    /// 0028 : OpCode.JMP 0E
    /// 002A : OpCode.DUP
    /// 002B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0034 : OpCode.JMPLE 0C
    /// 0036 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003F : OpCode.AND
    /// 0040 : OpCode.STSFLD0
    /// 0041 : OpCode.DROP
    /// 0042 : OpCode.LDSFLD0
    /// 0043 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT32 00000080
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD1
    /// 0007 : OpCode.DEC
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.PUSHINT32 00000080
    /// 000E : OpCode.JMPGE 04
    /// 0010 : OpCode.JMP 0A
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSHINT32 FFFFFF7F
    /// 0018 : OpCode.JMPLE 1E
    /// 001A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0023 : OpCode.AND
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 0C
    /// 002C : OpCode.PUSHINT64 0000000001000000
    /// 0035 : OpCode.SUB
    /// 0036 : OpCode.STSFLD1
    /// 0037 : OpCode.LDSFLD1
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.DEC
    /// 003A : OpCode.DUP
    /// 003B : OpCode.PUSHINT32 00000080
    /// 0040 : OpCode.JMPGE 04
    /// 0042 : OpCode.JMP 0A
    /// 0044 : OpCode.DUP
    /// 0045 : OpCode.PUSHINT32 FFFFFF7F
    /// 004A : OpCode.JMPLE 1E
    /// 004C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0055 : OpCode.AND
    /// 0056 : OpCode.DUP
    /// 0057 : OpCode.PUSHINT32 FFFFFF7F
    /// 005C : OpCode.JMPLE 0C
    /// 005E : OpCode.PUSHINT64 0000000001000000
    /// 0067 : OpCode.SUB
    /// 0068 : OpCode.STSFLD1
    /// 0069 : OpCode.DROP
    /// 006A : OpCode.LDSFLD1
    /// 006B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0009 : OpCode.STSFLD0
    /// 000A : OpCode.LDSFLD0
    /// 000B : OpCode.INC
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.JMPGE 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001B : OpCode.JMPLE 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.STSFLD0
    /// 001F : OpCode.LDSFLD0
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.INC
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSH0
    /// 0024 : OpCode.JMPGE 03
    /// 0026 : OpCode.THROW
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0031 : OpCode.JMPLE 03
    /// 0033 : OpCode.THROW
    /// 0034 : OpCode.STSFLD0
    /// 0035 : OpCode.DROP
    /// 0036 : OpCode.LDSFLD0
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT32 FFFFFF7F
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD1
    /// 0007 : OpCode.INC
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.PUSHINT32 00000080
    /// 000E : OpCode.JMPGE 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT32 FFFFFF7F
    /// 0017 : OpCode.JMPLE 03
    /// 0019 : OpCode.THROW
    /// 001A : OpCode.STSFLD1
    /// 001B : OpCode.LDSFLD1
    /// 001C : OpCode.DUP
    /// 001D : OpCode.INC
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSHINT32 00000080
    /// 0024 : OpCode.JMPGE 03
    /// 0026 : OpCode.THROW
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 03
    /// 002F : OpCode.THROW
    /// 0030 : OpCode.STSFLD1
    /// 0031 : OpCode.DROP
    /// 0032 : OpCode.LDSFLD1
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0009 : OpCode.STSFLD0
    /// 000A : OpCode.LDSFLD0
    /// 000B : OpCode.INC
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.JMPGE 04
    /// 0010 : OpCode.JMP 0E
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001C : OpCode.JMPLE 0C
    /// 001E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0027 : OpCode.AND
    /// 0028 : OpCode.STSFLD0
    /// 0029 : OpCode.LDSFLD0
    /// 002A : OpCode.DUP
    /// 002B : OpCode.INC
    /// 002C : OpCode.DUP
    /// 002D : OpCode.PUSH0
    /// 002E : OpCode.JMPGE 04
    /// 0030 : OpCode.JMP 0E
    /// 0032 : OpCode.DUP
    /// 0033 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003C : OpCode.JMPLE 0C
    /// 003E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0047 : OpCode.AND
    /// 0048 : OpCode.STSFLD0
    /// 0049 : OpCode.DROP
    /// 004A : OpCode.LDSFLD0
    /// 004B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.PUSHINT32 FFFFFF7F
    /// 0005 : OpCode.STSFLD1
    /// 0006 : OpCode.LDSFLD1
    /// 0007 : OpCode.INC
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.PUSHINT32 00000080
    /// 000E : OpCode.JMPGE 04
    /// 0010 : OpCode.JMP 0A
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSHINT32 FFFFFF7F
    /// 0018 : OpCode.JMPLE 1E
    /// 001A : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0023 : OpCode.AND
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 0C
    /// 002C : OpCode.PUSHINT64 0000000001000000
    /// 0035 : OpCode.SUB
    /// 0036 : OpCode.STSFLD1
    /// 0037 : OpCode.LDSFLD1
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.INC
    /// 003A : OpCode.DUP
    /// 003B : OpCode.PUSHINT32 00000080
    /// 0040 : OpCode.JMPGE 04
    /// 0042 : OpCode.JMP 0A
    /// 0044 : OpCode.DUP
    /// 0045 : OpCode.PUSHINT32 FFFFFF7F
    /// 004A : OpCode.JMPLE 1E
    /// 004C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0055 : OpCode.AND
    /// 0056 : OpCode.DUP
    /// 0057 : OpCode.PUSHINT32 FFFFFF7F
    /// 005C : OpCode.JMPLE 0C
    /// 005E : OpCode.PUSHINT64 0000000001000000
    /// 0067 : OpCode.SUB
    /// 0068 : OpCode.STSFLD1
    /// 0069 : OpCode.DROP
    /// 006A : OpCode.LDSFLD1
    /// 006B : OpCode.RET
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();

    #endregion

}
