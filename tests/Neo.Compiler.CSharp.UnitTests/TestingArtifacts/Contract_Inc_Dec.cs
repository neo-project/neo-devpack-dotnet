using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Inc_Dec(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Inc_Dec"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_Property_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""unitTest_Property_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":96,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":144,""safe"":false},{""name"":""unitTest_Local_Dec_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":176,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":227,""safe"":false},{""name"":""unitTest_Local_Inc_Checked"",""parameters"":[],""returntype"":""Integer"",""offset"":262,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked"",""parameters"":[],""returntype"":""Integer"",""offset"":321,""safe"":false},{""name"":""unitTest_Param_Dec_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":364,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":413,""safe"":false},{""name"":""unitTest_Param_Inc_Checked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":446,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":495,""safe"":false},{""name"":""unitTest_Property_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":528,""safe"":false},{""name"":""unitTest_Property_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":556,""safe"":false},{""name"":""unitTest_Property_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":638,""safe"":false},{""name"":""unitTest_Property_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":666,""safe"":false},{""name"":""unitTest_Local_Inc_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":748,""safe"":false},{""name"":""unitTest_Local_Inc_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":779,""safe"":false},{""name"":""unitTest_Local_Dec_Checked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":864,""safe"":false},{""name"":""unitTest_Local_Dec_UnChecked_Int"",""parameters"":[],""returntype"":""Integer"",""offset"":895,""safe"":false},{""name"":""unitTest_Param_Inc_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":980,""safe"":false},{""name"":""unitTest_Param_Inc_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1005,""safe"":false},{""name"":""unitTest_Param_Dec_Checked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1084,""safe"":false},{""name"":""unitTest_Param_Dec_UnChecked_Int"",""parameters"":[{""name"":""param"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1109,""safe"":false},{""name"":""unitTest_Not_DeadLoop"",""parameters"":[],""returntype"":""Void"",""offset"":1188,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1216,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3HBAP/////AAAAAGBYnEoQLgM6SgP/////AAAAADIDOmBYSpxKEC4DOkoD/////wAAAAAyAzpgRVhAA/////8AAAAAYFicA/////8AAAAAkWBYSpwD/////wAAAACRYEVYQBBgWJ1KEC4DOkoD/////wAAAAAyAzpgWEqdShAuAzpKA/////8AAAAAMgM6YEVYQBBgWJ0D/////wAAAACRYFhKnQP/////AAAAAJFgRVhAVwEAEHBonUoQLgM6SgP/////AAAAADIDOnBoSp1KEC4DOkoD/////wAAAAAyAzpwRWhAVwEAEHBonQP/////AAAAAJFwaEqdA/////8AAAAAkXBFaEBXAQAD/////wAAAABwaJxKEC4DOkoD/////wAAAAAyAzpwaEqcShAuAzpKA/////8AAAAAMgM6cEVoQFcBAAP/////AAAAAHBonAP/////AAAAAJFwaEqcA/////8AAAAAkXBFaEBXAAF4nUoQLgM6SgP/////AAAAADIDOoB4Sp1KEC4DOkoD/////wAAAAAyAzqARXhAVwABeJ0D/////wAAAACRgHhKnQP/////AAAAAJGARXhAVwABeJxKEC4DOkoD/////wAAAAAyAzqAeEqcShAuAzpKA/////8AAAAAMgM6gEV4QFcAAXicA/////8AAAAAkYB4SpwD/////wAAAACRgEV4QAL///9/YVmcSsoUMgM6YVlKnErKFDIDOmFFWUAC////f2FZnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FZSpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hRVlAAgAAAIBhWZ1KyhQyAzphWUqdSsoUMgM6YUVZQAIAAACAYVmdSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYVlKnUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2FFWUBXAQAC////f3BonErKFDIDOnBoSpxKyhQyAzpwRWhAVwEAAv///39waJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoQFcBAAIAAACAcGidSsoUMgM6cGhKnUrKFDIDOnBFaEBXAQACAAAAgHBonUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BoSp1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWhAVwABeJxKyhQyAzqAeEqcSsoUMgM6gEV4QFcAAXicSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgHhKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4BFeEBXAAF4nUrKFDIDOoB4Sp1KyhQyAzqARXhAVwABeJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEqdSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4QFcBABVwIhFoSp0D/////wAAAACRcEVoF7Uk7kBWAhBgEGFAUCwqLg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonUoQLgM6SgP/////AAAAADIDOnBoSp1KEC4DOkoD/////wAAAAAyAzpwRWhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
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
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
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
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KyhQyAzpwaEqdSsoUMgM6cEVoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBonQP/////AAAAAJFwaEqdA/////8AAAAAkXBFaEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAgAAAIBwaJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEqdSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicShAuAzpKA/////8AAAAAMgM6cGhKnEoQLgM6SgP/////AAAAADIDOnBFaEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
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
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
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
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKyhQyAzpwaEqcSsoUMgM6cEVoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAA/////8AAAAAcGicA/////8AAAAAkXBoSpwD/////wAAAACRcEVoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAAv///39waJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waEqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Local_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Local_Inc_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAFXAiEWhKnQP/////AAAAAJFwRWgXtSTuQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH5 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// JMP 11 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIF EE [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Not_DeadLoop")]
    public abstract void UnitTest_Not_DeadLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KEC4DOkoD/////wAAAAAyAzqAeEqdShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KyhQyAzqAeEqdSsoUMgM6gEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ0D/////wAAAACRgHhKnQP/////AAAAAJGARXhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEqdSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Dec_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKEC4DOkoD/////wAAAAAyAzqAeEqcShAuAzpKA/////8AAAAAMgM6gEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKyhQyAzqAeEqcSsoUMgM6gEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_Checked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJwD/////wAAAACRgHhKnAP/////AAAAAJGARXhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfgEV4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Param_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Param_Inc_UnChecked_Int(BigInteger? param);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnUoQLgM6SgP/////AAAAADIDOmBYSp1KEC4DOkoD/////wAAAAAyAzpgRVhA
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KyhQyAzphWUqdSsoUMgM6YUVZQA==
    /// PUSHINT32 00000080 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EGBYnQP/////AAAAAJFgWEqdA/////8AAAAAkWBFWEA=
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AgAAAIBhWZ1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hWUqdSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUVZQA==
    /// PUSHINT32 00000080 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Dec_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Dec_UnChecked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicShAuAzpKA/////8AAAAAMgM6YFhKnEoQLgM6SgP/////AAAAADIDOmBFWEA=
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKyhQyAzphWUqcSsoUMgM6YUVZQA==
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_Checked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_Checked_Int();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////8AAAAAYFicA/////8AAAAAkWBYSpwD/////wAAAACRYEVYQA==
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Av///39hWZxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hWUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUVZQA==
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_Property_Inc_UnChecked_Int")]
    public abstract BigInteger? UnitTest_Property_Inc_UnChecked_Int();

    #endregion
}
