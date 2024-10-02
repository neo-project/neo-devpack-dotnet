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
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : DEC
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : JMPGE
    // 000B : THROW
    // 000C : DUP
    // 000D : PUSHINT64
    // 0016 : JMPLE
    // 0018 : THROW
    // 0019 : STLOC0
    // 001A : LDLOC0
    // 001B : DUP
    // 001C : DEC
    // 001D : DUP
    // 001E : PUSH0
    // 001F : JMPGE
    // 0021 : THROW
    // 0022 : DUP
    // 0023 : PUSHINT64
    // 002C : JMPLE
    // 002E : THROW
    // 002F : STLOC0
    // 0030 : DROP
    // 0031 : LDLOC0
    // 0032 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();
    // 0000 : INITSLOT
    // 0003 : PUSHINT32
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : DEC
    // 000B : DUP
    // 000C : PUSHINT32
    // 0011 : JMPGE
    // 0013 : THROW
    // 0014 : DUP
    // 0015 : PUSHINT32
    // 001A : JMPLE
    // 001C : THROW
    // 001D : STLOC0
    // 001E : LDLOC0
    // 001F : DUP
    // 0020 : DEC
    // 0021 : DUP
    // 0022 : PUSHINT32
    // 0027 : JMPGE
    // 0029 : THROW
    // 002A : DUP
    // 002B : PUSHINT32
    // 0030 : JMPLE
    // 0032 : THROW
    // 0033 : STLOC0
    // 0034 : DROP
    // 0035 : LDLOC0
    // 0036 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : DEC
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : JMPGE
    // 000B : JMP
    // 000D : DUP
    // 000E : PUSHINT64
    // 0017 : JMPLE
    // 0019 : PUSHINT64
    // 0022 : AND
    // 0023 : STLOC0
    // 0024 : LDLOC0
    // 0025 : DUP
    // 0026 : DEC
    // 0027 : DUP
    // 0028 : PUSH0
    // 0029 : JMPGE
    // 002B : JMP
    // 002D : DUP
    // 002E : PUSHINT64
    // 0037 : JMPLE
    // 0039 : PUSHINT64
    // 0042 : AND
    // 0043 : STLOC0
    // 0044 : DROP
    // 0045 : LDLOC0
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();
    // 0000 : INITSLOT
    // 0003 : PUSHINT32
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : DEC
    // 000B : DUP
    // 000C : PUSHINT32
    // 0011 : JMPGE
    // 0013 : JMP
    // 0015 : DUP
    // 0016 : PUSHINT32
    // 001B : JMPLE
    // 001D : PUSHINT64
    // 0026 : AND
    // 0027 : DUP
    // 0028 : PUSHINT32
    // 002D : JMPLE
    // 002F : PUSHINT64
    // 0038 : SUB
    // 0039 : STLOC0
    // 003A : LDLOC0
    // 003B : DUP
    // 003C : DEC
    // 003D : DUP
    // 003E : PUSHINT32
    // 0043 : JMPGE
    // 0045 : JMP
    // 0047 : DUP
    // 0048 : PUSHINT32
    // 004D : JMPLE
    // 004F : PUSHINT64
    // 0058 : AND
    // 0059 : DUP
    // 005A : PUSHINT32
    // 005F : JMPLE
    // 0061 : PUSHINT64
    // 006A : SUB
    // 006B : STLOC0
    // 006C : DROP
    // 006D : LDLOC0
    // 006E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : LDLOC0
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSH0
    // 0011 : JMPGE
    // 0013 : THROW
    // 0014 : DUP
    // 0015 : PUSHINT64
    // 001E : JMPLE
    // 0020 : THROW
    // 0021 : STLOC0
    // 0022 : LDLOC0
    // 0023 : DUP
    // 0024 : INC
    // 0025 : DUP
    // 0026 : PUSH0
    // 0027 : JMPGE
    // 0029 : THROW
    // 002A : DUP
    // 002B : PUSHINT64
    // 0034 : JMPLE
    // 0036 : THROW
    // 0037 : STLOC0
    // 0038 : DROP
    // 0039 : LDLOC0
    // 003A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();
    // 0000 : INITSLOT
    // 0003 : PUSHINT32
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : INC
    // 000B : DUP
    // 000C : PUSHINT32
    // 0011 : JMPGE
    // 0013 : THROW
    // 0014 : DUP
    // 0015 : PUSHINT32
    // 001A : JMPLE
    // 001C : THROW
    // 001D : STLOC0
    // 001E : LDLOC0
    // 001F : DUP
    // 0020 : INC
    // 0021 : DUP
    // 0022 : PUSHINT32
    // 0027 : JMPGE
    // 0029 : THROW
    // 002A : DUP
    // 002B : PUSHINT32
    // 0030 : JMPLE
    // 0032 : THROW
    // 0033 : STLOC0
    // 0034 : DROP
    // 0035 : LDLOC0
    // 0036 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : STLOC0
    // 000D : LDLOC0
    // 000E : INC
    // 000F : DUP
    // 0010 : PUSH0
    // 0011 : JMPGE
    // 0013 : JMP
    // 0015 : DUP
    // 0016 : PUSHINT64
    // 001F : JMPLE
    // 0021 : PUSHINT64
    // 002A : AND
    // 002B : STLOC0
    // 002C : LDLOC0
    // 002D : DUP
    // 002E : INC
    // 002F : DUP
    // 0030 : PUSH0
    // 0031 : JMPGE
    // 0033 : JMP
    // 0035 : DUP
    // 0036 : PUSHINT64
    // 003F : JMPLE
    // 0041 : PUSHINT64
    // 004A : AND
    // 004B : STLOC0
    // 004C : DROP
    // 004D : LDLOC0
    // 004E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();
    // 0000 : INITSLOT
    // 0003 : PUSHINT32
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : INC
    // 000B : DUP
    // 000C : PUSHINT32
    // 0011 : JMPGE
    // 0013 : JMP
    // 0015 : DUP
    // 0016 : PUSHINT32
    // 001B : JMPLE
    // 001D : PUSHINT64
    // 0026 : AND
    // 0027 : DUP
    // 0028 : PUSHINT32
    // 002D : JMPLE
    // 002F : PUSHINT64
    // 0038 : SUB
    // 0039 : STLOC0
    // 003A : LDLOC0
    // 003B : DUP
    // 003C : INC
    // 003D : DUP
    // 003E : PUSHINT32
    // 0043 : JMPGE
    // 0045 : JMP
    // 0047 : DUP
    // 0048 : PUSHINT32
    // 004D : JMPLE
    // 004F : PUSHINT64
    // 0058 : AND
    // 0059 : DUP
    // 005A : PUSHINT32
    // 005F : JMPLE
    // 0061 : PUSHINT64
    // 006A : SUB
    // 006B : STLOC0
    // 006C : DROP
    // 006D : LDLOC0
    // 006E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();
    // 0000 : INITSLOT
    // 0003 : PUSH5
    // 0004 : STLOC0
    // 0005 : JMP
    // 0007 : LDLOC0
    // 0008 : DUP
    // 0009 : DEC
    // 000A : DUP
    // 000B : PUSH0
    // 000C : JMPGE
    // 000E : JMP
    // 0010 : DUP
    // 0011 : PUSHINT64
    // 001A : JMPLE
    // 001C : PUSHINT64
    // 0025 : AND
    // 0026 : STLOC0
    // 0027 : DROP
    // 0028 : LDLOC0
    // 0029 : PUSH7
    // 002A : LT
    // 002B : JMPIF
    // 002D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DEC
    // 0005 : DUP
    // 0006 : PUSH0
    // 0007 : JMPGE
    // 0009 : THROW
    // 000A : DUP
    // 000B : PUSHINT64
    // 0014 : JMPLE
    // 0016 : THROW
    // 0017 : STARG0
    // 0018 : LDARG0
    // 0019 : DUP
    // 001A : DEC
    // 001B : DUP
    // 001C : PUSH0
    // 001D : JMPGE
    // 001F : THROW
    // 0020 : DUP
    // 0021 : PUSHINT64
    // 002A : JMPLE
    // 002C : THROW
    // 002D : STARG0
    // 002E : DROP
    // 002F : LDARG0
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DEC
    // 0005 : DUP
    // 0006 : PUSHINT32
    // 000B : JMPGE
    // 000D : THROW
    // 000E : DUP
    // 000F : PUSHINT32
    // 0014 : JMPLE
    // 0016 : THROW
    // 0017 : STARG0
    // 0018 : LDARG0
    // 0019 : DUP
    // 001A : DEC
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : JMPGE
    // 0023 : THROW
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : THROW
    // 002D : STARG0
    // 002E : DROP
    // 002F : LDARG0
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DEC
    // 0005 : DUP
    // 0006 : PUSH0
    // 0007 : JMPGE
    // 0009 : JMP
    // 000B : DUP
    // 000C : PUSHINT64
    // 0015 : JMPLE
    // 0017 : PUSHINT64
    // 0020 : AND
    // 0021 : STARG0
    // 0022 : LDARG0
    // 0023 : DUP
    // 0024 : DEC
    // 0025 : DUP
    // 0026 : PUSH0
    // 0027 : JMPGE
    // 0029 : JMP
    // 002B : DUP
    // 002C : PUSHINT64
    // 0035 : JMPLE
    // 0037 : PUSHINT64
    // 0040 : AND
    // 0041 : STARG0
    // 0042 : DROP
    // 0043 : LDARG0
    // 0044 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DEC
    // 0005 : DUP
    // 0006 : PUSHINT32
    // 000B : JMPGE
    // 000D : JMP
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPLE
    // 0017 : PUSHINT64
    // 0020 : AND
    // 0021 : DUP
    // 0022 : PUSHINT32
    // 0027 : JMPLE
    // 0029 : PUSHINT64
    // 0032 : SUB
    // 0033 : STARG0
    // 0034 : LDARG0
    // 0035 : DUP
    // 0036 : DEC
    // 0037 : DUP
    // 0038 : PUSHINT32
    // 003D : JMPGE
    // 003F : JMP
    // 0041 : DUP
    // 0042 : PUSHINT32
    // 0047 : JMPLE
    // 0049 : PUSHINT64
    // 0052 : AND
    // 0053 : DUP
    // 0054 : PUSHINT32
    // 0059 : JMPLE
    // 005B : PUSHINT64
    // 0064 : SUB
    // 0065 : STARG0
    // 0066 : DROP
    // 0067 : LDARG0
    // 0068 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : INC
    // 0005 : DUP
    // 0006 : PUSH0
    // 0007 : JMPGE
    // 0009 : THROW
    // 000A : DUP
    // 000B : PUSHINT64
    // 0014 : JMPLE
    // 0016 : THROW
    // 0017 : STARG0
    // 0018 : LDARG0
    // 0019 : DUP
    // 001A : INC
    // 001B : DUP
    // 001C : PUSH0
    // 001D : JMPGE
    // 001F : THROW
    // 0020 : DUP
    // 0021 : PUSHINT64
    // 002A : JMPLE
    // 002C : THROW
    // 002D : STARG0
    // 002E : DROP
    // 002F : LDARG0
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : INC
    // 0005 : DUP
    // 0006 : PUSHINT32
    // 000B : JMPGE
    // 000D : THROW
    // 000E : DUP
    // 000F : PUSHINT32
    // 0014 : JMPLE
    // 0016 : THROW
    // 0017 : STARG0
    // 0018 : LDARG0
    // 0019 : DUP
    // 001A : INC
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : JMPGE
    // 0023 : THROW
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : THROW
    // 002D : STARG0
    // 002E : DROP
    // 002F : LDARG0
    // 0030 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : INC
    // 0005 : DUP
    // 0006 : PUSH0
    // 0007 : JMPGE
    // 0009 : JMP
    // 000B : DUP
    // 000C : PUSHINT64
    // 0015 : JMPLE
    // 0017 : PUSHINT64
    // 0020 : AND
    // 0021 : STARG0
    // 0022 : LDARG0
    // 0023 : DUP
    // 0024 : INC
    // 0025 : DUP
    // 0026 : PUSH0
    // 0027 : JMPGE
    // 0029 : JMP
    // 002B : DUP
    // 002C : PUSHINT64
    // 0035 : JMPLE
    // 0037 : PUSHINT64
    // 0040 : AND
    // 0041 : STARG0
    // 0042 : DROP
    // 0043 : LDARG0
    // 0044 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : INC
    // 0005 : DUP
    // 0006 : PUSHINT32
    // 000B : JMPGE
    // 000D : JMP
    // 000F : DUP
    // 0010 : PUSHINT32
    // 0015 : JMPLE
    // 0017 : PUSHINT64
    // 0020 : AND
    // 0021 : DUP
    // 0022 : PUSHINT32
    // 0027 : JMPLE
    // 0029 : PUSHINT64
    // 0032 : SUB
    // 0033 : STARG0
    // 0034 : LDARG0
    // 0035 : DUP
    // 0036 : INC
    // 0037 : DUP
    // 0038 : PUSHINT32
    // 003D : JMPGE
    // 003F : JMP
    // 0041 : DUP
    // 0042 : PUSHINT32
    // 0047 : JMPLE
    // 0049 : PUSHINT64
    // 0052 : AND
    // 0053 : DUP
    // 0054 : PUSHINT32
    // 0059 : JMPLE
    // 005B : PUSHINT64
    // 0064 : SUB
    // 0065 : STARG0
    // 0066 : DROP
    // 0067 : LDARG0
    // 0068 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();
    // 0000 : PUSH0
    // 0001 : STSFLD0
    // 0002 : LDSFLD0
    // 0003 : DEC
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSHINT64
    // 0013 : JMPLE
    // 0015 : THROW
    // 0016 : STSFLD0
    // 0017 : LDSFLD0
    // 0018 : DUP
    // 0019 : DEC
    // 001A : DUP
    // 001B : PUSH0
    // 001C : JMPGE
    // 001E : THROW
    // 001F : DUP
    // 0020 : PUSHINT64
    // 0029 : JMPLE
    // 002B : THROW
    // 002C : STSFLD0
    // 002D : DROP
    // 002E : LDSFLD0
    // 002F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();
    // 0000 : PUSHINT32
    // 0005 : STSFLD1
    // 0006 : LDSFLD1
    // 0007 : DEC
    // 0008 : DUP
    // 0009 : PUSHINT32
    // 000E : JMPGE
    // 0010 : THROW
    // 0011 : DUP
    // 0012 : PUSHINT32
    // 0017 : JMPLE
    // 0019 : THROW
    // 001A : STSFLD1
    // 001B : LDSFLD1
    // 001C : DUP
    // 001D : DEC
    // 001E : DUP
    // 001F : PUSHINT32
    // 0024 : JMPGE
    // 0026 : THROW
    // 0027 : DUP
    // 0028 : PUSHINT32
    // 002D : JMPLE
    // 002F : THROW
    // 0030 : STSFLD1
    // 0031 : DROP
    // 0032 : LDSFLD1
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();
    // 0000 : PUSH0
    // 0001 : STSFLD0
    // 0002 : LDSFLD0
    // 0003 : DEC
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : JMP
    // 000A : DUP
    // 000B : PUSHINT64
    // 0014 : JMPLE
    // 0016 : PUSHINT64
    // 001F : AND
    // 0020 : STSFLD0
    // 0021 : LDSFLD0
    // 0022 : DUP
    // 0023 : DEC
    // 0024 : DUP
    // 0025 : PUSH0
    // 0026 : JMPGE
    // 0028 : JMP
    // 002A : DUP
    // 002B : PUSHINT64
    // 0034 : JMPLE
    // 0036 : PUSHINT64
    // 003F : AND
    // 0040 : STSFLD0
    // 0041 : DROP
    // 0042 : LDSFLD0
    // 0043 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();
    // 0000 : PUSHINT32
    // 0005 : STSFLD1
    // 0006 : LDSFLD1
    // 0007 : DEC
    // 0008 : DUP
    // 0009 : PUSHINT32
    // 000E : JMPGE
    // 0010 : JMP
    // 0012 : DUP
    // 0013 : PUSHINT32
    // 0018 : JMPLE
    // 001A : PUSHINT64
    // 0023 : AND
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : PUSHINT64
    // 0035 : SUB
    // 0036 : STSFLD1
    // 0037 : LDSFLD1
    // 0038 : DUP
    // 0039 : DEC
    // 003A : DUP
    // 003B : PUSHINT32
    // 0040 : JMPGE
    // 0042 : JMP
    // 0044 : DUP
    // 0045 : PUSHINT32
    // 004A : JMPLE
    // 004C : PUSHINT64
    // 0055 : AND
    // 0056 : DUP
    // 0057 : PUSHINT32
    // 005C : JMPLE
    // 005E : PUSHINT64
    // 0067 : SUB
    // 0068 : STSFLD1
    // 0069 : DROP
    // 006A : LDSFLD1
    // 006B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();
    // 0000 : PUSHINT64
    // 0009 : STSFLD0
    // 000A : LDSFLD0
    // 000B : INC
    // 000C : DUP
    // 000D : PUSH0
    // 000E : JMPGE
    // 0010 : THROW
    // 0011 : DUP
    // 0012 : PUSHINT64
    // 001B : JMPLE
    // 001D : THROW
    // 001E : STSFLD0
    // 001F : LDSFLD0
    // 0020 : DUP
    // 0021 : INC
    // 0022 : DUP
    // 0023 : PUSH0
    // 0024 : JMPGE
    // 0026 : THROW
    // 0027 : DUP
    // 0028 : PUSHINT64
    // 0031 : JMPLE
    // 0033 : THROW
    // 0034 : STSFLD0
    // 0035 : DROP
    // 0036 : LDSFLD0
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();
    // 0000 : PUSHINT32
    // 0005 : STSFLD1
    // 0006 : LDSFLD1
    // 0007 : INC
    // 0008 : DUP
    // 0009 : PUSHINT32
    // 000E : JMPGE
    // 0010 : THROW
    // 0011 : DUP
    // 0012 : PUSHINT32
    // 0017 : JMPLE
    // 0019 : THROW
    // 001A : STSFLD1
    // 001B : LDSFLD1
    // 001C : DUP
    // 001D : INC
    // 001E : DUP
    // 001F : PUSHINT32
    // 0024 : JMPGE
    // 0026 : THROW
    // 0027 : DUP
    // 0028 : PUSHINT32
    // 002D : JMPLE
    // 002F : THROW
    // 0030 : STSFLD1
    // 0031 : DROP
    // 0032 : LDSFLD1
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();
    // 0000 : PUSHINT64
    // 0009 : STSFLD0
    // 000A : LDSFLD0
    // 000B : INC
    // 000C : DUP
    // 000D : PUSH0
    // 000E : JMPGE
    // 0010 : JMP
    // 0012 : DUP
    // 0013 : PUSHINT64
    // 001C : JMPLE
    // 001E : PUSHINT64
    // 0027 : AND
    // 0028 : STSFLD0
    // 0029 : LDSFLD0
    // 002A : DUP
    // 002B : INC
    // 002C : DUP
    // 002D : PUSH0
    // 002E : JMPGE
    // 0030 : JMP
    // 0032 : DUP
    // 0033 : PUSHINT64
    // 003C : JMPLE
    // 003E : PUSHINT64
    // 0047 : AND
    // 0048 : STSFLD0
    // 0049 : DROP
    // 004A : LDSFLD0
    // 004B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();
    // 0000 : PUSHINT32
    // 0005 : STSFLD1
    // 0006 : LDSFLD1
    // 0007 : INC
    // 0008 : DUP
    // 0009 : PUSHINT32
    // 000E : JMPGE
    // 0010 : JMP
    // 0012 : DUP
    // 0013 : PUSHINT32
    // 0018 : JMPLE
    // 001A : PUSHINT64
    // 0023 : AND
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : PUSHINT64
    // 0035 : SUB
    // 0036 : STSFLD1
    // 0037 : LDSFLD1
    // 0038 : DUP
    // 0039 : INC
    // 003A : DUP
    // 003B : PUSHINT32
    // 0040 : JMPGE
    // 0042 : JMP
    // 0044 : DUP
    // 0045 : PUSHINT32
    // 004A : JMPLE
    // 004C : PUSHINT64
    // 0055 : AND
    // 0056 : DUP
    // 0057 : PUSHINT32
    // 005C : JMPLE
    // 005E : PUSHINT64
    // 0067 : SUB
    // 0068 : STSFLD1
    // 0069 : DROP
    // 006A : LDSFLD1
    // 006B : RET

    #endregion

}
